module AdventOfCode2021.Day08

open System
open System.Linq

type Entry = { Left: string []; Right: string [] }

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines
    |> Seq.map (fun line ->
        let parts = line.Split([| "|" |], StringSplitOptions.RemoveEmptyEntries)
        let p1 = parts.[0].Split([| " " |], StringSplitOptions.RemoveEmptyEntries)
        let p2 = parts.[1].Split([| " " |], StringSplitOptions.RemoveEmptyEntries)
        { Left = p1; Right = p2 })
    |> Seq.toArray


let puzzle1 (input: string) =
    let data = loadData input
    let result =
        data
        |> Seq.collect (fun entry -> entry.Right)
        |> Seq.filter (fun text ->
            match text.Length with
            | 2
            | 3
            | 4
            | 7 -> true
            | _ -> false)
        |> Seq.length
    result |> string


type Segment =
    | None = 0
    | Top = 1
    | Middle = 2
    | Bottom = 4
    | TopLeft = 8
    | TopRight = 16
    | BottomLeft = 32
    | BottomRight = 64

let removeSegments segments =
    Enum.GetValues<Segment>() |> Seq.filter (fun s -> (s &&& segments) <> s) |> Seq.reduce (|||)

let SegmentsToDigits =
    Map [ Segment.Middle |> removeSegments, 0
          Segment.TopRight ||| Segment.BottomRight, 1
          Segment.TopLeft ||| Segment.BottomRight |> removeSegments, 2
          Segment.TopLeft ||| Segment.BottomLeft |> removeSegments, 3
          Segment.TopLeft ||| Segment.TopRight ||| Segment.Middle ||| Segment.BottomRight, 4
          Segment.TopRight ||| Segment.BottomLeft |> removeSegments, 5
          Segment.TopRight |> removeSegments, 6
          Segment.Top ||| Segment.TopRight ||| Segment.BottomRight, 7
          Segment.None |> removeSegments, 8
          Segment.BottomLeft |> removeSegments, 9 ]

let findSegmentsMapping (digits: string []) =
    let digitsByLength = digits |> Seq.groupBy (fun d -> d.Length) |> Map
    let d1 = digitsByLength.[2] |> Seq.exactlyOne
    let d7 = digitsByLength.[3] |> Seq.exactlyOne
    let d8 = digitsByLength.[7] |> Seq.exactlyOne
    let d4 = digitsByLength.[4] |> Seq.exactlyOne
    let d3 = digitsByLength.[5] |> Seq.filter (fun d -> d.Intersect(d1).Count() = 2) |> Seq.exactlyOne
    let d5 = digitsByLength.[5] |> Seq.filter (fun d -> d <> d3 && d.Intersect(d4).Count() = 3) |> Seq.exactlyOne
    let d2 = digitsByLength.[5] |> Seq.filter (fun d -> d <> d3 && d <> d5) |> Seq.exactlyOne
    let d6 = digitsByLength.[6] |> Seq.filter (fun d -> d.Intersect(d1).Count() = 1) |> Seq.exactlyOne
    let d9 = digitsByLength.[6] |> Seq.filter (fun d -> d.Intersect(d4).Count() = 4) |> Seq.exactlyOne
    let d0 = digitsByLength.[6] |> Seq.filter (fun d -> d <> d6 && d <> d9) |> Seq.exactlyOne
    let top = d7.Except(d1).Single()
    Map [ top, Segment.Top
          d8.Except(d0).Single(), Segment.Middle
          d8.Except(d9).Single(), Segment.BottomLeft
          d4.Except(d3).Single(), Segment.TopLeft
          d2.Intersect(d1).Single(), Segment.TopRight
          d5.Intersect(d1).Single(), Segment.BottomRight
          d9.Except(d4).Except(top.ToString()).Single(), Segment.Bottom ]

let puzzle2 (input: string) =
    let data = loadData input
    data
    |> Seq.sumBy (fun e ->
        let mapping = findSegmentsMapping e.Left
        let digits =
            e.Right
            |> Seq.map (fun n ->
                let segments = n |> Seq.map (fun c -> Map.find c mapping) |> Seq.reduce (|||)
                Map.find segments SegmentsToDigits)
        String.Join("", digits) |> Int32.Parse)
    |> string
