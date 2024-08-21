// executing the whole script at once in REPL (npm run repl -- ./seq/lazyList.ts) works fine,
// but running code "step by step" (SBS-REPL) does not work for each piece of code :/

// REPL and importing modules (common.js vs require())
// - common.js is not supported in SBS-REPL
// - require() does not inference types of imported functions
// - SBS-REPL provides some node modules by default with importing them (fs, assert, ...)
// - type defined below 'List<T>' is also defined in other file, using common.js resolves this problem

import * as assert from "assert";
import { pipe, take as takePS, map as mapPS, toarray } from "powerseq";

// ** (immutable) linked list

// OOP (C++, C#)
class Node<T> {
    constructor(public value: T, public next: Node<T> | null) { }
}
class List1<T> {
    constructor(public first: Node<T> | null) { }
    // add/remove/...()
}

const l1 = new List1(new Node(1, new Node(2, null)));

// FP (SML, Haskell)
type List2<T> =
    | { type: "empty" }
    | { type: "cons"; head: T; tail: List2<T> };

let l2: List2<number> = { type: "cons", head: 1, tail: { type: "cons", head: 2, tail: { type: "empty" } } };


// FP (LISP)
type List3<T> = null | readonly [head: T, tail: List3<T>];

let l3: List3<number> = [1, [2, null]];

// Listy leniwe (OCaml, w5.pdf)
// type a' llist = LNil | LConst of 'a * (unit -> 'a llist) 

// ** List<T>, LazyList<T>

type List<T> = null | readonly [head: T, tail: List<T>];

type LazyList<T> = () => null | readonly [head: T, tail: LazyList<T>];

// ** LazyList<T> collection conversions

function* fromLazyListToIterable<T>(list: LazyList<T>): Iterable<T> {
    let node = list();
    while (node != null) {
        const [head, tail] = node;
        yield head;
        node = tail();
    }
}

function fromArrayToLazyList<T>(array: T[]): LazyList<T> {
    return next(0);

    function next(index: number): LazyList<T> {
        return () => {
            return index >= array.length ? null : [array[index], next(index + 1)];
        };
    }
}

for (const [input, expected] of [[[], []], [[1], [1]], [[1, 2, 3], [1, 2, 3]]]) {
    assert.deepEqual(pipe(input, fromArrayToLazyList, fromLazyListToIterable, toarray()), expected);
}

// iterating over ES6 iterable object introduces side-effects/mutations, we need cache/memoize already read values
function callOnce<T extends () => any>(f: T): T {
    let wasCalled = false;
    let result: ReturnType<T>;

    return <T>function () {
        if (!wasCalled) {
            result = f();
            wasCalled = true;
        }
        return result;
    }; // as T;  // <- error in SBS-REPL :/
}



function fromIterableToLazyList<T>(iterable: Iterable<T>): LazyList<T> {
    const iterator = iterable[Symbol.iterator]();
    return getNextItem();

    function getNextItem(): LazyList<T> {
        return callOnce(() => {
            const result = iterator.next();
            return result.done ? null : [result.value, getNextItem()];
        });
    }
}

var lazylist1: LazyList<number> = () => [1, () => [2, () => [3, () => null]]]
var lazylist2: LazyList<number> = fromIterableToLazyList([1, 2, 3]);
assert.deepEqual([...fromLazyListToIterable(lazylist2)], [1, 2, 3]);


// ** List<T> collection conversions
function fromArrayToList<T>(array: T[]): List<T> {
    return array.reduceRight((tail, head) => [head, tail], null as List<T>);
}


function fromIterableToList<T>(items: Iterable<T>): List<T> {
    return next(fromIterableToLazyList(items));

    function next(list: LazyList<T>): List<T> {
        const node = list();
        if (node === null) {
            return null;
        }
        const [head, tail] = node;
        return [head, next(tail)];
    }
}


function fromListToIterable<T>(items: List<T>): Iterable<T> {
    return fromLazyListToIterable(next(items));

    function next(list: List<T>): LazyList<T> {
        return () => {
            const node = list;
            if (node === null) {
                return null;
            }
            const [head, tail] = node;
            return [head, next(tail)];
        };
    }
}

