module AdventOfCode2022.Day03

open System
open Common

let loadData (input: string) = input.Split Environment.NewLine

let priorityOfa = int 'a'
let priorityOfA = int 'A'

let calculatePriorityOfChar c = (int c) - (if Char.IsLower(c) then priorityOfa else priorityOfA - 26) + 1

let findCharIncludedInAllCollections (colls: array<_>) =
    let firstColl = colls.[0]
    let otherColls = colls |> Seq.skip 1 |> Seq.map Set |> Seq.toArray
    firstColl |> Seq.find (fun c -> Seq.forall (Set.contains c) otherColls)

let puzzle input partitioner =
    input
    |> loadData
    |> partitioner
    |> Seq.map findCharIncludedInAllCollections
    |> Seq.map calculatePriorityOfChar
    |> Seq.sum


let puzzle1 input = puzzle input (fun items -> Seq.map (Seq.splitInto 2 >> Seq.toArray) items)

let puzzle2 input = puzzle input (fun items -> Seq.chunkBySize 3 items)



//  ** ** **

let findCharIncludedInAllCollections' colls =
    let firstColl, restColls =
        colls
        |> Seq.fold
            (fun (first, sets) coll ->
                match first with
                | None -> Some coll, sets
                | _ -> first, (Set coll) :: sets)
            (None, [])
    firstColl |> Option.get |> Seq.find (fun c -> Seq.forall (Set.contains c) restColls)



findCharIncludedInAllCollections (Seq.splitInto 2 "vJrwpWtwJgWrhcsFMMfFFhFp" |> Seq.toArray) = 'p' |> assertTrue
findCharIncludedInAllCollections' (Seq.splitInto 2 "vJrwpWtwJgWrhcsFMMfFFhFp" |> Seq.toArray) = 'p' |> assertTrue

findCharIncludedInAllCollections (Seq.splitInto 2 "PmmdzqPrVvPwwTWBwg" |> Seq.toArray) = 'P' |> assertTrue
findCharIncludedInAllCollections' (Seq.splitInto 2 "PmmdzqPrVvPwwTWBwg" |> Seq.toArray) = 'P' |> assertTrue
