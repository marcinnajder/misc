// url: https://leetcode.com/problems/palindrome-number/
// tags: unfold, forall
// examples: 121 -> true

module LeetCode.P0009_PalindromeNumber

open Utils

let digitsOfIntReversed x = x |> Seq.unfold (fun n -> if n = 0 then None else Some(n % 10, n / 10))

let isArrayPalindrome (a: array<_>) = { 0 .. a.Length / 2 - 1 } |> Seq.forall (fun i -> a[i] = a[a.Length - 1 - i])

let isPalindrome x = if x < 0 then false else x |> digitsOfIntReversed |> Seq.toArray |> isArrayPalindrome

isPalindrome 121 === true
isPalindrome -121 === false
isPalindrome 10 === false
