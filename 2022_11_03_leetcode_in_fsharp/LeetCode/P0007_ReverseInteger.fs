// url: https://leetcode.com/problems/reverse-integer/
// tags: unfold, fold
// examples: 123 -> 321

module LeetCode.P0007_ReverseInteger

open System

let digitsOfIntReversed x = x |> Seq.unfold (fun n -> if n = 0 then None else Some((n % 10), (n / 10)))

let reverse x =
    x
    |> Seq.unfold (fun n -> if n = 0 then None else Some((n % 10), (n / 10))) // reversed digits
    |> Seq.fold (fun lst d -> d :: lst) [] // reverse items
    |> List.fold (fun (ten, total) d -> (ten * 10, total + d * ten)) (1, 0)
    |> snd

let _ = reverse 123 // -> 321
let _ = reverse -123 // -> -321
let _ = reverse 120 // -> 21


type Status =
    | Processing
    | Completed of int

let reverseSafe x =
    let isInsideRange total increase =
        if x >= 0 then Int32.MaxValue - total >= increase else Int32.MinValue - total <= increase

    let digits = digitsOfIntReversed x |> Seq.fold (fun lst d -> d :: lst) [] // reverse items

    (digits, 1, 0)
    |> Seq.unfold (fun state ->
        let lst, tens, total = state

        match lst with
        | d :: rest ->
            let increase = d * tens

            if isInsideRange total increase then
                Some(Processing, (rest, tens * 10, total + increase))
            else
                Some(Completed 0, state)
        | _ -> Some(Completed total, state))
    |> Seq.pick (function
        | Processing -> None
        | Completed result -> Some result)
