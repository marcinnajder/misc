
// https://leetcode.com/problems/pascals-triangle/

var { pipe, take, toarray, map, expand } = require("powerseq");


function* pairwise(items) {
    var iterator = items[Symbol.iterator]();
    var result;

    if (!(result = iterator.next()).done) {

        var prev = result.value;
        while (!(result = iterator.next()).done) {
            yield [prev, result.value];
            var prev = result.value;
        }
    }
}

function pascalsTriangle(rowNumber) {
    return pipe(
        expand([[1]], prev => [[1, ...map(pairwise(prev), ([x, y]) => x + y), 1]]),
        take(rowNumber),
        toarray());
}

pascalsTriangle(0); // -> [ ]
pascalsTriangle(1); // -> [ [ 1 ] ]
pascalsTriangle(2); // -> [ [ 1 ], [ 1, 1 ] ]
pascalsTriangle(3); // -> [ [ 1 ], [ 1, 1 ], [ 1, 2, 1 ] ]
pascalsTriangle(4); // -> [ [ 1 ], [ 1, 1 ], [ 1, 2, 1 ], [ 1, 3, 3, 1 ] ]
pascalsTriangle(5); // -> [ [ 1 ], [ 1, 1 ], [ 1, 2, 1 ], [ 1, 3, 3, 1 ], [ 1, 4, 6, 4, 1 ] ]



