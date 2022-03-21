module AdventOfCode2021.Day2

open System

type Direction =
    | Forward of int
    | Down of int
    | Up of int

let loadData (input: string) =
    input.Split Environment.NewLine
    |> Array.map
        (fun line ->
            let parts = line.Split(' ')
            let value = Int32.Parse(parts.[1])
            match parts.[0] with
            | "forward" -> Forward value
            | "down" -> Down value
            | "up" -> Up value
            | unknownDirection -> failwith $"Unknown direction '{unknownDirection}' found in data")

type Position1 = { Horizontal: int; Depth: int }

let puzzle1 (input: string) =
    let directions = loadData input
    let finalPosition =
        directions
        |> Array.fold
            (fun state direction ->
                match direction with
                | Forward value -> { state with Horizontal = state.Horizontal + value }
                | Down value -> { state with Depth = state.Depth + value }
                | Up value -> { state with Depth = state.Depth - value })
            { Horizontal = 0; Depth = 0 }
    let result = finalPosition.Horizontal * finalPosition.Depth
    result |> string

type Position2 = { Horizontal: int; Depth: int; Aim: int }

let puzzle2 (input: string) =
    let directions = loadData input
    let finalPosition =
        directions
        |> Array.fold
            (fun state direction ->
                match direction with
                | Forward value ->
                    { state with
                          Position2.Horizontal = state.Horizontal + value
                          Depth = state.Depth + (state.Aim * value) }
                | Down value -> { state with Aim = state.Aim + value }
                | Up value -> { state with Aim = state.Aim - value })
            { Horizontal = 0; Depth = 0; Aim = 0 }
    let result = finalPosition.Horizontal * finalPosition.Depth
    result |> string
