module AdventOfCode2021.Day06

open Common

let loadData (input: string) = parseNumbers ',' input

let executeStep (items: int64 []) =
    let zeroLengthCount = items.[0]
    for i = 0 to (items.Length - 2) do
        items.[i] <- items.[i + 1]
    items.[6] <- items.[6] + zeroLengthCount
    items.[8] <- zeroLengthCount
    ()


let countItemsAfterNIteration input n =
    let data = loadData input
    let map = data |> Seq.countBy id |> Map
    let items = Array.init 9 (fun index -> Map.tryFind index map |> Option.defaultValue 0 |> int64)
    for i = 0 to (n - 1) do
        executeStep items
    Array.sum items


let puzzle1 (input: string) = countItemsAfterNIteration input 80 |> string

let puzzle2 (input: string) = countItemsAfterNIteration input 256 |> string
