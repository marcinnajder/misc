var fs = require("fs");
var os = require("os");
var { pipe, skip: skip_, elementat: elementat_, find: find_, some: some_, map: map_, filter: filter_, repeatvalue: repeatvalue_ } = require("powerseq");

function wrongArgType(arg) {
    const argType = typeof arg !== "object" ? typeof arg : arg.constructor.name;
    throw new Error(`function '${wrongArgType.caller.name}' was called with argument of unexpected type '${argType}'`);
}

var throww = message => { throw new Error(message); };



// Global variables
var firstName = "marcin";
console.log("my name is:", firstName);

// First-class function
var add = (a, b) => a + b;

add(1, 2);
((a, b) => a + b)(1, 2);


// unction definition, defn macro
function add(a, b) {
    return a + b;
}

//  Macros
var a = 10;
var b = 5;
eval(`${a} / ${b}`);




// Variadic functions, comments

/** function takes at least 2 arguments */
function addAtLeast2(a, b, ...restArgs) {
    return a + b + restArgs.reduce((x, y) => x + y, 0);
}



// Control flow (if/then/else, variables, code blocks, simple loops)

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

func1(10, 9); // => 9
func1(9, 10); // => 666

// 'apply' function
add.apply(null, [1, 2]);


// 'partial' function
var inc = add.bind(null, 1);
inc(10); // => 11
add.bind(null, 1)(10); // => 11

// 'comp' function (function composition)
var comp = (...funcs) => arg => funcs.reduceRight((a, f) => f(a), arg);

var incrementTwiceThenToString = comp(str, inc, inc);
incrementTwiceThenToString(10); // => "12"







//  Built-in functions

var nilp = x => typeof x === "undefined" || x === null;
var stringp = x => typeof x === "string";
var booleanp = x => typeof x === "boolean";
var numberp = x => typeof x === "number";
var functionp = x => typeof x === "function";

functionp(add);
stringp("");

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

plus(1, 2, 3);
minus(10, 3, 1);
multiply(1, 2, 3);
max(1, 2, 3);
min(1, 2, 3);
evenp(10);
oddp(11);

var identity = x => x;
var constantly = x => () => x;
var complement = f => (...args) => !f(...args);


identity(10); // => 10
identity("mama"); // => "mama"
constantly(666)(1); // => 666
constantly(666)(1, 2, 3); // => 666
complement(array => array.length === 0)([]); // => false

(2 > 1) && "tata".endsWith("ta") && false; // => false
(2 > 1) || "tata".endsWith("ta") || false; // => true

//   jest tylko false oraz nil (np 0 i "" nie)
"" && 0; // => ""
"" && 0 && false; // => ""
"" && 0 && null; // => ""

"" || 0; // => 0








// Immutable singly-linked list

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
lengthOfList(list1);
listp(list()); // => true
conjToList(list(1, 2, 3), 4);



[...seqFromList(list())];
[...seqFromList(list(1, 2, 3))];


// Vector
var vector = (...args) => args;
var vectorp = coll => Array.isArray(coll);
var lengthOfVector = coll => coll.length;
var conjToVector = (coll, item) => [...coll, item];

vectorp(vector(1, 2, 3)); // => true
lengthOfVector(vector(1, 2, 3));
conjToVector(vector(1, 2, 3), 4);

// Map
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
mapp(list()); // => false
mapp(vector()); // => false
mapp(hashSet()); // => false
lengthOfMap({ name: "marcin" });
conjToMap({ name: "marcin" }, ["age", 123]);
conjToMap({ name: "marcin" }, { age: 124 });

// Set
var set = items => new Set(items);
var hashSet = (...items) => set(items);
var setp = coll => coll instanceof Set;
var lengthOfSet = coll => coll.size;
var conjToSet = (coll, item) => coll.has(item) ? coll : new Set([...coll, item]);

var set1 = set([1, 2, 3, 1]);
var set2 = hashSet(1, 2, 3, 1);
setp(hashSet(1, 2, 3, 4)); // => true
lengthOfSet(hashSet(1, 2, 3));
conjToSet(hashSet(1, 2, 3), 4);



// Seq
var seqp = coll => typeof coll[Symbol.iterator] !== "undefined";

function seq(coll) {
    return seqp(coll) ? coll // string, array, set
        : listp(coll) ? seqFromList(coll)
            : mapp(coll) ? Object.entries(coll)
                : wrongArgType(coll);
}


seqp(seq(list(1, 2, 3)));
seqp("abc");
seqp(new Set());
[...seq(list(1, 2, 3))];
seq(1)



// Collections operations

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

count(list(1, 2));
count(vector(1, 2, 3));
count(hashSet(1, 2, 3, 4));
count({ name: "marcin" });
count(1);

conj(list(1, 2, 3), 4, 5);
conj(vector(1, 2, 3), 4, 5);
conj(hashSet(1, 2, 3), 4, 5);
conj({ name: "marcin" }, ["age", 123], { address: "wroclaw" });

into(list(1, 2, 3), vector(4, 5));

var first = coll => find_(seq(coll));
var rest = coll => listp(coll) ? coll.tail : skip_(seq(coll), 1);
var nth = (coll, n) => elementat_(seq(coll), n);
var emptyp = coll => !some_(seq(coll));


first(list(1, 2, 3));
first(vector(1, 2, 3));
first({ name: "marcin" });
first(list());
[...rest(list(1, 2, 3))];
nth(list(1, 2, 3), 1);
emptyp(list(1, 2));
emptyp(list());
emptyp("");


//  Map operations

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
get(); // 2

assoc(p1, "age", 5);
assoc(p1, "age", 5, "address", "wroclaw");
assoc(vector(10, 20, 30), 0, 100, 1, 200);
assoc(vector(10, 20, 30), 5, 100); // => index out of bounds
assoc("mama", 0, "M", 3, "o"); // => Mamo
assoc("mama", 5, "!");// => index out of bounds


update(vector(1, 2, 3), 0, v => v + 10);
update(vector(1, 2, 3), 0, plus, 10, 5);
update(p1, "age", v => v - 100);
update(p1, "age", minus, 100);
update(p1, "age", _ => 0);

dissoc(p1, "name", "age"); // => { id: 1 }


// Nested maps

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

// dissoc-in does not exist, use update-in instead
updateIn(p1, ["address"], dissoc, "city")


// Seq operators (map, filter, reduce, ... )
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




// ->> ""thread - last"
into(vector(), map(x => x * 10, filter(oddp, list(1, 2, 3, 4, 5, 6))));
pipe(
    list(1, 2, 3, 4, 5, 6),
    o => filter(oddp, o),
    o => map(x => x * 10, o),
    o => into(vector(), o)
); // => [ 10, 30, 50 ]


// -> "thread-first" 
assoc(assoc({}, "name", "marcin"), "age", 123);
pipe(
    {},
    o => assoc(o, "name", "marcin"),
    o => assoc(o, "age", 123)
); // => { age: 123, name: 'marcin' }


// Advent Of Code 2022 Day 1 
// https://adventofcode.com/2022/day/1
// https://github.com/marcinnajder/misc/blob/master/2022_12_15_advent_of_code_in_clojure/src/advent_of_code_2022/day_01.clj

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
