// https://leetcode.com/problems/palindrome-number/

module LeetCode.P0009_PalindromeNumber

let digitsOfIntReversed x = x |> Seq.unfold (fun n -> if n = 0 then None else Some(n % 10, n / 10))

let isArrayPalindrome (a: array<_>) = { 0 .. a.Length / 2 - 1 } |> Seq.forall (fun i -> a[i] = a[a.Length - 1 - i])

let isPalindrome x = if x < 0 then false else x |> digitsOfIntReversed |> Seq.toArray |> isArrayPalindrome

let _ = isPalindrome 121 // -> true
let _ = isPalindrome -121 // -> false
let _ = isPalindrome 10 // ->   false
