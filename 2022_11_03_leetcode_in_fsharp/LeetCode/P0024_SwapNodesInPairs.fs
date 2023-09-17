// url: https://leetcode.com/problems/swap-nodes-in-pairs/
// tags: list, pattern-matching, rec
// examples: [1,2,3,4] -> [2,1,4,3]

module LeetCode.P0024_SwapNodesInPairs

open Utils

let rec swapPairs lst =
    match lst with
    | a :: b :: tail -> b :: a :: swapPairs tail
    | _ -> lst

swapPairs [ 1; 2; 3; 4 ] === [ 2; 1; 4; 3 ]
swapPairs ([]: list<int>) === []
swapPairs [ 1 ] === [ 1 ]
