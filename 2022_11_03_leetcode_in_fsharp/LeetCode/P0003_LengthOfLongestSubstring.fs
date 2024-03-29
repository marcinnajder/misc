﻿// url: https://leetcode.com/problems/longest-substring-without-repeating-characters/
// tags: scan, takeWhile
// examples: "abcabcbb" -> 3

module LeetCode.P0003_LengthOfLongestSubstring

open Utils

let lengthOfLongestSubstring (s: string) =
    { 0 .. s.Length - 1 }
    |> Seq.scan
        (fun maxLength i ->
            let currentLength =
                { i .. s.Length - 1 }
                |> Seq.scan
                    (fun (set, exits) j -> if Set.contains s.[j] set then set, true else (Set.add s.[j] set), false)
                    (Set.empty, false)
                |> Seq.takeWhile (fun (set, exits) -> not exits)
                |> Seq.map fst
                |> Seq.last
                |> Set.count

            max maxLength currentLength)
        0
    |> Seq.indexed
    |> Seq.takeWhile (fun (i, maxLength) -> maxLength < s.Length - i)
    |> Seq.map snd
    |> Seq.last


lengthOfLongestSubstring "abcabcbb" === 3
lengthOfLongestSubstring "bbbbb" === 1
lengthOfLongestSubstring "pwwkew" === 2
