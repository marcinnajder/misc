module AdventOfCode2021.Day15

// #load "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Common.fs"


let input =
    System.IO.File.ReadAllText
        "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day15.txt"

// ******************************************************************************

open System
open System.Collections.Generic

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines |> Seq.map (fun line -> line |> Seq.map (fun c -> c.ToString() |> Int32.Parse)) |> array2D

let min3 a b c = min (min a b) c

let fill' (data: int [,]) =
    let size = data.GetLength(0)
    let mins = Array2D.zeroCreate size size
    mins.[0, 0] <- data.[0, 0]
    for i = 1 to size - 1 do

        let minUpper1 = mins.[0, i - 1]
        let minUpper1Next = mins.[1, i - 1]
        let value1Next = data.[1, i]
        mins.[0, i] <- data.[0, i] + if i = 1 then minUpper1 else min minUpper1 (minUpper1Next + value1Next)

        let minUpper2 = mins.[i - 1, 0]
        let minUpper2Next = mins.[i - 1, 1]
        let value2Next = data.[i, 1]
        mins.[i, 0] <- data.[i, 0] + if i = 1 then minUpper2 else min minUpper2 (minUpper2Next + value2Next)

        for j = 1 to i - 1 do

            let minUpper1v1 = mins.[i - 1, j]
            let minUpper1v2 = mins.[i, j - 1]
            let minUpper1v3 = mins.[i - 1, j + 1]
            let value1vNext = data.[i, j + 1]
            mins.[i, j] <-
                data.[i, j]
                + if j = i - 1 then
                      min minUpper1v1 minUpper1v2
                  else
                      min3 minUpper1v1 minUpper1v2 (minUpper1v3 + value1vNext)


            let minUpper2v1 = mins.[j - 1, i]
            let minUpper2v2 = mins.[j, i - 1]
            let minUpper2v3 = mins.[j + 1, i - 1]
            let value2vNext = data.[j + 1, i]
            mins.[j, i] <-
                data.[j, i]
                + if j = i - 1 then
                      min minUpper2v1 minUpper2v2
                  else
                      min3 minUpper2v1 minUpper2v2 (minUpper2v3 + value2vNext)

        let minUpper5v1 = mins.[i - 1, i]
        let minUpper5v2 = mins.[i, i - 1]
        mins.[i, i] <- data.[i, i] + min minUpper5v1 minUpper5v2
    mins

let moveByOne (data: int [,]) =
    let size = data.GetLength(0)
    for x = 0 to size - 1 do
        for y = 0 to size - 1 do
            let value = data.[x, y]
            data.[x, y] <- if value = 9 then 1 else value + 1

let rippleEffect () =
    seq { 0 .. 8 }
    |> Seq.map (fun i -> i, seq { 0 .. i } |> Seq.map (fun j -> j, i - j) |> Seq.filter (fun (x, y) -> x < 5 && y < 5))


let replicateData (data: int [,]) =
    let size = data.GetLength(0)
    let result = Array2D.zeroCreate (size * 5) (size * 5)
    for index, positions in rippleEffect () do
        if index > 0 then moveByOne data
        for x, y in positions do
            result.[(x * size)..(x * size + size - 1), (y * size)..(y * size + size - 1)] <- data
    result


let puzzle (data: int [,]) =
    let result = fill' data
    let size = data.GetLength(0)
    let r = result.[size - 1, size - 1] - result.[0, 0]
    r |> string

let puzzle1 (input: string) = loadData input |> puzzle

let puzzle2 (input: string) = loadData input |> replicateData |> puzzle



// **
// 2882 my result, 2879  correct result :)
// http://theory.stanford.edu/~amitp/GameProgramming/AStarComparison.html
// https://www.redblobgames.com/pathfinding/a-star/introduction.html

let rec enqueue priority item queue =
    match queue with
    | head :: tail -> if priority < fst head then (priority, item) :: queue else head :: enqueue priority item tail
    | [] -> [ priority, item ]

let neighbors (x, y) upperBound =
    seq {
        if x > 0 then x - 1, y
        if y > 0 then x, y - 1
        if x < upperBound then x + 1, y
        if y < upperBound then x, y + 1
    }

let search (data: int [,]) =
    let size = data.GetLength(0)
    let upperBound = size - 1
    let theEnd = (upperBound, upperBound)

    let mutable queue = enqueue 0 (0, 0) []
    // let cameFrom = Dictionary<int * int, int * int>()
    let costSoFar = Dictionary<int * int, int>()
    costSoFar.[(0, 0)] <- 0

    let mutable result = 0
    while (result = 0) && not (List.isEmpty queue) do

        let (_, current) = List.head queue
        queue <- List.tail queue
        printfn "%A" current

        if current = theEnd then
            printfn " --> %d" costSoFar.Count
            result <- costSoFar.[current]
        else
            for next in neighbors current upperBound do
                let newCost = costSoFar.[current] + data.[fst next, snd next]
                let nextCostExists, nextCost = costSoFar.TryGetValue next
                if not nextCostExists || newCost < nextCost then
                    costSoFar.[next] <- newCost
                    //cameFrom.[next] <- current
                    // queue <- queue |> addToPriorityQueue newCost next // Dijkstra’s algorithm
                    let distance = (upperBound - fst next) + (upperBound - snd next)
                    // printfn $"{distance}"
                    queue <- queue |> enqueue (newCost + distance) next // A* algorithm
    result


//    for next in graph.neighbors(current):
//       new_cost = cost_so_far[current] + graph.cost(current, next)
//       if next not in cost_so_far or new_cost < cost_so_far[next]:
//          cost_so_far[next] = new_cost
//          priority = new_cost
//          frontier.put(next, priority)
//          came_from[next] = current



let puzzle3 () =
    let input =
        System.IO.File.ReadAllText
            "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day15.txt"
    let data = loadData input
    //let data = loadData input |> replicateData
    search data





// let rec walk (data: int [,]) upperBound (x, y) sum (minSum: int ref) =
//     let sum' = sum + data.[x, y]
//     if sum' < minSum.Value then
//         if x = upperBound && y = upperBound then
//             minSum := sum'
//         else
//             let goFurther =
//                 if x < upperBound then
//                     walk data upperBound (x + 1, y) sum' minSum
//                     sum' < minSum.Value
//                 else
//                     true
//             if goFurther && y < upperBound then walk data upperBound (x, y + 1) sum' minSum
