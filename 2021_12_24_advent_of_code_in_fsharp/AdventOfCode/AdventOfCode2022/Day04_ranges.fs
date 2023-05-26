module AdventOfCode2022.Day04

open System
open Common


let loadData (input: string) =
    input.Split Environment.NewLine |> Seq.map matchesNumbers4 |> Seq.map (fun (a, b, c, d) -> (a, b), (c, d))

let isRangeFullyContain (a, b) (c, d) = a <= c && b >= d

let isRangeOverlapping (a, b) (c, d) = c <= b && c >= a

let puzzle input pred =
    input
    |> loadData
    |> Seq.filter (fun (range1, range2) -> (pred range1 range2) || (pred range2 range1))
    |> Seq.length
    |> string

let puzzle1 input = puzzle input isRangeFullyContain

let puzzle2 input = puzzle input isRangeOverlapping
