
// - Iterable<T> type
let iterable: Iterable<number> = [1, 2, 3];

for (const item of iterable) {
    console.log(item);
}

// - Iterator<T>
let iterator = iterable[Symbol.iterator](); // creating new iterator, it's like "iterable.getIterator()"
console.log(iterator.next());
console.log(iterator.next());
console.log(iterator.next());
console.log(iterator.next());

// -- for/of loop and operator spread work only with Iterable<T> (not Iterator<T>)
//for(const item of iterator){ } // Type 'Iterator<number, any, undefined>' must have a '[Symbol.iterator]()' method that 
//[...iterator]; // Type 'Iterator<number, any, undefined>' must have a '[Symbol.iterator]()' method that returns an iterator.


// - for/of "under the hood"
iterator = iterable[Symbol.iterator]();
let iteratorResult: IteratorResult<number>;

while (!(iteratorResult = iterator.next()).done) {
    console.log(iteratorResult.value)
}

// - Is Iterator<T> type itself is lazy ? (yes)
const infiniteIterator: Iterator<number> = {
    next() {
        return { done: false, value: 666 }
    }
};
console.log(infiniteIterator.next());
console.log(infiniteIterator.next());

// - so what gives us Iterable<T> type ?
// -- we can iterate many times over the same collection
// -- many instances object of type Iterator<T> can exist at the same time

iterable = [1, 2, 3];
const iterator1 = iterable[Symbol.iterator]();
const iterator2 = iterable[Symbol.iterator]();
console.log(iterator1.next());
console.log(iterator2.next());
console.log(iterator2.next());

// - generator function (function*) returns type that is simultaneously Iterable<T> and Iterator<T> but
// -- still we can iterate over elemenets only once (the same instance of Iterator<T> is returned from Iterable<T>)
// -- so why generator result is also Iterable<T> ? (maybe because of for/of loop and spread operator)
function* return123() {
    yield 1;
    yield 2;
    yield 3;
}

const generatorResult = return123();

iterable = generatorResult; // Iterable<T> type
iterator = generatorResult; // Iterator<T> type

console.log(iterable[Symbol.iterator]() === iterable[Symbol.iterator]()); // true
console.log(iterable[Symbol.iterator]() === iterator); // true

// - TS defines IterableIterator<number> type that combines 2 interfaces (Iterable, Iterator) ...
// -- ... and this type is used in all places in ES6 API (Array|Map|Set.values|keys|entries)
// -- so it works the same as result returned from generator function (iterates only once)

const array = [1, 2, 3];
const iterableIterator1: IterableIterator<number> = array.values();

console.log([...array]); // [1,2,3]
console.log([...array]); // [1,2,3]

console.log([...iterableIterator1]); // [1,2,3]
console.log([...iterableIterator1]); // []

const functions = [
    Array.prototype.keys, Array.prototype.entries, Array.prototype.values,
    Map.prototype.keys, Map.prototype.entries, Map.prototype.values,
    Set.prototype.keys, Set.prototype.entries, Set.prototype.values,
];


// - what does Set have methods like 'keys' and 'entries; for ?
// -- The keys() method is exactly equivalent to the values() method.
// -- The entries() method of Set instances returns a new set iterator object that contains an array of [value, value] 
// for each element in this set, in insertion order. For Set objects there is no key like in Map objects. However, to keep the API similar
// to the Map object, each entry has the same value for its key and value here, so that an array [value, value]  is returned.


