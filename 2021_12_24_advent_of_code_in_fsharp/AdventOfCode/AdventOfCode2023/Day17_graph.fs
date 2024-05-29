module AdventOfCode2023.Day17

open Common
open GraphM

// let input =
//     System.IO.File.ReadAllText
//         "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2023/Day17_big.txt"

type Board = { Lines: int array array; Width: int; Height: int }

let loadData (input: string) =
    let lines = loadGridFromData input
    { Lines = lines; Width = lines.Length; Height = lines[0].Length }

type Direction =
    | Right
    | Left
    | Up
    | Down

type Position = { Row: int; Column: int }

type Move = { Prev: Direction; PrevCount: int; Position: Position }

let toOpposite direction =
    match direction with
    | Left -> Right
    | Right -> Left
    | Up -> Down
    | Down -> Up

let allDirections = [ Left; Right; Up; Down ]

// map [(Right, [Up; Down]); ...
let onlyTurnsMap =
    allDirections
    |> Seq.map (fun d ->
        let opp = toOpposite d
        d, allDirections |> List.filter (fun dd -> dd <> opp && dd <> d))
    |> Map

// map [(Right, [Right]); ... ]
let onlyStraightsOnMap = allDirections |> Seq.map (fun d -> d, [ d ]) |> Map

let getNextPositions position directionsWithOffsets board =
    directionsWithOffsets
    |> Seq.choose (fun (d, offset) ->
        match d with
        | Left ->
            let newColumn = position.Column - offset
            if newColumn < 0 then None else Some(d, offset, { position with Column = newColumn })
        | Right ->
            let newColumn = position.Column + offset
            if newColumn >= board.Width then None else Some(d, offset, { position with Column = newColumn })
        | Up ->
            let newRow = position.Row - offset
            if newRow < 0 then None else Some(d, offset, { position with Row = newRow })
        | Down ->
            let newRow = position.Row + offset
            if newRow >= board.Height then None else Some(d, offset, { position with Row = newRow }))

let getNextMoves move board minStraightOn maxStraightOn =
    let directionsMapsWithOffsets =
        if move.PrevCount = maxStraightOn then
            [ (onlyTurnsMap, minStraightOn) ]
        else
            [ (onlyTurnsMap, minStraightOn); (onlyStraightsOnMap, 1) ]

    let directionsWithOffsets =
        directionsMapsWithOffsets
        |> Seq.collect (fun (directionsMap, offset) -> directionsMap[move.Prev] |> Seq.map (fun d -> d, offset))

    getNextPositions move.Position directionsWithOffsets board
    |> Seq.map (fun (d, offset, p) ->
        { Prev = d; PrevCount = (if d = move.Prev then move.PrevCount + 1 else offset); Position = p })


let sumUpWeights fromPosition toPosition board =
    seq {
        if fromPosition.Row = toPosition.Row then // the same row
            let sign = if fromPosition.Column < toPosition.Column then 1 else -1
            for column in { fromPosition.Column + sign .. sign .. toPosition.Column } do
                board.Lines[fromPosition.Row][column]
        else // the same column
            let sign = if fromPosition.Row < toPosition.Row then 1 else -1
            for row in { fromPosition.Row + sign .. sign .. toPosition.Row } do
                board.Lines[row][fromPosition.Column]
    }
    |> Seq.sum


let buildGraph board minStraightOn maxStraightOn =
    seq {
        let mutable todoMoves =
            [ { Prev = Right; PrevCount = 0; Position = { Row = 0; Column = 0 } }
              { Prev = Down; PrevCount = 0; Position = { Row = 0; Column = 0 } } ]
        let mutable visited = Set todoMoves
        while not (List.isEmpty todoMoves) do
            let move = List.head todoMoves
            todoMoves <- List.tail todoMoves
            let nextMoves = getNextMoves move board minStraightOn maxStraightOn
            for nextMove in nextMoves do
                yield { From = move; To = nextMove; Weight = sumUpWeights move.Position nextMove.Position board }
                if not (Set.contains nextMove visited) then
                    visited <- Set.add nextMove visited
                    todoMoves <- nextMove :: todoMoves
    }

let puzzle (input: string) minStraightOn maxStraightOn =
    let board = loadData input
    let graph = buildGraph board minStraightOn maxStraightOn |> Seq.toArray
    let startNode = graph[0].From
    let finishPosition = { Row = board.Height - 1; Column = board.Width - 1 }
    dijkstraTraverse graph startNode
    |> Seq.pick (fun (cost, node) -> if node.Position = finishPosition then Some cost else None)
    |> string

