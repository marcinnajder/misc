module AdventOfCode2022.Day05

open System
open Common

type Move = { Quantity: int; From: int; To: int }

type Data = { Crates: Map<int, char list>; Moves: Move array }

let parseMoves lines =
    lines
    |> Seq.map matchesNumbers3
    |> Seq.map (fun (a, b, c) -> { Quantity = a; From = b - 1; To = c - 1 })
    |> Seq.toArray

let parseCrates (lines: string []) =
    let reversedlines = Array.rev lines
    let indexes = { 1..4 .. (reversedlines.[0].Length - 1) } |> Seq.toArray
    reversedlines
    |> Seq.skip 1
    |> Seq.fold
        (fun crates line ->
            indexes
            |> Seq.indexed
            |> Seq.fold
                (fun crates' (crateIndex, charIndex) ->
                    match line.[charIndex] with
                    | ' ' -> crates'
                    | c ->
                        Map.change
                            crateIndex
                            (function
                            | None -> Some [ c ]
                            | Some lst -> Some(c :: lst))
                            crates')
                crates)
        Map.empty<int, char list>

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    let indexOfBlankLine = Seq.findIndex String.IsNullOrEmpty lines
    let cratesLines, moveLines = Array.splitAt indexOfBlankLine lines
    { Crates = parseCrates cratesLines; Moves = parseMoves (Seq.skip 1 moveLines) }


let moveOneByOne fromStack toStack quantity =
    let rec loop n fromStack' toStack' =
        match n, fromStack' with
        | 0, _
        | _, [] -> fromStack', toStack'
        | _, head :: tail -> loop (n - 1) tail (head :: toStack')
    loop quantity fromStack toStack

let moveAllAtOnce fromStack toStack quantity =
    let rec loop n fromStack' movedItemsStack =
        match n, fromStack' with
        | 0, _
        | _, [] -> fromStack', movedItemsStack |> List.fold (fun lst item -> item :: lst) toStack
        | _, head :: tail -> loop (n - 1) tail (head :: movedItemsStack)
    loop quantity fromStack []


let puzzle input mover =
    let data = loadData input
    let finalCrates =
        data.Moves
        |> Seq.fold
            (fun crates' move ->
                let fromStack = Map.find move.From crates'
                let toStack = Map.find move.To crates'
                let fromStack', toStack' = mover fromStack toStack move.Quantity
                crates'
                |> Map.change move.From (fun _ -> Some fromStack')
                |> Map.change move.To (fun _ -> Some toStack'))
            data.Crates
    String.Concat(finalCrates |> Map.toSeq |> Seq.sortBy fst |> Seq.map (snd >> List.head))

let puzzle1 input = puzzle input moveOneByOne

let puzzle2 input = puzzle input moveAllAtOnce
