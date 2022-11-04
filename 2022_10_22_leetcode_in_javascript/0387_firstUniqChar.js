// https://leetcode.com/problems/first-unique-character-in-a-string/

var { findindex, every, range } = require("powerseq");


function firstUniqChar(s) {
    return findindex(s, (c, i) => every(range(0, s.length), j => i === j || c !== s[j])) ?? -1;
}

firstUniqChar("letmein") // -> 0
firstUniqChar("lifeislovepoem") // -> 2
firstUniqChar("aabb") // -> -1