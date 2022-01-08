module AdventOfCode2021.Day1

open System


let parseInput (input: string) = input.Split Environment.NewLine |> Array.map Int32.Parse

let countIncreases numbers = numbers |> Seq.pairwise |> Seq.filter (fun (prev, next) -> next > prev) |> Seq.length

let puzzle1 (input: string) =
    let numbers = parseInput input
    let increases = countIncreases numbers
    increases |> string

let puzzle2 (input: string) =
    let numbers = parseInput input
    let addedNumbers = numbers |> Seq.windowed 3 |> Seq.map Array.sum
    let increases = countIncreases addedNumbers
    increases |> string
