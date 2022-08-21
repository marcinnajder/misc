module AdventOfCode2015.Day01

open System

let loadData (input: string) = input

let count input = input |> Seq.scan (fun acc c -> (if c = '(' then (+) else (-)) acc 1) 0

let puzzle1 input = loadData input |> count |> Seq.last |> string
let puzzle2 input = loadData input |> count |> Seq.findIndex ((=) -1) |> string


// let input = "()"

// input
// |> count
// |> Seq.fold
//     (fun (_, index) n ->
//         let index' =
//             match index with
//             | Choice2Of2 _ -> index
//             | Choice1Of2 i when n = -1 -> Choice2Of2 i
//             | Choice1Of2 i -> Choice1Of2(i + 1)
//         (n, index'))
//     (0, Choice1Of2 0)
// |> ignore
