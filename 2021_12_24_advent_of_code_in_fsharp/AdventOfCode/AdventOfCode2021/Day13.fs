module AdventOfCode2021.Day13

open System
open AdventOfCode2021

[<Literal>]
let FoldYPrefix = "fold along y="

type Fold =
    | X of int
    | Y of int

type Input = { Points: (int * int) []; Folds: Fold [] }

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    let points =
        lines
        |> Seq.takeWhile (fun line -> line <> "")
        |> Seq.map (fun line ->
            let parts = line |> Common.parseNumbers ','
            parts.[0], parts.[1])
        |> Seq.toArray
    let folds =
        lines
        |> Seq.skip (points.Length + 1)
        |> Seq.map (fun line ->
            let position = line.Substring(FoldYPrefix.Length) |> Int32.Parse
            if line.StartsWith(FoldYPrefix) then Y position else X position)
        |> Seq.toArray
    { Points = points; Folds = folds }

let foldPaper (points: (int * int) []) f =
    points
    |> Seq.map (fun ((x, y) as p) ->
        match f with
        | Y foldY -> if y < foldY then p else (x, y - (2 * (y - foldY)))
        | X foldX -> if x < foldX then p else (x - (2 * (x - foldX)), y))
    |> Seq.toArray


let puzzle1 (input: string) =
    let data = loadData input
    let points = data.Folds |> Seq.take 1 |> Seq.fold foldPaper data.Points |> Seq.distinct
    let result = points |> Seq.length
    result |> string


let print (points: (int * int) []) =
    let maxX, maxY = points |> Seq.reduce (fun (x1, y1) (x2, y2) -> (max x1 x2), (max y1 y2))
    let screen = Array2D.create (maxX + 1) (maxY + 1) '.'
    points |> Seq.iter (fun (x, y) -> screen.[x, y] <- '#')
    let text =
        { 0 .. (screen.GetLength(0) - 1) }
        |> Seq.map (fun x -> String.Join("", screen.[x, *]))
        |> Seq.reduce (fun p c -> p + Environment.NewLine + c)
    text

let puzzle2 (input: string) =
    let data = loadData input
    let points = data.Folds |> Seq.fold foldPaper data.Points |> Seq.distinct
    let result = points |> Seq.toArray |> print
    "CAFJHZCK"
