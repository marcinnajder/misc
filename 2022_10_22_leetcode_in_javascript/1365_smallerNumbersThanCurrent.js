// https://leetcode.com/problems/how-many-numbers-are-smaller-than-the-current-number/

var { pipe, count, map, toarray, range } = require("powerseq");

function smallerNumbersThanCurrent(numbers) {
    return pipe(
        numbers,
        map((n, i) => count(range(0, numbers.length), j => i !== j && numbers[j] < n)),
        toarray()
    )
}

smallerNumbersThanCurrent([8, 1, 2, 2, 3]) // -> [4,0,1,1,3]
smallerNumbersThanCurrent([6, 5, 4, 8]) // -> [ 2, 1, 0, 3 ]
smallerNumbersThanCurrent([7, 7, 7, 7]) // -> [ 0, 0, 0, 0 ]