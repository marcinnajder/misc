import { apply, valid, Op, eval_, Expr, choices, values, solution, split, combine, ExprType, exprs, expressionToString, solutions, solutions2, valid2 } from "./choices";
import * as assert from "assert";
import { find, filter, orderby, zip, sequenceequal, take } from "powerseq";


setTimeout(runAllTests, 0);

function runAllTests() {
  const testsObject = module.exports;
  const tests = Object.keys(testsObject);
  console.log("Tests:", tests.length);

  for (const test of tests) {
    console.log(" ->", test);
    try {
      testsObject[test]();
      console.log(" Success");
    } catch (err) {
      console.log(" Failed: ", err);
    }
  }
}

export function applyTests() {
  assert.equal(apply(Op.Add, 20, 5), 20 + 5);
  assert.equal(apply(Op.Sub, 20, 5), 20 - 5);
  assert.equal(apply(Op.Mul, 20, 5), 20 * 5);
  assert.equal(apply(Op.Div, 20, 5), 20 / 5);
}

export function validTests() {
  assert.equal(valid(Op.Add, 20, 5), true);

  assert.equal(valid(Op.Sub, 20, 5), true);
  assert.equal(valid(Op.Sub, 20, 20), false);
  assert.equal(valid(Op.Sub, 20, 21), false);

  assert.equal(valid(Op.Mul, 20, 5), true);

  assert.equal(valid(Op.Div, 20, 5), true);
  assert.equal(valid(Op.Div, 20, 3), false);
}

export function evalTests() {
  assert.deepEqual([...eval_(Expr.Val(10))], [10]);
  assert.deepEqual([...eval_(Expr.Val(0))], []);

  assert.deepEqual([...eval_(Expr.App(Op.Add, Expr.Val(0), Expr.Val(10)))], []);
  assert.deepEqual([...eval_(Expr.App(Op.Add, Expr.Val(10), Expr.Val(0)))], []);

  assert.deepEqual([...eval_(Expr.App(Op.Sub, Expr.Val(10), Expr.Val(10)))], []);
  assert.deepEqual([...eval_(Expr.App(Op.Sub, Expr.Val(20), Expr.Val(10)))], [10]);
}

export function choicesTests() {
  assert.equal(equalsArrayOfArrays(
    [...choices([1, 2])],
    [[], [1], [2], [1, 2], [2, 1]]), true);
}

export function valuesTests() {
  assert.deepEqual([...values(Expr.Val(10))], [10]);
  assert.deepEqual([...values(Expr.App(Op.Add, Expr.Val(20), Expr.Val(10)))], [20, 10]);
}

export function solutionTests() {
  assert.equal(solution(Expr.Val(3), [1, 3], 4), false);
  assert.equal(solution(Expr.Val(3), [1, 2], 3), false);
  assert.equal(solution(Expr.Val(3), [1, 3], 3), true);
  assert.equal(solution(Expr.App(Op.Add, Expr.Val(20), Expr.Val(10)), [10, 20, 30], 30), true);
}

export function skipTests() {
  assert.deepEqual([...split([1])], []);
  assert.deepEqual([...split([1, 2])], [[[1], [2]]]);
  assert.deepEqual([...split([1, 2, 3])], [[[1], [2, 3]], [[1, 2], [3]]]);
}

export function combineTests() {
  assert.deepEqual(
    [...combine(Expr.Val(3), Expr.Val(4))],
    [...[Op.Add, Op.Div, Op.Mul, Op.Sub].map(op => ({ type: "App", op, l: Expr.Val(3), r: Expr.Val(4) }) as ExprType)]
  )
}

export function exprsTests() {
  assert.deepEqual([...exprs([])], []);
  assert.deepEqual([...exprs([4])], [Expr.Val(4)]);
  assert.deepEqual([...exprs([1, 2])].length, 4);

  // const expressions2 = [...exprs([1, 2, 3])];
  // for (const e of expressions2) {
  //   console.log(expressionToString(e));
  // }
}

export function solutionsTests() {
  // 780 rezultatow
  // solutions: 318121.019ms (5,3 min)  
  //   (generuje wszystkie mozliwe wyrazenie i dopiero liczy wynik)
  measure("solutions", () => {
    const expressions = solutions([1, 3, 7, 10, 25, 50], 765);
    printNExpressions(expressions, Number.MAX_VALUE);
  });

  // 780 rezultatow
  // text: 20227.183ms
  // (generujac wyrazenia od razu liczy i sprawdza poprawnosc)
  // measure("solutions2", () => {
  //   const expressions = solutions2([1, 3, 7, 10, 25, 50], 765);
  //   printNExpressions(expressions, Number.MAX_VALUE);
  // });

  // 49 rezultatow
  // solutions2 + valid2: 4323.602ms
  // (to co wczesniej ale jest bardziej rygorystyczny jesli chodzi o poprawnosc)
  // measure("solutions2 + valid2", () => {
  //   const expressions = solutions2([1, 3, 7, 10, 25, 50], 765, valid2);
  //   printNExpressions(expressions, Number.MAX_VALUE);
  // });

  function printNExpressions(expressions: Iterable<ExprType>, n: number) {
    let index = 1;
    for (const e of take(expressions, n)) {
      console.log(index++, expressionToString(e));
    }
  }
}





function measure(text: string, action: () => void) {
  console.time(text);
  action();
  console.timeEnd(text);
}




// -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- 

function pipe(value: any, ...fs: Function[]) {
  return fs.reduce((p, cf) => cf(p), value);
}

function inc(a: number) {
  return a + 1;
}

function toString<T>(a: T) {
  return a.toString();
}

function equalsArrayOfArrays<T>(a: T[][], b: T[][]) {
  return sequenceequal(normalizeAndSort(a), normalizeAndSort(b));

  function normalizeAndSort(x: T[][]) {
    return pipe(x.map(xx => xx.join()), sort);
  }
}

function sort<T>(items: T[][]) {
  return items.sort((a, b) => a.length !== b.length
    ? (a.length - b.length)
    : find(zip(a, b, compareArrays), r => r !== 0) || 0
  );
  function compareArrays(a: any[], b: any[]) {
    return a === b ? 0 : (a > b ? 1 : -1)
  }
}

