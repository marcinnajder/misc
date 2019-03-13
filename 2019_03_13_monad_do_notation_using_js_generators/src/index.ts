import { just, nothing, Maybe } from "./maybeMonad";
import { Result, error, ok } from "./resultMonad";
import { do_, do__ } from "./monad";
import { maybeMonadOps, promiseMonadOps, arrayMonadOps, flatmap, iterableMonadOps, resultMonadOps } from "./monadImplementations";

const samples = {
    maybeWithBind,
    maybeWithGenerator,
    resultWithBind,
    resultWithGenerator,
    promiseWithThen,
    promiseWithGenerator,
    arrayWithFlatmap,
    arrayWithGenerator,
    iterableWithFlatmap,
    iterableWithGenerator,
};
executeSamples(samples);


async function executeSamples(samplesObj: any) {
    for (const sampleName of Object.keys(samplesObj)) {
        console.log("--> ", sampleName);
        await Promise.resolve(samplesObj[sampleName]());
        console.log();
    }
}


// maybe

function tryParse(text: string): Maybe<number> {
    return /^-{0,1}\d+$/.test(text) ? just(parseInt(text)) : nothing<number>();
}

function maybeWithBind() {
    const result = tryParse("10")
        .bind(v1 => tryParse("50").bind(v2 => just(v1 + v2)))
        .bind(v3 => just(v3.toString()));

    console.log(result);
}

function maybeWithGenerator() {
    function* generator(): Iterator<Maybe> {
        const v1: number = yield tryParse("10");
        const v2: number = yield tryParse("50");
        const v3 = (v1 + v2).toString();
        return just(v3);
    }

    console.log(do_<Maybe<number>>(generator, maybeMonadOps));
}


// result

function tryParseR(text: string): Result<number, string> {
    const numberOption = tryParse(text);
    return numberOption.type === "just" ? ok<number, string>(numberOption.value) :
        error<string, number>(`'${text}' value is not a correct number`);
}

function resultWithBind() {
    const result = tryParseR("10")
        .bind(v1 => tryParseR("50").bind(v2 => ok(v1 + v2)))
        .bind(v3 => ok(v3.toString()));

    console.log(result);
}

function resultWithGenerator() {
    function* generator(): Iterator<Result> {
        const v1: number = yield tryParseR("10");
        const v2: number = yield tryParseR("50");
        const v3 = (v1 + v2).toString();
        return ok(v3);
    }
    console.log(do_<Result<number, string>>(generator, resultMonadOps));
}


// promise

function delay<T = undefined>(duration: number, value?: T): Promise<T> {
    return new Promise<T>((resolve, reject) => {
        setTimeout(() => {
            resolve(value);
        }, duration);
    });
}


function promiseWithThen() {
    const result = delay(100, 10)
        .then(v1 => delay(100, 50).then(v2 => Promise.resolve(v1 + v2)))
        .then(v3 => Promise.resolve(v3.toString()));

    return result.then(v => console.log(v));
}

function promiseWithGenerator() {
    function* generator(): Iterator<Promise<any>> {
        const v1: number = yield delay(100, 10);
        const v2: number = yield delay(100, 50);
        const v3 = (v1 + v2).toString();
        return Promise.resolve(v3);
    }

    return do_<Promise<number>>(generator, promiseMonadOps).then(v => console.log(v));
}


// arrays


/*

from i in [1,2,3]
from j in ["a","b"]
select i + j;

foreach(var i in [1,2,3]){
    foreach(var j in ["a", "b"]){
        yield i + j;
    }
}

var i = yield [1,2,3]
var j = yield ["a", "b"];
yield [i, j];
*/

function arrayWithFlatmap() {
    const result = [1, 2, 3]
        .flatmap(i => ["a", "b"].flatmap(j => [i + j]));

    console.log(result);
}

function arrayWithGenerator() {
    function* generator(): IterableIterator<any[]> {
        const i: number = yield [1, 2, 3];
        const j: string = yield ["a", "b"];

        // let j: string;
        // if (i % 2 === 0) {
        //     j = yield ["a", "b"];
        // } else {
        //     j = yield ["c", "d"];
        //     j = j.repeat(2);
        // }

        return [i + j];
    }

    const a = do__<number[]>(generator, arrayMonadOps);
    console.log(a);
}

// iterable

function* toIterable<T>(array: T[]): Iterable<T> {
    for (const item of array) {
        yield item;
    }
}

function iterableWithFlatmap() {
    const result =
        flatmap(toIterable([1, 2, 3]), i => flatmap(toIterable(["a", "b"]), j => toIterable([i + j])));

    console.log([...result]);
}

function iterableWithGenerator() {
    function* generator(): IterableIterator<Iterable<any>> {
        const i: number = yield toIterable([1, 2, 3]);
        const j: string = yield toIterable(["a", "b"]);
        return toIterable([i + j]);
    }

    const a = do__<Iterable<number>>(generator, iterableMonadOps);
    console.log([...a]);
}

