import * as assert from "assert";
import { SumType, matchUnion, none } from "powerfp";
import { pipe, range as rangeSeq, toarray } from "powerseq";


// Iterable vs Observable
// interface Iterable<T> {
//     iterator(): Iterator<T>;
// }
// interface Iterator<T> {
//     next(): { done: boolean; value: T };
// }

// interface Observable<T> {
//     subscribe(observer: Observator<T>): (() => void);
// }
// interface Observator<T> {
//     onNext(value: T): void;
//     onError(error: Error): void;
//     onCompleted(): void;
// }

const value = <T>(value: T) => ({ type: "value", value }) as const /*as IterR<number>*/;
const completed = { type: "completed" } as const;
const error = (error: Error) => ({ type: "error", err: error }) as const;

type Res<T> = SumType<typeof value<T> | typeof completed | typeof error>;

type Dis = () => void;

type Iter<T> = () => () => Res<T> /*& Dis*/;

type Obs<T> = (sub: (res: Res<T>) => void) => Dis;

type Reverse<F> = F extends (...args: infer Args) => infer Result ? (args: Reverse<Result>) => Reverse<Args> : F;

type Obs2<T> = Reverse<Iter<T>>; // type Obs2<T> = (args: (args: Res<T>) => []) => []



// ** Iter
function rangeIter(start: number, count: number): Iter<number> {
    return () => {
        const max = start + count;
        let current = start;
        return () => current < max ? value(current++) : completed;
    };
}

let iter1 = rangeIter(5, 3)();
assert.deepEqual(iter1(), value(5));
assert.deepEqual(iter1(), value(6));
assert.deepEqual(iter1(), value(7));
assert.deepEqual(iter1(), completed);

function fromSeqToIter<T>(items: Iterable<T>): Iter<T> {
    return () => {
        const iterator = items[Symbol.iterator]();
        return next;

        function next(): Res<T> {
            try {
                const res = iterator.next();
                return res.done ? completed : value(res.value);
            } catch (err: any) {
                return error(err);
            }
        }
    }
}

let iter2 = fromSeqToIter([1, 2])();
assert.deepEqual(iter2(), value(1));
assert.deepEqual(iter2(), value(2));
assert.deepEqual(iter2(), completed);

function* fromIterToSeq<T>(items: Iter<T>) {
    const iterator = items();
    let result = iterator();

    while (result.type === "value") {
        yield result.value;
        result = iterator();
    }

    if (result.type === "error") {
        throw result.err;
    }
}

assert.deepEqual([...fromIterToSeq(rangeIter(5, 3))], [5, 6, 7]);
assert.deepEqual([...fromIterToSeq(rangeIter(5, 0))], []);

function filterIter<T>(items: Iter<T>, f: (item: T) => boolean): Iter<T> {
    return () => {
        const iterator = items();
        return next;

        function next(): Res<T> {
            try {
                return matchUnion(iterator(), {
                    error: res => res,
                    completed: res => res,
                    value: res => f(res.value) ? res : next()
                });
            }
            catch (err: any) {
                return error(err);
            }
        }
    }
}

const trans = <T, R>(items: Iterable<T>, op: (iter: Iter<T>) => Iter<R>) => pipe(items, fromSeqToIter, op, fromIterToSeq, toarray())

assert.deepEqual(trans([1, 2, 3, 4, 5], x => filterIter(x, x => x % 2 === 0)), [2, 4]);


function mapIter<T, R>(items: Iter<T>, f: (item: T) => R): Iter<R> {
    return () => {
        const iterator = items();
        return next;

        function next(): Res<R> {
            try {
                return matchUnion(iterator(), {
                    error: res => res,
                    completed: completed,
                    value: res => value(f(res.value))
                });
            }
            catch (err: any) {
                return error(err);
            }
        }
    }
}

assert.deepEqual(trans([1, 2, 3], x => mapIter(x, x => x.toString())), ["1", "2", "3"]);


// ** Obs

function fromSeqToObs<T>(items: Iterable<T>): Obs<T> {
    return sub => {
        try {
            for (const item of items) {
                sub(value(item));
            }
            sub(completed);
        } catch (err: any) {
            sub(err);
        } finally {
            return () => { };
        }
    }
}

// let obs1 = fromSeqToObs([1, 2]);
// let buffer: Res<number>[] = [];
// obs1(x => buffer.push(x));
// assert.deepEqual(buffer, [value(1), value(2), completed])


function fromObsToIter<T>(obs: Obs<T>): Promise<T[]> {
    return new Promise(function (resolve, reject) {
        let buffer: T[] = [];
        const _ = obs(res => {
            matchUnion(res, {
                error: res => reject(res.err),
                completed: () => resolve(buffer),
                value: res => buffer.push(res.value)
            });
        });
    });
}


pipe([1, 2, 3], fromSeqToObs, fromObsToIter).then(items => assert.deepEqual(items, [1, 2, 3]));


function intervalObs(ms: number): Obs<number> {
    return sub => {
        let index = 0;
        const id = setInterval(() => {
            sub(value(index++));
        }, ms);
        return () => {
            sub(completed);
            clearInterval(id);
        }
    };
}

// let unsub = intervalObs(1000)(x => console.log(x));
// setTimeout(() => {
//     unsub();
// }, 4000);


function takeObs<T>(obs: Obs<T>, count: number): Obs<T> {
    if (count === 0) {
        return sub => {
            sub(completed);
            return () => { };
        };
    }

    return sub => {
        let index = 0;

        const unsub = obs(res => {
            matchUnion(res, {
                error: sub,
                completed: sub,
                value: r => {
                    sub(r);
                    if ((++index) === count) {
                        unsubscribe();
                    }
                }
            })
        })

        function unsubscribe() { sub(completed); unsub(); }
        return unsubscribe;
    };
}

pipe(intervalObs(10), obs => takeObs(obs, 3), fromObsToIter).then(items => assert.deepEqual(items, [0, 1, 2]));


function filterObs<T>(obs: Obs<T>, f: (item: T) => boolean): Obs<T> {
    return sub => {
        const unsub = obs(res => {
            matchUnion(res, {
                error: sub,
                completed: sub,
                value: r => {
                    if (f(r.value)) {
                        sub(r);
                    }
                }
            })
        })

        function unsubscribe() { sub(completed); unsub(); }
        return unsubscribe;
    };
}

pipe(intervalObs(10), obs => takeObs(obs, 3), obs => filterObs(obs, x => x % 2 === 0), fromObsToIter).then(items => assert.deepEqual(items, [0, 2]));

function mapObs<T, R>(obs: Obs<T>, f: (item: T) => R): Obs<R> {
    return sub => {
        const unsub = obs(res => {
            matchUnion(res, {
                error: sub,
                completed: sub,
                value: r => {
                    sub(value(f(r.value)));
                }
            })
        })

        function unsubscribe() { sub(completed); unsub(); }
        return unsubscribe;
    };
}

pipe(intervalObs(10), obs => takeObs(obs, 3), obs => mapObs(obs, x => x.toString()), fromObsToIter).then(items => assert.deepEqual(items, ["0", "1", "2"]));

