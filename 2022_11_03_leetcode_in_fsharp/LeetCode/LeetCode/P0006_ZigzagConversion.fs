// https://leetcode.com/problems/zigzag-conversion/

module LeetCode.P0006_ZigzagConversion

let convert (s: string) numRows =
    if numRows = 1 then
        s
    else
        (0, 1)
        |> Seq.unfold (fun (i, step) ->
            let i', step' =
                if i < 0 then 1, 1
                else if i = numRows then numRows - 2, -1
                else i, step
            Some(i', (i' + step', step')))
        |> Seq.zip s
        |> Seq.groupBy (fun (char, i) -> i)
        |> Seq.collect (fun (key, pairs) -> pairs)
        |> Seq.map fst
        |> System.String.Concat

let _ = convert "PAYPALISHIRING" 3 // -> PAHNAPLSIIGYIR
let _ = convert "PAYPALISHIRING" 4 // -> PINALSIGYAHRPI
let _ = convert "A" 1 // -> A
