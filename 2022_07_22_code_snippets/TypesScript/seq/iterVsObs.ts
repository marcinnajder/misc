import * as assert from "assert";
import { SumType, TypedObj, matchUnion as matchUnionFP, none } from "powerfp";
import { pipe, range as rangeSeq, toarray, map, find } from "powerseq";

const value = <T>(value: T) => ({ type: "value", value }) as const /*as IterR<number>*/;
const completed = { type: "completed" } as const;
const error = (error: Error) => ({ type: "error", err: error }) as const;

// ** poprawiona implementacja 'matchUnion' z 'powerfp'

// - okzalo sie ze takie typowe wywolanie 'matchUnion' zwraca 'unknown', ale akurat tak sie pisze kod ze blad nie leci ale typowania nie ma
// - myslalem ze to kwestia tego ze teraz jest nowa wersja TS a pisane bylo dla starej, tak sprawdzilem i kiedys takze tak bylo
// - 'matchUnion<T extends TypedObj, R>(unionType: T, obj: MatchTypedObj<T, R>): R;' taka byla sygnatura, ciezko sprawdzi w istniejacym kodzie
// jak dzialalo typowanie bo przyklady w repo powerfp sa bardzo bardzo proste (przykladowo w testach jednostkowych jawnie pisane sa argumenty generyczne),
// w repo cms2 uzyte jest tylko raz cms2 i to specyficznie:
// -- chodzi o to ze jak wywola sie tak `const res = matchUnion(..., ...)` to metoda nie wnioskuje i jest typ 'unknown', byc moze zalozenie bylo 
// takie ze jawnie trzeba podac np 'const res = matchUnion<..., string>(..., ...)' 
// -- ciekawe jest to ze czasami dobrze sie wywnioskuje typ zwracany np gdy napiszemy 'const res: string = matchUnion(..., ...)' lub gdy mamy 
// 'return matchUnion(..., ...)' i wiadomo jaki jest typ funkcji, to wtedy wnioskowanie dziala, tak bylo w repo cms2:
// addAuthorizationHeader(request: Request): Request { return matchUnion(this.currentCredentialsO, { ... 

// - no wiec probowalem to naprawic, ale jako tak aby nie atualizowac biblioteki bo to kupa roboty
// - kiedys bylo ExhaustiveMatchTypedObj<T, R> bo zalezy nam aby jakos R wywnioskowal sie obiektu matchotowania { ...: () => <tutaj> } ale
// problem jest taki ze nie chcemy podawac jawnie typu rezultat podczas wywolania metody wiec wnioskuje sie na 'unknown', byc moze
// da sie lepiej napisac to typowanie aby jednak dzialalo, ale ja tutaj obralem troche inne podejscie
// - teraz nie ma R ExhaustiveMatchTypedObj<T>, ale przez to musialem uzyc 'any' jako zezultat 'ValueOrFunc<Extract<T, {type: P; }>, any>;'
// - poniewaz R nie juz wnioskowany jakby "od gory do dolu" tylko jest podany 'any' to okazalo sie ze nie moge juz wspierac takiej definicji
// 'ValueOrFunc<T, R> = ((value: T) => R) | R;' poniewaz tutaj R=any i wszystko sprowadzane jest do 'any' czyli kompletnie nie dziala to
// ze argument moze byc funkcja przyjmujaca T, czyli cala idea wnioskowania obslugi przypadkow nie dziala, jest tak 'ValueOrFunc<T, R> = ((value: T) => R)' :/
// - finalnie sygnatura funkcji jest taka 'matchUnion<T extends TypedObj, M extends MatchTypedObj<T>>(unionType: T, obj: M): ExtractResult<M>' czyli 
// pojawil jest wnioskowany typ M a nastepnie dla niego potrafimy wyciagnac rezultat calej funkcji za pomoca nowego typu 'ExtractResult<M>' ktory uzywa 
// 'conditional types' wiec moze i tak trzeba by podniesc powerfp aby to zmienic w bibliotece


// type ValueOrFunc<T, R> =  // poprzednio
type ValueOrFunc<T, R> = ((value: T) => R);

type ExhaustiveMatchTypedObj<T extends TypedObj<string>> = {
    [P in T["type"]]: ValueOrFunc<Extract<T, { type: P; }>, any>;
};

type MatchTypedObj<T extends TypedObj<string>> =
    ExhaustiveMatchTypedObj<T> | (Partial<ExhaustiveMatchTypedObj<T>> & { _: ValueOrFunc<T, any> });

type ExtractResult<M> = M extends { [P in keyof M]: infer PR } ? (PR extends (...args: any) => infer R ? R : PR) : never;

function matchUnion<T extends TypedObj, M extends MatchTypedObj<T>>(unionType: T, obj: M): ExtractResult<M> {
    return matchUnionFP(unionType, obj) as any;
}


// ** Ites vs Obs

type Res<T> = SumType<typeof value<T> | typeof completed | typeof error>;

type Iter<T> = () => () => Res<T> /*& Dis*/;

type Dis = () => void;
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

                const x: Res<R> = matchUnion(iterator(), {
                    error: res => res,
                    //completed: completed,
                    completed: res => res,
                    value: res => value(f(res.value))
                });

                return x;
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


// ** fragmenty kodu do artykulu opisujacego


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


// var xxx123 = matchUnion(null as any as Res<string>, {
//     // completed: () => 123,
//     // error: res => res.err,
//     // value: res => res.value,
//     completed: () => 123,
//     error: res => res.err,
//     value: res => res,
// });


// -- F# -- 
// let results = [ 5; 10; 15 ] |> Seq.map Value // seq<Res<int>>
// let firstResult = results |> Seq.head // Res<int>
// let text =
//     match firstResult with
//     | Error err -> "blad: " + err.Message
//     | Completed -> "koniec"
//     | Value value -> "wartosc: " + value.ToString()

// type Iter_<T> = () => () => null | T;
// type Obs_<T> = Reverse<Iter_<T>>;
// // type Obs_<T> = (args: (args: Reverse<T> | null) => []) => []
// type Func = Reverse<(arg1: number) => string>;

// {
//     const results: Iterable<Res<number>> = pipe([5, 10, 15], map(value));
//     const firstResult = pipe(results, find())!; // Res<number>
//     const text = matchUnion(firstResult, {
//         error: ({ err }) => "blad: " + err.message,
//         _: (res) => "nie blad " + res
//     });
// }


const ys = pipe([1, 2, 3, 4, 5],
    fromSeqToIter,
    xs => filterIter(xs, x => x % 2 === 0),
    xs => mapIter(xs, x => x.toString()),
    fromIterToSeq,
    toarray());
assert.deepStrictEqual(ys, ["2", "4"]);


pipe(
    intervalObs(10),
    xs => takeObs(xs, 3),
    xs => filterObs(xs, x => x % 2 === 0),
    xs => mapObs(xs, x => x.toString()),
    fromObsToIter)
    .then(items => assert.deepEqual(items, ["0", "2"]));
