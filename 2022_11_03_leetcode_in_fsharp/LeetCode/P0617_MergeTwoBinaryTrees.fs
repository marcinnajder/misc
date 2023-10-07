// url: https://leetcode.com/problems/merge-two-binary-trees
// tags: tree
// examples: [1,3,2,5], [2,1,3,null,4,null,7] -> [3,4,5,5,4,null,7]

module LeetCode.P0617_MergeTwoBinaryTrees

open Utils
open BinaryTree

let rec mergeTrees tree1 tree2 =
    match tree1, tree2 with
    | Tip, Tip -> Tip
    | Tip, tree -> tree
    | tree, Tip -> tree
    | Node (value1, left1, right1), Node (value2, left2, right2) ->
        Node(value1 + value2, mergeTrees left1 left2, mergeTrees right1 right2)

mergeTrees (parseTree "[1,3,2,5]") (parseTree "[2,1,3,null,4,null,7]")
=== Node(3, Node(4, Node(5, Tip, Tip), Node(4, Tip, Tip)), Node(5, Tip, Node(7, Tip, Tip)))

mergeTrees (parseTree "[1]") (parseTree "[1,2]") === Node(2, Node(2, Tip, Tip), Tip)
