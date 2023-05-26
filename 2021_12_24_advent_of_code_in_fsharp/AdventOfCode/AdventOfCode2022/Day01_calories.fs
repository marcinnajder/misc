module AdventOfCode2022.Day01

open System
open Common

let input =
    System.IO.File.ReadAllText
        "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2022/Day0.txt"

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    Seq.append lines [ "" ]
    |> Seq.fold
        (fun (lists, lst) line ->
            match line with
            | "" -> lst :: lists, []
            | _ -> lists, (Int32.Parse line) :: lst)
        ([], [])
    |> fst

let rec insertSorted xs x =
    match xs with
    | [] -> [ x ]
    | head :: tail -> if x < head then x :: xs else head :: insertSorted tail x

let insertSortedPreservingLength xs x =
    match xs with
    | [] -> xs
    | head :: _ -> if x < head then xs else List.tail (insertSorted xs x)


let puzzle input topN =
    input
    |> loadData
    |> Seq.map List.sum
    |> Seq.fold insertSortedPreservingLength (List.replicate topN 0)
    |> List.sum
    |> string

let puzzle1 input = puzzle input 1

let puzzle2 input = puzzle input 3



//  ** ** **


(insertSorted [] 1 = [ 1 ]) |> assertTrue
(insertSorted [ 2 ] 1 = [ 1; 2 ]) |> assertTrue
(insertSorted [ 2; 3 ] 1 = [ 1; 2; 3 ]) |> assertTrue
(insertSorted [ 1; 3 ] 2 = [ 1; 2; 3 ]) |> assertTrue
(insertSorted [ 1; 2 ] 3 = [ 1; 2; 3 ]) |> assertTrue

(insertSortedPreservingLength [ 1; 3 ] 0 = [ 1; 3 ]) |> assertTrue
(insertSortedPreservingLength [ 1; 3 ] 1 = [ 1; 3 ]) |> assertTrue
(insertSortedPreservingLength [ 1; 3 ] 2 = [ 2; 3 ]) |> assertTrue
(insertSortedPreservingLength [ 1; 3 ] 3 = [ 3; 3 ]) |> assertTrue
(insertSortedPreservingLength [ 1; 3 ] 4 = [ 3; 4 ]) |> assertTrue
