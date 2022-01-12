module AdventOfCode2021.Day13

// #load "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Common.fs"


let input =
    System.IO.File.ReadAllText
        "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day13.txt"

// ******************************************************************************


// open System

// let loadData (input: string) =
//     let lines = input.Split Environment.NewLine
//     lines
//     |> Seq.map
//         (fun line ->
//             let parts = line.Split("-")
//             parts.[0], parts.[1])

// let isStart node = node = "start"
// let isEnd node = node = "end"
// let isSmall node = node |> Seq.forall Char.IsLower

// let addReverseEdges edges =
//     edges
//     |> Seq.collect
//         (fun ((from, to') as edge) ->
//             if isStart from then [ edge ]
//             elif isStart to' then [ (to', from) ]
//             elif isEnd to' then [ edge ]
//             elif isEnd from then [ (to', from) ]
//             else [ edge; (to', from) ])
//     |> Seq.toArray

// let rec move node edges path isSmallTwice =
//     seq {
//         if isEnd node then
//             yield node :: path
//         else
//             let matchingEdges =
//                 edges
//                 |> Seq.filter
//                     (fun (from, to') ->
//                         from = node && (not (isSmall to') || not (path |> List.contains to') || not isSmallTwice))
//             for (_, to') in matchingEdges do
//                 yield! move to' edges (node :: path) (isSmallTwice || (isSmall to' && path |> List.contains to'))
//     }



// let puzzle (input: string) isSmallTwice =
//     let data = loadData input |> addReverseEdges
//     let result = move "start" data [] isSmallTwice |> Seq.length
//     result |> string

// let puzzle1 (input: string) = puzzle input true
// let puzzle2 (input: string) = puzzle input false
