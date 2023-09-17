// url: https://leetcode.com/problems/how-many-numbers-are-smaller-than-the-current-number/
// tags: mapi, filter
// examples: [| 8; 1; 2; 2; 3 |] -> [4;0;1;1;3]

module LeetCode.P1365_SmallerNumbersThanCurrent

open Utils

let smallerNumbersThanCurrent numbers =
    numbers
    |> Seq.mapi (fun i n ->
        { 0 .. Array.length numbers - 1 } |> Seq.filter (fun j -> i <> j && numbers[j] < n) |> Seq.length)
    |> Seq.toArray


[| 8; 1; 2; 2; 3 |] |> smallerNumbersThanCurrent === [| 4; 0; 1; 1; 3 |]
[| 7; 7; 7; 7 |] |> smallerNumbersThanCurrent === [| 0; 0; 0; 0 |]
