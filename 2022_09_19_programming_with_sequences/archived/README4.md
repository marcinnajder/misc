## Programming with sequences part 3 - real world problems

#### Introduction

In this part of the series, we will implement two problems that I had to implement myself in production code. Some of you may argue that puzzles from [leetcode](https://leetcode.com/) or [advent of code](https://adventofcode.com/) are artificial and have little relevance to the day-to-day work of programmers. In some sense, that's true. Clients don't often request a new implementation of [Pascal's Triangle](https://en.wikipedia.org/wiki/Pascal%27s_triangle). On the other hand, almost every day, we retrieve data from the database into memory, process it, and return it to the client from the RESTful service. We use the same code constructs to solve both kinds of problems.

#### Traversing the three structure

Let's say we have a tree structure stored in the database. It's not excessively large, so we can load the entire structure into memory at once. Files and folders serve as a suitable representation for such a structure. We will be using the TypeScript language to describe the data model we'll be working with.

```javascript
interface Element {
  name: string;
  type: string;
  items?: Element[];
}

let root = {
  name: "C",
  type: "disc",
  items: [
    {
      name: "A",
      type: "folder",
      items: [
        { name: "file1", type: "file" },
        { name: "file1", type: "file" },
      ],
    },
    {
      name: "file666",
      type: "file",
    },
  ],
};
```

Now let's say we have to implement two different REST endpoints:

- The first one finds the first element from the tree for the specified `type` (`folder` or `file`).
- The second one returns all unique `type` values.

It would be beneficial to establish a general method for traversing the tree structure. We could use it during the implementation of both endpoints.

```javascript
function traverse(elements: Element[]): Iterable<Element> {
  return flatmap(elements, (e) =>
    !e.items ? [e] : concat([e], traverse(e.items))
  );
}
```

The `traverse` function converts the tree structure into a flat sequence of elements by utilising the `flatmap` and `concat` operators from the powerseq library, along with recursive calls. This function is lazy, meaning it serves as a blueprint for the process of traversing the tree. There is no requirement to traverse the entire structure. For example, in the first endpoint, our objective is to find the first element of a specified type, `type`.

```javascript
function findElementOfType(elements: Element[], elementType: string) {
  return pipe(
    traverse(elements),
    find((ee) => ee.type === elementType)
  );
}
```

In the second endpoint, we must process the entire structure because our goal is to identify all the unique types of elements.

```javascript
function getUniqueElementTypes(element: Element) {
  return pipe(
    traverse([element]),
    map((ee) => ee.type),
    distinct(),
    toarray()
  );
}
```

#### Generating PDF file

[pdfmake](http://pdfmake.org/#/) is an excellent library for generating PDF files. It is written entirely in JavaScript, so it can be run on the client side in the browser or on the server side using node.js. We send the JS object describing the file content to the pdfmake and it returns back the binary data of PDF. You can experiment with the structure of the JavaScript object in the [pdfmake playground](http://pdfmake.org/playground.html).

Let's imagine we're building an application where users fill out surveys, and the results are stored in a database. We can export or print the results as a PDF file later. A simplified data model for the survey results might look like this:

```javascript
interface Results {
  [id: string]: { label: string, value: string };
}

let results: Results = {
  companyName: { label: "CompanyName", value: "Super Company" },
  address: { label: "Address", value: "London" },
  nip: { label: "Nip", value: "00000" },
  regon: { label: "Regon", value: "000000" },
  email: { label: "Email", value: "example@example.com" },
  phone: { label: "Phone", value: "000000000" },
};
```

Each of the answers has its own `id` and a pair of values, `label` and `value`. The sample definition of a PDF document in pdfmake format could look like this:

```javascript
var dd = {
  content: [
    { columns: [{ text: "CompanyName: Super Company" }] },
    { columns: [{ text: "Address: London" }] },
    { columns: [{ text: "Nip: 00000" }, { text: "Regon: 000000" }] },
    {
      columns: [
        { text: "Email: example@example.com" },
        { text: "Phone: 000000000" },
      ],
    },
  ],
};
```

Once we paste this code into the [pdfmake playground](http://pdfmake.org/playground.html) we will see a layout with four rows, with the last two rows having two columns each. The challenge here is that all survey answers are optional. So, the user has the option to skip some of the questions, and we still want to present the PDF document in a visually pleasing and coherent manner. We can address this issue by introducing a JavaScript layout object and the `formatColumns` function. This function takes two JavaScript objects (the survey answer and the layout) and generates a JavaScript object representing the final PDF document. It is universal because it knows nothing about any specific layout and can handle correctly the lack of answers.

```javascript
let LAYOUT = [
  ["companyName"],
  ["address"],
  ["nip", "regon"],
  ["email", "phone"],
];

function formatColumns(layout: string[][], answersObj: Results) {
  return pipe(
    layout,
    map((row) =>
      pipe(
        row,
        map((id) => answersObj[id]),
        filter((a) => !!a),
        map(({ label, value }) => ({ text: `${label}: ${value}` })),
        toarray()
      )
    ),
    filter((columns) => columns.length > 0),
    map((columns) => ({ columns })),
    toarray()
  );
}
```

The great thing about this code is that we use very basic operators like `map` and `filter`, and the final result is straightforward and practical.

#### Summary

I hope you have learned something new throughout the entire series. The functional programming style changes the way we approach problems. Thanks to the lazy evaluation of sequences, our solutions become more readable, reusable, and performant at the same time. I use powerseq every day at work. I really hope that some day all sequence operators will be available [in the JavaScript API "out of the box"](https://twitter.com/rauschma/status/1567865231983919115).
