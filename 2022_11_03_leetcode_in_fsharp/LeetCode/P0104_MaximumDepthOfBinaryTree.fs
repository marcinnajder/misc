// url: https://leetcode.com/problems/maximum-depth-of-binary-tree/
// tags: tree
// examples: [3,9,20,null,null,15,7] -> 3

module LeetCode.P0104_MaximumDepthOfBinaryTree

open Utils
open BinaryTree

let rec calcDepth tree compare =
    match tree with
    | Tip -> 0
    | Node (_, left, Tip) -> calcDepth left compare + 1
    | Node (_, Tip, right) -> calcDepth right compare + 1
    | Node (_, left, right) -> compare (calcDepth left compare) (calcDepth right compare) + 1

let maxDepth tree = calcDepth tree max

"[3,9,20,null,null,15,7]" |> parseTree |> maxDepth === 3
"[1,null,2]" |> parseTree |> maxDepth === 2
