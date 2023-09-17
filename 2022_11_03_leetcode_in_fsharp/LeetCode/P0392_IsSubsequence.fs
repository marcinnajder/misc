// url: https://leetcode.com/problems/is-subsequence/
// tags: scan, forall
// examples: "abc" "ahbgdc" -> true

module LeetCode.P0392_IsSubsequence

open Utils

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


isSubsequence "abc" "ahbgdc" === false
isSubsequence "axc" "ahbgdc" === false

let isSubsequence' (s: string) (t: string) =
    t
    |> Seq.scan (fun index c -> if s[index] = c then index + 1 else index) 0
    |> Seq.exists (fun index -> index = s.Length)
