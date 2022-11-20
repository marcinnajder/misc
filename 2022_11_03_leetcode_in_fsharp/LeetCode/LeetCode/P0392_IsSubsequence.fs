﻿// https://leetcode.com/problems/is-subsequence/

module LeetCode.P0392_IsSubsequence

let isSubsequence (s: string) (t: string) =
    s
    |> Seq.scan
        (fun i c ->
            let start = i + 1
            let len = t.Length - start
            { start .. start + len - 1 } |> Seq.tryFindIndex (fun j -> t[j] = c) |> Option.defaultValue -1)
        -1
    |> Seq.skip 1
    |> Seq.forall (fun i -> i <> -1)


let _ = isSubsequence "abc" "ahbgdc" // -> true
let _ = isSubsequence "axc" "ahbgdc" // -> false
