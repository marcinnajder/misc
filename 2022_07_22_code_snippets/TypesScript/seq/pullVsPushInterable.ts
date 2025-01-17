import { pipe, flatmap, concat, map, distinct, toarray, find, range } from "powerseq";

// https://go.dev/blog/range-functions go lang


// ******************************************************************************************************************************
// (PULL) iterator, generators
// JS


interface Iterable___<T> {
    [Symbol.iterator](): Iterator___<T, any, undefined>;
}

interface Iterator___<T, R, N> {
    next(args?: N): Result__<T, R>;

    return?(value?: R): Result__<T, R>;
    throw?(e?: any): Result__<T, R>;
}

type Result__<T, R> = { value: T; done?: false; } | { value: R; done: true; }


function* rangeIter(from: number, count: number) {
    try {
        const end = from + count;
        for (let i = from; i < end; i++) {
            yield i;
        }
    }
    finally {
        console.log(`${rangeIter.name}: koniec`);
    }
}

var iterable1 = rangeIter(1, 3);
var iterator2 = iterable1[Symbol.iterator]()
console.log(iterator2.next())
console.log(iterator2.next())
//iterator2.return(); // dispose/stop/close/cancel/.. !!!

var iterable3 = rangeIter(1, 3);
for (const el of iterable3) {
    console.log(el)
}



// PUSH iterator ?? 
// [5, 10, 15].forEach(value => console.log(value));


// ******************************************************************************************************************************
// PUSH
// go ---> type Seq[V any] func(yield func(V) bool)

type PushSeq<T> = (_yield: (value: T) => boolean) => void;

// ~ new Promise( (res,rej) => { res(1);  })
// ~ new Observable( obs => { obs.next(1); obs.next(1); ...  })

function return123Push(): PushSeq<number> {
    return _yield => {
        if (!_yield(1)) {
            return;
        }
        if (!_yield(2)) {
            return;
        }
        if (!_yield(3)) {
            return;
        }
    }
}

var seq1 = return123Push();

// ~ rx --> obs.subscribe (value => console.log(value);)
seq1(value => {
    console.log(value);
    return true;
});


function rangePush(from: number, count: number): PushSeq<number> {
    return _yield => {
        const end = from + count;
        for (let i = from; i < end; i++) {
            if (!_yield(i)) {
                return;
            }
        }
    }
}

function filterPush<T>(seq: PushSeq<T>, f: (item: T) => boolean): PushSeq<T> {
    return _yield => {
        seq(v => {
            if (f(v)) {
                return _yield(v);
            }
            return true;
        });
    };
}

function mapPush<T, R>(seq: PushSeq<T>, f: (item: T) => R): PushSeq<R> {
    return _yield => {
        seq(v => {
            return _yield(f(v));
        });
    };
}

function takePush<T>(seq: PushSeq<T>, count: number): PushSeq<T> {
    return _yield => {
        let counter = count;
        seq(v => {
            if (counter === 0) {
                return false;
            }
            counter--;
            return _yield(v);
        });
    };
}

function printAll<T>(v: T) {
    console.log(v);
    return true;
}


const items = rangePush(1, 10);

const strings = pipe(items,
    xs => filterPush(xs, x => x % 2 === 0),
    xs => mapPush(xs, x => "$" + x),
    xs => takePush(xs, 3))

// items(printAll);
// strings(printAll);



function iterToPush<T>(iterable: Iterable<T>): PushSeq<T> {
    return _yield => {
        for (var item of iterable) {
            if (!_yield(item)) {
                return;
            }
        }
    };
}


// iterToPush([5, 10, 15])(printAll);


function zipPush<T1, T2>(seq1: PushSeq<T1>, seq2: PushSeq<T2>): PushSeq<[T1, T2]> {
    return _yield => {
        seq1(value1 => {

            // ???? :/
            seq2(value2 => {

                return true;
            })

            return true;
        })
    }
}



// ******************************************************************************************************************************
// PULL
// go --> func Pull[V any](seq Seq[V]) (next func() (V, bool), stop func()) {


