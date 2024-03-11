
// https://leetcode.com/problems/plus-one/

var { scan, reverse, pipe, toarray, map, concat, skipwhile, skip } = require("powerseq");

function plusOne(digits) {
    return pipe(
        concat([0], digits),
        reverse(),
        scan(([_, carry], digit) => carry === 0 ? [digit, 0] : (digit === 9 ? [0, 1] : [digit + 1, 0]), [0, 1]),
        skip(1),
        map(([d, _]) => d),
        reverse(),
        skipwhile(d => d === 0),
        toarray()
    );
}

plusOne([4, 3, 2, 2]) // ->  [ 4, 3, 2, 3 ]
plusOne([1, 2, 3]) // -> [1,2,4]
plusOne([9]) // -> [1, 0]

function plusOne_(digits) {
    var result = new Array(digits.length);
    var [newDigit, carry] = [0, 1];

    for (var i = digits.length - 1; i >= 0; i--) {
        var digit = digits[i];
        ([newDigit, carry] = carry === 0 ? [digit, 0] : (digit === 9 ? [0, 1] : [digit + 1, 0]));
        result[i] = newDigit;
    }

    if (carry === 1) {
        result.unshift(1);
    }

    return result;
}