module AdventOfCode2022.Day06

open Common

let loadData (input: string) = input

let isDistinct items =
    items
    |> Seq.scan (fun (s, _) item -> Set.add item s, Set.contains item s |> not) (Set.empty, true)
    |> Seq.forall snd

let puzzle input n =
    let chars = loadData input
    let firstN = chars |> Seq.take n |> Seq.indexed
    let other = chars |> Seq.skip n |> Seq.indexed
    let packageIndex =
        other
        |> Seq.scan (fun lastN (index, item) -> Map.change (index % n) (fun _ -> Some(item)) lastN) (Map firstN)
        |> Seq.findIndex (fun lastN -> lastN |> Map.toSeq |> Seq.map snd |> isDistinct)
    packageIndex + n |> string


let puzzle1 input = puzzle input 4

let puzzle2 input = puzzle input 14

// ** ** ** ** ** ** ** ** ** **

isDistinct "abcd" = true |> assertTrue
isDistinct "abca" = false |> assertTrue
