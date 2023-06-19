module AdventOfCode2021.Day22

// ionide v5.7.3
open System
//open Common
open System.Collections.Generic

let input =
    System.IO.File.ReadAllText
        "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day22.txt"

type Range = { Min: int; Max: int }
type Cube = { IsOn: bool; X: Range; Y: Range; Z: Range }

let newCube a b c d e f = { IsOn = true; X = { Min = a; Max = b }; Y = { Min = c; Max = d }; Z = { Min = e; Max = f } }


// matchesNumbers "on x=57743..70226,y=-61040..-49730,z=-9481..1779"



let loadData2 (input: string) =
    input.Split Environment.NewLine
    |> Array.map (fun line ->
        let isOn = line.StartsWith("on")
        match matchesNumbers line with
        | [| x1; x2; y1; y2; z1; z2 |] -> { newCube x1 x2 y1 y2 z1 z2 with IsOn = isOn }
        | _ -> failwith "Wrong format")

// loadData input = loadData2 input


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
            let allOffs = Seq.append offs (Seq.collect snd tail) |> Seq.toList
            Seq.collect (fun cube -> findOnPoints cube allOffs) ons)
        |> Seq.distinct
    points


let data = loadData2 input


let bla =
    data
    |> Seq.filter (fun cube ->
        (cube.X.Min >= -50 && cube.X.Max <= 50)
        && (cube.Y.Min >= -50 && cube.Y.Max <= 50)
        && (cube.Z.Min >= -50 && cube.Z.Max <= 50))
    |> findOnPointsForListOfCubes
    |> Seq.fold (fun a _ -> a + 1L) 0L


// wniosek z testow nizej: punktow nawet w jednym wymiarze jest zbyr duzo aby zapisywac jakies przedzialy Map<int*int, (int*int) list>



// Seq.allPairs [ Seq.min; Seq.max ] [
//     (fun (c: Cube) -> c.X)
//     (fun (c: Cube) -> c.Y)
//     (fun (c: Cube) -> c.X)
// ]
// |> Seq.map ( fun (minOrMax, selector) -> data |> Seq.map (fun c -> (selector c).Min )|> minOrMax, data |> Seq.map (fun c -> (selector c).Max )|> minOrMax)
// |> Seq.toArray

[ (fun (c: Cube) -> c.X); (fun (c: Cube) -> c.Y); (fun (c: Cube) -> c.Z) ]
|> Seq.map (fun selector ->
    data |> Seq.map (fun c -> (selector c).Min) |> Seq.min, data |> Seq.map (fun c -> (selector c).Max) |> Seq.max)
|> Seq.map (fun (a, b) -> a, b, b - a)
|> Seq.toArray
|> ignore

let i: int64 = 200000L * 200000L

// seq {
//     for x in 0..2000000 do
//         for y in 0..2000000 do
//             yield (x, y), 0
// }
// |> dict
// |> ignore



// let insertPoints (points: Map<int * int, int>) cube =
//     let innerPoints =
//         seq {
//             for x in cube.X.Min .. cube.X.Max do
//                 for y in cube.Y.Min .. cube.Y.Max do
//                     yield x, y
//         }
//     if cube.IsOn then
//         innerPoints |> Seq.fold (fun s p -> Map.change p (fun _ -> Some 0) s) points
//     else
//         innerPoints |> Seq.fold (fun s p -> Map.remove p s) points

// data |> Seq.take 30 |> Seq.fold insertPoints (Map [])

//data |> Seq.filter (fun c -> not c.IsOn ) |> Seq.map (fun c -> int64 ((c.X.Max - c.X.Min) * (c.Z.Max - c.Z.Min))) |> Seq.sum

// [ data |> Seq.map (fun c -> int64 ((c.X.Max - c.X.Min) * (c.Z.Max - c.Z.Min))) |> Seq.sum
//   data |> Seq.map (fun c -> int64 ((c.X.Max - c.X.Min) * (c.Y.Max - c.Y.Min))) |> Seq.sum
//   data |> Seq.map (fun c -> int64 ((c.Z.Max - c.Z.Min) * (c.Y.Max - c.Y.Min))) |> Seq.sum ]

// let a = 161638442878

// 161638442878L - 40000000000L


// data |> Seq.map (fun c -> c.X.Min) |> Seq.min
// data |> Seq.map (fun c -> c.X.Max) |> Seq.max

// let (a:int) = 2758514936282235

//let (b: int64) = 2758514936282235L

// let bla = findOnPointsForListOfCubes data |> Seq.toArray


// let aa = splitCubes data |> Seq.map (fun (ons, offs) -> List.length ons, List.length offs) |> Seq.toArray

// let width (c1, c2) = c2 - c1 + 1L
// let xyz = data |> Array.map (fun cube -> width cube.X * width cube.Y * width cube.Z) |> Array.max
// let a = 35350561691340L







// let puzzle1 (input: string) = input
// let puzzle2 (input: string) = input
