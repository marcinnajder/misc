
// https://leetcode.com/problems/is-subsequence/

var { pipe, scan, find, range } = require("powerseq");

function isSubsequence(s, t) {
    return pipe(
        s,
        scan((i, c) => find(range(i + 1, t.length - (i + 1)), j => t[j] === c) ?? -1, -1),
        toarray()
    );
}

isSubsequence("abc", "ahbgdc"); // -> true
isSubsequence("axc", "ahbgdc"); // -> false
