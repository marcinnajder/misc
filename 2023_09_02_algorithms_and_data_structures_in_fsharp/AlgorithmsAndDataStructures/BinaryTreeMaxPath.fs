module AlgorithmsAndDataStructures.BinaryTreeMaxPath

open Utils
open BinaryTree

// Find the longest path between two leaves, does not have to walk through root.

// calcMaxPath - finding only the length of the path
// calcMaxPath' - finding the length and the path itself

let rec calcMaxPath tree =
    match tree with
    | Tip -> 0, 0
    | Node (_, left, Tip) ->
        let maxDepth, maxPath = calcMaxPath left
        maxDepth + 1, maxPath
    | Node (_, Tip, right) ->
        let maxDepth, maxPath = calcMaxPath right
        maxDepth + 1, maxPath
    | Node (_, left, right) ->
        let leftMaxDepth, leftMaxPath = calcMaxPath left
        let rightMaxDepth, rightMaxPath = calcMaxPath right
        let p = leftMaxDepth + rightMaxDepth
        max leftMaxDepth rightMaxDepth + 1, Seq.max [ leftMaxPath; rightMaxPath; p ]

// 6
let t1 =
    Node(
        3,
        Node(9, Node(1, Tip, Tip), Node(2, Node(7, Tip, Tip), Tip)),
        Node(8, Node(4, Tip, Node(6, Tip, Tip)), Node(5, Tip, Tip))
    )

t1 |> calcMaxPath === (4, 6)

// 12
let t2 =
    Node(
        2,
        Node(4, Node(9, Tip, Tip), Node(7, Tip, Node(8, Tip, Tip))),
        Node(
            3,
            Node(
                12,
                Node(
                    10,
                    Node(
                        6,
                        Node(
                            19,
                            Node(17, Tip, Tip),
                            Node(22, Node(13, Tip, Node(42, Tip, Node(42, Tip, Tip))), Node(16, Tip, Tip))
                        ),
                        Node(31, Tip, Tip)
                    ),
                    Node(25, Tip, Tip)
                ),
                Node(
                    1,
                    Node(
                        18,
                        Node(28, Tip, Tip),
                        Node(
                            15,
                            Node(33, Node(11, Tip, Tip), Node(13, Tip, Tip)),
                            Node(38, Node(14, Tip, Tip), Node(26, Node(21, Tip, Tip), Tip))
                        )
                    ),
                    Node(44, Tip, Tip)
                )
            ),
            Node(5, Tip, Tip)
        )
    )

// 6 (6)
let tree0 =
    Node(
        1,
        Node(2, Node(4, Node(5, Tip, Tip), Tip), Node(6, Tip, Node(7, Node(8, Tip, Node(9, Tip, Tip)), Tip))),
        Node(3, Tip, Tip)
    )

// 8 (8)
let tree1 =
    Node(
        1,
        Node(
            2,
            Node(4, Node(5, Tip, Tip), Node(18, Tip, Tip)),
            Node(6, Tip, Node(7, Node(8, Tip, Node(9, Tip, Tip)), Tip))
        ),
        Node(3, Tip, Node(10, Tip, Node(19, Tip, Tip)))
    )


let rec calcMaxPath' tree =
    match tree with
    | Tip -> (0, []), (0, ([], Unchecked.defaultof<_>, []))
    | Node (value, left, Tip) ->
        let depth, path = calcMaxPath' left
        (fst depth + 1, value :: snd depth), path
    | Node (value, Tip, right) ->
        let depth, path = calcMaxPath' right
        (fst depth + 1, value :: snd depth), path
    | Node (value, left, right) ->
        let leftDepth, leftPath = calcMaxPath' left
        let rightDepth, rightPath = calcMaxPath' right
        let depth = [ leftDepth; rightDepth ] |> Seq.maxBy fst
        let path =
            [ leftPath; rightPath; (fst leftDepth + fst rightDepth, (snd leftDepth, value, snd rightDepth)) ]
            |> Seq.maxBy fst
        (fst depth + 1, value :: snd depth), path


let toPath (_, (_, (leftDepth, value, rigthDept))) =
    value :: leftDepth |> List.fold (fun lst item -> item :: lst) rigthDept

t1 |> calcMaxPath' |> toPath === [ 7; 2; 9; 3; 8; 4; 6 ]
t2 |> calcMaxPath' |> toPath === [ 42; 42; 13; 22; 19; 6; 10; 12; 1; 18; 15; 38; 26; 21 ]
tree0 |> calcMaxPath' |> toPath === [ 5; 4; 2; 6; 7; 8; 9 ]
tree1 |> calcMaxPath' |> toPath === [ 9; 8; 7; 6; 2; 1; 3; 10; 19 ]