var a: Iterable<string>;


let list1: List<number> = [1, [2, [3, null]]];
let list2: List<number> = fromArrayToList([1, 2, 3]);
let list3: List<number> = fromIterableToList([1, 2, 3]);

assert.deepEqual(list2, list1);
assert.deepEqual(list3, list1);
assert.deepEqual([...fromListToIterable(list1)], [1, 2, 3]);



// -- sample opertors for LazyList<T>


function repeatvalue<T>(value: T): LazyList<T> {
    return function returnValue() {
        return [value, returnValue];
    };
}

function range(start: number, count: number): LazyList<number> {
    return next(start, count);

    function next(number: number, counter: number): LazyList<number> {
        return () => {
            return counter <= 0 ? null : [number, next(number + 1, counter - 1)];
        };
    }
}

assert.deepEqual(pipe(repeatvalue(123), fromLazyListToIterable, takePS(3), toarray()), [123, 123, 123]);
assert.deepEqual(pipe(range(1, 3), fromLazyListToIterable, toarray()), [1, 2, 3]);


function take<T>(items: LazyList<T>, count: number): LazyList<T> {
    return next(items, count);

    function next(list: LazyList<T>, counter: number): LazyList<T> {
        return () => {
            if (counter <= 0) {
                return null;
            }
            const node = list();
            if (node === null) {
                return null;
            }
            const [head, tail] = node;
            return [head, next(tail, counter - 1)];
        };
    }
}

for (const [count, expected] of [[0, []], [2, [1, 2]], [3, [1, 2, 3]], [5, [1, 2, 3]]] as const) {
    assert.deepEqual(pipe([1, 2, 3], fromIterableToLazyList, l => take(l, count), fromLazyListToIterable, toarray()), expected);
}

function filter<T>(items: LazyList<T>, f: (item: T) => boolean): LazyList<T> {
    return next(items);

    function next(list: LazyList<T>): LazyList<T> {
        return () => {
            const node = list();
            if (node === null) {
                return null;
            }
            const [head, tail] = node;
            return f(head) ? [head, next(tail)] : next(tail)();
        };
    }
}

assert.deepEqual(
    pipe([1, 2, 3, 4, 5],
        fromIterableToLazyList,
        l => filter(l, x => x % 2 === 0),
        fromLazyListToIterable,
        toarray())
    , [2, 4]);

function map<T, R>(items: LazyList<T>, f: (item: T) => R): LazyList<R> {
    return next(items);

    function next(list: LazyList<T>): LazyList<R> {
        return () => {
            const node = list();
            if (node === null) {
                return null;
            }
            const [head, tail] = node;
            return [f(head), next(tail)];
        };
    }
}

assert.deepEqual(pipe([1, 2, 3], fromIterableToLazyList, l => map(l, x => (x * 10).toString()), fromLazyListToIterable, toarray()), ["10", "20", "30"]);



// testing memoization of 'fromIterableToLazyList' function
let lazylist3 = pipe([1, 2, 3], mapPS(x => { console.log("mapping", x); return x * 10; }), fromIterableToLazyList);
let node = lazylist3();
assert.equal(node === lazylist3(), true); // the same instance
if (node !== null) {
    const [head, tail] = node;
    console.log("value:", head);

    node = tail();
    assert.equal(node === tail(), true); // the same instance

    if (node !== null) {
        const [head, tail] = node;
        console.log("value:", head);
    }
}



// ** generalised implementation of conversion from/to iterable to/fom List/LazyList

commonImplementationOfConversionBetweenIterableAndLists();

type Unwrap<T, L> = (lst: L) => null | readonly [head: T, L];
type Wrap<T, L> = (f: () => null | readonly [head: T, L]) => L;


