// url: https://leetcode.com/problems/odd-even-linked-list/
// tags: list, pattern-matching, rec
// examples: [1,2,3,4,5] -> [1,3,5,2,4]

module LeetCode.P0328_OddEvenLinkedList

open Utils

// mutable list
type MList = { Value: int; mutable Next: MList option }

let rec split (odds: MList) (evens: MList) (mlst: MList option) =
    match mlst with
    | Some ({ Next = Some (even) } as odd) ->
        odds.Next <- Some(odd)
        evens.Next <- Some(even)
        split odd even even.Next
    | Some ({ Next = None } as odd) ->
        odds.Next <- Some(odd)
        evens.Next <- None
        (odd, evens)
    | None ->
        odds.Next <- None
        evens.Next <- None
        (odds, evens)

let oddEvenList (mlst: MList option) =
    match mlst with
    | Some ({ Next = Some ({ Next = Some _ as rest } as evens) } as odds) ->
        let odds', evens' = split odds evens rest
        odds'.Next <- Some(evens)
        Some(odds)
    | _ -> mlst




let seqToMList items =
    items
    |> Seq.fold
        (fun (first, last) value ->
            let current = { Value = value; Next = None }
            match first with
            | None -> (Some current, current)
            | _ ->
                last.Next <- Some current
                (first, current))
        (None, Unchecked.defaultof<MList>)
    |> fst

let rec mlistToSeq (mlst: MList option) =
    seq {
        match mlst with
        | Some ({ Value = value; Next = next }) ->
            yield value
            yield! mlistToSeq (next)
        | _ -> ()
    }

[ 1; 2; 3; 4; 5 ] |> seqToMList |> oddEvenList |> mlistToSeq |> Seq.toArray === [| 1; 3; 5; 2; 4 |]
[ 2; 1; 3; 5; 6; 4; 7 ] |> seqToMList |> oddEvenList |> mlistToSeq |> Seq.toArray === [| 2; 3; 6; 7; 1; 5; 4 |]


// imutable list
// - but it's not ""... in O(1) extra space complexity and O(n) time complexity."

let rec oddEvenList' lst =
    match lst with
    | odd :: even :: tail ->
        let odds, evens = oddEvenList' tail
        (odd :: odds), (even :: evens)
    | _ -> lst, []

[ 1; 2; 3; 4; 5 ] |> oddEvenList' |> (fun (odds, evens) -> odds @ evens) === [ 1; 3; 5; 2; 4 ]
[ 2; 1; 3; 5; 6; 4; 7 ] |> oddEvenList' |> (fun (odds, evens) -> odds @ evens) === [ 2; 3; 6; 7; 1; 5; 4 ]
