## Programming with sequences part 2 - frequently used operators


#### Introduction

Last time we have been talking about basic operators like `filter`, `map`, `toarray` and `take`. Once we start using them,  we perceive the code in a different way, a loop with `if` clause inside is a `filter`, a loop mapping elements of collection is a `map`. (powerseq)[https://github.com/marcinnajder/powerseq] library provides around 70 operators. When I do a code review, I see over and over again the same code patterns. Often there are a few lines of code that can be replaced with one single line calling an existing operator. Do we really have to use them ? Not really. Every programmer from day one knows how to use variables, loops and ifs. It's very easy to write such an imperative code, everyone knows how to do this. But such a code is not easy read, understand and maintain. It's not simple, it's just familiar. 

In this article we will walk through many code patterns written in an imperative style and try to translate them into a declarative style using operators from powerseq. In some cases we will implement those operators ourself to see how simple there are. We will be using the following data model:

```javascript
var products = [
    { id: "1", name: "iPhone 11", categories: ["Phone", "Apple"] },
    { id: "2", name: "Samsung xperia", categories: ["Phone", "Samsung"] },
    { id: "3", name: "Samsung TV", categories: ["TV", "Samsung"] },
];
```

#### flatmap

We work with nested data model almost every day. In our example, we have a list of products and each product contains a list of categories. Now let's say we would like to print all available categories.

```javascript
for (const p of products) {
    for (const c of p.categories) {
        console.log(c);
    }
}
```

If we want to get rid of a loop mapping items, we just use `map` operator. Let's try to use it to see what will happen.

```javascript 
function* map(items, f) {
    for (const item of items) {
        yield f(item);
    }
}

[...map(products, p => p.categories)];
// -> [ ["Phone", "Apple"], ["Phone", "Samsung"], ["TV", "Samsung"] ]
```

It's not exactly what we would expect, the result is a collection of nested collections. We would like to flatten the data somehow, `flatmap` operators is doing exactly this.

```javascript
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
```

If we look carefully at the implementation of `flatmap` and try to compare it with `map` operator, we will notice there are almost identical. The only difference is an asterisk `yield* f(item);`. This asterisk is responsible from flattening the final result. So we have replaced two loops inside each other with one loop combined with `flatmap` operator. But there is one disadvantage of this solution. Writing two loops manually gives us a chance to access both the item and the subitem the same time.

```javascript
for (const p of products) {
    for (const c of p.categories) {
        console.log(c, " - ", p.name);
    }
}
```

To solve this issue we can combine two operators `flatmap` and `map` together

```javascript
for (const { p, c } of flatmap(products, p => map(p.categories, c => ({ p, c })))) {
    console.log(c, " - ", p.name);
}
```

This code may seem scary at first. For some programmers it's too many brackets and the initial solution with two loops inside each other is just simpler. Fair enough. But remember, that was a very simple scenario. Very often `flatmap` is just a beginning of a bigger query that we need to write to achieve more complicated task. We don't want to go back to imperative code completely, we just would like to write that code a little bit simpler. To do that we can introduce a new overload to `flatmap` operator.

```javascript
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
```

#### distinct

The code pattern I also see is calculating the unique set of items from initial collection of items. Let's say we want to know the unique set of product categories. There are many ways to implement it. Let's assume we already know and use in practice `flatmap` operator, so we could write something like this


```javascript
var set = new Set(flatmap(products, p => p.categories));
var uniqueCategories = [...set];
```

What's wrong with this solution ? Nothing, to be honest it's quite elegant. We have used `Set` collection type introduced in ES6 to find all unique items in a performant way. This code could be even shortened into one line of code. But not many programmers would write code like that, most probably it would be written using variables, loops and ifs. Let's introduce a new operator called `distinct`

```javascript
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
```

The key benefit of using sequence operators is readability. We can clearly express the intent of the code and immediately see what code is doing. We extract all categories, then we filter them to the sequence of unique items. We have used `flatmap` operator because we had to flatten all categories. Let's change a little bit the task, we want to return a unique names of the products. Of course we can use `map` instead of `flatmap`, but this scenario is so common that the powerseq version of `distinct` operator takes an optional function parameter.

```javascript
for(var productName of pipe(map(products, p => p.name), distinct()) ) { ... }
for(var productName of distinct(products, p => p.name) ) { ... }
```

#### count

`count` is a very simple operator but yet very helpful. Let's say we would like to count all products in `iPhone` category. Some programmers use builtin `filter` method to do that.

```javascript
var iphoneCount = products.filter(p => p.name.includes("iPhone")).length;
```

Maybe not a big problem, but again this code creates unnecessary array in memory only to read the `length` property. We can implement our own general purpose `count` operator that works not only with arrays but any iterable object.

```javascript
function count(...args) {
    return args[0] && args[0][Symbol.iterator] ? count_(...args) : s => count_(s, ...args);

    function count_(items, f) {
        var sum = 0;
        if (!f) {
            for (const _ of items) {
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
```

In a simplest usage it just counts the items from the input sequence, but we can also pass a predicate function to filter out the specific items.

```javascript
var iphoneCount = pipe(products, filter(p => p.name.includes("iPhone")), count()); // no array is created
var iphoneCount = count(products, p => p.name.includes("iPhone"));
```

#### toobject

Next code pattern amazes me every time I see it. It's because there are so many different ways to implement it, take a look at few of them

```javascript
var productsMap = products.reduce((obj, p) => (obj[p.id] = p, obj), {});
var productsMap = products.reduce((obj, p) => ({ ...obj, [p.id]: p }), {});
var productsMap = Object.assign({}, ...products.map(p => ({ [p.id]: p })));

// {
//   '1': { id: '1', name: 'iPhone 11', categories: [ 'Phone', 'Apple' ] },
//   '2': { id: '2', name: 'Samsung xperia', categories: [ 'Phone', 'Samsung' ] },
//   '3': { id: '3', name: 'Samsung TV', categories: [ 'TV', 'Samsung' ] }
// }
```

Again, in general this code is OK.  It's concise, it includes a few JavaScript tricks, it uses builtin methods. The issue I have  with that code is that the process of converting the list of items into map object is so common in JavaScript. Maybe it deserves the dedicated function. Every time we encounter such a code we have to decode it in our heads. We have to read it carefully keeping track of all variables trying to understand what the final data model will be. It takes time and energy. Many junior developers are not even able to write such a "smart code", they would write imperative code with loops and variables. And that's OK too. But still they have to read and analyze such a code because other team members wrote it. Let's introduce `toobject` operator

```javascript
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
```

Then first argument of `toobject` operator is a function specifying the key of the map, the second argument is optional and it allows to specify the value stored in the map.

```javascript
// { '1': 'iPhone 11', '2': 'Samsung xperia', '3': 'Samsung TV' }
var productNamesMap = toobject(products, p => p.id, p => p.name);
```

From now the code creating an object representing the map is just a single call of `toobject` function.  As a homework, search your repository to find all usages of `reduce` method, try to count how many times it's just an implementation of the code pattern described above.

#### join

When our application is talking to the database, it is so obvious when and how to use `JOIN` or `GROUP BY` SQL clause. But when it comes to the application code, we forget about all those useful data manipulating patterns. We implement them manually over and over again. 

```javascript
var mongoProducts = products;
var externalSystemProducts = products;

// create a map of products for the first collection
var mongoProductsMap = mongoProducts.reduce((obj, p) => (obj[p.id] = p, obj), {});

// iterate over the second collection trying to find matching items
for (var ep of externalSystemProducts) {
    var mp = mongoProductsMap[ep.id];
    if (mp) {
        console.log(ep.name, " - ", mp.name);
    }
}
```

The same logic can be written using `join` operator from the powerseq library as follows:

```javascript
var { join } = require("powerseq");

var q = join(mongoProducts, externalSystemProducts, mp => mp.id, ep => ep.id, (mp, ep) => ({ mp, ep }));

for (var { mp, ep } of q) {
    console.log(ep.name, " - ", mp.name);
}
```

First lambda function specifies the key of the item from the first collection by which the data will be join. The second function does the same for the item from the second collection. The third function takes two matching items and returns the final result. It looks scary at first but with time it feels very natural. It is just an implementation of `INNER JOIN` operation.

#### groupby

Let's say we would like to group products by the company, `apple` products in one bucket, `samsung` products in other bucket and the rest of products in `other` bucket. We could implement this manually like this

```javascript
var groupByCompany = {};

for (const p of products) {
    var key = p.categories.includes("Apple") 
	    ? "apple" 
	    : (p.categories.includes("Samsung") ? "samsung" : "other");

    var values = groupByCompany[key];
    if (values) {
        values.push(p);
    } else {
        groupByCompany[key] = [p];
    }
}

for (var [key, value] of Object.entries(groupByCompany)) {
    console.log(key, "->", value);
}
```

Or we can use an existing `groupby` operator

```javascript
var { groupby } = require("powerseq");

var groupByCompany = groupby(products, p =>  p.categories.includes("Apple") 
	? "apple" 
	: (p.categories.includes("Samsung") ? "samsung" : "other"));

for (var group of groupByCompany) {
    console.log(group.key, "->", [...group]);
}
```

Sometimes we have to group the items and then execute some aggregations like `count`, `min`, `average` over them. Let's assume that for some reason many products can have the same `name` and we would like to find them.

```javascript
var productsWithTheSameNames = pipe(
    products,
    groupby(p => p.name),    
    filter(gr => count(gr) > 1),
    map(gr => gr.key)
);
```


#### concat

At the end the last operator `concat` let us iterate over the items from any number of collections sequentially. In contrast to `concat` method from `Array` type no array is created, the values flow through successive collections.

```javascript
var { concat } = require("powerseq");

for (var p of concat(mongoProducts, externalSystemProducts)) {
    console.log(p.name);
}
```

#### Summary

To make it clear, we don't have to implement ourself presented above operators. powerseq provides them all and even more, around 70 in total. I just wanted to show how simple there are. We can always read their source code when something is not clear or write our own when it's necessary. This particular library is not even important, there are many like this. It's all about the way of thinking and the fact that the same code patterns are re-implement over and over again. Even more, the  programming language is not so important, most languages support constructs that allows us to write this way. Just try it, this can make a big different. In the following parts of the serie we try to solve some real tasks using powerseq introducing new operators the same time.
