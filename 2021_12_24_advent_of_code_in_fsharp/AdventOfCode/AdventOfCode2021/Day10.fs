module AdventOfCode2021.Day10

open System

let loadData (input: string) = input.Split Environment.NewLine

type State =
    | Processing of char list
    | Corrupted of char
    | Completed of char list

let Close2OpenMap =
    Map [ (')', '(')
          ('>', '<')
          (']', '[')
          ('}', '{') ]

let Open2CloseMap = Close2OpenMap |> Map.toSeq |> Seq.map (fun (x, y) -> y, x) |> Map

let Points1 =
    Map [ (')', 3)
          (']', 57)
          ('}', 1197)
          ('>', 25137) ]

let Points2 =
    Map [ (')', 1L)
          (']', 2L)
          ('}', 3L)
          ('>', 4L) ]


let processText text =
    Seq.append text [ '-' ]
    |> Seq.scan
        (fun state c ->
            match state with
            | Processing items ->
                match c with
                | '-' -> Completed items
                | ')'
                | ']'
                | '}'
                | '>' ->
                    match items with
                    | head :: rest -> if Close2OpenMap.[c] = head then Processing rest else Corrupted c
                    | _ -> Corrupted c
                | _ -> Processing(c :: items)
            | _ -> state)
        (Processing [])

let puzzle1 (input: string) =
    let data = loadData input
    let result =
        data
        |> Seq.choose (
            processText
            >> Seq.tryPick (function
                | Corrupted c -> Some c
                | _ -> None)
        )
        |> Seq.sumBy (fun c -> Points1.[c])
    result |> string

let puzzle2 (input: string) =
    let data = loadData input
    let points =
        data
        |> Seq.choose (
            processText
            >> Seq.tryPick (function
                | Processing _ -> None
                | state -> Some state)
            >> Option.bind (function
                | Completed item -> Some item
                | _ -> None)
        )
        |> Seq.map (Seq.map (fun c -> Open2CloseMap.[c]) >> Seq.fold (fun a c -> (a * 5L) + Points2.[c]) (0L))
        |> Seq.sort
        |> Seq.toArray
    let result = points.[(points.Length - 1) / 2]
    result |> string
