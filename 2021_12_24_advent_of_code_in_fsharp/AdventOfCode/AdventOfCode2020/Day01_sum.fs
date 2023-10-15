module AdventOfCode2020.Day01

open Common

open System


let loadData (input: string) = input.Split Environment.NewLine |> Seq.map Int32.Parse

let puzzle1 input =
    let (a, b) = loadData input |> allUniquePairs |> Seq.find (fun (a, b) -> a + b = 2020)
    (a * b).ToString()

let puzzle2 input =
    let (a, b, c) = loadData input |> Seq.toArray |> allUniqueTriples |> Seq.find (fun (a, b, c) -> a + b + c = 2020)
    (a * b * c).ToString()



// using general function 'allUniqueTuples' instead of 'allUniquePairs', 'allUniqueTriples'
let puzzle' input n =
    let result =
        loadData input |> Seq.toList |> allUniqueTuples n |> Seq.find (fun lst -> List.sum lst = 2020) |> Seq.reduce (*)
    result.ToString()

let puzzle1' input = puzzle' input 2
let puzzle2' input = puzzle' input 3



// improve efficiency by using map of sums left
let findSumOfN n target numbers =
    let rec loop items map lists =
        match items with
        | [] -> None
        | item :: rest ->
            match Map.tryFind item map with
            | Some lst -> Some(item :: lst)
            | None ->
                let completed, lists' = insertThenPartition (n - 1) item lists
                let map' =
                    completed
                    |> Seq.fold (fun m lst -> m |> Map.change (target - List.sum lst) (Option.orElse (Some lst))) map
                loop rest map' lists'
    loop numbers Map.empty []


let puzzle'' input n =
    let result = loadData input |> Seq.toList |> findSumOfN n 2020 |> Option.get |> Seq.reduce (*)
    result.ToString()

let puzzle1'' input = puzzle'' input 3
let puzzle2'' input = puzzle'' input 3
