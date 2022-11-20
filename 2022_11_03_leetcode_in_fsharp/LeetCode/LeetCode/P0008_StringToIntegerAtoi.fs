// https://leetcode.com/problems/string-to-integer-atoi/


module LeetCode.P0008_StringToIntegerAtoi

open System

let rec readDigits chars =
    match chars with
    | d :: rest when Char.IsDigit(d) -> d :: readDigits rest
    | _ -> []

let rec readChars chars =
    match chars with
    | ' ' :: rest -> readChars rest
    | '+' :: rest -> readDigits rest
    | '-' :: rest -> '-' :: readDigits rest
    | d :: rest when Char.IsDigit(d) -> d :: readDigits rest
    | _ -> []

let myAtoi s =
    let digits = s |> List.ofSeq |> readChars
    (digits, (1, 0))
    ||> List.foldBack (fun d (tens, total) ->
        match d with
        | '-' -> (tens, total * -1)
        | _ -> (tens * 10, total + (d |> string |> Int32.Parse) * tens))
    |> snd

let _ = myAtoi "42" // -> 42
let _ = myAtoi "   -42" // -> -42
let _ = myAtoi "4193 with words" // -> 4193

// todo
// If the integer is out of the 32-bit signed integer range [-231, 231 - 1],
// then clamp the integer so that it remains in the range. Specifically,
// integers less than -231 should be clamped to -231, and integers greater than 231 - 1 should be clamped to 231 - 1.
// -> hint: P0007_ReverseInteger -> reverseSAfe
