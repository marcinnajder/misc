module AdventOfCode2015.Day03

let loadData (input: string) = input :> seq<char>

let move (x, y) direction =
    match direction with
    | '>' -> (x + 1, y)
    | '<' -> (x - 1, y)
    | 'v' -> (x, y - 1)
    | '^' -> (x, y + 1)
    | _ -> failwith "wrong format"

let countDistinct items = items |> Seq.distinct |> Seq.length

let puzzle1 input =
    let directions = loadData input
    let result = directions |> Seq.scan move (0, 0) |> countDistinct
    result |> string

let puzzle2 input =
    let directions = loadData input
    let result =
        directions
        |> Seq.chunkBySize 2
        |> Seq.scan (fun (p1, p2) direction -> (move p1 direction.[0]), (move p2 direction.[1])) ((0, 0), (0, 0))
        |> Seq.collect (fun (p1, p2) -> [ p1; p2 ])
        |> countDistinct
    result |> string
