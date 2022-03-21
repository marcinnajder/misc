module AdventOfCode2021.Day17

open System

type Range = { Min: int; Max: int }
type TargetArea = { X: Range; Y: Range }

let loadData (input: string) =
    let parts = input.Substring("target area: ".Length).Split([| "," |], StringSplitOptions.TrimEntries)
    let xParts = parts.[0].Substring("x=".Length).Split("..") |> Array.map Int32.Parse
    let yParts = parts.[1].Substring("y=".Length).Split("..") |> Array.map Int32.Parse
    { X = { Min = xParts.[0]; Max = xParts.[1] }; Y = { Min = yParts.[1]; Max = yParts.[0] } }

// 0, 1, 3, 6, 10, 15, ...
// let sums = Seq.unfold (fun (sum, index) -> let sum' = sum + index in Some(sum', (sum', index + 1))) (0, 0)
let sums = seq { 1 .. Int32.MaxValue } |> Seq.scan (+) 0


let shoot (velocity: int * int) =
    Seq.unfold
        (fun ((xv, yv), (x, y)) ->
            let position = (x + xv, y + yv)
            let velocity' = ((if xv = 0 then 0 else xv - 1), yv - 1)
            (position, (velocity', position)) |> Some)
        (velocity, (0, 0))

type ShootingResult =
    | Miss of int * int
    | Hit of int * int

let shootTarget (velocity: int * int) (ta: TargetArea) =
    shoot velocity
    |> Seq.pick
        (function
        | x, y when x >= ta.X.Min && x <= ta.X.Max && y <= ta.Y.Min && y >= ta.Y.Max -> Some(Hit(x, y))
        | x, y when x > ta.X.Max || y < ta.Y.Max -> Some(Miss(x, y))
        | _ -> None)

let findMinXVelocity minX = sums |> Seq.takeWhile (fun s -> s < minX) |> Seq.length

let puzzle1 (input: string) =
    let data = loadData input
    let result = Seq.item (abs data.Y.Max - 1) sums
    result |> string

let puzzle2 (input: string) =
    let data = loadData input
    let xs = seq { findMinXVelocity data.X.Min .. data.X.Max }
    let ys = seq { data.Y.Max .. abs data.Y.Max - 1 }
    let result =
        Seq.allPairs xs ys
        |> Seq.map (fun position -> shootTarget position data)
        |> Seq.filter
            (function
            | Hit _ -> true
            | Miss _ -> false)
        |> Seq.length
    result |> string




// let findXRange (targetArea: TargetArea) =
//     seq { 1 .. Int32.MaxValue }
//     |> Seq.scan (fun (_, s) x -> (x, (s + x))) (0, 0)
//     |> Seq.skipWhile (fun (_, s) -> s < targetArea.X.Min)
//     |> Seq.takeWhile (fun (_, s) -> s <= targetArea.X.Max)
//     |> Seq.map fst

// let sumsCache = ResizeArray<int>()
// let sumUpTo n =
//     if n >= sumsCache.Count then
//         let initValue = if sumsCache.Count = 0 then 0 else sumsCache.[sumsCache.Count - 1]
//         let newSums = seq { sumsCache.Count .. n } |> Seq.scan (+) initValue |> Seq.skip 1
//         sumsCache.AddRange(newSums)
//     sumsCache.[n]


// let calculateY yVelocity n =
//     let nZero = 2 * yVelocity + 1
//     if n = 0 || n = nZero then
//         printfn "zero %d" n
//         0
//     elif n < nZero then
//         printfn "elif %d" n
//         let nn = if n <= yVelocity then n else nZero - n
//         sumUpTo yVelocity - sumUpTo (yVelocity - nn)
//     else
//         printfn "else %d" n
//         -(sumUpTo (n - (yVelocity + 1)) - sumUpTo (yVelocity))


// seq { 0 .. 10 } |> Seq.map (fun n -> calculateY 3 n) |> Seq.toArray
// calculateY 1 8
