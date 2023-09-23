module AlgorithmsAndDataStructures.BinaryTreeOthers

open Utils
open BinaryTree

open System





let bst = "[10,5,15,2,7,13,20,1,3,6,8,12,14,19,25]" |> parseTree
let bstEmpty: BinaryTree<int> = Tip

let rec tryFindNode value tree =
    match tree with
    | Tip -> None
    | Node (v, _, _) when v = value -> Some(tree)
    | Node (v, left, right) -> if value < v then tryFindNode value left else tryFindNode value right

bst |> tryFindNode 25 === Some(Node(25, Tip, Tip))
bst |> tryFindNode 10 === Some(bst)
bst |> tryFindNode 16 === None





let rec findMinOrMax childSelector tree =
    match tree with
    | Tip -> Tip
    | Node (_, Tip, Tip) -> tree
    | Node (_, left, right) ->
        match childSelector (left, right) with
        | Tip -> tree
        | tree' -> findMinOrMax childSelector tree'

let findMin tree = findMinOrMax fst tree
let findMax tree = findMinOrMax snd tree

bstEmpty |> findMin === Tip
findMin bst === Node(1, Tip, Tip)
findMax bst === Node(25, Tip, Tip)
Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip)) |> findMin === Node(1, Tip, Tip)
Node(1, Tip, Node(10, Tip, Tip)) |> findMin === Node(1, Tip, Node(10, Tip, Tip))





let rec tryFindSuccessorPassingParent tree value parent =
    match tree with
    | Tip -> Tip
    | Node (v, _, Tip) when v = value -> parent
    | Node (v, _, right) when v = value -> findMin right
    | Node (v, left, right) ->
        if value <= v then
            tryFindSuccessorPassingParent left value tree
        else
            tryFindSuccessorPassingParent right value parent

let tryFindSuccessor value tree = tryFindSuccessorPassingParent tree value Tip

bstEmpty |> tryFindSuccessor 10 === Tip
Node(10, Tip, Tip) |> tryFindSuccessor 10 === Tip
Node(10, Tip, Node(15, Tip, Tip)) |> tryFindSuccessor 10 === Node(15, Tip, Tip)

bst |> tryFindSuccessor 11 === Tip
bst |> tryFindSuccessor 10 === Node(12, Tip, Tip)
bst |> tryFindSuccessor 20 === Node(25, Tip, Tip)
bst |> tryFindSuccessor 25 === Tip
bst |> tryFindSuccessor 5 === Node(6, Tip, Tip)

bst |> tryFindSuccessor 6 === Node(7, Node(6, Tip, Tip), Node(8, Tip, Tip))
bst |> tryFindSuccessor 8 === bst
bst |> tryFindSuccessor 1 === Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip))

bst |> tryFindSuccessor 3
=== Node(5, Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip)), Node(7, Node(6, Tip, Tip), Node(8, Tip, Tip)))

bst |> tryFindSuccessor 14
=== Node(15, Node(13, Node(12, Tip, Tip), Node(14, Tip, Tip)), Node(20, Node(19, Tip, Tip), Node(25, Tip, Tip)))






// Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip)) |> findMin === Node(1, Tip, Tip)
// Node(1, Tip, Node(10, Tip, Tip)) |> findMin === Node(1, Tip, Node(10, Tip, Tip))



// let tryFindSuccessorOrPredecessor childSelector nodeFinder value tree =
//     tryFindNode value tree
//     |> Option.bind (function
//         | Tip -> None
//         | Node (_, left, right) when (childSelector (left, right)) = Tip -> None
//         | Node (_, left, right) -> Some(childSelector (left, right)))
//     |> Option.bind (fun n ->
//         match nodeFinder n with
//         | Tip -> None
//         | s -> Some(s))

// let tryFindSuccessor value tree = tryFindSuccessorOrPredecessor snd findMin value tree
// let tryFindPredecessor value tree = tryFindSuccessorOrPredecessor fst findMin value tree


// bst |> tryFindSuccessor 11 === None
// bst |> tryFindSuccessor 10 === Some(Node(12, Tip, Tip))
// bst |> tryFindSuccessor 20 === Some(Node(25, Tip, Tip))
// bst |> tryFindSuccessor 25 === None
// bst |> tryFindSuccessor 5 === Some(Node(6, Tip, Tip))
// bst |> tryFindPredecessor 10 === Some(Node(8, Tip, Tip))





// https://leetcode.com/problems/kth-largest-sum-in-a-binary-tree/
// https://leetcode.com/problems/kth-smallest-element-in-a-bst/
// https://leetcode.com/problems/delete-node-in-a-bst/description/


// https://leetcode.com/problems/delete-node-in-a-bst/ (tylko moje usuwanie jest idetyczne jak w przykladach leecode bo usuwam od lewego)

// https://leetcode.com/problems/print-binary-tree/

// https://leetcode.com/problems/balanced-binary-tree/
// https://leetcode.com/problems/binary-tree-paths/
// https://leetcode.com/problems/lowest-common-ancestor-of-a-binary-search-tree/
// https://leetcode.com/problems/merge-two-binary-trees/