type Result<T> = { value: T; done?: boolean; } // done? -> ~ IteratorResult<T, TReturn>;

type PullSeq<T> = {
    next(): Result<T>;
    stop(): void;
}


function iterToPull<T>(iterable: Iterable<T>): PullSeq<T> {
    const iterator = iterable[Symbol.iterator]();
    return {
        next() {
            return iterator.next()
        },
        stop() {
            iterator.return?.();
        }
    }
}

function* pullToInter<T>(seq: PullSeq<T>): Iterable<T> {
    const { next, stop } = seq;
    try {
        let result: Result<T>;

        while (!(result = next()).done) {
            yield result.value;
        }
    } finally {
        stop();
    }
}



var seqPull1 = iterToPull([5, 10, 15]);

var { next: seqPull1_next, stop: seqPull1_top } = seqPull1

// console.log(seqPull1_next());
// console.log(seqPull1_next());
// console.log(seqPull1_next());
// console.log(seqPull1_next());


function zipPull<T1, T2>(seq1: PullSeq<T1>, seq2: PullSeq<T2>): PullSeq<[T1, T2]> {
    const { next: next1, stop: stop1 } = seq1;
    const { next: next2, stop: stop2 } = seq2;
    return {
        next() {
            const result1 = next1();
            if (!result1.done) {
                const result2 = next2();
                if (!result2.done) {
                    return { value: [result1.value, result2.value], done: false }
                }
            }

            return { value: undefined as any, done: true };
        },
        stop() {
            stop1();
            stop2();
        }
    }
}


var { next: zip_next } = zipPull(iterToPull([5, 10, 15]), iterToPull(['a', 'b']));

// console.log(zip_next());
// console.log(zip_next());
// console.log(zip_next());


// ******************************************************************************************************************************
// konwersja PULL <---> PUSH


function pullToPush<T>(seq: PullSeq<T>): PushSeq<T> {
    return _yield => {
        const { next, stop } = seq;
        let result: Result<T>;

        while (!(result = next()).done) {
            if (!_yield(result.value)) {
                stop();
                return; // break 'while' loop
            }
        }
    };
}

// pullToPush(iterToSeqPull([5, 10, 15]))(printAll);

function pushToPull<T>(seq: PushSeq<T>): PullSeq<T> {

    function next() {
        seq(v => {
            // ??????
            return true;
        })

        return { value: null as T, done: false };
    }

    return { next, stop() { } };

}

// ******************************************************************************************************************************
// konwersja PULL <---> PUSH (AWAIT)


type PushSeqAwait<T> = (_yield: (value: T) => PromiseLike<boolean>) => Promise<void>;

function rangeAwait(from: number, count: number): PushSeqAwait<number> {
    return async _yield => {
        try {
            console.log(rangeAwait.name, ": startuje");
            const end = from + count;
            for (let i = from; i < end; i++) {
                console.log(rangeAwait.name, ": generuje wartosc", i);
                const b = await _yield(i);
                if (!b) {
                    return;
                }
            }
        }
        finally {
            console.log(rangeAwait.name, ": koncze");
        }
    }
}


// rangeAwait(1, 10)(async value => {
//     console.log(value);
//     return true;
// });


type Resolve = (b: boolean) => void;

// Bool Promise Factory
interface BPFactory {
    create(): PromiseLike<boolean>;
    resolve: Resolve;
}

function realPromiseFactory(): BPFactory {
    var resolve: Resolve;

    return {
        create() {
            return new Promise<boolean>((res, rej) => {
                resolve = res;
            });
        },
        resolve(b) {
            resolve?.(b);
        }
    }
}

function customPromiseFactory(): BPFactory {
    const promise = {
        resolve: undefined as Resolve | undefined,
        then(...args: any[]): any {
            this.resolve = args[0];
            return this;
        },
    };

    return {
        create() {
            return promise;
        },
        resolve(b) {
            promise.resolve?.(b);
        }
    }
}

