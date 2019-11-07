import { MalType, MalType_list, MalType_symbol, MalType_fn, list, nil, fn, map, malEqual } from "./types";
import { ResultS, error, ok, some, resultSMapM, matchUnion, isUnion, Result_ok } from "powerfp";
import { Env } from "./env";
import { symbol, string_ } from "./adt.generated";
import { toobject } from "powerseq";
import { isMacroMeta } from "./core";

export function eval_(mal: MalType, env: Env): ResultS<MalType> {
  return macroexpand(mal, env).bind(mal => {
    switch (mal.type) {
      case "list": {
        if (mal.listType === "list") {
          if (mal.items.length === 0) {
            return ok(mal);
          }
          return apply_nonemptyList(mal, env);
        }
      }
    }
    return eval_ast(mal, env) as ResultS<MalType>;
  })
}

function eval_ast(mal: MalType, env: Env): ResultS<MalType> {
  return matchUnion(mal, {
    symbol: ({ name }) => env.get(name),          //  MalType_fn type is returned
    list: ({ items, listType }) => resultSMapM(items, m => eval_(m, env)).map(evaluatedItems => list(evaluatedItems, listType, nil) as MalType),
    map: (mal) => resultSMapM(Object.entries(mal.map), kv => eval_(kv[1], env).map(expr => ({ k: kv[0], v: expr })))
      .map(evaluatedItems => map(toobject(evaluatedItems, kv => kv.k, kv => kv.v), nil) as MalType),
    _: () => ok(mal) as ResultS<MalType>
  });
}


function apply_nonemptyList(mal: MalType_list, env: Env): ResultS<MalType> {
  const { items: [symbol, ...args] } = mal;

  switch (symbol.type) {
    case "symbol": {
      switch (symbol.name) {
        case "def!": return apply_def(args, env);
        case "defmacro!": return apply_defmacro(args, env);
        case "let*": return apply_let(args, env);
        case "do": return apply_do(args, env);
        case "if": return apply_if(args, env);
        case "fn*": return apply_fn(args, env);
        case "quote": return apply_quote(args, env);
        case "quasiquote": {
          return eval_(transform_quasiquote(args), env);
        }
        case "macroexpand": {
          return macroexpand(args[0], env);
        }
        case "try*": return apply_tryCatch(args, env);
        default: return apply_funcCall(mal, env);
      }
    } default: {
      return apply_funcCall(mal, env);
    }

  }
}

/** (... 1 2 3) */
function apply_funcCall(mal: MalType_list, env: Env): ResultS<MalType> {
  return eval_ast(mal, env).bind(m => {
    const { items: [fn, ...fnArgs] } = m as MalType_list;
    return matchUnion(fn, {
      fn: ({ fn }) => fn(fnArgs),
      _: () => error(`first element in a list should be 'fn' but it is '${fn.type}'`) as ResultS<MalType>
    })
  });
}

/** (def! a 1) */
function apply_def(args: MalType[], env: Env): ResultS<MalType> {
  return validateDefArguments(args).bind(([symbol, value]) => {
    return eval_(value, env).map(m => env.set(symbol.name, m));
  });
}

function validateDefArguments(args: MalType[]): ResultS<[MalType_symbol, MalType]> {
  if (args.length !== 2) {
    return error(`'def!' requires 2 arguments but got '${args.length}'`);
  } else {
    const [key, value] = args;
    if (key.type !== "symbol") {
      return error(`'def!' first argument should be 'symbol' but got '${key.type}'`);
    } else {
      return ok([key, value]);
    }
  }
}

/** (defmacro!  (fn (...) ... ) ) */
function apply_defmacro(args: MalType[], env: Env): ResultS<MalType> {
  return validateDefArguments(args).bind(([symbol, value]) => {
    return eval_(value, env).bind(m =>
      //m.type === "fn" ? (m.is_macro = true, ok(env.set(symbol.name, m)))
      m.type === "fn" ? ok(env.set(symbol.name, fn(m.fn, isMacroMeta)))
        : error(`'defmacro!' second argument should be 'fn' but got '${m.type}'`)
    );
  })
}



/** (let* (a 1) a + 2) */
function apply_let([bindings, expression]: MalType[], env: Env): ResultS<MalType> {
  if (typeof expression === "undefined") { // let* without bindings
    return eval_(bindings, env);
  }
  switch (bindings.type) {
    case "list": {
      return apply_bindings(bindings.items, env).bind(newEnv => eval_(expression, newEnv))
    }
    default: return error(`bindings argument in let* should be a list but it ts '${bindings.type}'`)
  }
}

/** ( a 1 b 3 ) */
function apply_bindings(bindings: MalType[], env: Env): ResultS<Env> {
  if (bindings.length === 0) {
    return ok(env);
  } else if (bindings.length == 1) {
    return error("bindings argument in let* should contain an even number of elements");
  } else {
    const [key, value, ...other] = bindings;
    return eval_(value, env).bind(m => {
      const newEnv = new Env(some(env));
      newEnv.set((key as MalType_symbol).name, m);
      return apply_bindings(other, newEnv);
    });
  }
}


/** (do (...) (...) (....) ) */
function apply_do(args: MalType[], env: Env): ResultS<MalType> {
  if (args.length === 0) {
    return error(`'do' requires at least one argument but got '${args.length}'`);
  } else {
    return resultSMapM(args, item => eval_(item, env)).bind(ms => ok(ms[ms.length - 1]));
    // but precisely according to guide the implementation should look like this
    // return eval_({ type: "list", listType: "vector", items: args }, env)
    //   .bind((aa: MalType) => aa.type === "list" ? ok(aa.items[aa.items.length - 1]) : error("should never be here :) "));
  }
}

