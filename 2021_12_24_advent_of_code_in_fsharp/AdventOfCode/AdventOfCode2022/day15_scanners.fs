module AdventOfCode2022.Day15

open System
open Common

let input =
    System.IO.File.ReadAllText
        "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2022/Day15.txt"

type Entry = { Sensor: int * int; Beacon: int * int; Distance: int }


let manhattanDistance (x1, y1) (x2, y2) = abs (x2 - x1) + abs (y2 - y1)

let loadData (input: string) =
    input.Split Environment.NewLine
    |> Seq.map matchesNumbers4
    |> Seq.map (fun (x1, y1, x2, y2) ->
        { Sensor = x1, y1; Beacon = x2, y2; Distance = manhattanDistance (x1, y1) (x2, y2) })
    |> Seq.toArray


// function copied from day 14 :)
let rec mergeLines lines line =
    match lines, line with
    | [], _ -> [ line ]
    | ((fromH, toH) as head) :: tail, (fromL, toL) ->
        if toH + 1 < fromL then head :: mergeLines tail line
        elif toL + 1 < fromH then line :: lines
        else mergeLines tail (min fromH fromL, max toH toL)


let linesInRow entries row =
    entries
    |> Seq.choose (fun { Sensor = x, y; Distance = distance } ->
        let distanceToRow = abs (row - y)
        let distanceDiff = distance - distanceToRow
        if distanceDiff < 0 then None else Some(x - distanceDiff, x + distanceDiff))



let mergeLinesInRow entries row = Seq.fold mergeLines [] (linesInRow entries row)

let countPoints lines = lines |> List.sumBy (fun (start, end') -> end' - start + 1)

let countBeaconsInRow entries row =
    entries |> Seq.choose (fun { Beacon = x, y } -> if y = row then Some x else None) |> Seq.distinct |> Seq.length

let puzzle1 input =
    let row = 2000000
    let entries = loadData input
    let points = mergeLinesInRow entries row |> countPoints
    let beaconsInRow = countBeaconsInRow entries row
    points - beaconsInRow |> string


let between value minValue maxValue = value >= minValue && value <= maxValue

let intersectionPointsOfTwoLines ((sx1, sy1), (sx2, sy2)) ((bx1, by1), (bx2, by2)) =
    let l = sx1 + sy1
    let yL = l + by1 - bx1
    let xL = l + bx1 - by1
    let y = yL / 2
    let x = xL / 2
    if between x sx1 sx2 && between x bx1 bx2 && between y sy2 sy1 && between y by1 by2 then
        if yL % 2 = 0 then
            [ (x, y) ]
        else
            let x1 = x
            let x2 = x + 1
            let y1 = y
            let y2 = y + 1
            [ (x1, y1); (x2, y1); (x1, y2); (x2, y2) ]
    else
        []


let linesForSidesOfExpandedDiamond { Sensor = (x, y); Distance = distance } =
    let expandedDistance = distance + 1
    let left = x - expandedDistance, y
    let right = x + expandedDistance, y
    let top = x, y - expandedDistance
    let bottom = x, y + expandedDistance
    ((left, top), (bottom, right)), ((top, right), (bottom, bottom))


let intersectionPointsForEntries entries =
    let slashes, backslashes =
        entries
        |> Seq.map linesForSidesOfExpandedDiamond
        |> Seq.fold
            (fun (slashes, backslashes) ((s1, s2), (b1, b2)) -> s1 :: s2 :: slashes, b1 :: b2 :: backslashes)
            ([], [])
    slashes |> Seq.collect (fun slash -> Seq.collect (intersectionPointsOfTwoLines slash) backslashes) |> Seq.distinct


let idDetectedByAnySensor entries point =
    Seq.exists (fun entry -> manhattanDistance point entry.Sensor <= entry.Distance) entries

let puzzle2 input =
    let from, to' = 0, 4000000
    let entries = loadData input
    let column, row =
        intersectionPointsForEntries entries
        |> Seq.filter (fun (x, y) -> between x from to' && between y from to')
        |> Seq.filter (fun p -> not (idDetectedByAnySensor entries p))
        |> Seq.head
    (int64 column) * 4000000L + (int64 row)


//  ** ** **


intersectionPointsOfTwoLines ((3, 7), (5, 4)) ((4, 4), (10, 10)) === [ (5, 5) ]
intersectionPointsOfTwoLines ((3, 7), (5, 4)) ((3, 5), (7, 9)) === [ (4, 6) ]

intersectionPointsOfTwoLines ((3, 7), (5, 4)) ((0, 8), (4, 12)) === []
intersectionPointsOfTwoLines ((0, 10), (5, 4)) ((0, 8), (4, 12)) === [ (1, 9) ]
