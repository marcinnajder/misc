
// https://leetcode.com/problems/is-subsequence/

var { pipe, scan, find, range, every } = require("powerseq");

function isSubsequence(s, t) {
    return pipe(
        s,
        scan((i, c) => find(range(i + 1, t.length - (i + 1)), j => t[j] === c) ?? -1, -1),
        every(i => i !== -1)
    );
}

isSubsequence("abc", "ahbgdc"); // -> true
isSubsequence("axc", "ahbgdc"); // -> false
