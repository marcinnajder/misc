// url: https://leetcode.com/problems/lowest-common-ancestor-of-a-binary-search-tree
// tags: tree
// examples: [6,2,8,0,4,7,9,null,null,3,5], 2, 8 -> 6

module LeetCode.P0235_LowestCommonAncestorOfABinarySearchTree

open Utils
open BinaryTree


let rec lowestCommonAncestor min' max' tree =
    match tree with
    | Tip -> Tip
    | Node (value, left, right) ->
        if value > max' then lowestCommonAncestor min' max' left
        elif value < min' then lowestCommonAncestor min' max' right
        else tree

let t = "[6,2,8,0,4,7,9,null,null,3,5]" |> parseTree
t |> lowestCommonAncestor 2 8 === t
t |> lowestCommonAncestor 2 4 === Node(2, Node(0, Tip, Tip), Node(4, Node(3, Tip, Tip), Node(5, Tip, Tip)))
"[2,1]" |> parseTree |> lowestCommonAncestor 1 2 === Node(2, Node(1, Tip, Tip), Tip)



bst |> lowestCommonAncestor 1 3 === Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip))
bst |> lowestCommonAncestor 6 8 === Node(7, Node(6, Tip, Tip), Node(8, Tip, Tip))
bst |> lowestCommonAncestor 1 2 === Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip))
bst |> lowestCommonAncestor 1 15 === bst

bst |> lowestCommonAncestor 1 8
=== Node(5, Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip)), Node(7, Node(6, Tip, Tip), Node(8, Tip, Tip)))
