module AdventOfCode2021.Day15

// #load "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Common.fs"


let input =
    System.IO.File.ReadAllText
        "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day14.txt"

// ******************************************************************************


open System
open System.Collections.Generic


type Input =
    { Template: string
      Mapping: (string * string) [] }

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

type MappingMap = Map<char, Map<char, char>>

[<Literal>]
let ACharIndex = 65 // 'A' |> int

let indexOfChar (c: char) = (c |> int) - ACharIndex

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

// let template = data.Template
// let step template mappingMap =
//     Seq.append template [ '-' ]
//     |> Seq.pairwise
//     |> Seq.collect
//         (fun (a, b) ->
//             let mapped = mappingMap |> Map.tryFind a |> Option.bind (Map.tryFind b)
//             match mapped with
//             | None -> [ a ]
//             | Some c -> [ a; c ])



// let step2 template (mappingMap: MappingMap) =
//     seq {
//         for (a, b) in Seq.append template [ '-' ] |> Seq.pairwise do
//             yield a
//             if b <> '-' then yield mappingMap.[a].[b]
//     }



// // let template = data.Template
// let rec insertNew (mappingMap: MappingMap) (counter: int64 []) steps (a, b) step =
//     let n = mappingMap.[a].[b]
//     let nIndex = indexOfChar n
//     counter.[nIndex] <- counter.[nIndex] + 1L
//     // printfn "%d %A -> %A" step (a, b) n
//     if step < steps then
//         insertNew mappingMap counter steps (a, n) (step + 1)
//         insertNew mappingMap counter steps (n, b) (step + 1)




type Result = Map<char, int64>

type Cache = Dictionary<int * char * char, Result>



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


// mergeMaps' [(Map [(1,3);(2,2);]); (Map [(1,3);(5,3);])]
// let args = (1, 'N', 'N')

let rec run args (mappingMap: MappingMap) (cache: Cache) =
    match cache.TryGetValue args with
    | true, result -> result
    | false, _ ->
        let (step, a, b) = args
        let n = mappingMap.[a].[b]
        let result =
            if step > 1 then
                let map1 = run (step - 1, a, n) mappingMap cache
                let map2 = run (step - 1, n, b) mappingMap cache
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



//     let data = loadData input
//     let mappingMap = toMappingMap data.Mapping
// let template = data.Template
// let steps = 40


let start (template: string) (mappingMap: MappingMap) steps =
    let cache = Cache()
    let map1 = template |> Seq.pairwise |> Seq.map (fun (a, b) -> run (steps, a, b) mappingMap cache) |> mergeMaps

    let map2 = template |> Seq.groupBy id |> Seq.map (fun (key, values) -> key, Seq.length values |> int64) |> Map

    let map = mergeMaps [ map1; map2 ]
    let min'', max'' =
        map
        |> Map.toSeq
        |> Seq.map snd
        |> Seq.fold (fun (min', max') c -> min min' c, max max' c) (Int64.MaxValue, Int64.MinValue)
    max'' - min''

// while not (List.isEmpty stack) do
//     match stack with
//     | (step, (a, b)) :: tail ->
//         let n = mappingMap.[a].[b]
//         let nIndex = indexOfChar n
//         counter.[nIndex] <- counter.[nIndex] + 1L
//         stack <- if step < steps then (step + 1, (a, n)) :: (step + 1, (n, b)) :: tail else tail
//     | _ -> ()

// let min'', max'' =
//     counter
//     |> Seq.filter (fun x -> x > 0L)
//     |> Seq.fold (fun (min', max') c -> min min' c, max max' c) (Int64.MaxValue, Int64.MinValue)
// max'' - min''





// ************************************************************
// let start template (mappingMap: MappingMap) steps =
//     let counter = Array.create (indexOfChar 'Z' + 1) 0L
//     template |> Seq.map indexOfChar |> Seq.iter (fun i -> counter.[i] <- counter.[i] + 1L)

//     let mutable stack = template |> Seq.pairwise |> Seq.map (fun pair -> 1, pair) |> Seq.toList
//     while not (List.isEmpty stack) do
//         match stack with
//         | (step, (a, b)) :: tail ->
//             let n = mappingMap.[a].[b]
//             let nIndex = indexOfChar n
//             counter.[nIndex] <- counter.[nIndex] + 1L
//             stack <- if step < steps then (step + 1, (a, n)) :: (step + 1, (n, b)) :: tail else tail
//         | _ -> ()

//     let min'', max'' =
//         counter
//         |> Seq.filter (fun x -> x > 0L)
//         |> Seq.fold (fun (min', max') c -> min min' c, max max' c) (Int64.MaxValue, Int64.MinValue)
//     max'' - min''


// ************************************************************
// let start template (mappingMap: MappingMap) steps =
//     let counter = Array.create (indexOfChar 'Z' + 1) 0L
//     template |> Seq.map indexOfChar |> Seq.iter (fun i -> counter.[i] <- counter.[i] + 1L)
//     template
//     |> Seq.pairwise
//     |> Seq.iter
//         (fun pair ->
//             printfn "%A" pair
//             insertNew mappingMap counter steps pair 1)


//     let min'', max'' =
//         counter
//         |> Seq.filter (fun x -> x > 0L)
//         |> Seq.fold (fun (min', max') c -> min min' c, max max' c) (Int64.MaxValue, Int64.MinValue)
//     max'' - min''







// Seq.append template [ '-' ]
// |> Seq.pairwise
// |> Seq.collect
//     (fun (a, b) ->
//         let mapped = mappingMap |> Map.tryFind a |> Option.bind (Map.tryFind b)
//         match mapped with
//         | None -> [ a ]
//         | Some c -> [ a; c ])



// let groupBy' items =
//     items
//     |> Seq.fold
//         (fun a item ->
//             a
//             |> Map.change
//                 item
//                 (function
//                 | None -> Some 1L
//                 | Some value -> Some(value + 1L)))
//         Map.empty




// let puzzle (input: string) steps =
//     let data = loadData input
//     let mappingMap = toMappingMap data.Mapping
//     let template =
//         seq { 1 .. 20 } |> Seq.fold (fun template _ -> step2 template mappingMap) (data.Template :> seq<char>)
//     let min'', max'' =
//         template
//         |> groupBy'
//         |> Map.toSeq
//         |> Seq.map snd
//         //|> Seq.groupBy id
//         //|> Seq.map (fun (_, values) -> Seq.length values |> int64)
//         |> Seq.fold (fun (min', max') c -> min min' c, max max' c) (Int64.MaxValue, Int64.MinValue)
//     let result = max'' - min''
//     result |> string


// let puzzle1 (input: string) = puzzle input 10

// let puzzle2 (input: string) = puzzle input 40
