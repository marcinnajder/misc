
// https://leetcode.com/problems/is-subsequence/

var { pipe, scan, find, range, every, some } = require("powerseq");

function isSubsequence(s, t) {
    return pipe(
        s,
        scan((i, c) => find(range(i + 1, t.length - (i + 1)), j => t[j] === c) ?? -1, -1),
        every(i => i !== -1)
    );
}

isSubsequence("abc", "ahbgdc"); // -> true
isSubsequence("axc", "ahbgdc"); // -> false

function isSubsequence2(s, t) {
    return pipe(
        t,
        scan((i, c) => s[i] === c ? i + 1 : i, 0),
        some(i => i === s.length)
    );
}
