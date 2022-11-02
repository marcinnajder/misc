## Programming with sequences part 1 - introduction to powerseq


#### Introduction

Functional programming gives us many wonderful things, one of best things for me is the way we can work with collections of data. Every developer, day in and day out, loops over collections, aggregates some values, maps one type of collection to the other type of collection. If you change the way you look at those kinds of operations, everything will be changed for you. It will have a bigger impact on you programming skills than many other fancy libraries, tips, patterns, tools, architectures, etc. It lets you write a simple, concise, easy to understand and maintain code.

In this article we will be talking about lazy sequences of values and the operators we can perform over them. All code samples will be written in JavaScript but you can easily translate this knowledge to any other language or runtime. I am aware that most of the developers know and use operators like map, filter, reduce in their code but I also saw so many times bad or even completely unnecessary usage of those operators. The true power comes from the amount of operators available for us and the way we can compose them together.

First I try to explain why declarative style of programming is better than imperative style (at least for me). Then we will extract some typical code snippets into equivalent operators that we implement ourself. We can treat this article as an introduction to library [powerseq](https://github.com/marcinnajder/powerseq). In the following parts of the series I will be showing the real world scenarios implemented using sequence operators. But to be honest, it's not about any particular library or even programming language, it's about the way of thinking.

#### Sequences vs collections (iterators, generators)

Let's take a look at `for/of` loop in JavaScript

```javascript
const numbers = [1, 5, 3, 9, -1, 5, -12, 0, 44, 12, -100];

for (var n of numbers) {
	console.log(`${n} zl`);
}
```

Looping over collection of items using `for/of` is the same as executing the following code 

```javascript
const iterable = numbers;
const iterator = iterable[Symbol.iterator]();
let result;

while (!(result = iterator.next()).done) {
    console.log(`${result.value} zl`);
}
```

It's a JavaScript way of implementing the "iterator design pattern" that can be expressed as a TypeScript interfaces `Iterable, Iterator`

```typescript
interface Iterable<T> {
    [Symbol.iterator](): Iterator<T>;
}
interface Iterator<T> {
    next(): { done: boolean; value: T }
}
```

`Iterable<T>` interface represents a **sequence** of values. In contrast to the typical **collection** of values like `Array`, the sequence does not have a property `length` returning number of items. Sequence is a lazy evaluated generator of values than can be possible infinite. The consumer of sequence decides how many items it needs to process, it can always stop processing any time escaping the loop calling `break` or `return`. 

Because `Array` type implements `Iterable<T>` interface so we iterate over the items of array using `for/of` loop. We can always create our own objects implementing `Iterable<T>` 

```javascript
function range(start, count) {
    return {
        [Symbol.iterator]() {
            const end = start + count;
            let i = start;
            return {
                next() {
                    return i < end ? { done: false, value: i++ } : { done: true };
                }
            }
        }
    };
}

var numbers = range(1, 5);
console.log(Array.from(numbers).join()); // -> 1,2,3,4,5 
console.log(Array.from(numbers).join()); // -> 1,2,3,4,5 
```

This function returns an object that creates a sequence of `count`  numbers starting with `start`.  We can iterate over the sequence or convert the sequence into an array using builtin static metod `Array.from(numbers)` or spread operator `[...numbers]`.  Returned object generates values only on demand, so we can even call `range(1,Number.MAX_VALUE)` and nothing happens. It's just a recipe for generating values, not the values itself.

```javascript
const numbers = range(1, Number.MAX_VALUE);
for (const number of numbers) {
    if (number > 5) { // leave the loop
        break;
    }
    console.log(number);
}
```

Writing objects implementing `Iterable<T>` interface manually is cumbersome. As we saw above, the code is very verbose and hard to understand, especially when the complexity of operation grows. This is why the creators of JavaScript language gave us a feature called generators. The same function `range` can be written as follows

```javascript
function* range(start, count) {
    const end = start + count;
    for (let i = start; i < end; i++) {
        yield i;
    }
}

const numbers = range(1, 5);
console.log(Array.from(numbers).join()); // -> 1,2,3,4,5 
console.log(Array.from(numbers).join()); // -> <no items> !
```

The generator function must be marked with an asterisk at the end of `function*` keyword. This means that function returns a JavaScript object implementing `Iterable<T>` interface. In addition to regular `return ...` (returning the last value in sequence) we can use `yield ...` every time our function generates new value. We have to get used to a new syntax but with time code is much more easer to write and understand comparing to manual implementation of `Iterable<T>` interface. As an example, take a look how natural implementation of infinite sequence looks like

```javascript
function* repeat(value) {
    while (true) {
        yield value;
    }
}
```

I'm not sure if you notice one small detail in the code above. The generator function creates an iterable object `numbers`, then we iterate over all items in the sequence down to the last item. If we try to iterate the same object again, we won't get any item. It was quite surprising when I encountered this behavior for the first time. In some languages like C#, F#, dart, ... generator function produces values from the beginning every time we start a new iteration. JavaScript (and for example Python) took a different approach, the second iteration process continues the previous one. It's because generator function creates iterable object that returns exactly the same instance of iterator object every time we execute `iterable[Symbol.iterator]()` method . I personally prefer the behavior from languages like C#, F#, dart, .. so I implemented it in `powerseq` library.

#### filter

Let's take a look at the following code

```javascript
const numbers = [1, 5, 3, 9, -1, 5, -12, 0, 44, 12, -100];

for (const n of numbers) {
    if (n > 0) {
        console.log(`${n} zl`);
    }
}
```

It's a very typical loop iterating over collection of items and performing some operation only for selected items. We could implement the same functionality using builtin into `Array` method called `filter`.

```javascript
for (const n of numbers.filter(n => n > 0) {
	console.log(`${n} zl`);
}
```

For me the second approach is definitely better. In the fist approach we filter collection of items using `for/of` loop.  The loop is a very primitive and general purpose structure, we can do anything with it. It's like a reverse engineering, we have to read every loop carefully to understand the intent of the programmer. In the second case, we immediately see what we are trying to achieve. But there is a small issue with this approach too. We call `filter` method and that creates a new temporary array in memory only to iterate over it once. This array has to be cleaned up by the garbage collector. To eliminate this small waste of resources, we can implement our own `filter` function based on JavaScript generators.

```javascript
function* filter(items, f) {
    for (const item of items) {
        if (f(item)) {
            yield item;
        }
    }
}

for (const n of filter(numbers, n => n > 0)) {
	console.log(`${n} zl`);
}
```

Now, the code is readable and doesn't introduce unnecessary array anymore.

#### map

We can go even further looking at the code above . We iterate over collection of numbers only to convert then into collection of strings. Once again, it's a very typical pattern we can find very often inside our code. We could use `map` method of `Array` type, but again it would introduce a temporary array. Let's implement our own `map` function


```javascript
function* map(items, f) {
    for (const item of items) {
        yield f(item);
    }
}

const positiveNumbers = filter(numbers, n => n > 0);
const positiveCurrencies = map(positiveNumbers, n => `${n} zl`);

for (const of positiveCurrencies) {
	console.log(c);
}
```

#### pipe

The final result looks like a SQL query, `filter` operator is a `WHERE` clause, `map` operator is a `SELECT` clause. We can image many additional operators like `distinct`, `max`, `join` and so on. You could be wondering, is there any simple way to avoid introducing temporary variables like `positiveNumbers` or `positiveCurrencies`? Yes, it is and it's very simple thanks to `pipe` function 

```javascript
function pipe(value, ...funcs) {
    return funcs.reduce((v, f) => f(v), value);
}

pipe(10, v => v + 1, v => v * 10, v => v.toString()); // -> "110"    
```

`pipe` function takes a `value` and any number of functions `funcs`, each function takes one argument and returns one result. `pipe` function calls the first function from `funcs` collection passing the `value` argument, returned result is passed to the second function, and so on and so forth. The final result becomes the result returned from `pipe` function. In many programming languages like OCaml, F#, ELM `pipe` function is available as an `|>` infix operator and it is crucial part of any piece of code. One day it will be also available in (JavaScript language)[https://github.com/tc39/proposal-pipeline-operator]. Now let's look how `pipe` function can help us with our previous example

```javascript
const positiveCurrencies = pipe(numbers,
	x => filter(x, n => n > 0),
	x => map(x, n => `${n} zl`) );
```

It's slightly better because we don't have to introduce temporary variables and the whole piece of code is a one single expression. Previously, there were two separated statements, declaration and initialization two variables. But still it doesn't look perfect, we can do better. Let's change the implementation of `filter` and `map` functions to support new overloads 

```javascript
function filter(...args) {
    return args.length === 2 ? filter_(...args) : s => filter_(s, ...args);

    function* filter_(items, f) {
        let index = 0;
        for (const item of items) {
            if (f(item, index)) {
                yield item;
            }
            index++;
        }
    }
}

function map(...args) {
    return args.length === 2 ? map_(...args) : s => map_(s, ...args);

    function* map_(items, f) {
        let index = 0;
        for (const item of items) {
            yield f(item, index);
            index++;
        }
    }
}

const positiveCurrencies = pipe(numbers, filter(n => n > 0), map(n => `${n} zl`) );
```

Now we can use those operators in one of two ways: as a stand-alone operator passing two arguments or as an operator used inside`pipe` call passing one argument. We have also introduced small additional feature, we pass optional  `index` argument next to the `item` argument to function `f` .

#### toarray

In our example we have printed to the console positive numbers in a currency format, but maybe we would like to create an array containing the final result. Let's introduce a helper function called `toarray`

```javascript
function toarray(...args) {
    return args.length === 1 ? toarray_(...args) : s => toarray_(s);

    function toarray_(items) {
        return Array.isArray(items) ? items : Array.from(items);
    }
}

pipe(numbers, filter(n => n > 0), map(n => `${n} zl`), toarray() );
// -> [ '1 zl', '5 zl', '3 zl', '9 zl', '5 zl', '44 zl', '12 zl' ]
```

#### take

At the end let's add final operator called `take`. It takes two arguments, the sequence of items and the number `n`.  It returns a new sequence containing `n`  first items from the original sequence. So it's like a `TOP` operator in SQL.

```javascript
function take(...args) {
    return args.length === 2 ? take_(...args) : s => take_(s, ...args);

    function* take_(items, n) {
        var i = n;
        if (i > 0) {
            for (var item of items) {
                yield item;
                if (--i === 0) {
                    return;
                }
            }
        }
    }
}

const numbers = [1, 5, 3, 9, -1, 5, -12, 0, 44, 12, -100];
[...take(numbers, 4)]; // -> [ 1, 5, 3, 9 ]
```

#### The power of lazy evaluation

The code below compares side by side two implementation of the same task. The first one is using builtin `Array` methods, the second one is using our functions implemented previously.

```javascript
const numbers = [1, 5, 3, 9, -1, 5, -12, 0, 44, 12, -100];

// 1. using builtin Array methods
var result = numbers
    .filter(n => n > 0)
    .map(n => `${n} zl`)
    .slice(0, 5);
// -> [ '1 zl', '5 zl', '3 zl', '9 zl', '5 zl' ]

// 2. using functions based on sequences
var result = pipe(numbers,
    filter(n => n > 0),
    map(n => `${n} zl`),
    take(5),
    toarray()
); // -> [ '1 zl', '5 zl', '3 zl', '9 zl', '5 zl' ]
```
The final result is exactly the same. In terms of the number of lines of code, the first one is even shorter. But what is the difference during the execution ?

In case of the first query, new array is created after each step, after `filter` and `map`  two new arrays are created completely unnecessary. But what is even worse, lambda functions passed to the methods are executed for all items stored  in arrays. In our example, input array contains 11 items, function passed to `filter` is executed 11 times (for all items), function passed to `map` is executed 8 times (for all items returned by `filter`). Just think how many times those functions would be executed for input array containing 1000 items when in fact we need only first 5 items?

In case of the second query, operators like `filter`, `map` and `take` are lazy. The last operator `toarray` kicks off the execution. Firstly, code inside `take` function is executed,  `take` pulls item from `map`,  `map` pulls item from `filter`. `filter` takes the first number `1` and calls predicate function `n => n > 0`. `1` is greater than `0` so it's returned further to `map`, `map` calls converter function ``n => `${n} zl` ``. Value `1 zl` is returned to `take` which is returned as a first element of the final result. So to produce the first value, the lambda functions had to be called only twice. If we analyze the execution process further we will see that the lambda functions are called minimum number of times necessary to produce the final collection. In addition, no temporary array that needs to be cleaned up is created in the meantime.

#### Summary

Let's recap what we have learned:
- starting with ES6 iterator pattern is supported directly in JavaScript as an API of iterable objects and a new kind of loop `for/of`, generator function `function*` was also introduced and it is a very convenient way of implementing iterable objects
- a typical imperative code contains variables, loops, ifs, ... and very often we can write the same code using functions like `filter`, `map`, `reduce`, such a declarative code is easer to read, understand and maintain 
- builtin type `Array` provides a few functions like `filter`, `map`, `reduce` ... but it can be not enough, especially once you had an experience with some functional programming languages, there is no elegant way do add a new operators to existing `Array` type
- we can write our own lazy operators using generator function `map`, `filter`, ... and combine them using `pipe` function in a very composable way
- lazy evaluation helps us write more performant code, but also let us divide a problem into smaller reusable functions and glue them together at the end as a final solution, it's because the consumer of sequence decides how and when the sequence will be executed
- finally, [powerseq](https://github.com/marcinnajder/powerseq) is a set of operators working with lazy sequences, the API is based on [LINQ](https://en.wikipedia.org/wiki/Language_Integrated_Query)  which is based on functional programming like Haskell and OCaml, it is not coincident why those operators are so powerful, the are "standing on the shoulders of giants"

In the next articles we will take a look closer to a new kind of operators and we will solve some real word problems using this declarative style of programming. 
