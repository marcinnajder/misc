module AdventOfCode2022.Day13

open System
open System.Text.RegularExpressions
open Common

type Packet =
    | List of Packet list
    | Value of int

let tokenize input = Regex.Matches(input, @"\d+|\[|\]") |> Seq.cast<Match> |> Seq.map (fun m -> m.Value) |> Seq.toList

let rec readTokens tokens =
    match tokens with
    | [] -> [], []
    | "]" :: tail -> [], tail
    | "[" :: tail ->
        let items, restTokens = readTokens tail
        let items', restTokens' = readTokens restTokens
        List items :: items', restTokens'
    | head :: tail ->
        let items, restTokens = readTokens tail
        Value(int head) :: items, restTokens


let loadData (input: string) =
    input.Split Environment.NewLine
    |> Seq.filter (fun line -> line <> "")
    |> Seq.map (tokenize >> readTokens >> fst >> List.head)


let rec comaprePackets left right =
    match left, right with
    | Value leftV, Value rightV -> compare leftV rightV
    | Value _, _ -> comaprePackets (List [ left ]) right
    | _, Value _ -> comaprePackets left (List [ right ])
    | List leftPs, List rightPs ->
        Seq.zip leftPs rightPs
        |> Seq.tryPick (fun (leftP, rightP) ->
            match comaprePackets leftP rightP with
            | 0 -> None
            | result -> Some result)
        |> Option.defaultWith (fun () -> compare (List.length leftPs) (List.length rightPs))

let puzzle1 input =
    input
    |> loadData
    |> Seq.chunkBySize 2
    |> Seq.indexed
    |> Seq.filter (function
        | _, [| p1; p2 |] -> comaprePackets p1 p2 = -1
        | _ -> failwith "I will never be here")
    |> Seq.sumBy (fst >> (+) 1)
    |> string

let puzzle2 input =
    let devider2 = List [ Value 2 ]
    let devider6 = List [ Value 6 ]
    let grouped = input |> loadData |> Seq.groupBy (fun p -> comaprePackets p devider2) |> Map
    let lowerThan2 = Map.find -1 grouped |> Seq.length
    let lowerThan6 = Map.find 1 grouped |> Seq.filter (fun p -> comaprePackets p devider6 = -1) |> Seq.length
    (lowerThan2 + 1) * (lowerThan2 + 1 + lowerThan6 + 1) |> string

// ** ** ** ** ** ** ** ** ** **

comaprePackets (Value 1) (Value 1) === 0
comaprePackets (Value 1) (Value 2) === -1
comaprePackets (Value 2) (Value 1) === 1
comaprePackets (List [ Value 1; Value 2 ]) (List [ Value 1; Value 2 ]) === 0
comaprePackets (List [ Value 1; Value 2; Value 3 ]) (List [ Value 1; Value 2 ]) === 1
comaprePackets (List [ Value 1 ]) (Value 1) === 0
