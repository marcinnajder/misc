// url: https://leetcode.com/problems/balanced-binary-tree
// tags: tree
// examples: [3,9,20,null,null,15,7] -> true

module LeetCode.P0110_BalancedBinaryTree

open Utils
open BinaryTree

let rec isBalanced tree =
    match tree with
    | Tip -> Some(0)
    | Node (_, left, right) ->
        isBalanced left
        |> Option.bind (fun leftD ->
            isBalanced right
            |> Option.bind (fun rightD -> if abs (leftD - rightD) > 1 then None else Some(max leftD rightD + 1)))

"[3,9,20,null,null,15,7]" |> parseTree |> isBalanced === Some(3)
"[1,2,2,3,3,null,null,4,4]" |> parseTree |> isBalanced === None
