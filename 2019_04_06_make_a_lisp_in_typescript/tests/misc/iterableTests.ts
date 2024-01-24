
import * as assert from "assert";

// functions
type F1<TR> = () => TR;
type F2<T1, TR> = (a: T1) => TR;
type F3<T1, T2, TR> = (a: T1, b: T2) => TR;

// entry
type E_n<T> = { type: "n", value: T };
type E<T> = E_n<T> | { type: "c" } | { type: "e", error: any };
const c = <T>() => ({ type: "c" }) as E<T>;
const e = <T>(error: any) => ({ type: "e", error }) as E<T>;
const n = <T>(value: T) => ({ type: "n", value }) as E<T>;
function bindE<T1, T2>(e: E<T1>, f: F2<T1, E<T2>>): E<T2> {
  return e.type === "n" ? f(e.value) : e;
}
function mapE<T1, T2>(e: E<T1>, f: F2<T1, T2>): E<T2> {
  return e.type === "n" ? n(f(e.value)) : e;
}
// iterable
type I<T> = F1<F1<E<T>>>;

// observable
type O<T> = F2<F2<E<T>, void>, void>;




function range(start: number, count: number): I<number> {
  return () => {
    let index = start;
    let end = start + count;
    return () => {
      return index >= end ? c() : n(index++);
    }
  }
}
function foreach<T>(i: I<T>, f: F2<T, void>): void {
  const ir = i();
  let e: E<T>;
  while ((e = ir()).type === "n") {
    f((e as E_n<T>).value);
  }
}
function from<T>(a: T[]): I<T> {
  return () => {
    let index = 0;
    return () => {
      return index >= a.length ? c() : n(a[index++]);
    }
  }
}
function toarray<T>(i: I<T>): T[] {
  const result: T[] = [];
  foreach(i, x => result.push(x));
  return result;
}
function map<T1, T2>(i: I<T1>, f: F2<T1, T2>): I<T2> {
  return () => {
    const ir = i();
    return () => {
      return mapE(ir(), f);
    }
  }
}
function filter<T>(i: I<T>, f: F2<T, boolean>): I<T> {
  return () => {
    const ir = i();
    return () => {
      return (function loop(): E<T> {
        return bindE(ir(), v => f(v) ? n(v) : loop());
      })();
    }
  }
}
function reduce<T, A>(i: I<T>, f: F3<A, T, A>, a: A): A {
  let result = a;
  foreach(i, x => result = f(result, x));
  return result;
}





function rangeO(start: number, count: number): O<number> {
  return or => {
    let end = start + count;
    for (var i = start; i < end; i++) {
      or(n(i));
    }
    or(c());
  }
}
function foreachO<T>(o: O<T>, f: F2<T, void>): void {
  o(e => {
    if (e.type === "n") {
      f(e.value);
    }
  });
}
function fromO<T>(a: T[]): O<T> {
  return or => {
    for (const i of a) {
      or(n(i));
    }
    or(c());
  }
}
function mapO<T1, T2>(o: O<T1>, f: F2<T1, T2>): O<T2> {
  return or => {
    o(e => {
      or(mapE(e, f));
    })
  }
}
function filterO<T>(o: O<T>, f: F2<T, boolean>): O<T> {
  return or => {
    o(e => {
      if ((e.type !== "n") || (e.type === "n" && f(e.value))) {
        or(e)
      }
    })
  }
}
function reduceO<T, A>(o: O<T>, f: F3<A, T, A>, a: A): O<A> {
  return (or) => {
    let result = a;
    o(e => {
      switch (e.type) {
        case "e": { or(e); break; }
        case "c": { or(n(result)); or(c()); break; }
        case "n": {
          result = f(result, e.value);
        }
      }
    });
  };
}

function timeoutO(timeout: number): O<undefined> {
  return (or) => {
    setTimeout(() => {
      or(n(undefined));
      or(c());
    }, timeout);
  };
}

function intervalO(timeout: number): O<number> {
  return (or) => {
    let index = 0;
    setInterval(() => {
      or(n(index++));
    }, timeout);
  };
}



// tests



// var r = pipe(
//   range(1, 10),
//   _ => filter(_, x => x % 2 === 0),
//   _ => map(_, x => x * 10),
//   toarray
// );
// console.log(r);


var o = pipe(
  //fromO([1, 2, 3, 4])
  // rangeO(1, 5),
  // _ => filterO(_, x => x % 2 === 0),
  // _ => mapO(_, x => x * 10),
  // _ => reduceO(_, (p, c) => p + c, 10)
  // _ => foreachO(_, x => console.log(x))

  intervalO(1000),
  _ => mapO(_, x => x * 10),
);
o(console.log);



//tests();
// function consoleObserver<T>(e:E<T>){
//   console.oo
// }

function tests() {

  const testsObj = {
    reduce() {
      assert.equal(reduce(from([]), (p, c) => p + c, 0), 0);
      assert.equal(reduce(from([1, 2, 3]), (p, c) => p + c, 0), 6);
    },
    filter() {
      assert_I(filter(from([]), x => x % 2 == 0), []);
      assert_I(filter(from([1]), x => x % 2 == 0), []);
      assert_I(filter(from([1, 2, 4, 5]), x => x % 2 == 0), [2, 4]);
    },
    map() {
      assert_I(map(from([]), x => x * 10), []);
      assert_I(map(from([1]), x => x * 10), [10]);
      assert_I(map(from([1, 2]), x => x * 10), [10, 20]);
    },
    from() {
      assert_I(from([]), []);
      assert_I(from([1]), [1]);
      assert_I(from([1, 2, 3]), [1, 2, 3]);
    },
    range() {
      assert_I(range(1, 0), []);
      assert_I(range(1, 1), [1]);
      assert_I(range(1, 5), [1, 2, 3, 4, 5]);
    },
  }

  for (const p of Object.keys(testsObj)/*.reverse()*/) {
    try {
      console.log(" ----> ", p);
      (testsObj as any)[p]();
    }
    catch (err) {
      console.log((err as Error).message);
      //throw err;
    }
  }
}

function assert_I<T>(i: I<T>, array: T[]) {
  assert.deepEqual(toarray(i), array);
}



function pipe<T1, T2, T3, T4, T5, T6>(a: T1, f1: F2<T1, T2>, f2: F2<T2, T3>, f3: F2<T3, T4>, f4: F2<T4, T5>, f5: F2<T5, T6>): T6;
function pipe<T1, T2, T3, T4, T5>(a: T1, f1: F2<T1, T2>, f2: F2<T2, T3>, f3: F2<T3, T4>, f4: F2<T4, T5>): T5;
function pipe<T1, T2, T3, T4>(a: T1, f1: F2<T1, T2>, f2: F2<T2, T3>, f3: F2<T3, T4>): T4;
function pipe<T1, T2, T3>(a: T1, f1: F2<T1, T2>, f2: F2<T2, T3>): T3;
function pipe<T1, T2>(a: T1, f1: F2<T1, T2>): T2;
function pipe<T1>(a: T1): T1;
function pipe(a: any, ...fs: Function[]) {
  return fs.reduce((prev, el) => el(prev), a);
}
