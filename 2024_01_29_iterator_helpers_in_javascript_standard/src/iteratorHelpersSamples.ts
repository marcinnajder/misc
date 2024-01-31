import { distinct, pipe } from "powerseq";
import { Iterator_ } from "./iteratorHelpers";


// - https://github.com/tc39/proposal-iterator-helpers?tab=readme-ov-file#mapmapperfn
function* naturals() {
    let i = 0;
    while (true) {
        yield i;
        i += 1;
    }
}
const result = naturals().map(value => { return value * value; });

console.log(result.next()); //  {value: 0, done: false};
console.log(result.next()); //  {value: 1, done: false};
console.log(result.next()); //  {value: 4, done: false};


// - https://github.com/tc39/proposal-iterator-helpers?tab=readme-ov-file#iteratorfromobject

Iterator_.from({
    next() {
        return { done: false, value: 666 }
    }
}).drop(5).take(10).toArray();


function* range(start: number, count: number) {
    const end = start + count;
    for (let i = start; i < end; i++) {
        yield i;
    }
}


// generators
const numbers = range(0, 5);

console.log(Iterator_.from(numbers).filter(x => x % 2 === 0).map(x => x.toString()).toArray());
console.log(Iterator_.from(numbers).filter(x => x % 2 === 0).map(x => x.toString()).toArray()); // [] !

console.log(range(0, 5).filter(x => x % 2 === 0).map(x => x.toString()).toArray());


// Array
console.log(Iterator_.from([0, 1, 2, 3, 4]).filter(x => x % 2 === 0).map(x => x.toString()).toArray());
console.log(Iterator_.from([0, 1, 2, 3, 4].values()).filter(x => x % 2 === 0).map(x => x.toString()).toArray());

console.log([0, 1, 2, 3, 4].values().filter(x => x % 2 === 0).map(x => x.toString()).toArray());


// Set, Map
console.log(new Set([0, 1, 2, 3, 4]).values().filter(x => x % 2 === 0).map(x => x.toString()).toArray());
console.log(new Map([[0, "0"], [1, "1"], [2, "2"], [3, "3"], [4, "4"]]).keys().filter(x => x % 2 === 0).map(x => x.toString()).toArray());
// console.log(new Set([0, 1, 2, 3, 4]).filter(x => x % 2 === 0).map(x => x.toString()).toArray()); // Property 'filter' does not exist on type 'Set<number>'.


// operators
console.log(range(0, 5).take(3).reduce((total, value) => total + value, ""));

console.log(range(0, 5).flatMap(x => [x, x]).toArray());

range(0, 5).foreach(x => {
    console.log(x, "!!!");
});




// pipe - composing with custom operators

function distinct_<T>(iterator: Iterator<T>) {
    return Iterator_.from(distinct({ [Symbol.iterator]: () => iterator }));
}
const result1 = pipe(
    [1, 2, 2, 3, 4, 4].values().filter(x => x % 2 === 0),
    iter => distinct_(iter).toArray()
);

const result2 = [1, 2, 2, 3, 4, 4].values().filter(x => x % 2 === 0).pipe_(iter => distinct_(iter)).toArray()

console.log(result1); // [2, 4]
console.log(result2); // [2, 4]