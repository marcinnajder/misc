// url: https://leetcode.com/problems/remove-nth-node-from-end-of-list/
// tags: list, pattern-matching, rec
// examples: [1,2,3,4,5] 2 -> [1,2,3,5]

module LeetCode.P0019_RemoveNthNodeFromEndOfList

let rec removeLastNth lst n =
    match lst with
    | [] -> [], 1
    | head :: tail ->
        let tail', pos = removeLastNth tail n
        if pos = n then tail', (pos + 1)
        else if pos < n then lst, (pos + 1)
        else head :: tail', (pos + 1)

let removeNthFromEnd lst n = removeLastNth lst n |> fst

let _ = removeNthFromEnd [ 1; 2; 3; 4; 5 ] 2 // -> [1; 2; 3; 5]
let _ = removeNthFromEnd [ 1; 2 ] 1 // -> [1]


// previous implementation may cause stack overflow, but we walk through the list only once
// in the implementation below we are copying the whole list reversing it
let rec removeNthFromEnd' lst n =
    lst
    |> Seq.fold (fun tail x -> x :: tail) [] // reverse list
    |> Seq.fold (fun (tail, pos) x -> ((if pos = n then tail else x :: tail), pos + 1)) ([], 1)
    |> fst

// very similar implementation to the previous one, we use builtin 'foldBack' function
// depending on the implementation of it stack overflow may potentially occur
let rec removeNthFromEnd'' lst n =
    (lst, ([], 1)) ||> Seq.foldBack (fun x (tail, pos) -> ((if pos = n then tail else x :: tail), pos + 1)) |> fst
