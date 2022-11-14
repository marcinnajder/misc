

## Programming with sequences part 3 - real world problems

#### Introduction

In this part of the series we will implement two problems that I had to implement myself in the production code. Some of you may say that the puzzles from [leetcode](https://leetcode.com/) or [advent of code](https://adventofcode.com/) are fake and have nothing to do with day to day programmers job. In some sense it's true, not very often clients want from us 
a new implementation of [pascal's triangle](https://en.wikipedia.org/wiki/Pascal%27s_triangle). But on the other hand, almost every day we fetch some data from the database into memory, process them and return to the client from the RESTful service. We use the same code constructs to solve both kinds of problems. 

#### Traversing the three structure

Let's say we have some three structure stored in the database. It is not too big so we can load all the structure into the memory at once. Files and folders are the good representation of such a structure. We use TypeScript language to describe the data model we will be working on.

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
                { name: "file1", type: "file" }
            ]
        },
        {
            name: "file666",
            type: "file",
        }
    ]
};
```

Now let's say we have to implement two different REST endpoints:
- the first one finds the first element from the tree for the specified `type` (`folder` or `file`)
- the second one returns all unique `type` values

It would be nice to have some general way of traversing the three. We could use it during the implementation of both endpoints.

```javascript
function traverse(elements: Element[]): Iterable<Element> {
    return flatmap(elements, e => !e.items ? [e] : concat([e], traverse(e.items)));
}
```

`traverse` function converts the tree structure into the flat sequence of elements. It uses `flatmap` and `concat` operators from the powerseq library and the recursion call. This function is lazy, it means it's just the prescription for the process of walking the tree. No one said we have to walk through the whole structure. For example, in the first endpoint we need to find the first element of a specified type `type`.

```javascript
function findElementOfType(elements: Element[], elementType: string) {
    return pipe(
        traverse(elements),
        find(ee => ee.type === elementType)
    );
}
```

In the second endpoint we have to process the whole structure because we are interested in all unique types of elements.

```javascript
function getUniqueElementTypes(element: Element) {
    return pipe(
        traverse([element]),
        map(ee => ee.type),
        distinct(),
        toarray()
    );
}
```

#### Generating PDF file

[pdfmake](http://pdfmake.org/#/) is a great library for generating PDF files. It is written entirely in JavaScript so it can be run on the client side in the browser or on the server side using node.js. We send the JS object describing the file content to the pdfmake and the binary data of PDF is returned back. We can play live with the JS object structure in [pdfmake playground](http://pdfmake.org/playground.html). 

Let's pretend we build an application where the user fills the surveys and the results are stored in the database. We can export or print the results as a PDF file later. The simple data model for the survey results could look like this:

```javascript

interface Results {
    [id: string]: { label: string; value: string; };
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

Each of the answers have its own `id` and the pair of values, `label` and `value`. The sample definition of PDF document in pdfmake format could look like this:

```javascript
var dd = {
 content:  [
        { columns: [{ text: 'CompanyName: Super Company' }] },
        { columns: [{ text: 'Address: London' }] },
        { columns: [{ text: 'Nip: 00000' }, { text: 'Regon: 000000' }] },
        { columns: [{ text: 'Email: example@example.com' }, { text: 'Phone: 000000000' },] }
    ]
}
```

Once we copy this code into [pdfmake playground](http://pdfmake.org/playground.html) we will the layout with four rows, the last two of them have two columns. The problem is that all survey answers are optional. So the user can skip some of the questions and still we would like to present the PDF document in decent way. We can resolve this problem by introducing the JS layout object and the `formatColumns` function. This function takes two JS objects (the survey answer and the layout) and it creates JS object representing the final PDF document. It is universal because it knows nothing about any specific layout and it can handle correctly the lack of answers.

```javascript
let LAYOUT = [
    ['companyName'],
    ['address'],
    ['nip', 'regon'],
    ['email', 'phone'],
];

function formatColumns(layout: string[][], answersObj: Results) {
    return pipe(layout,
        map(row =>
            pipe(row,
                map(id => answersObj[id]),
                filter(a => !!a),
                map(({ label, value }) => ({ text: `${label}: ${value}` })),
                toarray())
        ),
        filter(columns => columns.length > 0),
        map(columns => ({ columns })),
        toarray()
    );
}
```
The nice thing about this code is that we use a very basic operators like `map` and `filter` and the final result is simple and useful. 

#### Summary

I hope you have learned something new during the whole series. The function style of programming changes the way we think about the problems. Thanks to the lazy evaluation of sequence, our solutions are readable, reusable and performant the same time. I use powerseq every day at work. I really hope that some day all sequence operators will be available [in the box in JavaScript API](https://twitter.com/rauschma/status/1567865231983919115).