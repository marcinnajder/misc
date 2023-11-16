var { pipe, find, empty, filter, concat, range, map, distinct, toarray, take } = require("powerseq");

// quick sort algorithm using lazy powerseq operators in not so quick :)

function qs(xs) {
    const v = find(xs);
    return v === undefined ? empty() : concat(qs(filter(xs, x => x < v)), [v], qs(filter(xs, x => x > v)));
}

[...qs([10, 5, 9, 2, 4, 8, 0])] // -> [ 0, 2, 4, 5, 8, 9, 10 ]



function measureTime(f) {
    return pipe(Date.now(), start => (f(), Date.now() - start));
}

function randomNumbers(n = 5000) {
    return pipe(range(1, Number.MAX_VALUE), map(_ => Math.floor(Math.random() * 10000)), distinct(), take(n), toarray());
}

measureTime(() => {
    console.log(randomNumbers().sort((a, b) => a - b));
}); // 10 ms

measureTime(() => {
    console.log([...qs(randomNumbers())]);
}); // 5422 ms (95/200 ms after overriding 'filter' function)




// ** explanation

function wrapInIterable(generator) {
    return { [Symbol.iterator]: generator };
}

function filter(items, f) {
    return wrapInIterable(function* () {
        console.log(" filter: ", f.toString().replace("v", items.v));
        yield* require("powerseq").filter(items, f);
    });
}

function qs(xs) {
    const v = find(xs);
    console.log("find: ", v);
    xs.v = v;
    return v === undefined ? empty() : concat(qs(filter(xs, x => x < v)), [v], qs(filter(xs, x => x > v)));
}

[...qs([10, 5, 9, 2, 4, 8, 0])]


// explanation
// - filter and concat from powerseq are lazy, find forces execution anything
// - we have added printing to console just after 'find' call and during starting execution of 'filter' operator 
// - because of the lazy execution of 'filter' we have to read log in reverse order
// -- "find:  10" -> just take the fist element from the original 'array'
// -- "find:  5" ->  'array'.filter(x => x < 10).find()
// -- "find:  2" ->  'array'.filter(x => x < 10).filter(x => x < 5).find()
// -- "find:  0" ->  'array'.filter(x => x < 10).filter(x => x < 5).filter(x => x < 2).find()
// -- ...
// - because of laziness and recursion we just "record" the code to be executed later calling many times '.filter()', 'find' starts the execution
// but we run all predicates many times over and over again for each '.find' call



// > [...qs([10, 5, 9, 2, 4, 8, 0])]
// find:  10
//     filter:  x => x < 10
// find:  5
//     filter:  x => x < 5
//     filter:  x => x < 10
// find:  2
//     filter:  x => x < 2
//     filter:  x => x < 5
//     filter:  x => x < 10
// find:  0
//     filter:  x => x < 0
//     filter:  x => x < 2
//     filter:  x => x < 5
//     filter:  x => x < 10
// find:  undefined
//     filter:  x => x > 0
//     filter:  x => x < 2
//     filter:  x => x < 5
//     filter:  x => x < 10
// find:  undefined
//     filter:  x => x > 2
//     filter:  x => x < 5
//     filter:  x => x < 10
// find:  4
//     filter:  x => x < 4
//     filter:  x => x > 2
//     filter:  x => x < 5
//     filter:  x => x < 10
// find:  undefined
//     filter:  x => x > 4
//     filter:  x => x > 2
//     filter:  x => x < 5
//     filter:  x => x < 10
// find:  undefined
//     filter:  x => x > 5
//     filter:  x => x < 10
// find:  9
//     filter:  x => x < 9
//     filter:  x => x > 5
//     filter:  x => x < 10
// find:  8
//     filter:  x => x < 8
//     filter:  x => x < 9
//     filter:  x => x > 5
//     filter:  x => x < 10
// find:  undefined
//     filter:  x => x > 8
//     filter:  x => x < 9
//     filter:  x => x > 5
//     filter:  x => x < 10
// find:  undefined
//     filter:  x => x > 9
//     filter:  x => x > 5
//     filter:  x => x < 10
// find:  undefined
//     filter:  x => x > 10
// find:  undefined





// ** overriding 'filter' function

// 1. overriding 'filter' function, making it eager instead of being lazy
var filter = (...args) => Array.from(require("powerseq").filter(...args));


function memoize(iterable) {
    return {
        iterator: undefined,
        results: [],
        [Symbol.iterator]() {
            this.iterator ??= iterable[Symbol.iterator]();
            return {
                __proto__: this,
                i: 0,
                next() {
                    if (this.i < this.results.length) {
                        return this.results[this.i++];
                    }

                    const last = this.results[this.i - 1];
                    if (last?.done) {
                        return last;
                    }

                    return this.results[this.i++] = this.iterator.next();
                }
            };
        }
    };
}


function* return123() {
    yield 1;
    yield 2;
    return 3;
}

// var return123_ = return123();
var return123_ = memoize(return123());

// for (let item of return123_) {
//     console.log(item);
// }

var iterator1 = return123_[Symbol.iterator]();
iterator1.next();

var iterator2 = return123_[Symbol.iterator]();
iterator2.next();

var iterator3 = return123_[Symbol.iterator]();
iterator3.next();


// 2. overriding 'filter' function version that caches generated values
var filter = (...args) => memoize(require("powerseq").filter(...args));
