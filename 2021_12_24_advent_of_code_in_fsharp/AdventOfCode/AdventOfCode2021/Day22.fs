module AdventOfCode2021.Day22

// ionide v5.7.3
open System

let input =
    System.IO.File.ReadAllText
        "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day22.txt"


type Cube = { IsOn: bool; X: int64 * int64; Y: int64 * int64; Z: int64 * int64 }

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    let onOffPrefixLength = [ (true, "on ".Length); (false, "off ".Length) ] |> Map
    let xyzPrefixLength = "x=".Length
    lines
    |> Array.map (fun line ->
        let isOn = line.StartsWith("on")
        let line' = line.Substring(onOffPrefixLength.[isOn])
        let parts' = line'.Split(",")
        let cubes =
            parts' |> Array.map (fun part -> part.Substring(xyzPrefixLength).Split("..") |> Array.map Int64.Parse)
        match cubes with
        | [| [| x1; x2 |]; [| y1; y2 |]; [| z1; z2 |] |] -> { IsOn = isOn; X = (x1, x2); Y = (y1, y2); Z = (z1, z2) }
        | _ -> failwith "Wrong format")


let data = loadData input

let width (c1, c2) = c2 - c1 + 1L

let xyz = data |> Array.map (fun cube -> width cube.X * width cube.Y * width cube.Z) |> Array.max

let a = 35350561691340L
