import { MalType, MalType_list, MalType_symbol, MalType_fn } from "./types";
import { ResultS, error, ok } from "./utils/result";
import { mapRS, liftRS } from "./utils/monadicFunctions";
import { Env } from "./env";
import { some } from "./utils/option";
import { stringify } from "querystring";

export function eval_(mal: MalType, env: Env): ResultS<MalType> {
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
}

function eval_ast(mal: MalType, env: Env): ResultS<MalType> {
  switch (mal.type) {
    case "symbol": {
      return env.get(mal.name); // tutaj zwracany jest MalType_fn
    }
    case "list": {
      // works for list, vector, hash
      const itemsValues = mapRS(mal.items, m => eval_(m, env));
      const mapItems = (items: MalType[]) => ({ ...mal, items: items } as MalType);
      return liftRS(mapItems)(itemsValues);
    }
    default: {
      return ok(mal);
    }
  }
}


function apply_nonemptyList(mal: MalType_list, env: Env): ResultS<MalType> {
  const { items: [symbol, ...args] } = mal;

  switch (symbol.type) {
    case "symbol": {
      switch (symbol.name) {
        case "def!": return apply_def(args, env);
        case "let*": return apply_let(args, env);
        case "do": return apply_do(args, env);
        case "if": return apply_if(args, env);
        case "fn*": return apply_fn(args, env);
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
    switch (fn.type) {
      case "fn": {
        return fn.fn(fnArgs);
      }
      default: return error(`first element in a list should be 'fn' but it is '${fn.type}'`);
    }
  });
}

/** (def! a 1) */
function apply_def(args: MalType[], env: Env): ResultS<MalType> {
  if (args.length !== 2) {
    return error(`'def!' requires 2 arguments but got '${args.length}'`);
  } else {
    const [key, value] = args;
    switch (key.type) {
      case "symbol": return eval_(value, env).bind(m => ok(env.set(key.name, m)));
      default: return error(`'def!' first argument should be 'symbol' but got '${key.type}'`);
    }
  }
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
    ``
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
    return mapRS(args, item => eval_(item, env)).bind(ms => ok(ms[ms.length - 1]));
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
    const [condMal, trueMal, falseMal = { type: "nil" } as MalType] = args;
    return eval_(condMal, env).bind(condition => {
      switch (condition.type) {
        case "nil":
        case "false": {
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



        return mapRS(fnArgs.items, fnArg => fnArg.type === "symbol" ? ok(fnArg.name)
          : error<string, string>(`'fn*' argument should be a 'symbol' but it was ${fnArg.type}`)
        )
          .bind(fnArgsNames =>
            ok({
              type: "fn",
              fn: (fnCallArgs) => {
                // variadic function parameters ( (fn* (& more) (count more)) 1 2 3)
                const ampersandIndex = fnArgsNames.indexOf("&");
                if (ampersandIndex !== -1) {
                  fnArgsNames = fnArgsNames.filter((_, index) => index !== ampersandIndex); // remove &
                  fnCallArgs = [...fnCallArgs.slice(0, ampersandIndex), { type: "list", listType: "list", items: fnCallArgs.slice(ampersandIndex) }]
                }
                return eval_(fnBody, new Env(some(env), fnArgsNames, fnCallArgs));
              }
            } as MalType_fn)
          );
      }
      default: {
        return error(`first argument in 'fn*' should be a list of arguments but it was '${fnArgs.type}'`);
      }
    }
  }
}