function pushToPullAwait<T>(seq: PushSeqAwait<T>, bpf: BPFactory): PullSeq<T> {
    let value: T;
    let done = false
    let started = false;

    return {
        next() {
            if (!started) {
                started = true;

                seq(v => {
                    value = v;
                    return bpf.create();
                }).then(_ => done = true)

            } else {
                bpf.resolve(true)
            }

            return { value, done };
        },
        stop() {
            if (started) {
                bpf.resolve(false)
            }
        }
    };
}


function delay(ms: number) {
    return new Promise((res, rej) => setTimeout(res, ms));
}

async function testPushToPullAwait(pf: BPFactory) {
    var { next, stop } = pushToPullAwait(rangeAwait(200, 2), pf);

    var ms = 1000;
    //var ms = 0;

    for (var i = 0; i < 4; i++) {
        console.log(` ---- '${i}'. spie '${ms}'ms i potem wolam 'next'`);
        await (ms && delay(ms))
        console.log(next());
    }
}

// testPushToPullAwait(customPromiseFactory());
// testPushToPullAwait(realPromiseFactory());


// - testowanie pushToPullAwait czyli wykorzystanie async/await i Promise
// - niestety my potrzebujemy "synchronicznego dzialania await-owania" tzn implementacja polega na tym ze w momencie wywolania "await _yield(...)"
// od razu wykona sie wywolanie metody ".then(...)" na stworzonym obiekcie Promise tzn nawet przed kolejnym krokiem, ale to wykonanie robi sie
// jak JS ma chwile czasu (w kolejnym "event loop")
// - dokladnie widac problemy gdy nizej w petli for nie ma slowa "await ...", w takim przypadku my wielokrotnie pod rzad wykonujemy "next()"
// pierwsze wykonanie dziala ok bo "subsrybuje sie" na iterator, ale juz kolejne "next()" zaklada funkcja "resolve" bedzie juz ustawiona, 
// ale nie jest i efekt jest taki ze na ekran wypisuje sie nam 4 razy to samo -> { value: 200, done: false }
// - gdy dodamy "await ..." w petli i co ciekawe nie ma znaczenia co awaitujemy tzn wywolanie "await (ms && delay(ms))" dla ms=100 dziala tak samo
// jak ms=0 tzn teoretycznie nie ma nawet w pamieci obiektu Promise tylko number=0, to i tak widac ze dzialanie jest inne (pewnie nastepuje opakowanie w Promise)...
// - ... dzialanie jest takie ze wypisuje sie -> { value: 200, done: false } { value: 200, done: false } { value: 201, done: false } { value: 201, done: false }
// czyli nie do konca to co chcemy poniewaz 2 pierwsze sa takie same, ale jednak pojawia sie kolejno generowane wartosci
// - generalnie "problem" wynika z dzialania async/await oraz Promise, mozliwe ze bardziej samego Promise a "await 0" opakuje i tak wartosc w Promise
// - dlatego nizej testowalem dzialanie dla wlasnego obiektu "Promise" i efekt byl dokaldnie taki sam, ale tutaj to tez moze wynika z tego ze moj obiekt
// Promise i tak jest opakowywany w prawdziwy w moment wywolania "await ..."  to nawet w sumie poruszone jest tutaj:
// - https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/await#description/ Thenable object (including non-native promises,
// polyfill, proxy, child class, etc.): A new promise is constructed with the native Promise() constructor by calling the object's then() method 
// and passing in a handler that calls the resolve callback.


// ******************************************************************************************************************************
// konwersja PULL <---> PUSH (YIELD)

function* generator(): Generator<number, Date, string> {
    var str = yield 1;

    yield 2;
    yield 3;

    return new Date();
}


type Box<T> = T

const box = <T>(value: T) => value;

type PushSeqYield<T> = (_yield: (value: T) => Box<T>) => Generator<Box<T>, void, boolean>;


