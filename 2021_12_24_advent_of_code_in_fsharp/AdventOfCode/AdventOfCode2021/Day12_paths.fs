module AdventOfCode2021.Day12

open System

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines
    |> Seq.map (fun line ->
        let parts = line.Split("-")
        parts.[0], parts.[1])

let isStart node = node = "start"
let isEnd node = node = "end"
let isSmall node = node |> Seq.forall Char.IsLower

let processEdges edges =
    edges
    |> Seq.collect (fun ((from, to') as edge) ->
        if isStart from then [ edge ]
        elif isStart to' then [ (to', from) ]
        elif isEnd to' then [ edge ]
        elif isEnd from then [ (to', from) ]
        else [ edge; (to', from) ])
    |> Seq.groupBy (fun (from, _) -> from)
    |> Seq.map (fun (key, values) -> key, values |> Seq.map snd |> Seq.toArray)
    |> Map


let rec move node edges path isSmallTwice =
    seq {
        if isEnd node then
            yield node :: path
        else
            yield!
                edges
                |> Map.tryFind node
                |> Option.defaultValue Array.empty
                |> Seq.filter (fun to' -> not (isSmall to') || not (path |> List.contains to') || not isSmallTwice)
                |> Seq.collect (fun to' ->
                    move to' edges (node :: path) (isSmallTwice || (isSmall to' && path |> List.contains to')))
    }



let puzzle (input: string) isSmallTwice =
    let data = loadData input |> processEdges
    let result = move "start" data [] isSmallTwice |> Seq.length
    result |> string

let puzzle1 (input: string) = puzzle input true
let puzzle2 (input: string) = puzzle input false
