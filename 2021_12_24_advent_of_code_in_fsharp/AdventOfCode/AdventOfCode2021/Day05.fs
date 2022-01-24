module AdventOfCode2021.Day5

open AdventOfCode2021.Common
open System

type Point = { X: int; Y: int }
type Line = { Start: Point; End: Point }

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines
    |> Seq.map
        (fun line ->
            let parts = line.Split([| "->" |], StringSplitOptions.RemoveEmptyEntries)
            let p1 = parseNumbers ',' parts.[0]
            let p2 = parseNumbers ',' parts.[1]
            { Start = { X = p1.[0]; Y = p1.[1] }
              End = { X = p2.[0]; Y = p2.[1] } })

let getRange from to' =
    if from = to' then Seq.initInfinite (fun _ -> from)
    elif from < to' then seq { from .. 1 .. to' }
    else seq { from .. -1 .. to' }


let getPoints line =
    if line.Start = line.End then
        line.Start |> Seq.singleton
    else
        Seq.zip (getRange line.Start.X line.End.X) (getRange line.Start.Y line.End.Y)
        |> Seq.map (fun (x, y) -> { X = x; Y = y })

let countAtTeastTwoLinesOverlapping lines =
    lines |> Seq.collect getPoints |> Seq.countBy id |> Seq.filter (fun (key, points) -> points > 1) |> Seq.length

let puzzle1 (input: string) =
    let data = loadData input |> Seq.filter (fun l -> l.Start.X = l.End.X || l.Start.Y = l.End.Y) |> List.ofSeq
    countAtTeastTwoLinesOverlapping data |> string


let puzzle2 (input: string) =
    let data = loadData input
    countAtTeastTwoLinesOverlapping data |> string
