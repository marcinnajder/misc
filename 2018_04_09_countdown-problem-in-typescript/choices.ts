// https://channel9.msdn.com/Series/C9-Lectures-Erik-Meijer-Functional-Programming-Fundamentals/C9-Lectures-Dr-Graham-Hutton-Functional-Programming-Fundamentals-Chapter-11-of-13

import { find, filter, orderby, zip, sequenceequal, some, take, skip } from "powerseq";
import assert from "assert";
import util from "util";
import { exprsTests } from "./tests";

export enum Op { Add, Sub, Mul, Div };

export function apply(op: Op, x: number, y: number) {
  switch (op) {
    case Op.Add: return x + y;
    case Op.Div: return x / y;
    case Op.Mul: return x * y;
    case Op.Sub: return x - y;
  }
}

export function valid(op: Op, x: number, y: number) {
  switch (op) {
    case Op.Add: return true;
    case Op.Sub: return x > y;
    case Op.Mul: return true;
    case Op.Div: return x % y === 0;
  }
}

export type ExprType =
  { type: "Val", value: number } |
  { type: "App", op: Op, l: ExprType, r: ExprType };



export const Expr = {
  Val(value: number): ExprType {
    return { type: "Val", value };
  },
  App(op: Op, l: ExprType, r: ExprType): ExprType {
    return { type: "App", op, l, r };
  },
};

export function* eval_(expr: ExprType): IterableIterator<number> {
  switch (expr.type) {
    case "Val": {
      if (expr.value > 0) {
        yield expr.value;
      }
      break;
    };
    case "App": {
      for (const x of eval_(expr.l)) {
        for (const y of eval_(expr.r)) {
          if (valid(expr.op, x, y)) {
            yield apply(expr.op, x, y);
          } //  else printValidity(expr);
        }
      }
    }
  }
}



export function* choices3<T>(items: T[]) {
  yield [];
  for (var a of items) {
    yield [a];
    for (var b of items.filter(x => x !== a)) {
      yield [a, b];
      for (var c of items.filter(x => x !== a).filter(x => x !== b)) {
        yield [a, b, c];
      }
    }
  }
}

export function* choices<T>(items: T[]) {
  yield [];
  yield* loop([], items);
  function* loop(abc: T[], filtered: T[]): IterableIterator<T[]> {
    for (var a of filtered) {
      const abc_ = [...abc, a];
      yield abc_;
      yield* loop(abc_, filtered.filter(x => x !== a));
    }
  }
}

export function* values(expr: ExprType): IterableIterator<number> {
  switch (expr.type) {
    case "Val": {
      yield expr.value;
      break;
    }
    case "App": {
      yield* values(expr.l);
      yield* values(expr.r);
    }
  }
}

export function solution(expr: ExprType, init: number[], result: number) {
  return deepEqual([...eval_(expr)], [result]) && element([...values(expr)], choices(init));
}



export function* split(numbers: number[]) {
  for (let i = 1; i < numbers.length; i++) {
    yield [[...take(numbers, i)], [...skip(numbers, i)]];
  }
}

export function* combine(l: ExprType, r: ExprType) {
  for (const op of [Op.Add, Op.Div, Op.Mul, Op.Sub]) {
    yield { type: "App", op, l, r } as ExprType;
  }
}


export function* exprs(numbers: number[]): IterableIterator<ExprType> {
  switch (numbers.length) {
    case 0: {
      break;
    };
    case 1: {
      yield { type: "Val", value: numbers[0] };
      break;
    };
    default: {
      for (const [ls, rs] of split(numbers)) {
        for (const l of exprs(ls)) {
          for (const r of exprs(rs)) {
            yield* combine(l, r);
          }
        }
      }
    }
  }
}

export function* solutions(numbers: number[], result: number) {
  for (const choice of choices(numbers)) {
    for (const e of exprs(choice)) {
      const exprResult = [...eval_(e)];
      if (exprResult.length === 1 && exprResult[0] === result) {
        yield e;
      }
    }
  }
}

type Result = [ExprType, number];

export function* results(numbers: number[], validFunc = valid): IterableIterator<Result> {
  switch (numbers.length) {
    case 0: {
      break;
    };
    case 1: {
      const n = numbers[0];
      if (n > 0) {
        yield [{ type: "Val", value: numbers[0] }, n];
      }
      break;
    };
    default: {
      for (const [ls, rs] of split(numbers)) {
        for (const l of results(ls, validFunc)) {
          for (const r of results(rs, validFunc)) {
            yield* combineResults(l, r, validFunc);
          }
        }
      }
    }
  }
}


export function* combineResults([l, x]: Result, [r, y]: Result, validFunc = valid): IterableIterator<Result> {
  for (const op of [Op.Add, Op.Sub, Op.Mul, Op.Div]) {
    if (validFunc(op, x, y)) {
      yield [{ type: "App", op, l, r }, apply(op, x, y)];
    }
  }
}

export function* solutions2(numbers: number[], result: number, validFunc = valid) {
  for (const choice of choices(numbers)) {
    for (const [e, n] of results(choice, validFunc)) {
      if (n === result) {
        yield e;
      }
    }
  }
}


export function valid2(op: Op, x: number, y: number) {
  switch (op) {
    case Op.Add: return x <= y;
    case Op.Sub: return x > y;
    case Op.Mul: return x <= y && x !== 1 && y !== 1;
    case Op.Div: return x % y === 0 && y !== 1;
  }
}

export function expressionToString(expr: ExprType): string {
  switch (expr.type) {
    case "Val": {
      return expr.value.toString();
    }
    case "App": {
      return `(${expressionToString(expr.l)} ${operatorToString(expr.op)} ${expressionToString(expr.r)})`;
    }
  }
}

export function operatorToString(op: Op) {
  switch (op) {
    case Op.Add: return "+";
    case Op.Div: return "/";
    case Op.Mul: return "*";
    case Op.Sub: return "-";
  }
}



// -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
function element<T>(element: T, items: Iterable<T>) {
  return some(items, item => deepEqual(item, element))
}

function deepEqual<T>(a: T, b: T) {
  try {
    assert.deepEqual(a, b);
    return true;
  }
  catch (err) {
    return false;
  }
}

function printValidity(expr: ExprType): [boolean, number] {
  switch (expr.type) {
    case "Val": {
      return [true, expr.value];
    }
    case "App": {
      const [lValid, lResult] = printValidity(expr.l);
      const [rValid, rResult] = printValidity(expr.r);
      if (lValid && rValid) {
        const isValid = valid(expr.op, lResult, rResult);
        if (!isValid) {
          console.log("INVALID", expressionToString(expr))
        }
        return [isValid, apply(expr.op, lResult, rResult)];
      }

      return [false, 0];
    }
  }
}

