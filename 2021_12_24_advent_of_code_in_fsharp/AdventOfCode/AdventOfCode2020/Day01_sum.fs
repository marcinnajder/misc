module AdventOfCode2020.Day01

open Common

open System

// let input =
//     System.IO.File.ReadAllText
//         "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2020/Day01.txt"

let loadData (input: string) = input.Split Environment.NewLine |> Seq.map Int32.Parse

let puzzle1 input =
    let (a, b) = loadData input |> allUniquePairs |> Seq.find (fun (a, b) -> a + b = 2020)
    (a * b).ToString()

let puzzle2 input =
    let (a, b, c) = loadData input |> Seq.toArray |> allUniqueTriples |> Seq.find (fun (a, b, c) -> a + b + c = 2020)
    (a * b * c).ToString()



let puzzle input n =
    let result =
        loadData input |> Seq.toList |> allUniqueTuples n |> Seq.find (fun lst -> List.sum lst = 2020) |> Seq.reduce (*)
    result.ToString()

let puzzle1' input = puzzle input 2
let puzzle2' input = puzzle input 3
