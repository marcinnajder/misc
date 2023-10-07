// url: https://leetcode.com/problems/kth-smallest-element-in-a-bst
// tags: tree
// examples: [3,1,4,null,2] -> 1

module LeetCode.P0230_KthSmallestElementInABST

open Utils
open BinaryTree

type BTResult<'a> =
    | Found of BinaryTree<'a>
    | Index of int

let bindBTR f m =
    match m with
    | Index i -> f i
    | found -> found

let kthSmallest k tree =
    let rec loop node i =
        match node with
        | Tip -> Index i
        | Node (_, left, right) -> loop left i |> bindBTR (fun i' -> if i' = k then Found node else loop right (i' + 1))
    match loop tree 1 with
    | Found node -> Some node
    | _ -> None


let assertSubsequent ks tree =
    for k in ks do
        match kthSmallest k tree with
        | None
        | Some (Tip) -> 1 === 2 // fail
        | Some (Node (value, _, _)) -> value === k

"[3,1,4,null,2]" |> parseTree |> assertSubsequent { 1..4 }
"[3,1,4,null,2]" |> parseTree |> kthSmallest 5 === None
"[5,3,6,2,4,null,null,1]" |> parseTree |> assertSubsequent { 1..6 }
"[5,3,6,2,4,null,null,1]" |> parseTree |> kthSmallest 7 === None

bst |> kthSmallest 1 === Some(Node(1, Tip, Tip))
bst |> kthSmallest 2 === Some(Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip)))
bst |> kthSmallest 3 === Some(Node(3, Tip, Tip))
bst |> kthSmallest 9 === Some(Node(12, Tip, Tip))
bst |> kthSmallest 15 === Some(Node(25, Tip, Tip))
bst |> kthSmallest 16 === None




let rec inorder tree =
    seq {
        match tree with
        | Tip -> ()
        | Node (value, left, right) ->
            yield! inorder left
            yield value
            yield! inorder right
    }

let kthSmallest' k tree = tree |> inorder |> Seq.item (k - 1)

"[5,3,6,2,4,null,null,1]" |> parseTree |> kthSmallest' 2 === 2
