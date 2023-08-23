module AdventOfCode2021.Day22

open System
open Common

// let input =
//     System.IO.File.ReadAllText
//         "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day22.txt"

type Range = { Min: int64; Max: int64 }

type Cube = { IsOn: bool; X: Range; Y: Range; Z: Range }

let newCube x1 x2 y1 y2 z1 z2 =
    { IsOn = true; X = { Min = x1; Max = x2 }; Y = { Min = y1; Max = y2 }; Z = { Min = z1; Max = z2 } }


let loadData (input: string) =
    input.Split Environment.NewLine
    |> Array.map (fun line ->
        let isOn = line.StartsWith("on")
        match matchesNumbers line with
        | [| x1; x2; y1; y2; z1; z2 |] -> { newCube x1 x2 y1 y2 z1 z2 with IsOn = isOn }
        | _ -> failwith "Wrong format")

// directions are only for debugging purpose (for unit tests)
type Direction =
    | Top
    | Bottom
    | Right
    | Left
    | Back
    | Front

let splitCube (c: Cube) (ic: Cube) =
    seq {
        let vertical =
            newCube (max ic.X.Min c.X.Min) (min ic.X.Max c.X.Max) 0 0 (max ic.Z.Min c.Z.Min) (min ic.Z.Max c.Z.Max)
        if (ic.Y.Max + 1L) <= c.Y.Max then yield { vertical with Y = { Min = (ic.Y.Max + 1L); Max = c.Y.Max } }, Top
        if c.Y.Min <= (ic.Y.Min - 1L) then yield { vertical with Y = { Min = c.Y.Min; Max = (ic.Y.Min - 1L) } }, Bottom

        let horizontal = newCube 0 0 c.Y.Min c.Y.Max c.Z.Min c.Z.Max
        if (ic.X.Max + 1L) <= c.X.Max then yield { horizontal with X = { Min = (ic.X.Max + 1L); Max = c.X.Max } }, Right
        if c.X.Min <= (ic.X.Min - 1L) then yield { horizontal with X = { Min = c.X.Min; Max = (ic.X.Min - 1L) } }, Left

        let frontal = newCube (max ic.X.Min c.X.Min) (min ic.X.Max c.X.Max) c.Y.Min c.Y.Max 0 0
        if (ic.Z.Max + 1L) <= c.Z.Max then yield { frontal with Z = { Min = (ic.Z.Max + 1L); Max = c.Z.Max } }, Back
        if c.Z.Min <= (ic.Z.Min - 1L) then yield { frontal with Z = { Min = c.Z.Min; Max = (ic.Z.Min - 1L) } }, Front
    }

let countPointsInCube cube =
    abs (cube.X.Max - cube.X.Min + 1L) * abs (cube.Y.Max - cube.Y.Min + 1L) * abs (cube.Z.Max - cube.Z.Min + 1L)

let isRangeInside inner outer = outer.Min <= inner.Min && inner.Max <= outer.Max

let isCubeInside inner outer =
    isRangeInside inner.X outer.X && isRangeInside inner.Y outer.Y && isRangeInside inner.Z outer.Z

let isRangeOverlapping range1 range2 = not (range2.Max < range1.Min || range2.Min > range1.Max)

let isCubeOverlapping cube1 cube2 =
    isRangeOverlapping cube1.X cube2.X && isRangeOverlapping cube1.Y cube2.Y && isRangeOverlapping cube1.Z cube2.Z


let rec processCubes onCubes nextCube =
    match onCubes with
    | [] -> if nextCube.IsOn then [ nextCube ] else []
    | onCube :: restCubes ->
        if isCubeInside onCube nextCube then
            processCubes restCubes nextCube // skip cube
        elif nextCube.IsOn && isCubeInside nextCube onCube then
            onCube :: restCubes // take cube and stop processing
        elif isCubeOverlapping onCube nextCube then
            let splitted = splitCube onCube nextCube |> Seq.map fst |> Seq.toList
            splitted @ processCubes restCubes nextCube // split cube
        else
            onCube :: processCubes restCubes nextCube // take cube


