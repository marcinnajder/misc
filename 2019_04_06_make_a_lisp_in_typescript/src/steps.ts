import { read_str } from "./reader";
import { pr_str } from "./printer";
import { ResultS, error, ok } from "./utils/result";
import { throwHere } from "./utils/common";
import { eval_, envNumbers, } from "./eval";

export function step0_repl(text: string): ResultS<string> {
  return ok(text);
}

export function step1_read_print(text: string): ResultS<string> {
  const expressionE = read_str(text);
  switch (expressionE.type) {
    case "error": return error(expressionE.error);
    case "ok": {
      const expressionO = expressionE.value;
      switch (expressionO.type) {
        case "none": return ok("<empty>");
        case "some": {
          const s = pr_str(expressionO.value);
          return ok(s);
        }
      }
    }
  }
  return throwHere();
}


export function step2_eval(text: string): ResultS<string> {
  return read_str(text).bind(expressionO => {
    switch (expressionO.type) {
      case "none": return ok("<empty>");
      case "some": {
        return eval_(expressionO.value, envNumbers).bind(value => ok(pr_str(value)));
      }
    }
  });
}
