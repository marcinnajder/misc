
import * as repl from "repl";
import { Context } from "vm";
import { ResultS } from "./utils/result";
import { step1_read_print, step2_eval, StepFunc } from "./steps";
import { defaultEnv, Env } from "./env";
import { __printLine, ns } from "./core";
import * as os from "os";
import { PrintLineType, pr_str } from "./printer";

const defaultEnv_ = defaultEnv(ns);
const step: StepFunc = step2_eval;
initEnv(step, defaultEnv_);
type CallbackType = (err: Error | null, result?: any) => void;
repl.start({ prompt: '> ', eval: myEval }); // https://nodejs.org/api/repl.html




function myEval(this: repl.REPLServer, evalCmd: string, context: Context, file: string, cb: CallbackType) {

  // override "__printLine" method printing to console 
  ((ns as any)[__printLine] as PrintLineType) = (...s) => {
    console.log(...s);

    s.forEach(ss => this.outputStream.write(ss));
    this.outputStream.write(os.EOL);
  }


  //((ns as any)[__printLine] as PrintLineType) = s => console.log(s));

  print(step(evalCmd, defaultEnv_), cb);
}

export function initEnv(s: StepFunc, e: Env) {
  s("(def! not (fn* (a) (if a false true)))", e);
}




function print(result: ResultS, cb: CallbackType) {
  switch (result.type) {
    case "error": {
      cb(Error(result.error));
      break;
    }
    case "ok": {
      cb(null, result.value);
      break;
    }
  }
}

