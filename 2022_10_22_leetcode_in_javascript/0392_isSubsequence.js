
// https://leetcode.com/problems/is-subsequence/

var { pipe, scan, every, find, range } = require("powerseq");

function isSubsequence(s, t) {
    return pipe(
        s,
        scan((i, c) => find(range(i + 1, t.length - (i + 1)), j => t[j] === c) ?? -1, -1),
        toarray()
        //every(i => i !== -1)
    );
}

isSubsequence("abc", "ahbgdc"); // -> true
isSubsequence("axc", "ahbgdc"); // -> false

var { take, expand } = require("powerseq");

// [...take(expand([1,2, 3], n => [n * 10, n * 100]), 20)]

