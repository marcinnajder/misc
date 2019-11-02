import { ResultS, error, ok } from "powerfp";
import { MalType, true_, false_, nil, number_, string_, keyword, symbol } from "../types";


export function parseMalType(text: string): ResultS<MalType> {
  const n = Number(text);
  if (!Number.isNaN(n)) {
    return ok(number_(n));
  }
  if (text === "true") {
    return ok(true_);
  }
  if (text === "false") {
    return ok(false_);
  }
  if (text === "nil") {
    return ok(nil);
  }
  if (text[0] === '"') {
    if (text[text.length - 1] === '"') {
      return ok(string_(text.slice(1, text.length - 1).replace(/\\(.)/g, function (_, c) { return c === "n" ? "\n" : c })));
    }
    return error(`String value '${text}' in not closed`);
  }
  if (text[0] === ":") {
    return ok(keyword(text.substr(1)));
  }
  return ok(symbol(text, nil));
}


export function assertNever(x: never): never {
  throw new Error("Unexpected object: " + x);
}


// function guard<T extends { type: string }, TT extends T["type"]>(x: T, type: TT): x is Extract<T, { type: TT }> {
//   return x["type"] === type;
// }

// function cast<T extends { type: string }, TT extends T["type"]>(x: T, type: TT): Extract<T, { type: TT }> {
//   return x as Extract<T, { type: TT }>;
// }
