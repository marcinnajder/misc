// url: https://leetcode.com/problems/binary-tree-postorder-traversal/
// tags: tree
// examples: [1,null,2,3] -> [ 3; 2; 1 ]

module LeetCode.P0145_BinaryTreePostorderTraversal

open Utils
open BinaryTree

let rec postorderTraversal tree =
    match tree with
    | Tip -> []
    | Node (value, left, right) -> postorderTraversal left @ postorderTraversal right @ [ value ]

"[1,null,2,3]" |> parseTree |> postorderTraversal === [ 3; 2; 1 ]
"[]" |> parseTree |> postorderTraversal === []
"[1]" |> parseTree |> postorderTraversal === [ 1 ]
