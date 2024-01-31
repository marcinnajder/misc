/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-unused-vars */

import { every, find, flatmap, foreach, reduce, skip, some, take } from "powerseq";


/** https://github.com/tc39/proposal-iterator-helpers, https://github.com/zloirock/core-js#iterator-helpers */
abstract class IteratorHelpers<T> implements Iterator<T>{
    abstract next(): IteratorResult<T, any>;

    protected getIterable(): Iterable<T> {
        return { [Symbol.iterator]: () => this };
    }

    toArray(): T[] {
        return [...this.getIterable()];
    }

    map<R>(f: (item: T, index: number) => R) {
        return Iterator_.from(go(this.getIterable()));

        function* go(items: Iterable<T>) {
            let index = 0;
            for (const item of items) {
                yield f(item, index++);
            }
        }
    }

    map_<R>(f: (item: T, index: number) => R) {
        let index = 0;
        return Iterator_.from<R>({
            next: () => {
                const result = this.next();
                return result.done ? { done: true } : { done: false, value: f(result.value, index++) };
            }
        } as Iterator<R, R>)
    }

    filter(f: (item: T, index: number) => boolean) {
        return Iterator_.from(go(this.getIterable()));

        function* go(items: Iterable<T>) {
            let index = 0;
            for (const item of items) {
                if (f(item, index++)) {
                    yield item
                }
            }
        }
    }

    drop(limit: number) {
        return Iterator_.from(skip(this.getIterable(), limit));
    }

    every(f: (item: T, index: number) => boolean) {
        return every(this.getIterable(), f);
    }

    find(f: (item: T, index: number) => boolean) {
        return find(this.getIterable(), f);
    }

    foreach(f: (item: T, index: number) => void) {
        foreach(this.getIterable(), f);
    }

    some(f: (item: T, index: number) => boolean) {
        return some(this.getIterable(), f);
    }

    take(limit: number) {
        return Iterator_.from(take(this.getIterable(), limit));
    }

    reduce<A>(f: (total: A, value: T) => A, initialValue: A) {
        return reduce(this.getIterable(), f, initialValue); // +index
    }

    flatMap<R>(f: (value: T, index: number) => Iterable<R> | Iterator<R>) {
        return Iterator_.from(flatmap(this.getIterable(), (item, index) => {
            const iter = f(item, index);
            return isIterable(iter) ? iter : { [Symbol.iterator]: () => iter };
        }));
    }

    pipe_<R>(f: (value: Iterator_<T>) => Iterator_<R>) {
        return f(this as any);
    }
}



export type Iter<T> = Iterable<T> | Iterator<T>;

function isIterable<T>(iter: Iter<T>): iter is Iterable<T> {
    return (iter as any)[Symbol.iterator] !== undefined;
}


export class Iterator_<T> extends IteratorHelpers<T>  {
    private constructor(private iterator: Iterator<T>) {
        super();
    }

    next() {
        return this.iterator.next();
    }

    static from<TR>(iter: Iter<TR>) {
        return new Iterator_<TR>(isIterable(iter) ? iter[Symbol.iterator]() : iter);
    }
}




/** Object.assign copies only enumerable properties, prototype object has non-enumerable properties */
function assign(target: any, source: any) {
    for (const property of Object.getOwnPropertyNames(source)) {
        if (property !== "constructor") {
            target[property] = source[property];
        }
    }
}



const iteratorHelpersProto = Object.getPrototypeOf(Object.getPrototypeOf(Iterator_.from([])));

assign(Object.getPrototypeOf([1, 2, 3].values()), iteratorHelpersProto); // Array
assign(Object.getPrototypeOf(new Set([1, 2, 3]).values()), iteratorHelpersProto); // Set
assign(Object.getPrototypeOf(new Map([[1, 1]]).values()), iteratorHelpersProto); // Map
assign(Object.getPrototypeOf(Object.getPrototypeOf((function* () { })())), iteratorHelpersProto); // generator function


// // interface Iterator<T, TReturn = any, TNext = undefined> extends IteratoreHelpers<T> { } // "Found 14 errors." inside powerseq operators code
// // interface IterableIterator<T> extends IteratoreHelpers<T> { } // Named property 'next' of types ... and ... are not identical
// interface IterableIterator<T> extends Omit<IteratorHelpers<T>, "next"> { }
// interface Generator<T = unknown, TReturn = any, TNext = unknown> extends IteratorHelpers<T> { }
// declare namespace global {
//     //     interface User {
//     //       [index: string]: any;
//     //     }
//     //   }
// }


declare global {
    // we cannot extend Iterator<T> type because some code can be using this type directly -> "Found 14 errors." inside powerseq operators code
    // interface Iterator<T, TReturn = any, TNext = undefined> extends IteratorHelpers<T> { } 
    interface IterableIterator<T> extends Omit<IteratorHelpers<T>, "next"> { }
    interface Generator<T = unknown, TReturn = any, TNext = unknown> extends IteratorHelpers<T> { }
}