let puzzle1 input = puzzle input 1 3
let puzzle2 input = puzzle input 4 10







// ** tests

// **** getNextPositions

let testBoard = { Lines = Array.empty; Width = 10; Height = 10 }
let allDirectionsMovedByOne = allDirections |> Seq.map (fun d -> d, 1)

getNextPositions { Row = 0; Column = 0 } allDirectionsMovedByOne testBoard |> Seq.toList
=== [ (Right, 1, { Row = 0; Column = 1 }); (Down, 1, { Row = 1; Column = 0 }) ]

getNextPositions { Row = 0; Column = 1 } allDirectionsMovedByOne testBoard |> Seq.toList
=== [ (Left, 1, { Row = 0; Column = 0 }); (Right, 1, { Row = 0; Column = 2 }); (Down, 1, { Row = 1; Column = 1 }) ]

getNextPositions { Row = 1; Column = 1 } allDirectionsMovedByOne testBoard |> Seq.toList
=== [ (Left, 1, { Row = 1; Column = 0 })
      (Right, 1, { Row = 1; Column = 2 })
      (Up, 1, { Row = 0; Column = 1 })
      (Down, 1, { Row = 2; Column = 1 }) ]

getNextPositions { Row = 9; Column = 9 } allDirectionsMovedByOne testBoard |> Seq.toList
=== [ (Left, 1, { Row = 9; Column = 8 }); (Up, 1, { Row = 8; Column = 9 }) ]

getNextPositions { Row = 8; Column = 9 } allDirectionsMovedByOne testBoard |> Seq.toList
=== [ (Left, 1, { Row = 8; Column = 8 }); (Up, 1, { Row = 7; Column = 9 }); (Down, 1, { Row = 9; Column = 9 }) ]



// **** getNextMoves

let testMin = 2
let testMax = 3

let moves =
    getNextMoves
        { Prev = Right; PrevCount = testMin; Position = { Row = 0; Column = testMin } }
        testBoard
        testMin
        testMax
    |> Seq.toList

moves
=== [ { Prev = Down; PrevCount = 2; Position = { Row = 2; Column = 2 } }
      { Prev = Right; PrevCount = 3; Position = { Row = 0; Column = 3 } } ]

let afterR = moves |> List.find (fun m -> m.Prev = Right)
let nextAfterR = getNextMoves afterR testBoard testMin testMax |> Seq.toList
nextAfterR === [ { Prev = Down; PrevCount = 2; Position = { Row = 2; Column = 3 } } ]

let afterRD = nextAfterR |> List.find (fun m -> m.Prev = Down)
let nextAfterRD = getNextMoves afterRD testBoard testMin testMax |> Seq.toList

nextAfterRD
=== [ { Prev = Left; PrevCount = 2; Position = { Row = 2; Column = 1 } }
      { Prev = Right; PrevCount = 2; Position = { Row = 2; Column = 5 } }
      { Prev = Down; PrevCount = 3; Position = { Row = 3; Column = 3 } } ]

let afterRDD = nextAfterRD |> List.find (fun m -> m.Prev = Down)
let nextAfterRDD = getNextMoves afterRDD testBoard testMin testMax |> Seq.toList

nextAfterRDD
=== [ { Prev = Left; PrevCount = 2; Position = { Row = 3; Column = 1 } }
      { Prev = Right; PrevCount = 2; Position = { Row = 3; Column = 5 } } ]


// **** sumUpWeights

let testSmallBoard = { Lines = [| [| 1; 2; 3 |]; [| 4; 5; 6 |]; [| 7; 8; 9 |] |]; Width = 3; Height = 3 }
sumUpWeights { Row = 0; Column = 0 } { Row = 0; Column = 2 } testSmallBoard === 2 + 3
sumUpWeights { Row = 0; Column = 2 } { Row = 0; Column = 0 } testSmallBoard === 2 + 1
sumUpWeights { Row = 0; Column = 1 } { Row = 0; Column = 2 } testSmallBoard === 3
sumUpWeights { Row = 0; Column = 0 } { Row = 2; Column = 0 } testSmallBoard === 4 + 7
sumUpWeights { Row = 2; Column = 0 } { Row = 0; Column = 0 } testSmallBoard === 1 + 4
sumUpWeights { Row = 1; Column = 0 } { Row = 2; Column = 0 } testSmallBoard === 7
