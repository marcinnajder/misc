// url: https://leetcode.com/problems/zigzag-conversion/
// tags: unfold, zip, groupBy
// examples: "PAYPALISHIRING" 3 -> 'PAHNAPLSIIGYIR"

module LeetCode.P0006_ZigzagConversion

open Utils

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

convert "PAYPALISHIRING" 3 === "PAHNAPLSIIGYIR"
convert "PAYPALISHIRING" 4 === "PINALSIGYAHRPI"
convert "A" 1 === "A"
