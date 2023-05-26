module AdventOfCode2022.Day08

open System

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines |> Seq.map (fun line -> line |> Seq.map (fun c -> c.ToString() |> int)) |> array2D

let getHeight (data: int [,]) = data.GetLength(0)
let getWidth (data: int [,]) = data.GetLength(1)

let seqOfInteriorPositions (data: int [,]) =
    seq {
        for y = 1 to getHeight data - 2 do
            for x = 1 to getWidth data - 2 do
                yield y, x
    }


let isTreeVisible (data: int [,]) (y, x) =
    let value = data.[y, x]
    seq {
        Seq.forall (fun x' -> data.[y, x'] < value) { 0 .. x - 1 } // left
        Seq.forall (fun x' -> data.[y, x'] < value) { x + 1 .. getWidth data - 1 } // right
        Seq.forall (fun y' -> data.[y', x] < value) { 0 .. y - 1 } // up
        Seq.forall (fun y' -> data.[y', x] < value) { y + 1 .. getHeight data - 1 } // down
    }
    |> Seq.exists id


let puzzle1 input =
    let data = loadData input
    let visibleOnEdges = (getHeight data * 2) + ((getHeight data - 2) * 2)
    let visibleInterior = data |> seqOfInteriorPositions |> Seq.filter (isTreeVisible data) |> Seq.length
    visibleOnEdges + visibleInterior




// works similar to build-in takeWhile but includes also first falsy item
let takeWhileIncluded f (items: seq<_>) =
    seq {
        use iterator = items.GetEnumerator()
        let mutable completed = false
        while not completed && iterator.MoveNext() do
            let item = iterator.Current
            yield item
            completed <- not (f item)
    }

let countVisibleTreesInLine value getValueOfPosition positions =
    positions |> Seq.map getValueOfPosition |> takeWhileIncluded (fun v -> v < value) |> Seq.length

let countVisibleTrees (data: int [,]) (y, x) =
    let value = data.[y, x]
    seq {
        countVisibleTreesInLine value (fun x' -> data.[y, x']) { x - 1 .. -1 .. 0 } // left
        countVisibleTreesInLine value (fun x' -> data.[y, x']) { x + 1 .. getWidth data - 1 } // right
        countVisibleTreesInLine value (fun y' -> data.[y', x]) { y - 1 .. -1 .. 0 } // up
        countVisibleTreesInLine value (fun y' -> data.[y', x]) { y + 1 .. getHeight data - 1 } // down
    }
    |> Seq.reduce (*)



let puzzle2 input =
    let data = loadData input
    data |> seqOfInteriorPositions |> Seq.map (countVisibleTrees data) |> Seq.max
