module AdventOfCode2021.Day22

// ionide v5.7.3
open System

let input =
    System.IO.File.ReadAllText
        "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day22.txt"

type Range = { Min: int; Max: int }
type Cube = { IsOn: bool; X: Range; Y: Range; Z: Range }

let newCube a b c d e f = { IsOn = true; X = { Min = a; Max = b }; Y = { Min = c; Max = d }; Z = { Min = e; Max = f } }

let loadData (input: string) =
    let onOffPrefixLength = [ (true, "on ".Length); (false, "off ".Length) ] |> Map
    let xyzPrefixLength = "x=".Length
    let lines = input.Split Environment.NewLine
    lines
    |> Array.map (fun line ->
        let isOn = line.StartsWith("on")
        let parts = line.Substring(onOffPrefixLength.[isOn]).Split(",")
        let cubes =
            parts |> Array.map (fun part -> part.Substring(xyzPrefixLength).Split("..") |> Array.map Int32.Parse)
        match cubes with
        | [| [| x1; x2 |]; [| y1; y2 |]; [| z1; z2 |] |] -> { newCube x1 x2 y1 y2 z1 z2 with IsOn = isOn }
        | _ -> failwith "Wrong format")


let areRangesOverlapping range1 range2 = not (range2.Max < range1.Min || range2.Min > range1.Max)

let areCubesOverlapping cube1 cube2 =
    areRangesOverlapping cube1.X cube2.X && areRangesOverlapping cube1.Y cube2.Y && areRangesOverlapping cube1.Z cube2.Z

let isInsideRange value range = value >= range.Min && value <= range.Max

let isInsideCube (x, y, z) cube = isInsideRange x cube.X && isInsideRange y cube.Y && isInsideRange z cube.Z


let getPointsForCube cube =
    seq {
        for x = cube.X.Min to cube.X.Max do
            for y = cube.Y.Min to cube.Y.Max do
                for z = cube.Z.Min to cube.Z.Max do
                    yield x, y, z
    }

// getPointsForCube (newCube 10 12 10 12 10 12)


let splitCubes cubes =
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


let findOnPoints onCube offCubes =
    let offCubesOverlapping = offCubes |> List.filter (areCubesOverlapping onCube)
    match offCubesOverlapping with
    | [] -> getPointsForCube onCube
    | _ -> getPointsForCube onCube |> Seq.filter (fun p -> offCubesOverlapping |> Seq.exists (isInsideCube p) |> not)


let rec listToSeqOfHeadsTails lst =
    seq {
        match lst with
        | [] -> ()
        | head :: tail ->
            yield head, tail
            yield! listToSeqOfHeadsTails tail
    }

let findOnPointsForListOfCubes cubes =
    let splittedCubes = splitCubes cubes
    let points =
        splittedCubes
        |> listToSeqOfHeadsTails
        |> Seq.collect (fun ((ons, offs), tail) ->
            let allOffs = Seq.append offs (tail |> Seq.collect snd) |> Seq.toList
            ons |> Seq.collect (fun cube -> findOnPoints cube allOffs))
        |> Seq.distinct
    points


let data = loadData input


let bla =
    data
    // |> Seq.filter (fun cube ->
    //     (cube.X.Min >= -50 && cube.X.Max <= 50)
    //     && (cube.Y.Min >= -50 && cube.Y.Max <= 50)
    //     && (cube.Z.Min >= -50 && cube.Z.Max <= 50))
    |> findOnPointsForListOfCubes
    |> Seq.fold (fun a _ -> a + 1L) 0L

// let (a:int) = 2758514936282235

let (b: int64) = 2758514936282235L

// let bla = findOnPointsForListOfCubes data |> Seq.toArray


// let aa = splitCubes data |> Seq.map (fun (ons, offs) -> List.length ons, List.length offs) |> Seq.toArray

// let width (c1, c2) = c2 - c1 + 1L
// let xyz = data |> Array.map (fun cube -> width cube.X * width cube.Y * width cube.Z) |> Array.max
// let a = 35350561691340L



// let c = newCube 1 2 5 7 10 10
// if List.isEmpty offCubesOverlapping then
//     Set.

// let getPoints onCube offCubes =




let puzzle1 (input: string) = input
let puzzle2 (input: string) = input
