module Common

open System
open System.IO
open System.Linq
open System.Reflection
open System.Text.RegularExpressions

let ProjectFolderPath =
    Path.Combine [| FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName
                    ".."
                    ".."
                    ".." |]

let parseNumbers (separator: char) (strings: string) =
    strings.Split([| separator |], StringSplitOptions.RemoveEmptyEntries) |> Array.map Int32.Parse


let assertTrue value = if value then () else failwith "assert error"

let (===) actual expected = if actual = expected then () else failwithf "assertion failed: %A <> %A" actual expected

let matchesNumbers text =
    Regex.Matches(text, @"(\-?)\d+") |> Seq.cast<Match> |> Seq.map (fun m -> Int32.Parse(m.Value)) |> Seq.toArray


let matchesNumbers1 text =
    match matchesNumbers text with
    | [| a |] -> (a)
    | _ -> failwithf "wrong data format: %s" text

let matchesNumbers2 text =
    match matchesNumbers text with
    | [| a; b |] -> (a, b)
    | _ -> failwithf "wrong data format: %s" text

let matchesNumbers3 text =
    match matchesNumbers text with
    | [| a; b; c |] -> (a, b, c)
    | _ -> failwithf "wrong data format: %s" text

let matchesNumbers4 text =
    match matchesNumbers text with
    | [| a; b; c; d |] -> (a, b, c, d)
    | _ -> failwithf "wrong data format: %s" text



let allUniquePairs (items: seq<_>) =
    seq {
        use enumerator = items.GetEnumerator()
        if enumerator.MoveNext() then
            let mutable lst = List.empty
            let first = enumerator.Current
            while enumerator.MoveNext() do
                let current = enumerator.Current
                yield first, current
                yield! lst |> Seq.map (fun item -> current, item)
                lst <- current :: lst
    }

allUniquePairs [ 0; 1; 2; 3 ] |> Seq.toList === [ (0, 1); (0, 2); (2, 1); (0, 3); (3, 2); (3, 1) ]


let allUniqueTriples (items: array<_>) =
    seq {
        for i in 0 .. items.Length - 1 do
            for j in 0 .. i - 1 do
                for k in 0 .. j - 1 do
                    (items[k], items[j], items[i])
    }

allUniqueTriples [| 0; 1; 2; 3 |] |> Seq.toList === [ (0, 1, 2); (0, 1, 3); (0, 2, 3); (1, 2, 3) ]



let insertThenPartition n item lists =
    if n = 1 then
        [ [ item ] ], lists
    else
        lists
        |> Seq.fold
            (fun (completed, lists') (len, lst) ->
                let len' = len + 1
                let lst' = item :: lst
                if len' = n then lst' :: completed, lists' else completed, (len', lst') :: lists')
            ([], (1, [ item ]) :: lists)

insertThenPartition 1 10 [] === ([ [ 10 ] ], [])

insertThenPartition 3 10 [ (2, [ 0; 0 ]); (1, [ 0 ]) ]
=== ([ [ 10; 0; 0 ] ], [ (2, [ 10; 0 ]); (1, [ 10 ]); (2, [ 0; 0 ]); (1, [ 0 ]) ])



let allUniqueTuples n items =
    items |> Seq.scan (fun (_, lists) item -> insertThenPartition n item lists) ([], []) |> Seq.collect fst

allUniqueTuples 2 [ 0; 1; 2; 3 ] |> Seq.map List.sort |> Seq.sort |> Seq.toList
=== (allUniquePairs [ 0; 1; 2; 3 ] |> Seq.map (fun (a, b) -> List.sort [ a; b ]) |> Seq.sort |> Seq.toList)

allUniqueTuples 3 [ 0; 1; 2; 3 ] |> Seq.map List.sort |> Seq.sort |> Seq.toList
=== (allUniqueTriples [| 0; 1; 2; 3 |] |> Seq.map (fun (a, b, c) -> List.sort [ a; b; c ]) |> Seq.sort |> Seq.toList)



// this is my first implementation that is I still understand ...
let rec allUniqueTuples' n items acc =
    seq {
        match n, items with
        | 1, [] -> ()
        | 1, [ next ] -> yield next :: acc
        | 1, next :: rest ->
            yield next :: acc
            yield! (allUniqueTuples' 1 rest acc)
        | _, [] -> ()
        | _, next :: rest ->
            yield! (allUniqueTuples' (n - 1) rest (next :: acc))
            yield! (allUniqueTuples' n rest acc)
    }

// ... then I "simplified" to implementation below that I do not undestand :)
let allUniqueTuples'' n items =
    let rec allUniqueTuples_ n items acc =
        seq {
            match n, items with
            | _, [] -> ()
            | 1, item :: rest ->
                yield item :: acc
                yield! allUniqueTuples_ n rest acc
            | _, item :: rest ->
                yield! allUniqueTuples_ (n - 1) rest (item :: acc)
                yield! allUniqueTuples_ n rest acc
        }
    allUniqueTuples_ n items []