let puzzle cubes = cubes |> Seq.fold processCubes [] |> List.sumBy countPointsInCube |> string

let puzzle1 (input: string) =
    let cubes = loadData input
    let initRange = { Min = -50L; Max = 50L }
    cubes
    |> Seq.filter (fun cube ->
        isRangeInside cube.X initRange && isRangeInside cube.Y initRange && isRangeInside cube.Z initRange)
    |> puzzle

let puzzle2 (input: string) = loadData input |> puzzle




// ***** tests

let cubeToString cube = sprintf "%d-%d %d-%d %d-%d" cube.X.Min cube.X.Max cube.Y.Min cube.Y.Max cube.Z.Min cube.Z.Max

let countPointsInCubeList cubes = Seq.sumBy countPointsInCube cubes

let assertPoints c ic splittedCount =
    let splitted = splitCube c ic |> Seq.toArray
    assert (splitted.Length = splittedCount)
    splitted |> Seq.map fst |> Seq.append [ ic ] |> countPointsInCubeList === countPointsInCube c

let cube = newCube 0 5 10 15 100 105



// - next cube is fully inside
assertPoints cube (newCube 2 3 12 13 102 103) 6 // in the middle
assertPoints cube (newCube 0 1 12 13 102 103) 5 // in the middle on the wall
assertPoints cube (newCube 0 1 10 11 102 103) 4 // bottom on the wall
assertPoints cube (newCube 0 1 10 11 100 101) 3 // in the corner
assertPoints cube cube 0 // exactly the same

// - next cube is partially inside
// -- outside on the left
splitCube cube (newCube (-2) 3 12 13 102 103)
|> (fun r ->
    r |> Seq.map snd |> Seq.toArray === [| Top; Bottom; Right; Back; Front |]
    r |> Seq.map (fst >> cubeToString) |> Seq.toArray
    === [| "0-3 14-15 102-103"; "0-3 10-11 102-103"; "4-5 10-15 100-105"; "0-3 10-15 104-105"; "0-3 10-15 100-101" |])

// -- outside on the right
splitCube cube (newCube 2 (7) 12 13 102 103) |> Seq.map snd |> Seq.toArray === [| Top; Bottom; Left; Back; Front |]

// -- outside on the left and right
splitCube cube (newCube (-2) (7) 12 13 102 103)
|> (fun r ->
    r |> Seq.map snd |> Seq.toArray === [| Top; Bottom; Back; Front |]
    r |> Seq.map (fst >> cubeToString) |> Seq.toArray
    === [| "0-5 14-15 102-103"; "0-5 10-11 102-103"; "0-5 10-15 104-105"; "0-5 10-15 100-101" |])

// -- outside on the left and right and bottom
splitCube cube (newCube (-2) (7) (8) 13 102 103)
|> (fun r ->
    r |> Seq.map snd |> Seq.toArray === [| Top; Back; Front |]
    r |> Seq.map (fst >> cubeToString) |> Seq.toArray
    === [| "0-5 14-15 102-103"; "0-5 10-15 104-105"; "0-5 10-15 100-101" |])



// - next cube is outside sticking to the wall
// -- sticking in the middle
splitCube cube (newCube (-1) (0) 12 13 102 103)
|> (fun r ->
    r |> Seq.map snd |> Seq.toArray === [| Top; Bottom; Right; Back; Front |]
    r |> Seq.map fst |> countPointsInCubeList === (countPointsInCube cube) - 4L)

// -- sticking on the edge
splitCube cube (newCube (-1) (0) (9) (10) 102 103)
|> (fun r ->
    r |> Seq.map snd |> Seq.toArray === [| Top; Right; Back; Front |]
    r |> Seq.map fst |> countPointsInCubeList === (countPointsInCube cube) - 2L)


