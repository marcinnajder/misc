module AdventOfCode2022.Day12

open System
open FSharpx.Collections
// #r "nuget: FSharpx.Collections"

// let input =
//     System.IO.File.ReadAllText
//         "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2022/Day12.txt"

type Data = { Heightmap: int [,]; Start: int * int; Finish: int * int; Height: int; Width: int }


let getHeight (data: int [,]) = data.GetLength(0)
let getWidth (data: int [,]) = data.GetLength(1)

let seqOfAllPositions (heightmap: int [,]) =
    seq {
        for y = 0 to getHeight heightmap - 1 do
            for x = 0 to getWidth heightmap - 1 do
                yield y, x
    }


let findHeights (heightmap: int [,]) height =
    seqOfAllPositions heightmap |> Seq.filter (fun (y, x) -> heightmap.[y, x] = height)

let loadData (input: string) =
    let heightmap = input.Split Environment.NewLine |> Seq.map (Seq.map int) |> array2D
    let start = findHeights heightmap (int 'S') |> Seq.head
    let finish = findHeights heightmap (int 'E') |> Seq.head
    heightmap.[fst start, snd start] <- int 'a'
    heightmap.[fst finish, snd finish] <- int 'z'
    { Heightmap = heightmap
      Start = start
      Finish = finish
      Height = getHeight heightmap
      Width = getWidth heightmap }


let neighbours (y, x) height width =
    seq {
        if x > 0 then y, x - 1
        if x < width - 1 then y, x + 1
        if y > 0 then y - 1, x
        if y < height - 1 then y + 1, x
    }

let calculateMinimalCosts (data: Data) start =
    let rec loop (todoQueue: Queue<int * int>) costs =
        match Queue.tryHead todoQueue with
        | None -> costs
        | Some pos ->
            let posHeight = data.Heightmap.[fst pos, snd pos]
            let posCost = Map.find pos costs
            let posCostNext = posCost + 1
            let posNeighbours =
                neighbours pos data.Height data.Width
                |> Seq.filter (fun (y, x) -> data.Heightmap.[y, x] - posHeight <= 1)
                |> Seq.filter (fun n ->
                    match Map.tryFind n costs with
                    | None -> true
                    | Some nConst -> posCostNext < nConst)
                |> Seq.toArray

            let todoQueue' = posNeighbours |> Seq.fold (fun q p -> Queue.conj p q) (Queue.tail todoQueue)
            let costs' = posNeighbours |> Seq.fold (fun c p -> c |> Map.change p (fun _ -> Some posCostNext)) costs
            loop todoQueue' costs'
    loop (Queue.conj start Queue.empty) (Map [ (start, 0) ])



let puzzle1 input =
    let data = loadData input
    let costs = calculateMinimalCosts data data.Start
    costs |> Map.find data.Finish |> string

let heightOfA = int 'a'

let isInteriorA (data: Data) pos =
    Seq.append [ pos ] (neighbours pos data.Height data.Width)
    |> Seq.forall (fun (y, x) -> data.Heightmap.[y, x] = heightOfA)

let puzzle2 input =
    let data = loadData input
    findHeights data.Heightmap heightOfA
    |> Seq.filter (fun p -> not (isInteriorA data p))
    |> Seq.choose (fun p -> calculateMinimalCosts data p |> Map.tryFind data.Finish)
    |> Seq.min
    |> string
