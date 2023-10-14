module AdventOfCode2021.Day19

open Common
open System


type Point = int * int * int

let reversedListToArray lst count =
    let array = Array.zeroCreate count
    List.iteri (fun index item -> array.[count - index - 1] <- item) lst
    array

let chunkBySeparator sep items =
    seq {
        let mutable chunk = []
        let mutable chunkCount = 0
        for item in items do
            if sep item then
                if chunkCount > 0 then yield reversedListToArray chunk chunkCount
                chunk <- []
                chunkCount <- 0
            else
                chunk <- item :: chunk
                chunkCount <- chunkCount + 1
        if chunkCount > 0 then yield reversedListToArray chunk chunkCount
    }

let loadData (input: string) : Point [] [] =
    let lines = input.Split Environment.NewLine
    lines
    |> Seq.filter (fun line -> line.Length > 0)
    |> chunkBySeparator (fun line -> line.StartsWith("--- "))
    |> Seq.map (Array.map (fun line -> let parts = Common.parseNumbers ',' line in parts.[0], parts.[1], parts.[2]))
    |> Seq.toArray




let rotate90 (a, b) =
    seq {
        a, b
        b, -a
        -a, -b
        -b, a
    }

let findAllOrientations (x, y, z) =
    seq {
        yield! rotate90 (y, z) |> Seq.map (fun (a, b) -> x, a, b) // x
        yield! rotate90 (y, -z) |> Seq.map (fun (a, b) -> -x, a, b) // -x
        yield! rotate90 (-x, z) |> Seq.map (fun (a, b) -> y, a, b) // y
        yield! rotate90 (x, z) |> Seq.map (fun (a, b) -> -y, a, b) // -y
        yield! rotate90 (y, -x) |> Seq.map (fun (a, b) -> z, a, b) // z
        yield! rotate90 (y, x) |> Seq.map (fun (a, b) -> -z, a, b) // -x
    }

type Scanner = { Points: Point []; AllOrientations: Point [] [] }

let seriesToScanners (series: Point [] []) =
    series
    |> Array.map (fun s ->
        { Points = s
          AllOrientations =
            s
            |> Seq.collect (findAllOrientations >> Seq.indexed)
            |> Seq.groupBy (fun (index, _) -> index)
            |> Seq.sortBy (fun (index, _) -> index)
            |> Seq.map (fun (index, points) -> points |> Seq.map snd |> Seq.toArray)
            |> Seq.toArray })



let existsAtLeastXItems items itemsLength pred x =
    items
    |> Seq.scan (fun left x -> if pred x then left - 1 else left) x
    |> Seq.skip 1 // 'scan' adds first fake item
    |> Seq.indexed
    |> Seq.takeWhile (fun (index, left) -> itemsLength - (index + 1) >= left)
    |> Seq.tryFind (fun (index, left) -> left = 0)
    |> Option.isSome

type SeriesMatchingResult = { Shift: Point; ShiftedSeries: Point [] }

[<Literal>]
let MinMatchingPoints = 12

let matchSeries (serie1: Set<Point>) (serie2: Point []) =
    let len = serie1.Count
    Seq.allPairs serie1 serie2
    |> Seq.tryPick (fun ((x1, y1, z1), (x2, y2, z2)) ->
        let (xx, yy, zz) as shift = x1 - x2, y1 - y2, z1 - z2
        let shifted = serie2 |> Seq.map (fun (x, y, z) -> x + xx, y + yy, z + zz) |> Seq.cache
        if existsAtLeastXItems shifted len (fun p -> Set.contains p serie1) MinMatchingPoints then
            Some { Shift = shift; ShiftedSeries = shifted |> Seq.toArray }
        else
            None)


type ScannersMatchingResult =
    { SerieFrom: Point []
      SerieTo: Point []
      ScannerTo: Scanner
      SeriesMatchingResult: SeriesMatchingResult }


let matchScanners serie1 scanner2 =
    let serie1Map = serie1 |> Set
    scanner2.AllOrientations
    |> Seq.tryPick (fun serie2 ->
        matchSeries serie1Map serie2
        |> Option.map (fun r -> { SerieFrom = serie1; SerieTo = serie2; ScannerTo = scanner2; SeriesMatchingResult = r }))



