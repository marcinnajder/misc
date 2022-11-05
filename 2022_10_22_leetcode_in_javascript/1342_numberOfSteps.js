
// https://leetcode.com/problems/number-of-steps-to-reduce-a-number-to-zero/

var { pipe, expand, takewhile, count } = require("powerseq");

function numberOfSteps(num) {
    return pipe(
        [num],
        expand(n => n % 2 === 0 ? [n / 2] : [n - 1]),
        takewhile(n => n !== 0),
        count()
    );
}

numberOfSteps(14); // -> 6
numberOfSteps(8); // -> 4
numberOfSteps(123); // -> 12