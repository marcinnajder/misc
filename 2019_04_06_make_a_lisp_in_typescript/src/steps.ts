import { read_str } from "./reader";
import { pr_str } from "./printer";
import { ResultS, error, ok, matchUnion } from "powerfp";
import { eval_, } from "./eval";
import { Env } from "./env";
import { inspect } from "util";

export type StepFunc = (text: string, env: Env) => ResultS<string>;

export function step_repl(text: string): ResultS<string> {
  return ok(text);
}

export function step_read_print(text: string): ResultS<string> {
  return read_str(text).map(expressionO => matchUnion(expressionO, {
    none: () => "<empty>",
    some: ({ value: expression }) => pr_str(expression, true)
  }));
}

export function step_eval(text: string, env: Env): ResultS<string> {
  return read_str(text).bind(expressionO => matchUnion(expressionO, {
    none: () => ok("<empty>"),
    some: ({ value: expression }) => eval_(expression, env).map(value => pr_str(value, true))
  }));
}


export function step_ast(text: string, env: Env): ResultS<string> {
  return read_str(text).bind(expressionO => matchUnion(expressionO, {
    none: () => ok("<empty>"),
    some: ({ value: expression }) => {
      expression = JSON.parse(JSON.stringify(expression, (key: string, value: any) => key === "meta" ? undefined : value));
      console.log(inspect(expression, false, 10000));
      return ok("");
    }
  }));
}

// export function step_Graphviz(text: string, env: Env): ResultS<string> {
//   return read_str(text).bind(expressionO => matchUnion(expressionO, {
//     none: () => ok("<empty>"),
//     some: ({ value: expression }) => {
//       return ok(printGraphviz(expression));
//     }
//   }));
// }

