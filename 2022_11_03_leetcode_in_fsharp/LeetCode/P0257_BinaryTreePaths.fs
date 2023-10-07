// url: https://leetcode.com/problems/binary-tree-paths
// tags: tree
// examples: [1,2,3,null,5] -> ["1->2->5","1->3"]

module LeetCode.P0257_BinaryTreePaths

open Utils
open BinaryTree

let rec binaryTreePaths tree =
    match tree with
    | Tip -> []
    | Node (value, Tip, Tip) -> [ [ value ] ]
    | Node (value, left, right) -> binaryTreePaths left @ binaryTreePaths right |> List.map (fun lst -> value :: lst)


"[1,2,3,null,5]" |> parseTree |> binaryTreePaths === [ [ 1; 2; 5 ]; [ 1; 3 ] ]
"[1]" |> parseTree |> binaryTreePaths === [ [ 1 ] ]


let rec binaryTreePaths' tree =
    match tree with
    | Tip -> []
    | Node (value, Tip, Tip) -> [ [ value ] ]
    | Node (value, left, right) ->
        [ binaryTreePaths' left; binaryTreePaths' right ]
        |> Seq.concat
        |> Seq.map (fun lst -> value :: lst)
        |> Seq.toList


let rec binaryTreePathsSeq tree =
    match tree with
    | Tip -> Seq.empty
    | Node (value, Tip, Tip) -> seq { [ value ] }
    | Node (value, left, right) ->
        seq {
            binaryTreePathsSeq left
            binaryTreePathsSeq right
        }
        |> Seq.concat
        |> Seq.map (fun lst -> value :: lst)


let rec binaryTreePathsSeq' tree =
    seq {
        match tree with
        | Tip -> ()
        | Node (value, Tip, Tip) -> yield [ value ]
        | Node (value, left, right) ->
            for lst in [ binaryTreePathsSeq' left; binaryTreePathsSeq' right ] |> Seq.concat do
                yield value :: lst
    }

bst |> binaryTreePathsSeq |> Seq.toList
=== [ [ 10; 5; 2; 1 ]
      [ 10; 5; 2; 3 ]
      [ 10; 5; 7; 6 ]
      [ 10; 5; 7; 8 ]
      [ 10; 15; 13; 12 ]
      [ 10; 15; 13; 14 ]
      [ 10; 15; 20; 19 ]
      [ 10; 15; 20; 25 ] ]
