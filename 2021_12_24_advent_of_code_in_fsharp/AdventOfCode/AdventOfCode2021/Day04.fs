module AdventOfCode2021.Day4

open Common
open System
open System.Linq

type Cell = { Value: int; mutable IsChecked: bool }

type Board = { Cells: Cell [,]; mutable IsChecked: bool }

type Data = { Numbers: int list; Boards: Board list }

[<Literal>]
let BoardSize = 5

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    let boards =
        lines
        |> Seq.skip 2
        |> Seq.filter (fun line -> line.Length > 0)
        |> Seq.chunkBySize BoardSize
        |> Seq.map (
            Seq.map (parseNumbers ' ')
            >> Seq.map (Array.map (fun n -> { Value = n; IsChecked = false }))
            >> array2D
            >> (fun cells -> { Cells = cells; IsChecked = false })
        )
        |> Seq.toList
    { Numbers = (lines.[0] |> parseNumbers ',' |> Array.toList); Boards = boards }


let getRowsAndColumns (b: Board) =
    [ 0 .. BoardSize - 1 ] |> Seq.collect (fun index -> [ b.Cells.[index, *]; b.Cells.[*, index] ])

let checkCell n (cell: Cell) =
    cell.IsChecked <- cell.Value = n
    cell.IsChecked

let checkBoard n (board: Board) =
    board.IsChecked <-
        board
        |> getRowsAndColumns
        |> Seq.map (fun line -> line |> Seq.filter (fun c -> c.IsChecked || checkCell n c) |> Seq.length)
        |> Seq.exists ((=) BoardSize)
    board.IsChecked

let findWinningBoards (data: Data) =
    data.Numbers
    |> Seq.collect (fun n ->
        data.Boards |> Seq.filter (fun b -> not b.IsChecked) |> Seq.filter (checkBoard n) |> Seq.map (fun b -> n, b))

let countScoreForBoard (board: Board) =
    board.Cells
    |> Enumerable.OfType<Cell>
    |> Seq.choose (fun cell ->
        match cell with
        | { IsChecked = true } -> None
        | { Value = value } -> Some value)
    |> Seq.sum

let puzzle1 (input: string) =
    let data = loadData input
    let n, b = data |> findWinningBoards |> Seq.head
    let result = n * countScoreForBoard b
    result |> string


let puzzle2 (input: string) =
    let data = loadData input
    let n, b = findWinningBoards data |> Seq.last
    let result = n * countScoreForBoard b
    result |> string
