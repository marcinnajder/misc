// url: https://leetcode.com/problems/container-with-most-water/
// tags: list, pattern-matching, rec
// examples: [| 1; 8; 6; 2; 5; 4; 8; 3; 7 |] -> 49

module LeetCode.P0011_ContainerWithMostWater

let maxArea height =
    let lastIndex = Array.length height - 1
    height
    |> Seq.take lastIndex // avoid calling 'Seq.max' for empty collection
    |> Seq.mapi (fun i h -> { i + 1 .. lastIndex } |> Seq.map (fun j -> (min h height[j]) * (j - i)) |> Seq.max)
    |> Seq.max


let _ = maxArea [| 1; 8; 6; 2; 5; 4; 8; 3; 7 |] // -> 49
let _ = maxArea [| 1; 1 |] // -> 1



let rec insertSortedDesc item lst =
    match item, lst with
    | x, [] -> [ x ]
    | (x1, _) as x, ((y1, _) as y) :: rest -> if x1 >= y1 then x :: lst else y :: insertSortedDesc item rest

let maxAreaOptimized height =
    let maxWidth = Array.length height - 1
    let heightSorted = height |> Seq.indexed |> Seq.fold (fun lst (i, h) -> insertSortedDesc (h, i) lst) []
    heightSorted
    |> Seq.scan (fun lst pair -> pair :: lst) [] // seq of sorted asc sub-lists
    |> Seq.skip 2 // seq of sub-lists with at least 2 items
    |> Seq.scan
        (fun state lst ->
            match state, lst with
            | Some maxValue, (h, i) :: rest ->
                if h * maxWidth > maxValue then
                    let m = h * (rest |> Seq.map (fun (_, j) -> abs (i - j)) |> Seq.max)
                    Some(max maxValue m)
                else
                    None
            | _ -> state)
        (Some 0)
    |> Seq.takeWhile Option.isSome
    |> Seq.last
    |> Option.get
