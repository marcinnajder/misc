// url: https://leetcode.com/problems/binary-tree-preorder-traversal/
// tags: tree
// examples: [1,null,2,3] -> [ 1; 2; 3 ]

module LeetCode.P0144_BinaryTreePreorderTraversal

open Utils
open BinaryTree

let rec preorderTraversal tree =
    match tree with
    | Tip -> []
    | Node (value, left, right) -> value :: preorderTraversal left @ preorderTraversal right

"[1,null,2,3]" |> parseTree |> preorderTraversal === [ 1; 2; 3 ]
"[]" |> parseTree |> preorderTraversal === []
"[1]" |> parseTree |> preorderTraversal === [ 1 ]
