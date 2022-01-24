module AdventOfCode2021.Day9

open System
open System.Collections.Generic

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines |> Seq.map (fun line -> line |> Seq.map (fun c -> c.ToString() |> Int32.Parse)) |> array2D

let findLowPoints (data: int [,]) =
    seq {
        let xUpperBound = data.GetLength(0) - 1
        let yUpperBound = data.GetLength(1) - 1
        for x = 0 to xUpperBound do
            for y = 0 to yUpperBound do
                let value = data.[x, y]
                if (x = 0 || value < data.[x - 1, y])
                   && (x = xUpperBound || value < data.[x + 1, y])
                   && (y = 0 || value < data.[x, y - 1])
                   && (y = yUpperBound || value < data.[x, y + 1]) then
                    yield (value)
    }

let puzzle1 (input: string) =
    let data = loadData input
    let result = findLowPoints data |> Seq.sumBy (fun p -> p + 1)
    result |> string


type Line = { Index: int; Count: int }
type NumberedLine = { Number: int; Line: Line }

let getLines (items: seq<int>) =
    let _, _, lines =
        Seq.append items [ 9 ]
        |> Seq.indexed
        |> Seq.fold
            (fun state (index, n) ->
                let (isInsideBasin, basinBeginningIndex, lines: ResizeArray<Line>) = state
                match n, isInsideBasin with
                | 9, false -> state
                | 9, true ->
                    lines.Add(
                        { Index = basinBeginningIndex
                          Count = index - basinBeginningIndex }
                    )
                    (false, 0, lines)
                | _, false -> (true, index, lines)
                | _, true -> state)
            (false, 0, ResizeArray<Line>())
    lines

let numberLines (lines: seq<ResizeArray<Line>>) =
    lines
    |> Seq.scan (fun (i, _) lines -> (i + lines.Count, lines)) (0, ResizeArray<Line>())
    |> Seq.skip 1
    |> Seq.map
        (fun (i, lines) ->
            lines
            |> Seq.mapi
                (fun ii line ->
                    { Number = i - lines.Count + ii
                      Line = line })
            |> Seq.toList)
    |> Seq.toArray

let findCrossingLinesNumbers y (yLine: Line) (xLines: (NumberedLine list) []) =
    seq { yLine.Index .. (yLine.Index + yLine.Count - 1) }
    |> Seq.choose
        (fun i ->
            xLines.[i]
            |> List.tryHead
            |> Option.filter (fun line -> y >= line.Line.Index && y < line.Line.Index + line.Line.Count)
            |> Option.map (fun line -> line.Number))
    |> Seq.toArray


let mergeSets (sets: seq<int []>) =
    let uniqueSets = ResizeArray<HashSet<int>>()
    for set in sets do
        let overlappingSets = uniqueSets |> Seq.filter (fun s -> s.Overlaps(set)) |> Seq.toArray
        match overlappingSets.Length with
        | 0 -> set |> HashSet |> uniqueSets.Add
        | 1 -> set |> Seq.iter (overlappingSets.[0].Add >> ignore)
        | _ ->
            overlappingSets |> Seq.iter (uniqueSets.Remove >> ignore)
            uniqueSets.Add(overlappingSets |> Seq.concat |> Seq.append set |> HashSet)
    uniqueSets

let moveToY y (xLines: (NumberedLine list) []) =
    for x = 0 to xLines.Length - 1 do
        let lines = xLines.[x]
        match lines with
        | line :: rest -> if line.Line.Index + line.Line.Count - 1 < y then xLines.[x] <- rest
        | _ -> ()
    ()

let puzzle2 (input: string) =
    let data = loadData input
    let yLines = seq { 0 .. data.GetLength(1) - 1 } |> Seq.map (fun i -> getLines data.[*, i])
    let xLines = seq { 0 .. data.GetLength(0) - 1 } |> Seq.map (fun i -> getLines data.[i, *])
    let xLinesNumbered = xLines |> numberLines
    let xLinesNumberedMovedToY = xLinesNumbered |> Array.copy
    let mergedSets =
        yLines
        |> Seq.mapi
            (fun y yLines ->
                moveToY y xLinesNumberedMovedToY
                yLines |> Seq.map (fun yLine -> findCrossingLinesNumbers y yLine xLinesNumberedMovedToY))
        |> Seq.concat
        |> mergeSets
    let linesByNumber = xLinesNumbered |> Seq.concat |> Seq.toArray
    let result =
        mergedSets
        |> Seq.map (fun set -> set |> Seq.sumBy (fun n -> linesByNumber.[n].Line.Count))
        |> Seq.sortDescending
        |> Seq.take 3
        |> Seq.reduce (*)
    // printToFile data mergedSets linesByNumber xLinesNumbered
    result |> string


// **********
// let data = loadData input


// let printToFile
//     (data: int [,])
//     (mergedSets: ResizeArray<HashSet<int>>)
//     (linesByNumber: NumberedLine [])
//     (xLinesNumbered: (NumberedLine list) [])
//     =
//     let data1 = Array2D.create (data.GetLength(0)) (data.GetLength(1)) '-'
//     for setId, set in (mergedSets |> Seq.indexed) do
//         let lines = set |> Seq.map (fun item -> linesByNumber.[item])
//         for line in lines do
//             let y =
//                 seq { 0 .. (xLinesNumbered.Length - 1) }
//                 |> Seq.find (fun yy -> xLinesNumbered.[yy] |> List.exists (fun l -> l.Number = line.Number))
//             for x = line.Line.Index to (line.Line.Index + line.Line.Count - 1) do
//                 data1.[y, x] <- ((line.Number % 10) |> string).[0]
//     // data1.[y, x] <- ((setId % 10) |> string).[0]
//     // data1.[y, x] <- (200 + setId) |> char

//     let text =
//         seq { 0 .. (data1.GetLength(0) - 1) }
//         |> Seq.map (fun x -> String.Join("", data1.[x, *]))
//         |> Seq.reduce (fun p c -> p + Environment.NewLine + c)

//     System.IO.File.WriteAllText(
//         $"/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day09___.txt",
//         text
//     )
