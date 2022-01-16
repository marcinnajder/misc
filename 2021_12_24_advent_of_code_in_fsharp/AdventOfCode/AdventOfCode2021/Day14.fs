module AdventOfCode2021.Day14

open System
open System.Collections.Generic

type Input =
    { Template: string
      Mapping: (string * string) [] }

type MappingMap = Map<char, Map<char, char>>

type Result = Map<char, int64>

type Cache = Dictionary<int * char * char, Result>


let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    let template = lines.[0]
    let mapping =
        lines
        |> Seq.skip 2
        |> Seq.map
            (fun line ->
                let parts = line.Split " -> "
                parts.[0], parts.[1])
        |> Seq.toArray
    { Template = template
      Mapping = mapping }


let toMappingMap (mapping: (string * string) []) : MappingMap =
    mapping
    |> Seq.fold
        (fun a (from, to') ->
            a
            |> Map.change
                (from.[0])
                (function
                | None -> [ from.[1], to'.[0] ] |> Map |> Some
                | Some map -> map |> Map.add from.[1] to'.[0] |> Some))
        Map.empty

let mergeMaps (maps: seq<Result>) =
    maps
    |> Seq.reduce
        (fun map1 map2 ->
            map2
            |> Map.toSeq
            |> Seq.fold
                (fun map (key, value) ->
                    map
                    |> Map.change
                        key
                        (function
                        | None -> Some value
                        | Some v -> Some(v + value)))
                map1)


let rec go args (mappingMap: MappingMap) (cache: Cache) =
    match cache.TryGetValue args with
    | true, result -> result
    | false, _ ->
        let (step, a, b) = args
        let n = mappingMap.[a].[b]
        let result =
            if step > 1 then
                let map1 = go (step - 1, a, n) mappingMap cache
                let map2 = go (step - 1, n, b) mappingMap cache
                mergeMaps [ map1; map2 ]
                |> Map.change
                    n
                    (function
                    | None -> Some 1L
                    | Some v -> Some(v + 1L))
            else
                Map [ n, 1L ]
        cache.Add(args, result)
        result


let puzzle input steps =
    let data = loadData input
    let mappingMap = toMappingMap data.Mapping
    let cache = Cache()
    let map1 = data.Template |> Seq.pairwise |> Seq.map (fun (a, b) -> go (steps, a, b) mappingMap cache) |> mergeMaps
    let map2 = data.Template |> Seq.groupBy id |> Seq.map (fun (key, values) -> key, Seq.length values |> int64) |> Map
    let map = mergeMaps [ map1; map2 ]
    let min'', max'' =
        map
        |> Map.toSeq
        |> Seq.map snd
        |> Seq.fold (fun (min', max') c -> min min' c, max max' c) (Int64.MaxValue, Int64.MinValue)
    let result = max'' - min''
    result |> string


let puzzle1 (input: string) = puzzle input 10

let puzzle2 (input: string) = puzzle input 40
