// url: https://leetcode.com/problems/minimum-depth-of-binary-tree/
// tags: tree
// examples: [3,9,20,null,null,15,7] -> 2

module LeetCode.P0111_MinimumDepthOfBinaryTree

open Utils
open BinaryTree

open P0104_MaximumDepthOfBinaryTree

let minDepth tree = calcDepth tree min

"[3,9,20,null,null,15,7]" |> parseTree |> minDepth === 2
"[2,null,3,null,4,null,5,null,6]" |> parseTree |> minDepth === 5
