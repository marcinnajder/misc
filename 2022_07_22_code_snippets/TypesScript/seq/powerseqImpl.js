var numbers = [1, 5, 3, 9, -1, 5, -12, 0, 44, 12, -100];

// ---------------------------------------------------------------------------------------------------------
// - filter
// ---------------------------------------------------------------------------------------------------------

for (var n of numbers) {
    if (n > 0) {
        console.log(`${n} zl`);
    }
}

// ~ Array.prototype.filter implementation
function filter(items, f) {
    var result = [];
    for (const item of items) {
        if (f(item)) {
            result.push(item);
        }
    }
    return result;
}

// unnecessary temporary array
for (var n of numbers.filter(n => n > 0)) {
    console.log(`${n} zl`);
}


// - filter implementation using  generator generatorgenerators generators
function* filter(items, f) {
    for (const item of items) {
        if (f(item)) {
            yield item;
        }
    }
}

var q = filter(numbers, n => n > 0);
for (var n of q) {
    console.log(`${n} zl`);
}


// filter function is not executed here
var q = filter(numbers, n => n > 0);

// it executes starting iteration
for (var n of q) {
    console.log(`${n} zl`);
}


// here it starts executing once again
for (var n of q) {
    console.log(`${n} zl`);
}

var iable = filter(numbers, n => n > 0);
var itor = iable[Symbol.iterator](); // ~ iable.getIterator()

itor.next(); // -> { value: 1, done: false }
itor.next(); // -> { value: 5, done: false }

var nextResult;
while (!(nextResult = itor.next()).done) {
    console.log(nextResult.value);
}



// ---------------------------------------------------------------------------------------------------------
// - multiple iterations
// ---------------------------------------------------------------------------------------------------------

function toarray(items) {
    var result = [];
    for (const item of items) {
        result.push(item);
    }
    return result;
}


var q = filter(numbers, n => n > 0);
var qq = toarray(q); // the same as '[...q]' or 'Array.from(q)'

for (var n of qq) {
    console.log(`${n} zl`);
}
for (var n of qq) {
    console.log(`${n} zl`);
}



// note: 'generator function' creates 'iterable object' returning always the same instance of `iterator object`
var q = filter(numbers, n => n > 0);

var qq1 = toarray(q); // -> [ 1,  5,  3, 9, 5, 44, 12 ]
var qq1 = toarray(q); // -> []

q[Symbol.iterator]() === q[Symbol.iterator](); // -> TRUE

var q = [1, 2, 3];
q[Symbol.iterator]() === q[Symbol.iterator](); // -> FALSE


// note: powerseq works differently, always new instance of 'iterator' object is created

// ---------------------------------------------------------------------------------------------------------
// - map (Array)
// ---------------------------------------------------------------------------------------------------------


for (var n of numbers) {
    console.log(`${n} zl`);
}

// ~ Array.prototype.map implementation
function map(items, f) {
    var result = [];
    for (const item of items) {
        result.push(f(item));
    }
    return result;
}

// unnecessary temporary array
for (var c of numbers.map(n => `${n} zl`)) {
    console.log(c);
}

// - map implementation using generators 
function* map(items, f) {
    for (const item of items) {
        yield f(item);
    }
}

var q = map(numbers, n => `${n} zl`);
for (var c of q) {
    console.log(c);
}


// ---------------------------------------------------------------------------------------------------------
// - pipe
// ---------------------------------------------------------------------------------------------------------

for (var n of numbers) {
    if (n > 0) {
        console.log(`${n} zl`);
    }
}

var filtered = filter(numbers, n => n > 0);
var filteredAndMapped = map(filtered, n => `${n} zl`);

for (var c of filteredAndMapped) {
    console.log(c);
}


// const pipe = (value, ...funcs) => funcs.reduce((v, f) => f(v), value);

function pipe(value, ...funcs) {
    return funcs.reduce((v, f) => f(v), value);
}