function commonImplementationOfConversionBetweenIterableAndLists() {

    function* fromLToIterable<T, L>(list: L, unwrap: Unwrap<T, L>): Iterable<T> {
        let node = unwrap(list);
        while (node != null) {
            const [head, tail] = node;
            yield head;
            node = unwrap(tail);
        }
    }

    function fromList<T>(list: List<T>): Iterable<T> {
        return fromLToIterable(list, tail => tail);
    }

    function fromLazyList<T>(list: LazyList<T>): Iterable<T> {
        return fromLToIterable(list, tail => tail());
    }

    // error in SBS-REPL :/
    function fromIterableToL<T, L>(iterable: Iterable<T>, wrap: Wrap<T, L>): L {
        const iterator = iterable[Symbol.iterator]();
        return getNextItem();

        function getNextItem(): L {
            return wrap(() => {
                const result = iterator.next();
                return result.done ? null : [result.value, getNextItem()];
            });
        }
    }

    function toList<T>(iterable: Iterable<T>): List<T> {
        return fromIterableToL(iterable, f => f());
    }

    function toLazyList<T>(iterable: Iterable<T>): LazyList<T> {
        return fromIterableToL(iterable, callOnce);
    }


    let list1: List<number> = [1, [2, [3, null]]];
    let list2: List<number> = toList([1, 2, 3]);
    let list3: List<number> = pipe(list2, fromList, toList);
    assert.deepEqual(list2, list1);
    assert.deepEqual(list3, list1);

    var lazylist1: LazyList<number> = () => [1, () => [2, () => [3, () => null]]]
    var lazylist2: LazyList<number> = toLazyList([1, 2, 3]);
    assert.deepEqual([...fromLazyList(lazylist2)], [1, 2, 3]);
}


function commonImplementationOfConversionBetweenIterableAndLists2() {
    // const {fromLToIterable, fromIterableToL } = module1<T, List<T>>( tail => tail, tail => tail());

    // const {fromLToIterable, fromIterableToL } = module1<T, LazyList<T>>( tail => tail(), callOnce);

    // functions are generic of 'T', so (I guess) there is no simpler way of creating the final functions for List<T> and LazyList<T> types :/
    function fromList<T>(list: List<T>) {
        return module1<T, List<T>>(tail => tail, f => f()).fromLToIterable(list);
    }

    const fromList_ = <T>(list: List<T>) => module1<T, List<T>>(tail => tail, f => f()).fromLToIterable(list);


    // wrap into function to avoid passing helper (wrap, unwrap) functions manually 
    function module1<T, L>(unwrap: Unwrap<T, L>, wrap: Wrap<T, L>) {

        return { fromLToIterable, fromIterableToL };

        function* fromLToIterable(list: L): Iterable<T> {
            let node = unwrap(list);
            while (node != null) {
                const [head, tail] = node;
                yield head;
                node = unwrap(tail);
            }
        }

        function fromIterableToL(iterable: Iterable<T>): L {
            const iterator = iterable[Symbol.iterator]();
            return getNextItem();

            function getNextItem(): L {
                return wrap(() => {
                    const result = iterator.next();
                    return result.done ? null : [result.value, getNextItem()];
                });
            }
        }
    }
}


// ** immutable version of Iterable<T> interface

type PureIterable<T> = {
    iterator(): PureIterator<T>;
}
// type PureIterator<T> = {
//     next(): [{ done: boolean; value: T }, next: PureIterator<T>];
// }
type PureIterator<T> = () => null | [value: T, next: PureIterator<T>];


// ** implementation of pair/tuple (used to linked list) using only functions from SICP book
// (define (cons x y) (define (dispatch m) (cond ((= m 0) x) ((= m 1) y) (else (error "Argument not 0 or 1: CONS" m)))) dispatch)
// (define (car z) (z 0))
// (define (cdr z) (z 1))


function cons(x: any, y: any) {
    return dispatch;

    function dispatch(m: number) {
        switch (m) {
            case 0: return x;
            case 1: return y;
            default: throw new Error("Argument not 0 or 1: CONS " + m);
        }
    }
}

function car(z: ReturnType<typeof cons>) {
    return z(0);
}

function cdr(z: ReturnType<typeof cons>) {
    return z(1);
}

const lst = cons(10, cons(20, null));

console.log(car(lst)); // -> 10

console.log(car(cdr(lst))); // -> 20
console.log(cdr(cdr(lst))); // -> null

