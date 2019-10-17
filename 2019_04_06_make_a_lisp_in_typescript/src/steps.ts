import { read_str } from "./reader";
import { pr_str } from "./printer";
import { ResultS, error, ok } from "powerfp";
import { assertNever } from "./utils/common";
import { eval_, } from "./eval";
import { Env } from "./env";

export type StepFunc = (text: string, env: Env) => ResultS<string>;

export function step0_repl(text: string): ResultS<string> {
  return ok(text);
}


// function abc(a: ResultS<number>): number {
//   switch (a.type) {
//     case "error": return 1;
//     case "ok": return 1;
//   }
// }

// function abc2(b: Option<number>): number {
//   switch (b.type) {
//     case "none": return 1;
//     case "some": return 1;
//   }
// }

// function abc3(a: Option<number>, b: Option<number>): number {
//   switch (a.type) {
//     case "none": return 1;
//     case "some": {
//       switch (b.type) {
//         case "none": return 1;
//         case "some": return 1;
//       }
//       // return 123;
//     }
//   }
// }



export function step1_read_print(text: string): ResultS<string> {
  const expressionE = read_str(text);

  switch (expressionE.type) {
    case "error": return error(expressionE.error);
    case "ok": {
      const expressionO = expressionE.value;
      switch (expressionO.type) {
        case "none": return ok("<empty>");
        case "some": {
          const s = pr_str(expressionO.value, true);
          return ok(s);
        }
        default: { return assertNever(expressionO); }
      }
    }
  }
}

export function step2_eval(text: string, env: Env): ResultS<string> {
  return read_str(text).bind(expressionO => {
    switch (expressionO.type) {
      case "none": return ok("<empty>");
      case "some": {
        return eval_(expressionO.value, env).bind(value => ok(pr_str(value, true)));
      }
    }
  });
}

export const step3_env = step2_eval;
export const step4_if_fn_do = step2_eval;

