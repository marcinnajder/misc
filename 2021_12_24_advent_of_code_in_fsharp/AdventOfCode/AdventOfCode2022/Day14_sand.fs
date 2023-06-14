module AdventOfCode2022.Day14

open System
open Common

let input =
    System.IO.File.ReadAllText
        "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2022/Day14.txt"

type Board = Map<int, (int * int) list>

let rec mergeLines lines line =
    match lines, line with
    | [], _ -> [ line ]
    | ((fromH, toH) as head) :: tail, (fromL, toL) ->
        if toH + 1 < fromL then head :: mergeLines tail line
        elif toL + 1 < fromH then line :: lines
        else mergeLines tail (min fromH fromL, max toH toL)


let insertStraightLine (board: Board) ((x1, y1), (x2, y2)) =
    if x1 = x2 then // vertical line
        Map.change x1 (fun lines -> Some(mergeLines (Option.defaultValue [] lines) (min y1 y2, max y1 y2))) board
    else // horizontal line
        { min x1 x2 .. max x1 x2 }
        |> Seq.fold
            (fun board' x ->
                Map.change x (fun lines -> Some(mergeLines (Option.defaultValue [] lines) (y1, y1))) board')
            board

let insertSnakeLine (board: Board) lineText =
    matchesNumbers lineText
    |> Seq.chunkBySize 2
    |> Seq.map (fun array -> array.[0], array.[1])
    |> Seq.pairwise
    |> Seq.fold insertStraightLine board

let loadData (input: string) = input.Split Environment.NewLine |> Seq.fold insertSnakeLine Map.empty


type FreeSpaceResult =
    | FSNotFound
    | FSFound of row: int
    | FSOccupied

let rec findFreeSpaceInColumn lines row =
    match lines with
    | [] -> FSNotFound
    | (fromH, toH) :: tail ->
        if row < fromH then FSFound(fromH - 1)
        elif row <= toH then FSOccupied
        else findFreeSpaceInColumn tail row



type FreeOnLeftAndRightResult =
    | FONotFound of column: int
    | FOFound of pos: (int * int)

let rec findFreeSpaceOnLeftAndRight (board: Board) (column, row) =
    let leftColumn = column - 1
    let leftRow = row + 1
    match findFreeSpaceInColumn (Map.tryFind leftColumn board |> Option.defaultValue []) leftRow with
    | FSNotFound -> FONotFound leftColumn
    | FSFound result -> findFreeSpaceOnLeftAndRight board (leftColumn, result)
    | FSOccupied ->
        let rightColumn = column + 1
        let rightRow = row + 1
        match findFreeSpaceInColumn (Map.tryFind rightColumn board |> Option.defaultValue []) rightRow with
        | FSNotFound -> FONotFound rightColumn
        | FSFound result -> findFreeSpaceOnLeftAndRight board (rightColumn, result)
        | FSOccupied -> FOFound(column, row)



let dropSand (board: Board) (floorRow: int option) =
    let rec loop b index =
        match findFreeSpaceInColumn (Map.find 500 b) 0 with
        | FSOccupied -> index
        | FSNotFound -> failwith "no worries, I won't be here"
        | FSFound row ->
            match findFreeSpaceOnLeftAndRight b (500, row) with
            | FOFound pos -> loop (insertStraightLine b (pos, pos)) (index + 1)
            | FONotFound column ->
                match floorRow with
                | None -> index
                | Some floorRowR -> loop (insertStraightLine b ((column, floorRowR - 1), (column, floorRowR))) index + 1
    loop board 0


// let sw = System.Diagnostics.Stopwatch.StartNew()

let puzzle1 input = dropSand (loadData input) None |> string

let puzzle2 input =
    let board = loadData input
    let maxHight = board |> Map.toSeq |> Seq.map (snd >> List.last >> snd) |> Seq.max
    dropSand board (Some(maxHight + 2)) |> string


// let p1 = puzzle1 input
// let p2 = puzzle2 input
// let TIME = sw.ElapsedMilliseconds

//  ** ** **


mergeLines [] (6, 8) === [ (6, 8) ]
mergeLines [ (10, 12) ] (14, 16) === [ (10, 12); (14, 16) ]
mergeLines [ (10, 12); (18, 20) ] (14, 16) === [ (10, 12); (14, 16); (18, 20) ]


mergeLines [ (10, 12); (16, 18) ] (6, 9) === [ (6, 12); (16, 18) ]
mergeLines [ (10, 12); (16, 18) ] (13, 14) === [ (10, 14); (16, 18) ]

mergeLines [ (10, 12); (16, 18) ] (8, 11) === [ (8, 12); (16, 18) ]
mergeLines [ (10, 12); (16, 18) ] (11, 13) === [ (10, 13); (16, 18) ]

mergeLines [ (10, 12); (16, 18) ] (13, 15) === [ (10, 18) ]
mergeLines [ (10, 12); (16, 18) ] (11, 17) === [ (10, 18) ]

mergeLines [ (10, 12); (16, 18); (25, 27) ] (7, 20) === [ (7, 20); (25, 27) ]
mergeLines [ (10, 12); (16, 18); (25, 27) ] (7, 30) === [ (7, 30) ]

mergeLines [ (10, 12) ] (6, 8) === [ (6, 8); (10, 12) ]


findFreeSpaceInColumn [] 5 === FSNotFound
findFreeSpaceInColumn [ (10, 12); (16, 18) ] 20 === FSNotFound

findFreeSpaceInColumn [ (10, 12); (16, 18) ] 17 === FSOccupied
findFreeSpaceInColumn [ (10, 12); (16, 18) ] 16 === FSOccupied
findFreeSpaceInColumn [ (10, 12); (16, 18) ] 18 === FSOccupied

findFreeSpaceInColumn [ (10, 12); (16, 18) ] 5 === FSFound 9
findFreeSpaceInColumn [ (10, 12); (16, 18) ] 13 === FSFound 15
