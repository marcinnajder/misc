// url: https://leetcode.com/problems/binary-search-tree-iterator/
// tags: tree
// examples: [7,3,15,null,null,9,20] -> [ 3; 7; 9; 15; 20 ]

module LeetCode.P0173_BinarySearchTreeIterator

open Utils
open BinaryTree

let rec inorderTraversalSeq tree =
    seq {
        match tree with
        | Tip -> ()
        | Node (value, left, right) ->
            yield! inorderTraversalSeq left
            yield value
            yield! inorderTraversalSeq right
    }

"[7,3,15,null,null,9,20]" |> parseTree |> inorderTraversalSeq |> Seq.toList === [ 3; 7; 9; 15; 20 ]
