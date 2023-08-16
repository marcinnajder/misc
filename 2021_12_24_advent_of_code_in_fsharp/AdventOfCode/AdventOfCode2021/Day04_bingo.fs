module AdventOfCode2021.Day04

open Common
open System

type Cell = { Value: int; IsChecked: bool }
type Board = Cell list list
type Data' = { Numbers: int list; Boards: Board list }

[<Literal>]
let BoardSize = 5

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    let boards =
        lines
        |> Seq.skip 2
        |> Seq.filter (fun line -> line.Length > 0)
        |> Seq.chunkBySize BoardSize
        |> Seq.map (fun lines ->
            lines
            |> Seq.map (fun line ->
                parseNumbers ' ' line |> Seq.map (fun n -> { Value = n; IsChecked = false }) |> Seq.toList)
            |> Seq.toList)
        |> Seq.toList
    { Numbers = (lines.[0] |> parseNumbers ',' |> Array.toList); Boards = boards }


let checkRow row n =
    row
    |> List.mapFold
        (fun allCellsChecked cell ->
            match cell with
            | { IsChecked = true } -> cell, allCellsChecked
            | { Value = value } when value = n -> { cell with IsChecked = true }, allCellsChecked
            | _ -> cell, allCellsChecked && false)
        true

let checkRowList (board: Board) n =
    board |> List.mapFold (fun anyRowChecked row -> if anyRowChecked then row, anyRowChecked else checkRow row n) false


let rec heads lol =
    match lol with
    | lst :: t ->
        match lst with
        | head :: tail ->
            let head', tail' = heads t
            head :: head', tail :: tail'
        | [] -> [], []
    | [] -> [], []

// returns the same result but creates unnecessary intermediate list after calling List.choose
// let rec heads' lol =
//     lol
//     |> List.choose (function
//         | head :: tail -> Some(head, tail)
//         | _ -> None)
//     |> List.unzip

let rec zipN (listOfLists: _ list list) =
    seq {
        let heads, tails = heads listOfLists
        if not (List.isEmpty heads) then
            yield heads
            yield! zipN tails
    }

let checkColumnList (b: Board) = zipN b |> Seq.exists (fun cells -> cells |> Seq.forall (fun c -> c.IsChecked))


let checkBoard (board: Board) n =
    let (newBoard, anyRowChecked) as result = checkRowList board n
    if not anyRowChecked && (checkColumnList newBoard) then newBoard, true else result

let rec checkBoardList (boards: Board list) n =
    match boards with
    | board :: rest ->
        let unchecked, checked' = checkBoardList rest n
        match checkBoard board n with
        | board', true -> unchecked, board' :: checked'
        | board', false -> board' :: unchecked, checked'
    | [] -> [], []


// works similar to build-in takeWhile but includes also first falsy item
let takeWhileIncluded f (items: seq<_>) =
    seq {
        use iterator = items.GetEnumerator()
        let mutable completed = false
        while not completed && iterator.MoveNext() do
            let item = iterator.Current
            yield item
            completed <- not (f item)
    }

let findWinningBoards (data: Data') =
    data.Numbers
    |> Seq.scan (fun ((unchecked, checked'), n') n -> (checkBoardList unchecked n), n) ((data.Boards, []), 0)
    |> Seq.skip 1
    |> Seq.filter (fun ((unchecked, checked'), n') -> not (List.isEmpty checked'))
    |> takeWhileIncluded (fun ((unchecked, checked'), n') -> not (List.isEmpty unchecked))
    |> Seq.collect (fun ((unchecked, checked'), n') -> checked' |> Seq.map (fun b -> (n', b)))


let countScoreForBoard (board: Board) =
    board
    |> Seq.collect id
    |> Seq.choose (fun cell ->
        match cell with
        | { IsChecked = true } -> None
        | { Value = value } -> Some value)
    |> Seq.sum

let puzzle (input: string) chooseBoard =
    let data = loadData input
    let n, b = data |> findWinningBoards |> chooseBoard
    let result = n * countScoreForBoard b
    result |> string

let puzzle1 (input: string) = puzzle input Seq.head

let puzzle2 (input: string) = puzzle input Seq.last


// *********************************************************************************************************
// mutable implementation


// open Common
// open System
// open System.Linq

// type Cell = { Value: int; mutable IsChecked: bool }
// type Board = { Cells: Cell [,]; mutable IsChecked: bool }
// type Data = { Numbers: int list; Boards: Board list }

// [<Literal>]
// let BoardSize = 5

// let loadData (input: string) =
//     let lines = input.Split Environment.NewLine
//     let boards =
//         lines
//         |> Seq.skip 2
//         |> Seq.filter (fun line -> line.Length > 0)
//         |> Seq.chunkBySize BoardSize
//         |> Seq.map (
//             Seq.map (parseNumbers ' ')
//             >> Seq.map (Array.map (fun n -> { Cell.Value = n; IsChecked = false }))
//             >> array2D
//             >> (fun cells -> { Board.Cells = cells; IsChecked = false })
//         )
//         |> Seq.toList
//     { Data.Numbers = (lines.[0] |> parseNumbers ',' |> Array.toList); Boards = boards }

// let getRowsAndColumns (b: Board) =
//     [ 0 .. BoardSize - 1 ] |> Seq.collect (fun index -> [ b.Cells.[index, *]; b.Cells.[*, index] ])

// let checkCell n (cell: Cell) =
//     cell.IsChecked <- cell.Value = n
//     cell.IsChecked

// let checkBoard n (board: Board) =
//     board.IsChecked <-
//         board
//         |> getRowsAndColumns
//         |> Seq.map (fun line -> line |> Seq.filter (fun c -> c.IsChecked || checkCell n c) |> Seq.length)
//         |> Seq.exists ((=) BoardSize)
//     board.IsChecked

// let findWinningBoards (data: Data) =
//     data.Numbers
//     |> Seq.collect (fun n ->
//         data.Boards |> Seq.filter (fun b -> not b.IsChecked) |> Seq.filter (checkBoard n) |> Seq.map (fun b -> n, b))

// let countScoreForBoard (board: Board) =
//     board.Cells
//     |> Enumerable.OfType<Cell>
//     |> Seq.choose (fun cell ->
//         match cell with
//         | { IsChecked = true } -> None
//         | { Value = value } -> Some value)
//     |> Seq.sum

// let puzzle1 (input: string) =
//     let data = loadData input
//     let n, b = data |> findWinningBoards |> Seq.head
//     let result = n * countScoreForBoard b
//     result |> string


// let puzzle2 (input: string) =
//     let data = loadData input
//     let n, b = findWinningBoards data |> Seq.last
//     let result = n * countScoreForBoard b
//     result |> string



// let printBoard (board: Board) =
//     let strings = board |> Seq.collect id |> Seq.map (fun c -> $"""{(c.Value)}{(if c.IsChecked then "C" else "")}""")
//     printfn "%s" (String.Join(',', strings))
//     strings

// let printBoards (boards: Board seq) =
//     for board in boards do
//         printBoard board |> ignore
