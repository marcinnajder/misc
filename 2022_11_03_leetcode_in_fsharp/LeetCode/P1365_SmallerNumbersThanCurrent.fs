// url: https://leetcode.com/problems/how-many-numbers-are-smaller-than-the-current-number/
// tags: mapi, filter
// examples: [| 8; 1; 2; 2; 3 |] -> [4;0;1;1;3]

module P1365_SmallerNumbersThanCurrent

let smallerNumbersThanCurrent numbers =
    numbers
    |> Seq.mapi (fun i n ->
        { 0 .. Array.length numbers - 1 } |> Seq.filter (fun j -> i <> j && numbers[j] < n) |> Seq.length)
    |> Seq.toArray

let _ =
    smallerNumbersThanCurrent [| 8
                                 1
                                 2
                                 2
                                 3 |] // -> [4;0;1;1;3]

let _ =
    smallerNumbersThanCurrent [| 6
                                 5
                                 4
                                 8 |] // -> [ 2; 1; 0; 3 ]

let _ =
    smallerNumbersThanCurrent [| 7
                                 7
                                 7
                                 7 |] // -> [ 0; 0; 0; 0 ]
