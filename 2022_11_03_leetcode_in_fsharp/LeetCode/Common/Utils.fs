module LeetCode.Utils

let (===) actual expected = if actual = expected then () else failwithf "assertion failed: %A <> %A" actual expected
