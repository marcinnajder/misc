// url: https://leetcode.com/problems/merge-k-sorted-lists/
// tags: list, pattern-matching, rec
// examples: [[1,4,5],[1,3,4],[2,6]] -> [1,1,2,3,4,4,5,6]

module LeetCode.P0023_MergeKSortedLists


let lists = [ [ 1; 4; 5 ]; [ 1; 3; 4 ]; [ 2; 6 ] ]

let rec mergeKLists lists =
    lists
    |> Seq.choose (fun lst ->
        match lst with
        | head :: tail -> Some(head, tail, lst)
        | _ -> None)
    |> Seq.fold
        (fun (mins, todo) trio ->
            let (head, _, lst) = trio
            match mins with
            | [] -> [ trio ], todo
            | (headM, _, _) :: _ ->
                if head = headM then trio :: mins, todo
                else if head < headM then [ trio ], mins |> Seq.map (fun (_, _, lst) -> lst) |> Seq.append todo
                else mins, [ lst ] |> Seq.append todo)
        ([], Seq.empty)
    |> function
        | [], _ -> []
        | mins, todo ->
            let done' = (mins |> Seq.map (fun (_, t, _) -> t)) |> Seq.append todo |> mergeKLists
            mins |> Seq.fold (fun s (h, _, _) -> h :: s) done'

let _ =
    mergeKLists [ [ 1; 4; 5 ]
                  [ 1; 3; 4 ]
                  [ 2; 6 ] ] // -> [1; 1; 2; 3; 4; 4; 5; 6]

let _ = mergeKLists ([ [] ]: seq<list<int>>)