assert (isRangeInside { Min = 1; Max = 4 } { Min = 0; Max = 5 })
assert (isRangeInside { Min = 0; Max = 5 } { Min = 0; Max = 5 })
assert (isRangeInside { Min = 0; Max = 6 } { Min = 0; Max = 5 } |> not)
assert (isRangeInside { Min = 6; Max = 8 } { Min = 0; Max = 5 } |> not)

assert (isCubeInside (newCube 0 5 0 5 0 5) (newCube 0 5 0 5 0 5))
assert (isCubeInside (newCube 1 4 3 3 0 5) (newCube 0 5 0 5 0 5))
assert (isCubeInside (newCube 0 6 0 5 0 5) (newCube 0 5 0 5 0 5) |> not)

let ranges =
    [ ({ Min = 0; Max = 5 }, { Min = 0; Max = 5 }, true)
      ({ Min = 1; Max = 4 }, { Min = 0; Max = 5 }, true)
      ({ Min = 1; Max = 6 }, { Min = 0; Max = 5 }, true)
      ({ Min = 5; Max = 6 }, { Min = 0; Max = 5 }, true)
      ({ Min = 6; Max = 8 }, { Min = 0; Max = 5 }, false)
      ({ Min = 6; Max = 8 }, { Min = 0; Max = 5 }, false) ]

for range1, range2, overlapping in ranges do
    isRangeOverlapping range1 range2 === overlapping
    isRangeOverlapping range2 range1 === overlapping







// ***** alternative implementation of part 1 (only)

let unzipCubes cubes =
    cubes
    |> Seq.fold
        (fun (ons_offs, isPrevOn) cube ->
            match cube.IsOn, isPrevOn with
            | true, false -> ([ cube ], []) :: ons_offs, cube.IsOn
            | _ ->
                let (ons, offs), tail = List.head ons_offs, List.tail ons_offs
                match cube.IsOn with
                | true -> (cube :: ons, offs) :: tail, cube.IsOn
                | false -> (ons, cube :: offs) :: tail, cube.IsOn)
        ([], false)
    |> fst
    |> List.rev


let getPointsForCube cube =
    seq {
        for x = cube.X.Min to cube.X.Max do
            for y = cube.Y.Min to cube.Y.Max do
                for z = cube.Z.Min to cube.Z.Max do
                    yield x, y, z
    }


let isNumberInsideRange value range = value >= range.Min && value <= range.Max

let isPointInsideCube (x, y, z) cube =
    isNumberInsideRange x cube.X && isNumberInsideRange y cube.Y && isNumberInsideRange z cube.Z


let findOnPoints onCube offCubes =
    let offCubesOverlapping = List.filter (isCubeOverlapping onCube) offCubes
    match offCubesOverlapping with
    | [] -> getPointsForCube onCube
    | _ ->
        getPointsForCube onCube |> Seq.filter (fun p -> offCubesOverlapping |> Seq.exists (isPointInsideCube p) |> not)


let rec listToSeqOfHeadsTails lst =
    seq {
        match lst with
        | [] -> ()
        | head :: tail ->
            yield head, tail
            yield! listToSeqOfHeadsTails tail
    }


let findOnPointsForListOfCubes cubes =
    let splittedCubes = unzipCubes cubes
    let points =
        splittedCubes
        |> listToSeqOfHeadsTails
        |> Seq.collect (fun ((ons, offs), tail) ->
            let allOffs = Seq.append offs (Seq.collect snd tail) |> Seq.toList
            Seq.collect (fun cube -> findOnPoints cube allOffs) ons)
        |> Seq.distinct
    points



let puzzle1' (input: string) =
    let cubes = loadData input
    let initRange = { Min = -50L; Max = 50L }
    cubes
    |> Seq.filter (fun cube ->
        isRangeInside cube.X initRange && isRangeInside cube.Y initRange && isRangeInside cube.Z initRange)
    |> findOnPointsForListOfCubes
    |> Seq.length
    |> string
