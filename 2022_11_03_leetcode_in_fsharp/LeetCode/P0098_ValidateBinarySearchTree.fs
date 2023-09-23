// url: https://leetcode.com/problems/validate-binary-search-tree/
// tags: tree
// examples: [2,1,3] -> true

module LeetCode.P0098_ValidateBinarySearchTree

open Utils
open BinaryTree

let rec isValidBst tree =
    match tree with
    | Tip
    | Node (_, Tip, Tip) -> true
    | Node (value, left, right) ->
        match left, right with
        | Node (leftValue, _, _), Tip -> value > leftValue && isValidBst left
        | Tip, Node (rightValue, _, _) -> value < rightValue && isValidBst right
        | Node (leftValue, _, _), Node (rightValue, _, _) ->
            value > leftValue && value < rightValue && isValidBst left && isValidBst right
        | _ -> false

"[2,1,3]" |> parseTree |> isValidBst === true
"[5,1,4,null,null,3,6]" |> parseTree |> isValidBst === false
