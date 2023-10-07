// url: https://leetcode.com/problems/kth-largest-sum-in-a-binary-tree
// tags: tree
// examples: [5,8,9,2,1,3,7,4,6] -> 2

module LeetCode.P2583_KthLargestSumInABinaryTree

open Utils
open BinaryTree


let rec insertSorted xs x =
    match xs with
    | [] -> [ x ]
    | head :: tail -> if x < head then x :: xs else head :: insertSorted tail x

let insertSortedPreservingLength xs x =
    match xs with
    | [] -> xs
    | head :: _ -> if x < head then xs else List.tail (insertSorted xs x)


let kthLargestLevelSum tree k =
    let rec loop nodes topK =
        match nodes with
        | [] -> List.head topK
        | _ ->
            let children, sum =
                nodes
                |> List.fold
                    (fun (children, sum as state) node ->
                        match node with
                        | Tip -> state
                        | Node (value, left, Tip) -> left :: children, value + sum
                        | Node (value, Tip, right) -> right :: children, value + sum
                        | Node (value, left, right) -> left :: right :: children, value + sum)
                    ([], 0)
            loop children (insertSortedPreservingLength topK sum)
    loop [ tree ] (List.replicate k 0)

kthLargestLevelSum ("[5,8,9,2,1,3,7,4,6]" |> parseTree) 2 === 13
kthLargestLevelSum ("[1,2,null,3]" |> parseTree) 1 === 3
kthLargestLevelSum Tip 1 === 0
