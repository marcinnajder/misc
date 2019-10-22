import "./useDevPowerfp";
import * as repl from "repl";
import { Context } from "vm";
import { ResultS } from "powerfp";
import { step1_read_print, step2_eval, StepFunc } from "./steps";
import { defaultEnv, Env } from "./env";
import { __printLine, ns } from "./core";
import * as os from "os";
import { PrintLineType, pr_str } from "./printer";
import { fn, MalType, list } from "./types";
import { eval_ } from "./eval";
import { string_ } from "./adt.generated";

const [nodeProcessPath, indexFilePath, malScriptPath, ...argv] = process.argv


const defaultEnv_ = defaultEnv(ns);
const step: StepFunc = step2_eval;
initEnv(step, defaultEnv_);
type CallbackType = (err: Error | null, result?: any) => void;

if (malScriptPath) {      // execute script file
  executeScript(malScriptPath);
} else {                  // execute repl
  repl.start({ prompt: '> ', eval: executeRepl }); // https://nodejs.org/api/repl.html
}



function executeScript(scriptPath: string) {
  ((ns as any)[__printLine] as PrintLineType) = (...s) => {
    console.log(...s);
  }

  print(step(`(load-file "${scriptPath}")${os.EOL}`, defaultEnv_), (err, text) => {
    if (err) {
      console.error(err);
    } else {
      console.log(text);
    }
  });
}

function executeRepl(this: repl.REPLServer, evalCmd: string, context: Context, file: string, cb: CallbackType) {
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
  // add *ARGV* before anything else
  e.set("*ARGV*", list(malScriptPath ? argv.map(s => string_(s)) : [], "list"))

  e.set("eval", fn(([ast]: MalType[]) => eval_(ast, e)));

  s(`(def! not (fn* (a) (if a false true)))`, e);
  s(`(def! load-file (fn* (f) (eval (read-string (str "(do " (slurp f) "\nnil)")))))`, e);
}


function print(result: ResultS<unknown>, cb: CallbackType) {
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

