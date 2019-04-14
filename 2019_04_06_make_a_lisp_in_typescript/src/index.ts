
import * as repl from "repl";
import { Context } from "vm";
import { ResultS } from "./utils/result";
import { step1_read_print, step2_eval } from "./steps";


// https://nodejs.org/api/repl.html
repl.start({ prompt: '> ', eval: myEval });

type CallbackType = (err: Error | null, result?: any) => void;

function myEval(this: repl.REPLServer, evalCmd: string, context: Context, file: string, cb: CallbackType) {
  // const t = tokenize(evalCmd);
  // console.log(t);

  // print(step1_read_print(evalCmd), cb);
  print(step2_eval(evalCmd), cb);
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

