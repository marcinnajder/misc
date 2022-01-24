module AdventOfCode2021.Day11

open System

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines |> Seq.map (fun line -> line |> Seq.map (fun c -> c.ToString() |> Int32.Parse)) |> array2D

let eachXY (data: int [,]) =
    seq {
        let xUpperBound = data.GetLength(0) - 1
        let yUpperBound = data.GetLength(1) - 1
        for x = 0 to xUpperBound do
            for y = 0 to yUpperBound do
                yield x, y
    }

let increment (data: int [,]) = data |> eachXY |> Seq.iter (fun (x, y) -> data.[x, y] <- data.[x, y] + 1)

let flash (data: int [,]) =
    let xUpperBound = data.GetLength(0) - 1
    let yUpperBound = data.GetLength(1) - 1
    let xys = data |> eachXY |> Seq.filter (fun (x, y) -> data.[x, y] > 9)
    let mutable wasFlash = false
    for (x, y) in xys do
        data.[x, y] <- Int32.MinValue
        wasFlash <- true
        let xxyys =
            Seq.allPairs
                (seq { max (x - 1) 0 .. min (x + 1) xUpperBound })
                (seq { max (y - 1) 0 .. min (y + 1) yUpperBound })
            |> Seq.filter (fun (xx, yy) -> (xx, yy) <> (x, y))
        for (xx, yy) in xxyys do
            data.[xx, yy] <- data.[xx, yy] + 1
    wasFlash

let zero (data: int [,]) =
    data
    |> eachXY
    |> Seq.filter (fun (x, y) -> data.[x, y] < 0)
    |> Seq.map (fun (x, y) -> data.[x, y] <- 0)
    |> Seq.length

let step (data: int [,]) =
    increment data
    while flash data do
        ()
    zero data

let puzzle1 (input: string) =
    let data = loadData input
    let result = seq { 1 .. 100 } |> Seq.sumBy (fun _ -> step data)
    result |> string

let puzzle2 (input: string) =
    let data = loadData input
    let result = seq { 1 .. Int32.MaxValue } |> Seq.findIndex (fun _ -> step data = data.Length)
    result + 1 |> string
