## Programming with sequences part 2 - frequently used operators

#### Introduction

Last time, we discussed basic operators such as `filter`, `map`, `toarray` and `take`. Once we start using them, we perceive the code in a different way. A loop with an `if` clause inside becomes a `filter`, a loop mapping elements of collection becomes a `map`. The (powerseq)[https://github.com/marcinnajder/powerseq] library provides around 70 operators. When I do a code review, I repeatedly encounter the same code patterns. Frequently, a few lines of code can be substituted with a single line that calls an existing operator. Do we really have to use them? Not really. Every programmer, from day one, knows how to use variables, loops and ifs. It's very straightforward to write such an imperative code, everyone knows how to do this. However, such code is not easy to read, understand, and maintain. It might be familiar, but it's not necessarily simple.

In this article, we will walk through many code patterns written in an imperative style and attempt to transform them into a declarative style using operators from powerseq. In some cases, we will implement those operators ourselves to demonstrate their simplicity. We will be working with the following data model:

```javascript
var products = [
  { id: "1", name: "iPhone 11", categories: ["Phone", "Apple"] },
  { id: "2", name: "Samsung xperia", categories: ["Phone", "Samsung"] },
  { id: "3", name: "Samsung TV", categories: ["TV", "Samsung"] },
];
```

#### flatmap

We work with nested data models almost every day. In our example, we have a list of products and each product contains a list of categories. Now, let's consider the task of printing all the available categories.

```javascript
for (const p of products) {
  for (const c of p.categories) {
    console.log(c);
  }
}
```

If we wish to eliminate a loop that maps items, we can simply use the `map` operator. Let's give it a try to see what happens.

```javascript
function* map(items, f) {
  for (const item of items) {
    yield f(item);
  }
}

[...map(products, (p) => p.categories)];
// -> [ ["Phone", "Apple"], ["Phone", "Samsung"], ["TV", "Samsung"] ]
```

It's not exactly what we would expect, the result is a collection of nested collections. We would like to flatten the data somehow, to achieve the desired outcome, we can utilize the `flatmap` operator, which precisely flattens the data.

```javascript
function flatmap(...args) {
  return args.length === 2 ? flatmap_(...args) : (s) => flatmap_(s, ...args);

  function* flatmap_(items, f) {
    for (const item of items) {
      yield* f(item);
    }
  }
}

for (const c of flatmap(products, (p) => p.categories)) {
  console.log(c);
}
```

If we look carefully at the implementation of `flatmap` and try to compare it with `map` operator, we will notice they are almost identical. The only difference is an asterisk `yield* f(item);`. This asterisk is responsible for flattening the final result. So we have replaced two loops inside each other with one loop combined with `flatmap` operator. However, there is one drawback to this solution: writing two loops manually allows us to access both the item and the subitem simultaneously.

```javascript
for (const p of products) {
  for (const c of p.categories) {
    console.log(c, " - ", p.name);
  }
}
```

To solve this issue we can combine two operators `flatmap` and `map` together

```javascript
for (const { p, c } of flatmap(products, (p) =>
  map(p.categories, (c) => ({ p, c }))
)) {
  console.log(c, " - ", p.name);
}
```

This code may seem scary at first. Some programmers may find it overwhelming with all the brackets, and the initial solution with two nested loops might seem simpler. Fair enough. However, it's essential to remember that this was a very straightforward scenario. Very often `flatmap` is just a beginning of a bigger query that we need to write to achieve more complex task. We don't intend to revert entirely to imperative code; our goal is to make the code a bit simpler. To achieve this, we can introduce a new overload for the `flatmap` operator.

```javascript
function flatmap(...args) {
  return typeof args[0] !== "function"
    ? flatmap_(...args)
    : (s) => flatmap_(s, ...args);

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

for (const { p, c } of flatmap(
  products,
  (p) => p.categories,
  (p, c) => ({ p, c })
)) {
  console.log(c, " - ", p.name);
}
```

#### distinct

Another code pattern I often encounter involves calculating the unique set of items from an initial collection. Let's say we want to know the unique set of product categories. There are several ways to implement it. Let's assume we already know and use operator `flatmap` in practice, so we could write something like this:

```javascript
var set = new Set(flatmap(products, (p) => p.categories));
var uniqueCategories = [...set];
```

What's wrong with this solution? To be honest, nothing. It's quite elegant. We have used `Set` collection type introduced in ES6 to efficiently find all unique items. This code could even be shortened into one line of code. However, not many programmers would write code in this manner, most would likely resort to using variables, loops, and conditionals. Let's introduce a new operator called `distinct`:

```javascript
function distinct(...args) {
  return args.length === 1 ? distinct_(...args) : (s) => distinct_(s);

  function* distinct_(items, f) {
    var set = new Set();
    for (const item of items) {
      if (!set.has(item)) {
        set.add(item);
        yield item;
      }
    }
  }
}

var uniqueCategories = pipe(
  flatmap(products, (p) => p.categories),
  distinct(),
  toarray()
);
```

The key benefit of using sequence operators is readability. We can clearly express the intent of the code and immediately see what code is doing. We extract all categories and then filter them to obtain a sequence of unique items. We chose to use the `flatmap` operator because we needed to flatten all the categories. Let's modify the task slightly, we want to return a unique names of the products. Of course we can use `map` instead of `flatmap`, but this scenario is so common that the powerseq version of `distinct` operator takes an optional function parameter.

```javascript
for(var productName of pipe(map(products, p => p.name), distinct()) ) { ... }
for(var productName of distinct(products, p => p.name) ) { ... }
```

#### count

The `count` operator is straightforward yet quite helpful. Let's say we would like to count all products in the `iPhone` category. Some programmers use the built-in `filter` method to achieve this.

```javascript
var iphoneCount = products.filter((p) => p.name.includes("iPhone")).length;
```

It might not be a significant issue, but this code generates an unnecessary array in memory solely to access the `length` property. To address this, we can implement our own general-purpose `count` operator that works not only with arrays but also with any iterable object.

```javascript
function count(...args) {
  return args[0] && args[0][Symbol.iterator]
    ? count_(...args)
    : (s) => count_(s, ...args);

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

In its simplest usage, it counts the items from the input sequence. However, we can also pass a predicate function to filter out specific items.

```javascript
var iphoneCount = pipe(
  products,
  filter((p) => p.name.includes("iPhone")),
  count()
); // no array is created
var iphoneCount = count(products, (p) => p.name.includes("iPhone"));
```

#### toobject

The next code pattern never fails to amaze me whenever I encounter it. It's because there are so many different ways to implement it, let's take a look at a few of them:

```javascript
var productsMap = products.reduce((obj, p) => ((obj[p.id] = p), obj), {});
var productsMap = products.reduce((obj, p) => ({ ...obj, [p.id]: p }), {});
var productsMap = Object.assign({}, ...products.map((p) => ({ [p.id]: p })));

// {
//   '1': { id: '1', name: 'iPhone 11', categories: [ 'Phone', 'Apple' ] },
//   '2': { id: '2', name: 'Samsung xperia', categories: [ 'Phone', 'Samsung' ] },
//   '3': { id: '3', name: 'Samsung TV', categories: [ 'TV', 'Samsung' ] }
// }
```

Overall, this code is fine. It is concise, it includes a few JavaScript tricks, it uses built-in methods. The problem I have with this code is that the task of converting a list of items into a map object is a common one in JavaScript. Perhaps it deserves its own dedicated function. Whenever we come across such code, we have to decode it in our heads. We often need to read it carefully, keeping track of all the variables and trying to understand what the final data model will be. This process can be time-consuming and mentally taxing. Moreover, many junior developers may not even be capable of writing such "smart code", they would write imperative code with loops and variables. That's perfectly fine as well. However, they still need to read and analyse such code because it may be written by other team members. Let's introduce the `toobject` operator:

```javascript
function toobject(...args) {
  return typeof args[0] !== "function"
    ? toobject_(...args)
    : (s) => toobject_(s, ...args);

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

var productsMap = toobject(products, (p) => p.id);
```

The `toobject` operator takes two arguments. The first argument is a function that specifies the key of the map, and the second argument, which is optional, allows us to specify the value stored in the map.

```javascript
// { '1': 'iPhone 11', '2': 'Samsung xperia', '3': 'Samsung TV' }
var productNamesMap = toobject(
  products,
  (p) => p.id,
  (p) => p.name
);
```

From now on, the code creating an object representing the map is just a single call of `toobject` function. As a homework, search your repository to find all usages of `reduce` method, try to count how many times it's just an implementation of the code pattern described above.

#### join

When our application interacts with the database, it's quite evident when and how to use SQL's `JOIN` or `GROUP BY` clauses. However, when it comes to the application code, we often overlook these useful data manipulation patterns and end up implementing them manually repeatedly.

```javascript
var mongoProducts = products;
var externalSystemProducts = products;

// create a map of products for the first collection
var mongoProductsMap = mongoProducts.reduce(
  (obj, p) => ((obj[p.id] = p), obj),
  {}
);

// iterate over the second collection trying to find matching items
for (var ep of externalSystemProducts) {
  var mp = mongoProductsMap[ep.id];
  if (mp) {
    console.log(ep.name, " - ", mp.name);
  }
}
```

The same logic can be expressed using the `join` operator from the powerseq library, as demonstrated below:

```javascript
var { join } = require("powerseq");

var q = join(
  mongoProducts,
  externalSystemProducts,
  (mp) => mp.id,
  (ep) => ep.id,
  (mp, ep) => ({ mp, ep })
);

for (var { mp, ep } of q) {
  console.log(ep.name, " - ", mp.name);
}
```

The first lambda function specifies the key of the item from the first collection by which the data will be joined. The second function does the same for the item from the second collection. The third function takes two matching items and returns the final result. It looks scary at first but with time it becomes quite natural. It is just an implementation of the `INNER JOIN` operation.

#### groupby

Let's say we would like to group products by the company, `apple` products in one bucket, `samsung` products in other bucket and the rest of the products in `other` bucket. We could implement this manually like this:

```javascript
var groupByCompany = {};

for (const p of products) {
  var key = p.categories.includes("Apple")
    ? "apple"
    : p.categories.includes("Samsung")
    ? "samsung"
    : "other";

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

Or we can use an existing `groupby` operator:

```javascript
var { groupby } = require("powerseq");

var groupByCompany = groupby(products, (p) =>
  p.categories.includes("Apple")
    ? "apple"
    : p.categories.includes("Samsung")
    ? "samsung"
    : "other"
);

for (var group of groupByCompany) {
  console.log(group.key, "->", [...group]);
}
```

Sometimes, we need to group items and then perform aggregations such as `count`, `min`, `average` over them. Let's assume that, for some reason, many products can have the same `name`, and we want to find them.

```javascript
var productsWithTheSameNames = pipe(
  products,
  groupby((p) => p.name),
  filter((gr) => count(gr) > 1),
  map((gr) => gr.key)
);
```

#### concat

At the end the last operator `concat` allows us to iterate over items from any number of collections sequentially. Unlike the `concat` method from the `Array` type, no array is created; instead, the values flow through successive collections.

```javascript
var { concat } = require("powerseq");

for (var p of concat(mongoProducts, externalSystemProducts)) {
  console.log(p.name);
}
```

#### Summary

To clarify, we don't need to implement the operators presented above ourselves. The powerseq library provides all of them and even more, around 70 in total. I just wanted to demonstrate how simple they are. We can always read their source code when something is not clear or write our own when it's necessary. This particular library is not even important, there are many similar ones available. It's all about mindset and observation that identical code patterns are often reinvented repeatedly. Furthermore, the programming language is not so important, most languages offer constructs that enable us to write in this manner. Give it a try, it can make a significant difference. In the upcoming parts of the series, we will attempt to solve real tasks using powerseq while introducing new operators along the way.
