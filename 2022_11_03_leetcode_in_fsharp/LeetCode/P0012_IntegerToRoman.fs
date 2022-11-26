// url: https://leetcode.com/problems/integer-to-roman/
// tags: unfold, fold, pattern-matching
// examples: 3 -> "III"

module LeetCode.P0012_IntegerToRoman

let intToDigits (I, V, X) i =
    match i with
    | 1 -> [ (I: char) ]
    | 2 -> [ I; I ]
    | 3 -> [ I; I; I ]
    | 4 -> [ I; V ]
    | 5 -> [ V ]
    | 6 -> [ V; I ]
    | 7 -> [ V; I; I ]
    | 8 -> [ V; I; I; I ]
    | 9 -> [ I; X ]
    | _ -> []
    |> System.String.Concat


let intToRoman num =
    let tens = [ ('I', 'V', 'X'); ('X', 'L', 'C'); ('C', 'D', 'M'); ('M', '?', '?') ]
    (num, tens)
    |> Seq.unfold (fun (n, t) -> if n = 0 then None else Some(intToDigits (List.head t) (n % 10), (n / 10, List.tail t)))
    |> Seq.fold (fun lst digits -> digits :: lst) [] // reverse list
    |> System.String.Concat


let _ = intToRoman 3 // -> "III"
let _ = intToRoman 58 // -> "LVIII"
let _ = intToRoman 1994 // -> "MCMXCIV"