/** (if (...) (...) (...) ) */
function apply_if(args: MalType[], env: Env): ResultS<MalType> {
  if (args.length === 1) {
    return error(`'if' requires at least 2 arguments but got '${args.length}'`);
  } else {
    const [condMal, trueMal, falseMal = nil] = args;
    return eval_(condMal, env).bind(condition => {
      switch (condition.type) {
        case "nil":
        case "false_": {
          return eval_(falseMal, env);
        }
        default: {
          return eval_(trueMal, env);
        }
      }
    });
  }
}

/** (fn* (...) ...) */
function apply_fn(args: MalType[], env: Env): ResultS<MalType> {
  if (args.length === 1) {
    return error(`'fn*' requires at least 2 arguments but got '${args.length}'`);
  } else {
    const [fnArgs, fnBody] = args;
    switch (fnArgs.type) {
      case "list": {
        return resultSMapM(fnArgs.items, fnArg => fnArg.type === "symbol" ? ok(fnArg.name)
          : error(`'fn*' argument should be a 'symbol' but it was ${fnArg.type}`)
        )
          .bind(fnArgsNames =>
            ok(fn((fnCallArgs) => {
              // do not override functions arguments, copy them
              let fnArgsNames__ = fnArgsNames, fnCallArgs__ = fnCallArgs;

              // variadic function parameters ( (fn* (& more) (count more)) 1 2 3)
              const ampersandIndex = fnArgsNames.indexOf("&");
              if (ampersandIndex !== -1) {
                fnArgsNames__ = fnArgsNames.filter((_, index) => index !== ampersandIndex); // remove &
                fnCallArgs__ = [...fnCallArgs.slice(0, ampersandIndex), list(fnCallArgs.slice(ampersandIndex), "list", nil)]
              }

              return eval_(fnBody, new Env(some(env), fnArgsNames__, fnCallArgs__));
            }, nil
            ))
          );
      }
      default: {
        return error(`first argument in 'fn*' should be a list of arguments but it was '${fnArgs.type}'`);
      }
    }
  }
}


/** (quote (...) ) */
function apply_quote(args: MalType[], env: Env): ResultS<MalType> {
  const [first]: MalType_list[] = args as any;
  return ok(first);
}


/** (try* ... (catch* ... ...) ) */
function apply_tryCatch(args: MalType[], env: Env): ResultS<MalType> {
  const [tryMal, catchMal]: [MalType, MalType_list] = args as any;

  if (args.length !== 1 && args.length !== 2) {
    return error("'try*' should take 1 or 2 arguments");
  }

  if (args.length === 1) {
    return eval_(tryMal, env);
  }

  if (catchMal.type !== "list" || catchMal.items.length !== 3 ||
    catchMal.items[0].type !== "symbol" || (catchMal.items[0] as MalType_symbol).name !== "catch*" ||
    catchMal.items[1].type !== "symbol") {
    return error("'try*' should look like (try* ... (catch* symbol ...))");
  }

  try {
    return matchUnion(eval_(tryMal, env), {
      "error": ({ error }) => { throw string_(error) },
      "ok": result => result
    });
  } catch (err) {
    const [, errorSymbol, catchBodyMal] = catchMal.items;
    return eval_(catchBodyMal, new Env(some(env), [errorSymbol.name], [err]));
  }
}


/** (quasiquote (...) ) */
function transform_quasiquote(args: MalType[]): MalType {
  const [arg]: MalType_list[] = args as any;

  if (is_pair(arg) === false) { // arg is not a list
    return list([symbol("quote", nil), arg], "list", nil);
  } else {
    const [head, ...tail] = arg.items;
    if (isUnion(head, "symbol") && head.name == "unquote") {
      return tail[0];
    } else if (is_pair(head) && isUnion(head, "list") && isUnion(head.items[0], "symbol") &&
      (head.items[0] as MalType_symbol).name === "splice-unquote") {
      return list([symbol("concat", nil), head.items[1], transform_quasiquote([list(tail, "list", nil)])], "list", nil);
    } else {
      return list([symbol("cons", nil), transform_quasiquote([head]), transform_quasiquote([list(tail, "list", nil)])], "list", nil);
    }
  }
}




/** returns true if mal type is a list with at least one element inside */
function is_pair(mal: MalType) {
  return mal.type === "list" && mal.items.length > 0 ? true : false;
}


/** returns true if mal type is a macro call  */
function is_macro_call(mal: MalType, env: Env) {
  let firstSymbol: MalType;
  let fnMal: ResultS<MalType>;

  if (mal.type === "list" && mal.items.length > 0 &&
    (firstSymbol = mal.items[0]) && (firstSymbol.type === "symbol") &&
    (fnMal = env.get(firstSymbol.name)) && fnMal.type === "ok" && fnMal.value.type === "fn" && malEqual(fnMal.value.meta, isMacroMeta)) {
    return true;
  }
  return false;
}

function macroexpand(mal: MalType, env: Env): ResultS<MalType> {
  if (is_macro_call(mal, env)) {
    const [head, ...rest] = (mal as MalType_list).items;
    const fn = (env.get((head as MalType_symbol).name) as Result_ok<MalType>).value as MalType_fn;
    return fn.fn(rest).bind(result => macroexpand(result, env));
  } else {
    return ok(mal);
  }
}
