module AdventOfCode2021.Day07

open Common
open System

let loadData (input: string) = parseNumbers ',' input

let minMax items = items |> Seq.fold (fun (min', max') n -> (min min' n, max max' n)) (Int32.MaxValue, Int32.MinValue)

let distance a b = if a > b then a - b else b - a

let findMinFuelUsage items minMaxValue fuelUsage =
    let minValue, maxValue = minMaxValue
    let minFuel = items |> Array.sumBy (fun y -> fuelUsage minValue y)
    { minValue + 1 .. maxValue }
    |> Seq.fold
        (fun a x ->
            let _, minFuel' = a
            let result =
                items
                |> Seq.scan (fun p c -> p + (fuelUsage x c)) 0
                |> Seq.skip 1 // first item is an aggregate "0"
                |> Seq.takeWhile (fun t -> t < minFuel')
                |> Seq.zip { 0 .. items.Length - 1 }
                |> Seq.tryLast
            match result with
            | Some (index, total) -> if total < minFuel' && index = items.Length - 1 then (x, total) else a
            | None -> a)
        (minValue, minFuel)


let puzzle1 (input: string) =
    let data = loadData input
    let minMaxValue = minMax data
    let result = findMinFuelUsage data minMaxValue distance
    result |> string

let puzzle2 (input: string) =
    let data = loadData input
    let minValue, maxValue = minMax data
    let sumCache = maxValue - minValue + 1 |> Array.zeroCreate<int>
    { 1 .. sumCache.Length - 1 } |> Seq.scan (+) 0 |> Seq.iteri (fun index value -> sumCache.[index] <- value)
    let result = findMinFuelUsage data (minValue, maxValue) (fun a b -> sumCache.[distance a b])
    result |> string