function rangeYield(from: number, count: number): PushSeqYield<number> {
    return function* (_yield) {
        try {
            console.log(rangeYield.name, ": startuje");
            const end = from + count;
            for (let i = from; i < end; i++) {
                console.log(rangeYield.name, ": generuje wartosc", i);
                const b = yield _yield(i);
                if (!b) {
                    return;
                }
            }
        }
        finally {
            console.log(rangeYield.name, ": koncze");
        }
    }
}


function pushToPullYield<T>(seq: PushSeqYield<T>): PullSeq<T> {
    const iterator = seq(box)[Symbol.iterator]();
    return {
        next() {
            return iterator.next(true) as Result<T>;
        },
        stop() {
            iterator.return();
        }
    };
}



var pullseq7 = pushToPullYield(rangeYield(100, 3));

// console.log(pullseq7.next());
// console.log(pullseq7.next());
// console.log(pullseq7.next());
// console.log(pullseq7.next());
// console.log(pullseq7.next());
// pullseq7.stop()


// go --> map push iterator  
// func Map[T, R any](f func(T) R) Operator[T, R] {
// 	return func(s iter.Seq[T]) iter.Seq[R] {
// 		return func(yield func(R) bool) {
// 			for v := range s {
// 				if !yield(f(v)) {
// 					return
// 				}
// 			}
// 		}
// 	}
// }

// go --> map push iterator  
// function mapPush<T, R>(seq: PushSeq<T>, f: (item: T) => R): PushSeq<R> {
//     return _yield => {
//         seq(v => {
//             return  _yield(f(v));
//         });
//     };
// }

function mapPushYield1<T, R>(seq: PushSeqYield<T>, f: (item: T) => R): PushSeqYield<R> {
    return function* (_yield) {

        const iterator = seq(box)[Symbol.iterator]();

        let result: Result<T>;
        let gonext = true;

        while (!(result = iterator.next(gonext) as Result<T>).done) {
            const b = yield _yield(f(result.value));
            if (!b) {
                gonext = false;
            }
        }
    };
}



function mapPushYield2<T, R>(seq: PushSeqYield<T>, f: (item: T) => R): PushSeqYield<R> {
    return function* (_yield) {
        for (const item of pullToInter(pushToPullYield(seq))) {
            const b = yield _yield(f(item));
            if (!b) {
                return;
            }
        }
    };
}

const { next: map_next } = pushToPullYield(mapPushYield1(rangeYield(500, 3), x => "$" + x));

// console.log(map_next());
// console.log(map_next());
// console.log(map_next());
// console.log(map_next());


// - implementacja wykorzystujaca yield
// - tutaj nie mamy juz do czynienia z Promise wiec w sposob synchroniczny mozemy jednoznaczenie (dokladnie w momencie ktorym chcemy) wznawiac
// wykonanie generatora
// - tutaj tylko dla analogii z iteratorami typu push z go pozostawilismy funkcje _yield zweacajaca bool, chodzi o to aby skladoniowo zachowac 
// podobienstwo do poprzednich przykladow bo my bardziej probujemy implementowac podejscie go jak po prostu typowe uzycie generatorow js
// - faktycznie funkcja _yield przekazana do funkcji iteratora to 'identity' czyli nic nie robi, ale samo iterowanie wykonujemy przekazujac 
// "iterator.next(true)" czyli finalnie kod iteratora dowiaduje sie tym czy isc dalej czy nie, moze np posprzatac na koniec
// - probowalem na koniec napisac sobie powiedzy map/... z yield w taki sposob aby przypominala implementacje map dla "push iterators" tzn
// aby generatory/yield uzyte byl jakby tylko hack/szczegol_implementacyjny, ale finalnie srednio sie udalo bo od razu w kodzie widac
// generator ktorego jedyny sposob to iteracja czyli musimy jakby bezposrdnio z nim pracowac
// - sama moja implementacja "Map" w go korzysta z petli "for item := range ..." czyli wygodnego iterowania jakby "foreach", wiec tak to
// zaimplementowalem na koncu w 'mapPushYield2'