var r = pipe(10,
    v => v + 1,
    v => v * 10,
    v => v.toString()); // "110"


var filteredAndMapped = pipe(numbers,
    s => filter(s, n => n > 0),
    s => map(s, n => `${n} zl`)
);

for (var c of filteredAndMapped) {
    console.log(c);
}

function filter(...args) {
    return args.length === 2 ? filter_(...args) : s => filter_(s, ...args);

    function* filter_(items, f) {
        let index = 0; // index!
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


function toarray(...args) {
    return args.length === 1 ? toarray_(...args) : s => toarray_(s);

    function toarray_(items) {
        var result = [];
        for (const item of items) {
            result.push(item);
        }
        return result;
    }
}


// the whole query
var filteredAndMapped = pipe(numbers,
    filter(n => n > 0),
    map(n => `${n} zl`),
    toarray()
);
// -> [ '1 zl',  '5 zl', '3 zl',  '9 zl', '5 zl',  '44 zl', '12 zl' ]


// uzycie pojedynczego operatora
for (var n of filter(numbers, n => n > 0)) {
    console.log(`${n} zl`);
}



// ---------------------------------------------------------------------------------------------------------
// - sample data, list of products
// ---------------------------------------------------------------------------------------------------------

var products = [
    { id: "1", name: "iPhone 11", categories: ["Phone", "Apple"] },
    { id: "2", name: "Samsung xperia", categories: ["Phone", "Samsung"] },
    { id: "3", name: "Samsung TV", categories: ["TV", "Samsung"] },
];


// ---------------------------------------------------------------------------------------------------------
// - flatmap
// ---------------------------------------------------------------------------------------------------------


// accessing subitems
for (const p of products) {
    for (const c of p.categories) {
        console.log(c);
    }
}

function flatmap(...args) {
    return args.length === 2 ? flatmap_(...args) : s => flatmap_(s, ...args);

    function* flatmap_(items, f) {
        for (const item of items) {
            yield* f(item);
        }
    }
}

for (const c of flatmap(products, p => p.categories)) {
    console.log(c);
}


// accessing subitems and items the same time
for (const p of products) {
    for (const c of p.categories) {
        console.log(c, " - ", p.name);
    }
}

// combination of flatmap and map
for (const { p, c } of flatmap(products, p => map(p.categories, c => ({ p, c })))) {
    console.log(c, " - ", p.name);
}


function flatmap(...args) {
    return typeof args[0] !== "function" ? flatmap_(...args) : s => flatmap_(s, ...args);

    function* flatmap_(items, f, ff) {
        if (!ff) {
            for (const item of items) {
                yield* f(item);
            }
        } else {
            for (const item of items) {
                for (const subitem of f(item)) {
                    yield ff(item, subitem);
                }
            }
        }
    }
}

for (const { p, c } of flatmap(products, p => p.categories, (p, c) => ({ p, c }))) {
    console.log(c, " - ", p.name);
}


// ---------------------------------------------------------------------------------------------------------
// - distinct
// ---------------------------------------------------------------------------------------------------------

var set = new Set(flatmap(products, p => p.categories));
var uniqueCategories = [...set];

function distinct(...args) {
    return args.length === 1 ? distinct_(...args) : s => distinct_(s);

    function* distinct_(items, f) {
        var set = new Set()
        for (const item of items) {
            if (!set.has(item)) {
                set.add(item);
                yield item;
            }
        }
    }
}


var uniqueCategories = pipe(flatmap(products, p => p.categories), distinct(), toarray());

// 'distinct' by specified property
var uniqueProductNames = pipe(map(products, p => p.name), distinct(), toarray());

// powerseq version of 'distinct' operator provides additional overload
// var uniqueProductNames = pipe(distinct(p => p.name), toarray()); 

// for(var productName in distinct(products, p => p.name)){ }




// ---------------------------------------------------------------------------------------------------------
// - count
// ---------------------------------------------------------------------------------------------------------

// unnecessary temporary array
var iphoneCount = products.filter(p => p.name.includes("iPhone")).length;

// const realizedVisit = services.filter(service => service.statusId === 1).length;
// const cancelledVisit = services.filter(service => service.statusId === 2).length;

function count(...args) {
    return args[0] && args[0][Symbol.iterator] ? count_(...args) : s => count_(s, ...args);

    function count_(items, f) {
        var sum = 0;
        if (!f) {
            for (const item of items) {
                sum++;
            }
        } else {
            var index = 0;
            for (const item of items) {
                if (f(item, index)) {
                    sum++;
                }
                index++;
            }
        }
        return sum;
    }
}

var iphoneCount = pipe(products, filter(p => p.name.includes("iPhone")), count());

var iphoneCount = count(products, p => p.name.includes("iPhone"));




// ---------------------------------------------------------------------------------------------------------
// - toobject
// ---------------------------------------------------------------------------------------------------------

// { '1': { ... }, '2': { ... }, '3': { ... } }


var productsMap = products.reduce((obj, p) => (obj[p.id] = p, obj), {});

var productsMap = products.reduce((obj, p) => ({ ...obj, [p.id]: p }), {});

var productsMap = Object.assign({}, ...products.map(p => ({ [p.id]: p })));



function toobject(...args) {
    return typeof args[0] !== "function" ? toobject_(...args) : s => toobject_(s, ...args);

    function toobject_(items, f, ff) {
        var obj = {};
        if (!ff) {
            for (const item of items) {
                obj[f(item)] = item;
            }
        } else {
            for (const item of items) {
                obj[f(item)] = ff(item);
            }
        }
        return obj;
    }
}


var productsMap = toobject(products, p => p.id);

// { '1': 'iPhone 11', '2': 'Samsung xperia', '3': 'Samsung TV' }
var productNamesMap = toobject(products, p => p.id, p => p.name);

// ---------------------------------------------------------------------------------------------------------
// - join
// ---------------------------------------------------------------------------------------------------------

var mongoProducts = products;
var externalSystemProducts = products;

var mongoProductsMap = mongoProducts.reduce((obj, p) => (obj[p.id] = p, obj), {});

for (var ep of externalSystemProducts) {
    var mp = mongoProductsMap[ep.id];
    if (mp) {
        console.log(ep.name, " - ", mp.name);
    }
}

var { join, groupby } = require("powerseq");

var q = join(mongoProducts, externalSystemProducts, mp => mp.id, ep => ep.id, (mp, ep) => ({ mp, ep }));

for (var { mp, ep } of q) {
    console.log(ep.name, " - ", mp.name);
}

// ---------------------------------------------------------------------------------------------------------
// - group
// ---------------------------------------------------------------------------------------------------------


var groupByCompany = {};

for (const p of products) {
    var key = p.categories.includes("Apple") ? "apple" : (p.categories.includes("Samsung") ? "samsung" : "other");

    var values = groupByCompany[key];
    if (values) {
        values.push(p);
    } else {
        groupByCompany[key] = [p];
    }
}


var { groupby } = require("powerseq");

var q = pipe(products,
    groupby(p => p.categories.includes("Apple") ? "apple" : (p.categories.includes("Samsung") ? "samsung" : "other")),
    toobject(gr => gr.key, gr => [...gr])
);

[...pipe(
    products,
    // pairwise(),
    groupby(p => p.name),
    map(gr => ({ name: gr.key, count: count(gr) })))]
//map(gr => ({ name: gr.key, items: toarray(gr) })))


// note: powerseq operators are just regular functions, we can always add our own functions 



// ---------------------------------------------------------------------------------------------------------
// - concat
// ---------------------------------------------------------------------------------------------------------

var { concat } = require("powerseq");

for (var p of concat(mongoProducts, externalSystemProducts)) {
    console.log(p.name);
}