type ProcessPointsFunc = Point [] -> Scanner list -> ScannersMatchingResult list * Scanner list

let processPointsSequentially serie scannersLeft =
    scannersLeft
    |> List.fold
        (fun (matched, unmatched) s ->
            match matchScanners serie s with
            | None -> (matched, s :: unmatched)
            | Some (r) -> (r :: matched, unmatched))
        ([], [])


let processPointsInParallel serie scannersLeft =
    scannersLeft
    |> List.toArray
    |> Array.Parallel.map (fun s -> s, matchScanners serie s)
    |> Array.toList
    |> List.fold
        (fun (matched, unmatched) (s, result) ->
            match result with
            | None -> (matched, s :: unmatched)
            | Some (r) -> (r :: matched, unmatched))
        ([], [])

let logger (f: ProcessPointsFunc) : ProcessPointsFunc =
    (fun serie scannersLeft ->
        printfn "scannersLeft %d " (scannersLeft |> List.length)
        let sw = Diagnostics.Stopwatch.StartNew()
        let matchedResults, scannersLeft' as result = f serie scannersLeft
        printfn " matched %d scannersLeft %d -  %d ms" matchedResults.Length scannersLeft'.Length sw.ElapsedMilliseconds
        result)


// let processPoints = processPointsInParallel |> logger
let processPoints = processPointsInParallel




type WalkResult<'S> = { State: 'S; ScannersLeft: Scanner list }

let rec walk serie state folder =
    let matchedResults, scannersLeft' = processPoints serie state.ScannersLeft
    let result = folder { state with ScannersLeft = scannersLeft' } matchedResults
    let len = List.length matchedResults

    matchedResults
    |> Seq.scan (fun r serie -> walk serie.SeriesMatchingResult.ShiftedSeries r folder) result
    |> Seq.indexed
    |> Seq.pick (fun (i, r) -> if List.isEmpty r.ScannersLeft || i = len then Some r else None)



let puzzle1 (input: string) =
    let fromCache = true
    if fromCache then
        "392"
    else
        let series = loadData input
        let scanners = seriesToScanners series
        let scanner = scanners |> Array.head
        let scannersLeft = scanners |> Array.toList |> List.tail
        let walkResult =
            walk scanner.Points { State = scanner.Points :> seq<_>; ScannersLeft = scannersLeft } (fun state res ->
                { state with
                    State = Seq.append state.State (res |> Seq.collect (fun r -> r.SeriesMatchingResult.ShiftedSeries)) })
        let result = walkResult.State |> Set |> Set.count
        result |> string



let puzzle2 (input: string) =
    let fromCache = true
    if fromCache then
        "13332"
    else
        let series = loadData input
        let scanners = seriesToScanners series
        let scanner = scanners |> Array.head
        let scannersLeft = scanners |> Array.toList |> List.tail
        let walkResult =
            walk scanner.Points { State = (Seq.singleton (0, 0, 0)); ScannersLeft = scannersLeft } (fun state res ->
                { state with State = Seq.append state.State (res |> Seq.map (fun r -> r.SeriesMatchingResult.Shift)) })
        let result =
            walkResult.State
            |> allUniquePairs
            |> Seq.map (fun ((x1, y1, z1), (x2, y2, z2)) -> abs (x1 - x2) + abs (y1 - y2) + abs (z1 - z2))
            |> Seq.max
        result |> string










// ----

// let allPairs' (items: seq<_>) =
//     items
//     |> Seq.pairwise
//     |> Seq.scan
//         (fun (first, _) (x, y) ->
//             match first with
//             | None -> Some x, (x, y)
//             | Some v -> first, (v, y))
//         (None, (Unchecked.defaultof<_>, Unchecked.defaultof<_>))
//     |> Seq.skip 1
//     |> Seq.map snd
//     |> Seq.scan
//         (fun (lst, _) ((x, y) as pair) ->
//             y :: lst, Seq.append (Seq.singleton pair) (lst |> Seq.map (fun item -> y, item)))
//         ([], Seq.empty)
//     |> Seq.skip 1
//     |> Seq.collect snd

// let c1 = [ 1; 2; 3; 4; 5 ] |> allPairs |> Seq.toArray
// let c2 = [ 1; 2; 3; 4; 5 ] |> allPairs' |> Seq.toArray
