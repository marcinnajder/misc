// url: https://leetcode.com/problems/swap-nodes-in-pairs/
// tags: list, pattern-matching, rec
// examples: [1,2,3,4] -> [2,1,4,3]

module LeetCode.P0024_SwapNodesInPairs

let rec swapPairs lst =
    match lst with
    | a :: b :: tail -> b :: a :: swapPairs tail
    | _ -> lst

let _ = swapPairs [ 1; 2; 3; 4 ] // -> [2; 1; 4; 3]
let _ = swapPairs ([]: list<int>) // -> []
let _ = swapPairs [ 1 ] // -> [1]
