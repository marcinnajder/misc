
// https://leetcode.com/problems/is-subsequence/

var { pipe, scan, every, find, range } = require("powerseq");

function isSubsequence(s, t) {
    return pipe(
        s,
        scan((i, c) => find(range(i, t.length - i), j => t[j] === c) ?? -1, 0),
        every(i => i !== -1)
    );
}

isSubsequence("abc", "ahbgdc"); // -> true
isSubsequence("axc", "ahbgdc"); // -> false