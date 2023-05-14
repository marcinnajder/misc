module AdventOfCode2021.Day01

open System

let loadData (input: string) = input.Split Environment.NewLine |> Array.map Int32.Parse

let countIncreases numbers = numbers |> Seq.pairwise |> Seq.filter (fun (prev, next) -> next > prev) |> Seq.length

let puzzle1 (input: string) =
    let numbers = loadData input
    let increases = countIncreases numbers
    string increases

let puzzle2 (input: string) =
    let numbers = loadData input
    let addedNumbers = numbers |> Seq.windowed 3 |> Seq.map Array.sum
    let increases = countIncreases addedNumbers
    increases |> string
