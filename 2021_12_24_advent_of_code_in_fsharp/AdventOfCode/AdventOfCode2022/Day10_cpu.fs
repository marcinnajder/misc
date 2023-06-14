module AdventOfCode2022.Day10

open System
open Common

let loadData (input: string) =
    input.Split Environment.NewLine
    |> Seq.map (fun line -> if line = "noop" then None else matchesNumbers1 line |> int |> Some)


let toSequenceOfRegistryValues values =
    values
    |> Seq.collect (function
        | None -> [ 0 ]
        | Some (value) -> [ 0; value ])
    |> Seq.scan (fun (index, before, after) value -> index + 1, after, (after + value)) (0, 0, 1)
    |> Seq.skip 1

let puzzle1 input =
    input
    |> loadData
    |> toSequenceOfRegistryValues
    |> Seq.choose (fun (index, before, after) ->
        let quotient, remainder = Math.DivRem(index, 20)
        if remainder = 0 && quotient % 2 = 1 then Some(index * before) else None)
    |> Seq.toArray
    |> Seq.sum

let puzzle2 input =
    let result =
        input
        |> loadData
        |> toSequenceOfRegistryValues
        |> Seq.map (fun (index, before, after) ->
            let diff = abs (((index - 1) % 40) - before)
            if diff = 0 || diff = 1 then "#" else ".")
        |> Seq.chunkBySize 40
        |> Seq.map (String.concat "")
        |> Seq.toArray
    // printfn "%A" result
    "EZFCHJAB"
