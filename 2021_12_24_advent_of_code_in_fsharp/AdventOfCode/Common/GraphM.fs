module GraphM

open System
open FSharpx.Collections // #r "nuget: FSharpx.Collections"

type Edge<'T> = { From: 'T; To: 'T; Weight: int }
// type Graph<'T> = Edge<'T> list

let dijkstraTraverse graph start =
    seq {
        let neighbors = graph |> Seq.groupBy (fun e -> e.From) |> Map
        let mutable costSoFar = [ (start, 0) ] |> Map
        let mutable queue = PriorityQueue.empty false |> PriorityQueue.insert (0, start)
        let mutable result = None

        while Option.isNone result && not (PriorityQueue.isEmpty queue) do
            let (currentCost, current), queue' = PriorityQueue.pop queue
            queue <- queue'
            yield currentCost, current

            for next in Map.tryFind current neighbors |> Option.defaultValue Seq.empty do
                let newCost = costSoFar.[current] + next.Weight
                let nextCostExists, nextCost = costSoFar.TryGetValue next.To
                if (not nextCostExists) || (newCost < nextCost) then
                    costSoFar <- Map.change next.To (fun _ -> Some newCost) costSoFar
                    queue <- PriorityQueue.insert (newCost, next.To) queue
    }

let dijkstraShortestPath graph start finish =
    dijkstraTraverse graph start |> Seq.tryPick (fun (cost, node) -> if node = finish then Some cost else None)



// building graph from "board representations" very often used in advent of code puzzles
let loadGridFromData (input: string) =
    input.Split Environment.NewLine
    |> Array.map (fun l -> l.ToCharArray() |> Array.map (fun a -> Char.GetNumericValue(a) |> int))

let neighbours (r, c) rMax cMax =
    seq {
        if r > 0 then r - 1, c
        if r < rMax then r + 1, c
        if c > 0 then r, c - 1
        if c < cMax then r, c + 1
    }

let loadGraphFromGrid (rows: int array array) =
    let rMax = rows.Length - 1
    let cMax = rows.[0].Length - 1
    Seq.allPairs { 0..rMax } { 0..cMax }
    |> Seq.collect (fun p ->
        neighbours p rMax cMax |> Seq.map (fun n -> { From = p; To = n; Weight = rows[fst n][snd n] }))

// https://adventofcode.com/2021/day/15
let input =
    """1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581"""

let graph = input |> loadGridFromData |> loadGraphFromGrid
dijkstraShortestPath graph (0, 0) (9, 9) |> ignore // 40

// let input =
//     System.IO.File.ReadAllText
//         "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day15.txt"
// dijkstraShortestPath graph (0,0) (99,99) // 540
