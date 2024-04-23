module AdventOfCode2015.Day02

open System

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines
    |> Seq.map (fun line ->
        match line.Split "x" |> Array.map Int32.Parse with
        | [| w; l; h |] -> (w, l, h)
        | _ -> failwith "wrong format")

let calculateAreaOfWrappingPaper (w, l, h) =
    let area = [ w * l; w * h; l * h ]
    2 * List.sum area + List.min area

let calculateLengthOfRibbon (w, l, h) =
    let min1, min2 = (min w l), min (max w l) h
    2 * (min1 + min2) + w * l * h

let puzzle1 input = loadData input |> Seq.sumBy calculateAreaOfWrappingPaper
let puzzle2 input = loadData input |> Seq.sumBy calculateLengthOfRibbon
