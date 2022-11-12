// https://leetcode.com/problems/longest-substring-without-repeating-characters/

module LeetCode.P0003_LengthOfLongestSubstring

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


let _ = lengthOfLongestSubstring "abcabcbb"
let _ = lengthOfLongestSubstring "bbbbb"
let _ = lengthOfLongestSubstring "pwwkew"
