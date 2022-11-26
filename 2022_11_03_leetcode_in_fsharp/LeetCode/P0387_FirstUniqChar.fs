// url: https://leetcode.com/problems/first-unique-character-in-a-string/
// tags: indexed, tryFindIndex, forall
// examples: "letmein" -> 0

module LeetCode.P0387_FirstUniqChar

let firstUniqChar (s: string) =
    s
    |> Seq.indexed
    |> Seq.tryFindIndex (fun (i, c) -> { 0 .. s.Length - 1 } |> Seq.forall (fun j -> i = j || c <> s[j]))
    |> Option.defaultValue -1

let _ = firstUniqChar "letmein" // -> 0
let _ = firstUniqChar "lifeislovepoem" // -> 2
let _ = firstUniqChar "aabb" // -> -1
