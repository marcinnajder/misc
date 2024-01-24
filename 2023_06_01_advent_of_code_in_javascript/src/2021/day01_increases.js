var { readFileSync } = require("fs");
var { EOL } = require("os");
var { pipe, filter, map, buffer, count, sum } = require("powerseq");


function loadData(input) {
    return pipe(input.split(EOL), map(l => parseInt(l)));
}

function countIncreases(numbers) {
    return pipe(
        numbers,
        buffer(2, 1),
        filter(a => a.length === 2),
        count(([prev, next]) => next > prev)
    );
}

function puzzle1(input) {
    return pipe(input, loadData, countIncreases);
}

function puzzle2(input) {
    return pipe(input,
        loadData,
        buffer(3, 1),
        filter(items => items.length === 3),
        map(items => sum(items)),
        countIncreases
    );
}

var input = readFileSync("./src/2021/day01.txt", "utf-8");

console.log(puzzle1(input));
console.log(puzzle2(input));
