// // https://leetcode.com/problems/plus-one/

module LeetCode.P0066_PlusOne

let digits = [ 4; 3; 2; 2 ]

let plusOne digits =
    Seq.append [ 0 ] digits
    |> Seq.rev
    |> Seq.scan
        (fun (_, carry) digit ->
            if carry = 0 then digit, 0
            else if digit = 9 then 0, 1
            else digit + 1, 0)
        (0, 1)
    |> Seq.skip 1
    |> Seq.map fst
    |> Seq.rev
    |> Seq.skipWhile (fun d -> d = 0)
    |> Seq.toArray

let _ = plusOne [ 4; 3; 2; 2 ] // -> [|4; 3; 2; 3|]
let _ = plusOne [ 1; 2; 3 ] // -> [|1; 2; 4|]
let _ = plusOne [ 9 ] // -> [|1; 0|]
