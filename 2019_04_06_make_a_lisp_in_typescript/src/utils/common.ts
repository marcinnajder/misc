import { some, none, Option } from "./option";
import { MalType } from "../types";
import { ResultS, error, ok } from "./result";




export function tryParseNumber(text: string): Option<number> {
  const n = Number(text);
  return Number.isNaN(n) ? none() : some(n);
}


export function parseMalType(text: string): ResultS<MalType> {
  const n = Number(text);
  if (!Number.isNaN(n)) {
    return ok({ type: "number", value: n });
  }
  if (text === "true") {
    return ok({ type: "true" });
  }
  if (text === "false") {
    return ok({ type: "false" });
  }
  if (text === "nil") {
    return ok({ type: "nil" });
  }
  if (text[0] === '"') {
    if (text[text.length - 1] === '"') {
      return ok({ type: "string", value: text.slice(1, text.length - 1).replace(/\\(.)/g, function (_, c) { return c === "n" ? "\n" : c }) });
    }
    return error(`String value '${text}' in not closed`);
  }
  if (text[0] === ":") {
    return ok({ type: "keyword", name: text.substr(1) });
  }

  return ok({ type: "symbol", name: text });
}


export function assertNever(x: never): never {
  throw new Error("Unexpected object: " + x);
}


function guard<T extends { type: string }, TT extends T["type"]>(x: T, type: TT): x is Extract<T, { type: TT }> {
  return x["type"] === type;
}

function cast<T extends { type: string }, TT extends T["type"]>(x: T, type: TT): Extract<T, { type: TT }> {
  return x as Extract<T, { type: TT }>;
}
