module AdventOfCode2021.Day18

open System

type Number = NumberItem * NumberItem
and NumberItem =
    | Value of int
    | Number of Number

let rec printNumber numberItem =
    match numberItem with
    | Value (value) -> value |> string
    | Number (left, right) -> $"[{(printNumber left)},{printNumber right}]"


let wrongFormat () = failwith "Wrong format"

let rec readNumber tokens =
    match tokens with
    | '[' :: restTokens ->
        let item1, restTokens' = readNumber restTokens
        let item2, restTokens'' = readNumber restTokens'
        match restTokens'' with
        | ']' :: restTokens''' -> Number(item1, item2), restTokens'''
        | _ -> wrongFormat ()
    | token :: restTokens when Char.IsDigit(token) -> Value(token |> string |> int), restTokens
    | _ -> wrongFormat ()

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines |> Seq.map (Seq.filter (fun c -> c <> ',') >> List.ofSeq >> readNumber >> fst)



type ExplodeResult =
    | Explode of left: int option * right: int option
    | NoExplode

let rec addValue numberItem value leftOrRight =
    match numberItem with
    | Value (v) -> Value(v + value)
    | Number (left, rigth) ->
        if leftOrRight then
            Number(addValue left value leftOrRight, rigth)
        else
            Number(left, addValue rigth value leftOrRight)

let rec checkExplode numberItem level =
    match numberItem with
    | Number (Value (leftValue), Value (rightValue)) when level = 5 ->
        Value(0), Explode(Some(leftValue), Some(rightValue))
    | Number (left, right) ->
        match (checkExplode left (level + 1)) with
        | item, (Explode (leftExplosion, rightExplosion) as explode) ->
            match leftExplosion, rightExplosion with
            | _, Some (rightExplosionValue) ->
                Number(item, (addValue right rightExplosionValue true)), Explode(leftExplosion, None)
            | _ -> Number(item, right), explode
        | _, NoExplode ->
            match (checkExplode right (level + 1)) with
            | item, (Explode (leftExplosion, rightExplosion) as explode) ->
                match leftExplosion, rightExplosion with
                | Some (leftExplosionValue), _ ->
                    Number((addValue left leftExplosionValue false), item), Explode(None, rightExplosion)
                | _ -> Number(left, item), explode
            | _, NoExplode -> numberItem, NoExplode
    | Value _ -> numberItem, NoExplode


type SplitResult =
    | Split
    | NoSplit

let rec checkSplit numberItem =
    match numberItem with
    | Number (left, right) ->
        match (checkSplit left) with
        | item, Split -> Number(item, right), Split
        | _, NoSplit ->
            match (checkSplit right) with
            | item, Split -> Number(left, item), Split
            | _, NoSplit -> numberItem, NoSplit
    | Value (value) when value >= 10 -> Number(Value(value / 2), Value(value - (value / 2))), Split
    | Value _ -> numberItem, NoSplit


let rec tryReduceNumberOnce numberItem =
    match (checkExplode numberItem 1) with
    | item, Explode _ -> Some item
    | _, NoExplode _ ->
        match (checkSplit numberItem) with
        | item, Split -> Some item
        | _ -> None

let reduceNumber numberItem =
    Seq.unfold (fun state -> tryReduceNumberOnce state |> Option.map (fun item -> item, item)) numberItem
    |> Seq.tryLast
    |> Option.defaultValue numberItem

let reduceListOfNumbers numberItems = numberItems |> Seq.reduce (fun prev next -> Number(prev, next) |> reduceNumber)


let rec magnitude numberItem =
    match numberItem with
    | Value value -> value
    | Number (left, right) -> 3 * magnitude left + 2 * magnitude right


let puzzle1 (input: string) =
    let data = loadData input
    let result = data |> reduceListOfNumbers |> magnitude
    result |> string

let puzzle2 (input: string) =
    let data = loadData input
    let data' = data |> ResizeArray
    let result =
        Seq.allPairs data' data'
        |> Seq.filter (fun (a, b) -> not (Object.ReferenceEquals(a, b)))
        |> Seq.map (fun (a, b) -> Number(a, b) |> reduceNumber |> magnitude)
        |> Seq.max
    result |> string
