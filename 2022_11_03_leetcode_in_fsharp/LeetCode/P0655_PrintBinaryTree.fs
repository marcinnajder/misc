// url: https://leetcode.com/problems/print-binary-tree/
// tags: tree
// examples: [1,2] -> [["","1",""], ["2","",""]]

module LeetCode.P0655_PrintBinaryTree

open Utils
open BinaryTree

open System


let printNode node =
    match node with
    | Tip -> "X"
    | Node (v, _, _) -> string v

printNode Tip === "X"
printNode (Node(1, Tip, Tip)) === "1"


let isLastLevel nodes =
    nodes
    |> List.forall (function
        | Tip
        | Node (_, Tip, Tip) -> true
        | _ -> false)


[] |> isLastLevel === true
[ Tip ] |> isLastLevel === true
[ Tip; Tip; Tip ] |> isLastLevel === true
[ Tip; Node(1, Tip, Tip); Tip ] |> isLastLevel === true
[ Tip; Node(1, Node(2, Tip, Tip), Tip); Tip ] |> isLastLevel === false


let createLastLevel nodes =
    let rec loop nodes' i =
        match nodes' with
        | [] -> [], i, []
        | node :: rest ->
            let values, length, indexes = loop rest (i + 1)
            "" :: (printNode node) :: values, length, i * 2 :: indexes
    let values, length, indexes = loop nodes 0
    List.tail values, 2 * length - 1, indexes

[ Tip; Node(1, Tip, Tip); Tip; Tip ] |> createLastLevel === ([ "X"; ""; "1"; ""; "X"; ""; "X" ], 7, [ 0; 2; 4; 6 ])


let createNonLastLevel nodes width indexes =
    let rec loop nodes' indexes' i =
        match nodes', indexes' with
        | _ when i = width -> []
        | node :: restN, index :: restI when i = index -> (printNode node) :: loop restN restI (i + 1)
        | _ -> "" :: loop nodes' indexes' (i + 1)
    loop nodes indexes 0

createNonLastLevel [ Tip; Node(1, Tip, Tip); Tip; Tip ] 15 [ 1; 5; 9; 13 ]
=== [ ""; "X"; ""; ""; ""; "1"; ""; ""; ""; "X"; ""; ""; ""; "X"; "" ]




let printTree tree =
    let rec loop nodes =
        if isLastLevel nodes then
            let level, width, indexes = createLastLevel nodes
            [ level ], width, indexes
        else
            let childNodes =
                nodes
                |> List.collect (function
                    | Tip -> [ Tip; Tip ]
                    | Node (_, left, right) -> [ left; right ])
            let childLevels, width, childIndexes = loop childNodes
            let indexes = childIndexes |> Seq.chunkBySize 2 |> Seq.map (fun a -> (a.[0] + a.[1]) / 2) |> Seq.toList
            let level = createNonLastLevel nodes width indexes
            level :: childLevels, width, indexes
    let levels, _, _ = loop [ tree ]
    levels


"[1,2]" |> parseTree |> printTree === [ [ ""; "1"; "" ]; [ "2"; ""; "X" ] ]

"[1,2,3,null,4]" |> parseTree |> printTree
=== [ [ ""; ""; ""; "1"; ""; ""; "" ]; [ ""; "2"; ""; ""; ""; "3"; "" ]; [ "X"; ""; "4"; ""; "X"; ""; "X" ] ]




let levelsToString levels =
    String.Join(Environment.NewLine, levels |> Seq.map (fun l -> String.Join(" ", l |> Seq.map (sprintf "%2s"))))

Console.WriteLine(bst |> printTree |> levelsToString)
