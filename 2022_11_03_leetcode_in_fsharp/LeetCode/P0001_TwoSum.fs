// url: https://leetcode.com/problems/two-sum/
// tags: collect, find
// examples: [| 2; 7; 11; 15 |] -> 9

module LeetCode.P0001_TwoSum

let twoSum nums target =
    { 0 .. Array.length nums - 1 }
    |> Seq.collect (fun i -> { 0 .. i - 1 } |> Seq.map (fun j -> (j, i)))
    |> Seq.find (fun (j, i) -> nums.[i] + nums.[j] = target)

let _ = twoSum [| 2; 7; 11; 15 |] 9 // -> [0, 1]
let _ = twoSum [| 3; 2; 4 |] 6 // -> [ 1, 2 ]
let _ = twoSum [| 3; 3 |] 6 // -> [ 0, 1 ]
