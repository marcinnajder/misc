# Clojure for JavaScript developers part 1 - ... [Clojure, JavaScript]

todo: zmienić adres https://marcinnajder.github.io/2022/11/02/sequences-in-javascript-part-1-introduction-to-powerseq.html generujac nowa stronce https://github.com/marcinnajder/powerseq


## Introduction

> Lisp is a family of programming languages with a long history and a distinctive, fully parenthesized prefix notation. Originally specified in the late 1950s, it is the second-oldest high-level programming language still in common use, afterFortran.

That is an excerpt from wiki page about [Lisp](https://en.wikipedia.org/wiki/Lisp_(programming_language) programming language. Throughout so many years so many programmers felt in love with Lisp again and again. There is some magic in that specified language, it just looks completely different comparing to almost any other language. It's dynamic and functional, considered as a scripting language, favoring immutable data structures and recursion. REPL (read-evaluate-print-loop) support is almost legendary.

It's really interesting that many programmers (me too) see [Lisp in JavaScript](https://www.crockford.com/javascript/javascript.html).

>Lisp in C's Clothing - JavaScript's C-like syntax, including curly braces and the clunky for statement, makes it appear to be an ordinary procedural language. This is misleading because JavaScript has more in common with functional languages like Lisp or Scheme than with C or Java. It has arrays instead of lists and objects instead of property lists. Functions are first class. It has closures. You get lambdas without having to balance all those parens.

In this series I will try to explain Lisp fundamentals using JavaScript language. We will see a piece of List code  (it will be the [Clojure](https://en.wikipedia.org/wiki/Clojure) Lisp dialect exactly) side by side with JavaScript code. Source code explains faster and better than a milion words. I hope that in the end of the series you will gain a new skill called "can read Lisp code" and you appreciate the beauty of this unique ancestor of your programming language of choice. Lisp introduced so many [features](https://www.paulgraham.com/diff.html) so obvious today but in a very elegant and simple form.


## JS helper functions

Let's start by defining a few helper JS functions. Functions like `pipe, map_, filter_, skip_, ...` come from my [poweseq](https://github.com/marcinnajder/powerseq) library and there are operators working with a lazy sequences (iterator API) in JS. You can read more about the library in this [series](https://marcinnajder.github.io/2022/11/02/sequences-in-javascript-part-1-introduction-to-powerseq.html) of articles. 

```js
var fs = require("fs");
var os = require("os");
var { pipe, skip: skip_, elementat: elementat_, find: find_, some: some_, map: map_, filter: filter_, repeatvalue: repeatvalue_ } = require("powerseq");

function wrongArgType(arg) {
    const argType = typeof arg !== "object" ? typeof arg : arg.constructor.name;
    throw new Error(`function '${wrongArgType.caller.name}' was called with argument of unexpected type '${argType}'`);
}

var throww = message => { throw new Error(message); };
```

In general, functions like `map, filter, reduce` working with collections were introduced by Lisp and there are the part of standard library. We will implement them from scratch, that's way imported functions from powerseq are named with `_` postfix. The standard `throw` keyword in JS a statement (not expression - it's not returning any value), that's way function `throww` was introduced - calling a function is an expression. JavaScript and Lisp are dynamic. It's very easy to pass a function argument with invalid data type and  `wrongArgType` function helps tracking such mistakes. It's 
a little magical, it always throws an error with message text including the name of function calling our `wrongArgType`. 

## Syntax, global variables, builtin types

Not many programming languages support something like "global state" or "environment". It's like a map of variables available everywhere in code where we can add, remove, change and access any variables at any time. 

```scheme
(def first-name "marcin")
(println "my name is:" first-name)
```

```js
var firstName = "marcin";
console.log("my name is:", firstName);
```

This is the first code snippet of Lisp. The syntax looks more like human readable text data format (JSON or XML) then a typical programming language. Any piece of Lisp code looks like this `(def first-name "marcin")` , it's a list of items where the first item specifies the type of operation that is performed. In our example, `def` means "define global variable" with a name `first-name` and a value `"marcin"`. In JS we can also define global variable directly inside a script put on a website, as an opposite to variables inside functions which are treated as local variables. JS is dynamic like Lisp, it has data types like `number`, `string`, `boolean` , ... objects and collections. Exactly the same as Lisp. In dynamic languages we don't specify the types explicitly next to variables oraz function parameters, it's like specifying type `object` everywhere in statically typed languages like Java, C# or Go. But the types exists, they define a set of correct values (`1`, `2.2`, ... is a `number`, `"marcin"` is a `string`) and operations that can be performed with them (`+` operator can be used with `number` or `string`, but cannot be used with `boolean`).

## First-class functions

JS was created in 1995 and it was one of the first programming languages officially not considered as "functional" supporting "first-class functions". In object oriented programming term "method" is mostly used when we think about "function". Method is a just function defined in context of some type and later executed with combination of some object (concrete instance of type. But we would like to treat a function as a regular type like `number` or `string`. We would like to define a variable of function type, store it as an item inside some collection, and pass into or return from the other function. And all of that without connection to any type or object. Additionally, we would like to have ability to define a anonymous functions (aka "lambda expressions"), just by specifying function signature and body. Both JS and Lisp support that.

```scheme
(def add
  (fn [a b] (+ a b)))

(add 1 2) ;; => 3
(add 1) ;; => Execution error (ArityException) at .. Wrong number of args (1) passed to ...
((fn [a b] (+ a b)) 10 2) ;; => 12
```

```js
var add = (a, b) => a + b;

add(1, 2);
((a, b) => a + b)(1, 2);
```

`fn` symbol means that a new function is defined, it takes 2 arguments `a` and `b` and returns sum of them. After definition of `add` function,  we can place `add` as a first symbol in the list representing Lisp code, `(add 1 2)` executes our function passing values `1` and `2`. This is a quite unique feature of Lisp, we can extends "keyword" used in programming language. In some sense, there is no difference between `def`, `fn`, ... and our custom function called `add`. As we said before, the first item in the list represent performed operation, here `((fn [a b] (+ a b)) 10 2)`  is a completely valid code, often called "Immediately Invoked Function Expression" (IIFE).  

## defn, variadic functions, comments

It's too early to talk about macros in Lisp in details, so let's only say that we can define a function in special way using `defn` symbol. It's just a shortcut, instead of `(def add (fn [a b] (+ a b)))` we can write `(defn add [a b] (+ a b))`. Similarly, from the beginning of JS functions were defined using `function` keyword.

```scheme
(defn add [a b]
  (+ a b))

(macroexpand '(defn add [a b] (+ a b))) ;; => (def add (clojure.core/fn ([a b] (+ a b))))
```

```js
function add(a, b) {
    return a + b;
}
```

Variadic functions allows to handle variable number of arguments. For instance, instead of passing exactly 2 arguments we can pass 2 or more arguments. 

```scheme
(defn add-at-least-2
  "function takes at least 2 arguments"
  [a b & rest-args]
  (+ a b (apply + rest-args)))

(add-at-least-2 1 2 3 4) ;; => 10
```

```js
/** function takes at least 2 arguments */
function addAtLeast2(a, b, ...restArgs) {
    return a + b + restArgs.reduce((x, y) => x + y, 0);
}
```

This feature is supported not only in JS but in almost all other existing programming languages. `&` symbol means that the following `rest-args` parameter will be a collection type. You can probably guess that calling`(apply + rest-args)` just sums up all items, but it will explained later. Optional function comments are defined just after the name of the function.

## Control flow (if/then/else, variables, code blocks, simple loops)

Now let's talk about a typical control flow constructs like local variable, conditions, loops. 

```scheme
(defn func-1 [a b]
  (if (> a b)
    (let [sum (+ a b)
          value (mod sum 10)]
      (println "value:" value)
      value)
    (do
      (dotimes [n 3]
        (println n "hej")
        (println n "yo"))
      (doseq [n '(3 4 5)]
        (println n "hej")
        (println n "yo"))
      666)))

(func-1 10 9) ;; => 9
(func-1 9 10) ;; => 666
```

```js
function func1(a, b) {
    if (a > b) {
        var sum = a + b;
        var value = sum % 10;
        console.log("value:", value);
        return value;
    } else {
        for (var n = 0; n < 3; n++) {
            console.log(n, "hej");
            console.log(n, "yo");
        }
        for (var n of [3, 4, 5]) {
            console.log(n, "hej");
            console.log(n, "yo");
        }
        return 666;
    }
}
```

It is hard to believe that programmers would like to write the code this way,  all that closing brackets `))))`  at the end. To be honest, I wrote some code in Clojure, with tools like formatters, linters, ... and  vs code editor, and very fast you can get used to.  In most cases, the programmer uses only few "builtin keywords" like `let, if, do, loop`, that's all. Brackets gives us a chance to nest some code structure inside the other code structure. For instance, if/then/else requires 3 parts `(if (> a b) a (+ b 10))`. The crucial details here is that each part of the code like `(... )` is an expression, so in contrast to statement it always returns some value. We can very easy compose smaller expressions in the bigger ones. It's one of the key pillars of functional programming, 


## Built-in functions

Next to the small number of "builtin keywords" like  `let, if, do, loop`, there are many small helper functions. In a "typical" programming language like JS, we think in terms of builtin operators like `+, -, >, ==, &&, ||` and existing functions like `parseInt, console.log, JSON.parse, Math.max`. In Lisp there is no difference between keywords, operators or functions, there are used in exactly the same way. Below you can find a list of standard builtin functions also implemented in JS. They could be grouped in categories: predicates like`nil?`or`string?`, arithmetic and logic operators like `+` or `*`, math functions like`inc` or `dec`, functions working with functions like `identity` or `constantly`, ... 

```scheme
(nil? nil) ;; => true
(string? "") ;; => true
(boolean? true) ;; => true
(int? 1) ;; => true
(fn? +) ;; => true
(vector? [1 2 3]) ;; => true

(+ 1 2 3) ;; => 6
(* 1 2 3) ;; => 6
(< 1 2) ;; => true

(inc 10) ;; => 11
(dec 10) ;; => 9
(mod 10 3) ;; => 1
(abs -10) ;; => 10
(max 5 10 15) ;; => 15

(zero? 0) ;; => true
(pos? 1) ;; => true
(even? 10) ;; => true
(odd? 11) ;; => true

(identity 10) ;; => 10
(identity "mama") ;; => "mama"

((constantly 666) 1) ;; => 666
((constantly 666) 1 2 3) ;; => 666

((complement empty?) [])   ;; => false

(and (> 2 1) (string/ends-with? "tata" "ta") false) ;; => false
(or (> 2 1) (string/ends-with? "tata" "ta") false) ;; => true

;; falshy - only false oraz nil
(and "" 0) ;; => 0
(and "" 0 false) ;; => false
(and "" 0 nil) ;; => nil

(or "" 0) ;; => ""
```

```js
var nilp = x => typeof x === "undefined" || x === null;
var stringp = x => typeof x === "string";
var booleanp = x => typeof x === "boolean";
var numberp = x => typeof x === "number";
var functionp = x => typeof x === "function";

var plus = (...args) => args.reduce((p, c) => p + c, 0);
var minus = (...args) => args.length === 1 ? -args[0] : args.reduce((p, c) => p - c);
var multiply = (...args) => args.reduce((p, c) => p * c, 1);
var mod = (...args) => args.reduce((p, c) => p * c, 1);
var inc = x => x + 1;
var dec = x => x - 1;
var mod = (x, div) => x % div;
var abs = x => Math.abs(x);
var max = (...xs) => Math.max(...xs);
var min = (...xs) => Math.min(...xs);
var evenp = x => x % 2 === 0;
var oddp = x => x % 2 === 1;
var zerop = x => x === 0;
var posp = x => x > 0;
var str = (...args) => args.join("");

var identity = x => x;
var constantly = x => () => x;
var complement = f => (...args) => !f(...args);

(2 > 1) && "tata".endsWith("ta") && false; // => false
(2 > 1) || "tata".endsWith("ta") || false; // => true

// ... in JS "" and 0 are also falsy
"" && 0; // => ""
"" && 0 && false; // => ""
"" && 0 && null; // => ""

"" || 0; // => 0
```

Characters like `?` or `-` can be used as a part of identifiers, for instance variable or function name.  There is also a naming convention for predicates (functions returning `boolean` values) with `?` at the end, like `(nil? ..)` or `(even? ...)`. 

Lisp uses `nil` values which are the same as `null` in many other programming languages. But there is also an additional concept called "falsy and truthy values", which I encountered so far only in JS. Boolean values are mostly used inside conditions for instance in `if` or `while` statements, "falsy and truthy values" mean that an expression representing condition does not have to be of type `boolean`. For instance, in JS values like `false`, `null`, `undefined`, `0`, `""` are treated as a "true" condition, and an expressions of any other type are "false".  That's why very often we can find the code like `var city = (person && person.address && person.address.city) || ""` , meaning "define new variable `city`  with value of `person.address.city` but set empty string `""` if `person` or `person.address` or `person.address.city` don't exists. In Clojure dialect of List "falsy and truthy values" work similar to JS, but not exactly the same. According to [the documentation](https://clojure.org/guides/learn/flow#_truth) : " In Clojure, all values are logically true or false. The only "false" values are `false` and `nil` - all other values are logically true".

The last interesting detail worth mentioning here is that many of the functions above are variadic functions. Calling them is quite convenient, because we can write conditions using expressions like `(and ... (or ... ... ...) ...)`, or arithmetic operations like `(+ 5 10 15)` , `(min 5 10 15)`. We will see further that the same functions can be called not only for fixed number of items but also for collection of items. We can sum all numbers stored in variable call `my-numbers` by calling `(apply + my-numbers)`.


## Summary 

At this point you have some basic understanding of Lisp language. I hope you can spot many similarities with JS, despite the syntax of course. Both of them are dynamic, support basic data types and even first-call functions. But they also support not so commonly known features like: global variables or "falsy and truthy values". 



## Introduction 

We already know how to define functions using basic types like `string`, `boolean` or `nil` in Lisp.  The syntax of the language is very different from the other programming languages but commonly used building block like variables, conditions and loops are the same. One of the main features of functional programming is treating functions as values. We can pass them as arguments or return from the other functions, there are names "heiher-

#### 'apply' function

```scheme
(apply add [1 2]) ;; => 3
(apply * [2 3]) ;; => 6
```

```js
add.apply(null, [1, 2]);
```

#### 'partial' function

```scheme
(def increment
  (partial add 1))
(increment 10) ;; => 11
((partial add 1) 10) ;; => 11
```

```js
var inc = add.bind(null, 1);
inc(10); // => 11
add.bind(null, 1)(10); // => 11
```

#### 'comp' function (function composition)

```scheme
(def increment-twice-then-to-string
  (comp str inc increment))

(increment-twice-then-to-string 10) ;; => "12"
```

```js
var comp = (...funcs) => arg => funcs.reduceRight((a, f) => f(a), arg);

var incrementTwiceThenToString = comp(str, inc, inc);
incrementTwiceThenToString(10); // => "12"
```





## Macros

```scheme
(def forms-1 '(/ (+ 10 5) 3))

(let [[op-1 [op-2 a b] c] forms-1]
  (println "operators:" op-1 op-2) ;; operators: / +
  (println "values: " a b c) ;; values:  10 5 3
  `(/ ~a ~b)) ;; => 2
```

```js
var a = 10;
var b = 5;
eval(`${a} / ${b}`);
```





#### Immutable singly-linked list

```scheme
(def empty-list-1 '())
(def empty-list-2 (list))

(= empty-list-1 empty-list-2) ;; => true
(empty? empty-list-1) ;; => true

(def list-1 (cons 1 (cons 2 '())))
(def list-2 '(1 2))
(def list-3 (list 1 2))

(= list-1 list-2 list-3) ;; => true
(= '(1 (+ 1 1)) (list 1 (+ 1 1))) ;; => false
```

```js
var empty = { type: "list-empty" };
var cons = (head, tail) => ({ type: "list-cons", head, tail });

function lengthOfList(list) {
    return list.type === "list-empty" ? 0 : 1 + lengthOfList(list.tail);
}

var list = (...args) => args.reduceRight((tail, head) => cons(head, tail), empty);
var listp = coll => coll.type === "list-empty" || coll.type === "list-cons";
var conjToList = (coll, item) => cons(item, coll);

function* seqFromList(coll) {
    var node = coll;

    while (node.type !== "list-empty") {
        yield node.head;
        node = node.tail;
    }
}

var list1 = cons(1, cons(2, cons(3, (empty))));
var list2 = list(1, 2, 3);;
lengthOfList(list1); // => 3
listp(list()); // => true
```

#### Vector

```scheme
(def vector-1 [1 2])
(def vector-2 (vector 1 2))
(= vector-1 vector-2) ;; => true
```

```js
var vector = (...args) => args;
var vectorp = coll => Array.isArray(coll);
var lengthOfVector = coll => coll.length;
var conjToVector = (coll, item) => [...coll, item];

vectorp(vector(1, 2, 3)); // => true
lengthOfVector(vector(1, 2, 3)); // => 3
```

#### Map

```scheme
(def map-1 {:name "marcin" :age 123})
(def map-2 {:age 123 :name "marcin"})
(= map-1 map-2) ;; => true
```

```js
var lengthOfMap = coll => Object.keys(coll).length;
function mapp(coll) {
    return !listp(coll) && !vectorp(coll) && !setp(coll) && !functionp(coll)
        && !numberp(coll) && !stringp(coll) && !functionp(coll) && !nilp(coll);
}
function conjToMap(coll, item) {
    var [key, value] = Array.isArray(item) ? item : Object.entries(item)[0];
    return { ...coll, [key]: value };
}

var map1 = { name: "marcin", age: 123 };
mapp({ name: "marcin" }); // => true
lengthOfMap({ name: "marcin" }); // => 2
```

#### Set

```scheme
(def set-1 #{1 2})
(def set-2 (hash-set 1 2))
(= set-1 set-2) ;; => true
```

```js
var set = items => new Set(items);
var hashSet = (...items) => set(items);
var setp = coll => coll instanceof Set;
var lengthOfSet = coll => coll.size;
var conjToSet = (coll, item) => coll.has(item) ? coll : new Set([...coll, item]);

var set1 = set([1, 2, 3, 1]);
var set2 = hashSet(1, 2, 3, 1);
setp(hashSet(1, 2, 3, 4)); // => true
lengthOfSet(hashSet(1, 2, 3)); // => 3
```

#### Seq

```scheme
(seq? "abc") ;; => false
(seq? (seq "abc")) ;; => true
(seq? (vector 1 2 2)) ;; => false
(seq? (seq (vector 1 2 2))) ;; => false
(seq? (list 1 2 3)) ;; => true
```

```js
var seqp = coll => typeof coll[Symbol.iterator] !== "undefined";
function seq(coll) {
    return seqp(coll) ? coll // string, array, set
        : listp(coll) ? seqFromList(coll)
            : mapp(coll) ? Object.entries(coll)
                : wrongArgType(coll);
}
```

#### Collections operations

```scheme
(count list-1) ;; => 2
(count vector-1) ;; => 2
(count map-1) ;; => 2
(count set-1) ;; => 2

(conj list-1 3 4 5) ;; => (5 4 3 1 2)
(conj vector-1 3 4 5) ;; => [1 2 3 3 4 5]
(conj set-1 3 4 5 1) ;; => #{1 4 3 2 5}

(into list-1 [3 4 5]) ;; => (5 4 3 1 2)
(into vector-1 [3 4 5]) ;; => [1 2 3 3 4 5]
(into set-1 [3 4 5 1]) ;; => #{1 4 3 2 5}

(let [coll vector-1]
  [(first coll)
   (rest coll)
   (empty? coll)
   (nth coll 1)]) ;; => [1 (2) false 2]
```

```js
function count(coll) {
    return listp(coll) ? lengthOfList(coll)
        : vectorp(coll) ? lengthOfVector(coll)
            : setp(coll) ? lengthOfSet(coll)
                : mapp(coll) ? lengthOfMap(coll)
                    : wrongArgType(coll);
}

function conj(coll, ...items) {
    const conjItem = listp(coll) ? conjToList
        : vectorp(coll) ? conjToVector
            : setp(coll) ? conjToSet
                : mapp(coll) ? conjToMap
                    : wrongArgType(coll)

    return items.reduce(conjItem, coll);
}

var into = (coll, items) => conj(coll, ...items);

into(list(1, 2, 3), vector(4, 5));

var first = coll => find_(seq(coll));
var rest = coll => listp(coll) ? coll.tail : skip_(seq(coll), 1);
var nth = (coll, n) => elementat_(seq(coll), n);
var emptyp = coll => !some_(seq(coll));
```

#### Map operations

```scheme
(get map-1 :name) ;; => "marcin"
(map-1 :name) ;; => "marcin" (... mapa jest funkcja)
(:name map-1) ;; => "marcin" (... keyword jest funkcja)

(assoc map-1 :address "wroclaw" :id 1) ;; => {:name "marcin", :age 123, :address "wroclaw", :id 1}
(dissoc map-1 :name :id) ;; => {:age 123}

(assoc map-1 :name "lukasz") ;; => {:name "lukasz", :age 123}

(update map-1 :name (fn [value] (str value "!"))) ;; => {:name "marcin!", :age 123}
(update map-1 :name str "!") ;; => {:name "marcin!", :age 123}
```

```js
var get = (coll, key) => vectorp(coll) || stringp(coll) || mapp(coll) ? coll[key] : wrongArgType(coll);

function update(coll, key, func, ...args) {
    return vectorp(coll) ? coll.map((e, i) => i === key ? func(e, ...args) : e)
        : mapp(coll) ? Object.entries({ [key]: null, ...coll }).reduce((o, [k, v]) => ({ ...o, [k]: k === key ? func(v, ...args) : v }), {})
            : wrongArgType(coll);
}

function assoc(coll, ...kvs) {
    if (nilp(coll) || kvs.length === 0) {
        return coll;
    }

    const [key, value, ...other] = kvs;

    const coll2 = (vectorp(coll) || stringp(coll)) && (key < 0 || key >= coll.length) ? throww("index out of bounds")
        : stringp(coll) ? coll.substring(0, key) + value + coll.substring(key + 1)
            : update(coll, key, _ => value);

    return assoc(coll2, ...other);
}


function dissoc(coll, ...keys) {
    const keysSet = new Set(keys);
    return mapp(coll)
        ? Object.entries(coll).reduce((o, [k, v]) => keysSet.has(k) ? o : { ...o, [k]: v }, {})
        : wrongArgType(coll);
}


var p1 = { id: 1, name: "marcin", age: 123, address: { city: "wroclaw", country: "poland" } };

get(p1, "name"); // => "marcin"
get(vector(1, 2, 3), 1); // 2
assoc(p1, "age", 5, "address", "wroclaw"); // => { address: 'wroclaw', age: 5, id: 1, name: 'marcin' }
update(vector(1, 2, 3), 0, v => v + 10); // => [ 11, 2, 3 ]
update(vector(1, 2, 3), 0, plus, 10, 5); // => [ 11, 2, 3 ]
dissoc(p1, "name", "age"); // => { id: 1 }
```

#### Nested maps

```scheme
(def line {:start {:x 1 :y 2} :end {:x 10 :y 20}})

(get-in line [:start :x]) ;; => 1
(assoc-in line [:start :x] 0) ;; => {:start {:x 0, :y 2}, :end {:x 10, :y 20}}

(update-in line [:start :x] inc) ;; => {:start {:x 2, :y 2}, :end {:x 10, :y 20}}
(update-in line [:start :x] + 1) ;; => {:start {:x 2, :y 2}, :end {:x 10, :y 20}}
```

```js
function getIn(coll, ks) {
    const [key, ...other] = ks;
    return nilp(coll) || ks.length === 0 ? coll : getIn(get(coll, key), other);
}


function updateIn(coll, ks, func, ...args) {
    const [key, ...other] = ks;
    return nilp(coll) || ks.length === 0 ? coll
        : ks.length === 1 ? update(coll, key, func, ...args)
            : update(coll, key, v => updateIn(nilp(v) ? {} : v, other, func, ...args));
}

function assocIn(coll, ks, value) {
    return updateIn(coll, ks, _ => value, value);
}

getIn(p1, ["address", "city"]);
getIn(p1, ["address", "city", 0]); // => w

updateIn(p1, ["name"], x => x + "!");
updateIn(p1, ["lastName"], x => x === null ? "null" : "!null");
updateIn(p1, ["address", "city"], city => city.toUpperCase());
updateIn(p1, ["address", "postalCode"], _ => 666);
updateIn([p1, p1], [0, "name"], name => name + "!");

assocIn(p1, ["name"], "wojtek");
assocIn(p1, ["lastName"], "w");
assocIn(p1, ["address", "city"], "Krakow");
assocIn(p1, ["address", "postalCode"], 666);
assocIn([p1, p1], [0, "name"], "wojtek");
assocIn(assocIn({}, ["C", "photos", "image1.jpg"], 11111), ["C", "photos", "image2.jpg"], 22222);
// => { C: { photos: { 'image2.jpg': 22222, 'image1.jpg': 11111 } } }
updateIn(p1, ["address"], dissoc, "city")
```

#### Map operations and vectors

```scheme
(def users [{:name "James" :age 26}  {:name "John" :age 43}])
(get users 1) ;; => {:name "John", :age 43}
(get-in users [1 :name]) ;; => "John"
(update-in users [1 :age] inc) ;; => [{:name "James", :age 26} {:name "John", :age 44}]
```



#### Seq operators (map, filter, reduce, ... )

```scheme
(map
 (fn [x] (* x 10))
 (filter odd? [1 2 3 4 5 6])) ; => (10 30 50)

(map
 #(* % 10)
 (filter odd? [1 2 3 4 5 6])) ; => (10 30 50)
```

```js
var map = (f, coll) => map_(seq(coll), f);
var filter = (f, coll) => filter_(seq(coll), f);
var repeat = (n, x) => repeatvalue_(x, n);

var reducedS = Symbol();
var reduced = v => ({ s: reducedS, v });
function reduce(f, val, coll) {
    return iter(seq(coll)[Symbol.iterator](), val);

    function iter(iterator, total) {
        var { done, value } = iterator.next();
        if (done) {
            return typeof value === "undefined" ? total : f(total, value);
        }
        var t = f(total, value);
        return t.s === reducedS ? t.v : iter(iterator, t);
    }
}

map(filter(oddp, list(1, 2, 3, 4, 5, 6)), x => x * 10);

reduce((p, c) => p + c, 0, list());
reduce((p, c) => p + c, 0, list(1, 2, 3));
reduce((p, c) => (c === 2 ? reduced(1000 + p) : p + c), 0, list(1, 2, 3));
reduce((p, c) => p + c, 0, (function* () {
    yield 10;
    yield 20;
    return 30;
})());
```

#### ->> ""thread-last"

```scheme
(->>
 [1 2 3 4 5 6]
 (filter odd?)
 (map #(* % 10)))

(macroexpand
 '(->>
   [1 2 3 4 5 6]
   (filter odd?)
   (map #(* % 10))))

;; => (map (fn* [p1__8212#] (* p1__8212# 10)) (filter odd? [1 2 3 4 5 6]))
```

```js
into(vector(), map(x => x * 10, filter(oddp, list(1, 2, 3, 4, 5, 6))));
pipe(
    list(1, 2, 3, 4, 5, 6),
    o => filter(oddp, o),
    o => map(x => x * 10, o),
    o => into(vector(), o)
); // => [ 10, 30, 50 ]
```

#### -> "thread-first" 

```scheme
(assoc (assoc {} :name "marcin") :age 123) ;; => {:name "marcin", :age 123}

(->
 {}
 (assoc :name "marcin")
 (assoc :age 123))
;; => {:name "marcin", :age 123}

(macroexpand
 `(->
   {}
   (assoc :name "marcin")
   (assoc :age 123)))
;; => (clojure.core/assoc (clojure.core/assoc {} :name "marcin") :age 123)
```

```js
assoc(assoc({}, "name", "marcin"), "age", 123);
pipe(
    {},
    o => assoc(o, "name", "marcin"),
    o => assoc(o, "age", 123)
); // => { age: 123, name: 'marcin' }
```

#### Recursion

```scheme
(defn sum-numbers [numbers]
  (if
   (empty? numbers)
    0
    (let [[head & tail] numbers]
      (+ head (sum-numbers tail)))))

(sum-numbers '())
(sum-numbers '(1 2 3))

(defn sum-numbers-with-tail-call [numbers total]
  (if
   (empty? numbers)
    total
    (let [[head & tail] numbers]
      (sum-numbers-with-tail-call tail (+ head total)))))

(sum-numbers-with-tail-call '(1 2 3) 0)

(defn sum-numbers-with-tail-call-recur [numbers total]
  (if
   (empty? numbers)
    total
    (let [[head & tail] numbers]
      (recur tail (+ head total)))))


(sum-numbers-with-tail-call-recur '(1 2 3) 0)

(defn sum-numbers-with-tail-loop [numbers]
  (loop [lst numbers
         total 0]
    (if
     (empty? lst)
      total
      (let [[head & tail] lst]
        (recur tail (+ head total))))))

(sum-numbers-with-tail-loop '(1 2 3))
```


#### Advent Of Code 2022 Day 1 

```scheme
;; https://adventofcode.com/2022/day/1
;;https://github.com/marcinnajder/misc/blob/master/2022_12_15_advent_of_code_in_clojure/src/advent_of_code_2022/day_01.clj

(defn load-data [text]
  (->> text
       string/split-lines
       (reduce (fn  [lists line]
                 (if (string/blank? line)
                   (cons '() lists)
                   (let [[inner-list & tail] lists]
                     (cons (cons (Integer/valueOf line) inner-list) tail))))
               '(()))))

(defn insert-sorted [xs x]
  (if (empty? xs)
    (list x)
    (let [[head & tail] xs]
      (if (< x head)
        (cons x xs)
        (cons head (insert-sorted tail x))))))

(defn insert-sorted-preserving-length [xs x]
  (if (<= x (first xs))
    xs
    (rest (insert-sorted xs x))))

(defn puzzle [text top-n]
  (->> text
       load-data
       (map #(apply + %))
       (reduce insert-sorted-preserving-length (into '() (repeat top-n 0)))
       (apply +)))

(defn puzzle-1 [text]
  (puzzle text 1))

(defn puzzle-2 [text]
  (puzzle text 3))
  
(comment
  (def file-path "src/advent_of_code_2022/day_01.txt")
  (def text (slurp file-path))
  (load-data text)
  (puzzle-1 text)
  (puzzle-2 text)
  :rfc)
```

```js
var apply = (f, args) => f.apply(null, [...seq(args)]);
apply(add, list(1, 2));

var partial = (f, ...args) => f.bind(null, ...args);
partial(add, 1)(10);

var integer_valueOf = text => parseInt(text);
var string_splitLines = text => text.split(os.EOL);
var string_blankp = text => text.trim().length === 0;
var slurp = filePath => fs.readFileSync(filePath, "utf-8");

function loadData(text) {
    return pipe(text,
        string_splitLines,
        o => reduce((lists, line) =>
            string_blankp(line)
                ? cons(list(), lists)
                : cons(cons(integer_valueOf(line), first(lists)), rest(lists)),
            list(list()), o));
}

function insertSorted(xs, x) {
    return emptyp(xs)
        ? list(x)
        : (x < first(xs) ? cons(x, xs) : cons(first(xs), insertSorted(rest(xs), x)));
}

function insertSortedPreservingLength(xs, x) {
    return x <= first(xs) ? xs : rest(insertSorted(xs, x))
}

function puzzle(text, topN) {
    return pipe(text,
        loadData,
        o => map(xs => apply(plus, xs), o),
        o => into(vector(), o),
        o => reduce(insertSortedPreservingLength, into(list(), repeat(topN, 0)), o),
        o => apply(plus, o)
    );
}

function puzzle1(text) {
    return puzzle(text, 1);
}
function puzzle2(text) {
    return puzzle(text, 3);
}

function comment() {
    var filePath = "./day01.txt";
    var text = slurp(filePath);
    puzzle1(text);
    puzzle2(text);
}
```