﻿// url: https://leetcode.com/problems/pascals-triangle/
// tags: unfold, pairwise, take
// examples: 2 -> [ [ 1 ], [ 1, 1 ] ]

module LeetCode.P0118_PascalsTriangle

open Utils

let pascalsTriangle rowNumber =
    [ 1 ]
    |> Seq.unfold (fun prev ->
        let lst =
            List.concat [ [ 1 ]
                          prev |> List.pairwise |> List.map (fun (x, y) -> x + y)
                          [ 1 ] ]
        Some(lst, lst))
    |> Seq.append [ [ 1 ] ]
    |> Seq.take rowNumber
    |> Seq.toArray

pascalsTriangle 0 === [||]
pascalsTriangle 1 === [| [ 1 ] |]
pascalsTriangle 2 === [| [ 1 ]; [ 1; 1 ] |]
pascalsTriangle 3 === [| [ 1 ]; [ 1; 1 ]; [ 1; 2; 1 ] |]
pascalsTriangle 4 === [| [ 1 ]; [ 1; 1 ]; [ 1; 2; 1 ]; [ 1; 3; 3; 1 ] |]
pascalsTriangle 5 === [| [ 1 ]; [ 1; 1 ]; [ 1; 2; 1 ]; [ 1; 3; 3; 1 ]; [ 1; 4; 6; 4; 1 ] |]
