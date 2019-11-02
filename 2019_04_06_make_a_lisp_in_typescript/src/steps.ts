import { read_str } from "./reader";
import { pr_str } from "./printer";
import { ResultS, error, ok, matchUnion } from "powerfp";
import { assertNever } from "./utils/common";
import { eval_, } from "./eval";
import { Env } from "./env";

export type StepFunc = (text: string, env: Env) => ResultS<string>;

export function step0_repl(text: string): ResultS<string> {
  return ok(text);
}

export function step1_read_print(text: string): ResultS<string> {
  return read_str(text).map(expressionO => matchUnion(expressionO, {
    "none": () => "<empty>",
    "some": ({ value: expression }) => pr_str(expression, true)
  }));
}

export function step2_eval(text: string, env: Env): ResultS<string> {
  return read_str(text).bind(expressionO => matchUnion(expressionO, {
    "none": () => ok("<empty>"),
    "some": ({ value: expression }) => eval_(expression, env).map(value => pr_str(value, true))
  }));
}
