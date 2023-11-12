var { pipe, find, empty, filter, concat, range, map, distinct, toarray, take } = require("powerseq");

function qs(xs) {
    const v = find(xs);
    return v === undefined ? empty() : concat(qs(filter(xs, x => x < v)), [v], qs(filter(xs, x => x > v)));
}

var filter = (...args) => [...require("powerseq").filter(...args)];

// [...qs([10, 5, 9, 2, 4, 8, 0])]


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
}); // 5422 ms


