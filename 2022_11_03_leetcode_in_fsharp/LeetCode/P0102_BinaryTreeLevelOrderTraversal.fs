// url: https://leetcode.com/problems/binary-tree-level-order-traversal/
// tags: tree
// examples: [3,9,20,null,null,15,7] -> [ [ 3 ]; [ 9; 20 ]; [ 15; 7 ] ]

module LeetCode.P0102_BinaryTreeLevelOrderTraversal

open Utils
open BinaryTree

let rec splitLevel level =
    match level with
    | [] -> [], []
    | Tip :: rest -> splitLevel rest
    | Node (value, left, right) :: rest ->
        let values, nodes = splitLevel rest
        value :: values, left :: right :: nodes

let rec levelOrder level =
    let values, nextLevel = splitLevel level
    if List.isEmpty values then [] else values :: levelOrder nextLevel


[ "[3,9,20,null,null,15,7]" |> parseTree ] |> levelOrder === [ [ 3 ]; [ 9; 20 ]; [ 15; 7 ] ]
[ "[1]" |> parseTree ] |> levelOrder === [ [ 1 ] ]
[ "[]" |> parseTree ] |> levelOrder === []
