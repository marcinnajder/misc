module AlgorithmsAndDataStructures.Graph

open Utils

open System.Xml.Linq
open System
open System.IO
open System.Linq
open System.Collections.Generic
open FSharpx.Collections // #r "nuget: FSharpx.Collections"

type Edge<'T> = { From: 'T; To: 'T; Weight: int }

type Graph<'T> = Edge<'T> list

let simpleGraph =
    [ { From = 1; To = 2; Weight = 5 }
      { From = 1; To = 3; Weight = 1 }
      { From = 3; To = 2; Weight = 2 }
      { From = 2; To = 4; Weight = 6 }
      { From = 1; To = 4; Weight = 10 } ]

// ** generic implementation (dfs vs bfs) using abstract collection (stack vs queue)

type IItemsProcessor<'T, 'C> =
    abstract CreateEmpty: 'C
    abstract IsEmpty: 'C -> bool
    abstract Insert: 'T -> 'C -> 'C
    abstract GetNext: 'C -> 'T * 'C

let walkGraph (processor: IItemsProcessor<_, _>) graph start =
    seq {
        let neighbors = graph |> Seq.groupBy (fun e -> e.From) |> Map
        let mutable visited = Set.singleton start
        let mutable nextItems = processor.Insert start processor.CreateEmpty
        while not (processor.IsEmpty nextItems) do
            let item, items' = processor.GetNext nextItems
            yield item
            nextItems <- items'
            for { To = nextItem } in Map.tryFind item neighbors |> Option.defaultValue Seq.empty do
                if not (Set.contains nextItem visited) then
                    visited <- Set.add nextItem visited
                    nextItems <- processor.Insert nextItem nextItems
    }

