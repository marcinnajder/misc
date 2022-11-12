// https://leetcode.com/problems/roman-to-integer/

module LeetCode.P0013_RomanToInt

let digitsToInt I V X t digits =
    match digits with
    | d :: rest when d = I ->
        match rest with
        | d :: rest when d = I ->
            match rest with
            | d :: rest when d = I -> Some(3 * t), rest
            | _ -> Some(2 * t), rest
        | d :: rest when d = V -> Some(4 * t), rest
        | d :: rest when d = X -> Some(9 * t), rest
        | _ -> Some(1 * t), rest
    | d :: rest when d = V ->
        match rest with
        | d :: rest when d = I ->
            match rest with
            | d :: rest when d = I ->
                match rest with
                | d :: rest when d = I -> Some(8 * t), rest
                | _ -> Some(7 * t), rest
            | _ -> Some(6 * t), rest
        | d :: rest when d = V -> Some(4 * t), rest
        | _ -> Some(5 * t), rest
    | _ -> None, digits

let romanToInt (s: string) =
    let digits = s |> List.ofSeq
    seq {
        digitsToInt 'M' '?' '?' 1000
        digitsToInt 'C' 'D' 'M' 100
        digitsToInt 'X' 'L' 'C' 10
        digitsToInt 'I' 'V' 'X' 1
    }
    |> Seq.scan
        (fun (restDigits, total) f ->
            match f restDigits with
            | Some (value), rest -> rest, total + value
            | _, rest -> rest, total)
        (digits, 0)
    |> Seq.find (fun (restDigits, total) -> List.isEmpty restDigits)
    |> snd


let _ = romanToInt "III" // -> 3
let _ = romanToInt "LVIII" // -> 58
let _ = romanToInt "MCMXCIV" // ->  1994

// https://archeologia.com.pl/cyfry-rzymskie-liczby/
// [ "IV"; "VI"; "VII"; "XCV"; "CM"; "MIX"; "MXXV"; "MMXIV"; "MMCLXVI" ]
// |> List.map (fun s -> printfn "%s -> %d" s (romanToInt s))
// |> ignore

// I               X           C       M
// II              XX          CC      MM
// III             XXX         CCC     MMM
// IV              XL          CD
// V               L           D
// VI              LX          DC
// VII             LXX         DCC
// VIII            LXXX        DCCC
// IX              XC          CM
