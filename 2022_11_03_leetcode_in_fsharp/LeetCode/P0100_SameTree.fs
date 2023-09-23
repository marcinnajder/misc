// url: https://leetcode.com/problems/same-tree/
// tags: tree
// examples: [1,2,3], [1,2,3] -> true

module LeetCode.P0100_SameTree

open Utils
open BinaryTree

let rec isSameTree tree1 tree2 =
    match tree1, tree2 with
    | Tip, Tip -> true
    | Tip, _
    | _, Tip -> false
    | Node (value1, left1, right1), Node (value2, left2, right2) ->
        value1 = value2 && isSameTree left1 left2 && isSameTree right1 right2

(parseTree "[1,2,3]", parseTree "[1,2,3]") ||> isSameTree === true
(parseTree "[1,2]", parseTree "[1,null,2]") ||> isSameTree === false
(parseTree "[1,2,1]", parseTree "[1,1,2]") ||> isSameTree === false
(parseTree "[1,2,1]", parseTree "[1,1,2]") ||> (=) === false