// the same more "functional style" without mutationing of variables
let walkGraph2 (processor: IItemsProcessor<_, _>) graph start =
    let neighbors = graph |> Seq.groupBy (fun e -> e.From) |> Map

    let loop (visited, nextItems) =
        if processor.IsEmpty nextItems then
            None
        else
            let item, nextItems' = processor.GetNext nextItems
            let state =
                Map.tryFind item neighbors
                |> Option.defaultValue Seq.empty
                |> Seq.filter (fun n -> not (Set.contains n.To visited))
                |> Seq.fold
                    (fun (visited'', nextItems'') n -> (Set.add n.To visited'', processor.Insert n.To nextItems''))
                    (visited, nextItems')
            Some(item, state)

    Seq.unfold loop (Set.singleton start, processor.Insert start processor.CreateEmpty)

// the same but using (anonymous) records instead of tuples
let walkGraph3 (processor: IItemsProcessor<_, _>) graph start =
    let neighbors = graph |> Seq.groupBy (fun e -> e.From) |> Map
    {| Visited = Set.singleton start; NextItems = processor.Insert start processor.CreateEmpty |}
    |> Seq.unfold (fun state ->
        if processor.IsEmpty state.NextItems then
            None
        else
            let item, nextItems' = processor.GetNext state.NextItems
            let state'' =
                Map.tryFind item neighbors
                |> Option.defaultValue Seq.empty
                |> Seq.filter (fun n -> not (Set.contains n.To state.Visited))
                |> Seq.fold
                    (fun state' n ->
                        {| Visited = Set.add n.To state'.Visited; NextItems = processor.Insert n.To state'.NextItems |})
                    {| state with NextItems = nextItems' |}
            Some(item, state''))


let stackProcessor =
    { new IItemsProcessor<'T, 'T list> with
        member this.CreateEmpty = List.empty
        member this.IsEmpty coll = List.isEmpty coll
        member this.Insert item coll = item :: coll // !
        member this.GetNext coll =
            match coll with
            | head :: tail -> head, tail
            | _ -> Unchecked.defaultof<'T>, [] }


type Queue<'a> = { Front: 'a list; Back: 'a list }

let queueProcessor =
    { new IItemsProcessor<'T, Queue<'T>> with
        member this.CreateEmpty = { Front = []; Back = [] }
        member this.IsEmpty coll = List.isEmpty coll.Front && List.isEmpty coll.Back
        member this.Insert item coll = { coll with Back = item :: coll.Back }
        member this.GetNext coll =
            match coll with
            | { Front = []; Back = [] } -> Unchecked.defaultof<'T>, coll
            | { Front = []; Back = back } ->
                let front = List.rev back
                List.head front, { Front = List.tail front; Back = [] }
            | { Front = head :: tail } -> head, { coll with Front = tail } }

// testing processors
let q1: Queue<int> = queueProcessor.CreateEmpty
queueProcessor.IsEmpty q1 === true
let q2 = q1 |> queueProcessor.Insert 5 |> queueProcessor.Insert 10 |> queueProcessor.Insert 15
queueProcessor.IsEmpty q2 === false
q2 |> queueProcessor.GetNext |> fst === 5
q2 |> queueProcessor.GetNext |> snd |> queueProcessor.GetNext |> fst === 10


// building graph from XML file, XML representation is easier to understand than list of edges
let loadGraphFromXml (xml: XElement) =
    xml.Descendants()
    |> Seq.append [ xml ]
    |> Seq.collect (fun parent -> parent.Elements() |> Seq.map (fun child -> parent, child))
    |> Seq.map (fun (parent, child) ->
        { From = parent.Attribute("value").Value
          To = child.Attribute("value").Value
          Weight = child.Attribute("weight").Value |> Int32.Parse })
    |> Seq.toList


// testing dfs and bfs
let graphFromXml = XElement.Load("./AlgorithmsAndDataStructures/Data/graphSimple.xml") |> loadGraphFromXml

let dfs graph start = walkGraph stackProcessor graph start
let bfs graph start = walkGraph queueProcessor graph start

dfs graphFromXml (graphFromXml.Head.From) |> Seq.toList === [ "1"; "3"; "6"; "7"; "2"; "5"; "4" ]
bfs graphFromXml (graphFromXml.Head.From) |> Seq.toList === [ "1"; "2"; "3"; "4"; "5"; "6"; "7" ]




// ** dijkstra

let walkDijkstra graph start finish =
    let neighbors = graph |> Seq.groupBy (fun e -> e.From) |> Map
    let mutable costSoFar = [ (start, 0) ] |> Map
    let mutable queue = PriorityQueue.empty false |> PriorityQueue.insert (0, start)
    let mutable result = None

    while Option.isNone result && not (PriorityQueue.isEmpty queue) do
        let (currentCost, current), queue' = PriorityQueue.pop queue
        queue <- queue'
        if current = finish then
            result <- Some currentCost
        else
            for next in Map.tryFind current neighbors |> Option.defaultValue Seq.empty do
                let newCost = costSoFar.[current] + next.Weight
                let nextCostExists, nextCost = costSoFar.TryGetValue next.To
                if (not nextCostExists) || (newCost < nextCost) then
                    costSoFar <- Map.change next.To (fun _ -> Some newCost) costSoFar
                    queue <- PriorityQueue.insert (newCost, next.To) queue
    result.Value


// building graph from "board representations" very often used in advent of code puzzles
let loadData (input: string) =
    input.Split Environment.NewLine
    |> Array.map (fun l -> l.ToCharArray() |> Array.map (fun a -> Char.GetNumericValue(a) |> int))

let neighbours (r, c) rMax cMax =
    seq {
        if r > 0 then r - 1, c
        if r < rMax then r + 1, c
        if c > 0 then r, c - 1
        if c < cMax then r, c + 1
    }

let loadGraphFromGrid (rows: int array array) =
    let maxR = rows.Length - 1
    let maxC = rows.[0].Length - 1
    let positions = { 0..maxR } |> Seq.collect (fun r -> { 0..maxC } |> Seq.map (fun c -> r, c))
    positions
    |> Seq.collect (fun p ->
        neighbours p maxR maxC |> Seq.map (fun n -> { From = p; To = n; Weight = rows[fst n][snd n] }))
    |> Seq.toArray


// testing dijkstra with data from https://adventofcode.com/2021/day/15
let graphGridSmall = File.ReadAllText("./AlgorithmsAndDataStructures/Data/Day15_.txt") |> loadData |> loadGraphFromGrid
let graphGridBig = File.ReadAllText("./AlgorithmsAndDataStructures/Data/Day15.txt") |> loadData |> loadGraphFromGrid

walkDijkstra graphGridSmall (0, 0) (9, 9) === 40
walkDijkstra graphGridBig (0, 0) (99, 99) === 540
