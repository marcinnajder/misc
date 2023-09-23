// url: https://leetcode.com/problems/binary-tree-inorder-traversal/
// tags: tree
// examples: [1,null,2,3] -> [ 1; 3; 2 ]

module LeetCode.P0094_BinaryTreeInorderTraversal

open Utils
open BinaryTree

let rec inorderTraversal tree =
    match tree with
    | Tip -> []
    | Node (value, left, right) -> inorderTraversal left @ value :: inorderTraversal right

"[1,null,2,3]" |> parseTree |> inorderTraversal === [ 1; 3; 2 ]
"[]" |> parseTree |> inorderTraversal === []
"[1]" |> parseTree |> inorderTraversal === [ 1 ]
