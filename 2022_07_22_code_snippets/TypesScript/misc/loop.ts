//https://clojuredocs.org/clojure.core/loop
//var { } = require("powerseq");
import { pipe } from "powerseq";

// https://clojuredocs.org/clojure.core/loop
// https://pl.wikipedia.org/wiki/Clojure

// import { promiseBind, promiseMap } from "powerfp";

// function inc(n: number) {
//   return n + 1;
// }
// async function getUserNameById(id: number) {
//   return "admin";
// }
// function toUpperCase(text: string) {
//   return text.toUpperCase();
// }
// async function saveUserName(name: string) {
//   return true;
// }

// const result = pipe(10, inc, getUserNameById, promiseMap(toUpperCase), promiseBind(saveUserName));
// console.log(result);


function loop_<S, R>(state: S, body: (state: S) => R): R {
  return body(state);
}

const loop = pipe;

// https://clojuredocs.org/clojure.core/loop#example-542692d4c026201cdc326ff6
// (loop [x 10]
//   (when (> x 1)
//     (println x)
//     (recur (- x 2))))
// ;;=> 10 8 6 4 2

function evenFrom10to2() {
  loop(10, function recur(x) {
    if (x > 1) {
      console.log(x);
      recur(x - 2);
    }
  }); // -> void
}



// https://clojuredocs.org/clojure.core/loop#example-55a33ad7e4b020189d740551
// ;; sum from 1 to 10
// (loop [iter 1
//        acc  0]
//   (if (> iter 10)
//     (println acc)
//     (recur (inc iter) (+ acc iter))))
// ;; => 55


function sumFrom1to10() {
  loop({ iter: 1, acc: 0 }, function recur({ iter, acc }) {
    if (iter > 10) {
      console.log(acc);
    } else {
      recur({ iter: iter + 1, acc: acc + iter });
    }
  }); // -> void
}


// https://clojuredocs.org/clojure.core/loop#example-57404d05e4b0a1a06bdee497
// ;; calculate the factorial of n
// (loop [n (bigint 5), accumulator 1]
//   (if (zero? n)
//     accumulator  ; we're done
//     (recur (dec n) (* accumulator n))))
// ;;=> 120N
//

function factorialOf5() {
  const result = loop({ n: 5, accumulator: 1 }, function recur({ n, accumulator }) {
    if (n === 0) {
      return accumulator;
    } else {
      return recur({ n: n - 1, accumulator: accumulator * n });
    }
  });

  console.log(result);
}

function factorialOf5__() {
  const result = loop([5, 1] as [number, number], function recur([n, accumulator]) {
    if (n === 0) {
      return accumulator;
    } else {
      return recur([n - 1, accumulator * n]);
    }
  });

  console.log(result);
}

function factorialOf5___() {

  function factorialOfN(n: number) {
    function loop(n: number, accumulator: number) {
      if (n === 0) {
        return accumulator;
      } else {
        return loop(n - 1, accumulator * n);
      }
    }

    return loop(n, 1);
  }

  const result = factorialOfN(5);
  console.log(result); // 120
}



// ;; square each number in the vector
// (loop [xs (seq (+ 1 2 3 4 5))
//        result []]
//   (if xs
//     (let [x (first xs)]
//       (recur (next xs) (conj result (* x x))))
//     result))
// ;; => [1 4 9 16 25]

type List<T> = null | readonly [head: T, tail: List<T>];

function squareEachNumberInTheList() {
  const result = loop({
    xs: [1, [2, [3, [4, [5, null]]]]] as List<number>,
    result: null as List<number>
  }, function recur({ xs, result }) {
    if (xs !== null) {
      const [x, rest] = xs;
      return recur({ xs: rest, result: [x * x, result] });
    } else {
      return result;
    }
  });

  console.log(result);
}


// evenFrom10to2();
// sumFrom1to10();
factorialOf5__();
factorialOf5___();
// factorialOf5();

//squareEachNumberInTheList();
