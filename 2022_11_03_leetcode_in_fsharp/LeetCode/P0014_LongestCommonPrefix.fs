﻿// url: https://leetcode.com/problems/longest-common-prefix/
// tags: forall, takeWhile
// examples: [| "flower"; "flow"; "flight" |] -> "fl"

module LeetCode.P0014_LongestCommonPrefix

open Utils

let longestCommonPrefix (strs: string []) =
    { 0 .. (strs |> Seq.map (fun str -> str.Length) |> Seq.min) - 1 }
    |> Seq.map (fun i ->
        let char = strs[0][i]
        char, strs |> Seq.forall (fun str -> str[i] = char))
    |> Seq.takeWhile snd
    |> Seq.map fst
    |> System.String.Concat


[| "flower"; "flow"; "flight" |] |> longestCommonPrefix === "fl"
[| "dog"; "racecar"; "car" |] |> longestCommonPrefix === ""
