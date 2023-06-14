module AdventOfCode2022.Day09

open System

type Direction =
    | Right
    | Left
    | Up
    | Down

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines
    |> Seq.map (fun line ->
        match line.Split(' ') with
        | [| "R"; value |] -> Right, int value
        | [| "L"; value |] -> Left, int value
        | [| "U"; value |] -> Up, int value
        | [| "D"; value |] -> Down, int value
        | _ -> failwith "wrong data format")
    |> Seq.toArray

let moveHead (xh, yh) direction =
    match direction with
    | Right -> xh + 1, yh
    | Left -> xh - 1, yh
    | Up -> xh, yh + 1
    | Down -> xh, yh - 1

let moveTail (xt, yt as tail) (xh, yh) =
    let yDelta = abs (yh - yt)
    let xDelta = abs (xh - xt)
    if xDelta = 2 || yDelta = 2 then
        (if xDelta = 2 then ((xh + xt) / 2) else xh), (if yDelta = 2 then ((yh + yt) / 2) else yh)
    else
        tail

let moveSnake direction snake =
    let snakeWithHeadMoved = Map.change 0 (Option.map (fun p -> moveHead p direction)) snake
    { 1 .. Map.count snakeWithHeadMoved - 1 }
    |> Seq.scan
        (fun (_, snake') i ->
            let head = Map.find (i - 1) snake'
            let tail = Map.find i snake'
            let newTail = moveTail tail head
            let wasMoved = tail <> newTail
            wasMoved, (if wasMoved then Map.change i (fun _ -> Some(newTail)) snake' else snake'))
        (true, snakeWithHeadMoved)
    |> Seq.takeWhile fst
    |> Seq.last
    |> snd

let puzzle input lengthOfSnake =
    let initSnake = { 0 .. lengthOfSnake - 1 } |> Seq.map (fun i -> i, (0, 0)) |> Map
    input
    |> loadData
    |> Seq.collect (fun (direction, value) -> Seq.replicate value direction)
    |> Seq.fold
        (fun (snake, trace) direction ->
            let snake' = moveSnake direction snake
            snake', Set.add (Map.find (Map.count snake' - 1) snake') trace)
        (initSnake, Set.empty)
    |> snd
    |> Set.count
    |> string

let puzzle1 input = puzzle input 2

let puzzle2 input = puzzle input 10
