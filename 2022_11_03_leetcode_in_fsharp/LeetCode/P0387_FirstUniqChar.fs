// url: https://leetcode.com/problems/first-unique-character-in-a-string/
// tags: indexed, tryFindIndex, forall
// examples: "letmein" -> 0

module LeetCode.P0387_FirstUniqChar

open Utils

let firstUniqChar (s: string) =
    s
    |> Seq.indexed
    |> Seq.tryFindIndex (fun (i, c) -> { 0 .. s.Length - 1 } |> Seq.forall (fun j -> i = j || c <> s[j]))
    |> Option.defaultValue -1

firstUniqChar "letmein" === 0
firstUniqChar "lifeislovepoem" === 2
firstUniqChar "aabb" === -1
