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

// https://leetcode.com/problems/print-binary-tree/



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



// bst |> printTree

"[1,2]" |> parseTree |> printTree === [ [ ""; "1"; "" ]; [ "2"; ""; "X" ] ]

"[1,2,3,null,4]" |> parseTree |> printTree
=== [ [ ""; ""; ""; "1"; ""; ""; "" ]; [ ""; "2"; ""; ""; ""; "3"; "" ]; [ "X"; ""; "4"; ""; "X"; ""; "X" ] ]


let levelsToString levels =
    String.Join(Environment.NewLine, levels |> Seq.map (fun l -> String.Join(" ", l |> Seq.map (sprintf "%2s"))))

Console.WriteLine(bst |> printTree |> levelsToString)











// https://leetcode.com/problems/merge-two-binary-trees/

let rec mergeTrees tree1 tree2 =
    match tree1, tree2 with
    | Tip, Tip -> Tip
    | Tip, tree -> tree
    | tree, Tip -> tree
    | Node (value1, left1, right1), Node (value2, left2, right2) ->
        Node(value1 + value2, mergeTrees left1 left2, mergeTrees right1 right2)

mergeTrees (parseTree "[1,3,2,5]") (parseTree "[2,1,3,null,4,null,7]")
=== Node(3, Node(4, Node(5, Tip, Tip), Node(4, Tip, Tip)), Node(5, Tip, Node(7, Tip, Tip)))

mergeTrees (parseTree "[1]") (parseTree "[1,2]") === Node(2, Node(2, Tip, Tip), Tip)



// https://leetcode.com/problems/kth-largest-sum-in-a-binary-tree/


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


// https://leetcode.com/problems/kth-smallest-element-in-a-bst/


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





// https://leetcode.com/problems/lowest-common-ancestor-of-a-binary-search-tree/

let rec lowestCommonAncestor min' max' tree =
    match tree with
    | Tip -> Tip
    | Node (value, left, right) ->
        if value > max' then lowestCommonAncestor min' max' left
        elif value < min' then lowestCommonAncestor min' max' right
        else tree

let t = "[6,2,8,0,4,7,9,null,null,3,5]" |> parseTree
t |> lowestCommonAncestor 2 8 === t
t |> lowestCommonAncestor 2 4 === Node(2, Node(0, Tip, Tip), Node(4, Node(3, Tip, Tip), Node(5, Tip, Tip)))
"[2,1]" |> parseTree |> lowestCommonAncestor 1 2 === Node(2, Node(1, Tip, Tip), Tip)


bst |> lowestCommonAncestor 1 3 === Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip))
bst |> lowestCommonAncestor 6 8 === Node(7, Node(6, Tip, Tip), Node(8, Tip, Tip))
bst |> lowestCommonAncestor 1 2 === Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip))
bst |> lowestCommonAncestor 1 15 === bst

bst |> lowestCommonAncestor 1 8
=== Node(5, Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip)), Node(7, Node(6, Tip, Tip), Node(8, Tip, Tip)))


// https://leetcode.com/problems/balanced-binary-tree/

let rec isBalanced tree =
    match tree with
    | Tip -> Some(0)
    | Node (_, left, right) ->
        isBalanced left
        |> Option.bind (fun leftD ->
            isBalanced right
            |> Option.bind (fun rightD -> if abs (leftD - rightD) > 1 then None else Some(max leftD rightD + 1)))

"[3,9,20,null,null,15,7]" |> parseTree |> isBalanced === Some(3)
"[1,2,2,3,3,null,null,4,4]" |> parseTree |> isBalanced === None


// https://leetcode.com/problems/binary-tree-paths/


// let rec binaryTreePaths tree =
//     match tree with
//     | Tip -> []
//     | Node (value, Tip, Tip) -> [ [ value ] ]
//     | Node (value, left, right) -> binaryTreePaths left @ binaryTreePaths right |> List.map (fun lst -> value :: lst)

// let rec binaryTreePaths tree =
//     match tree with
//     | Tip -> []
//     | Node (value, Tip, Tip) -> [ [ value ] ]
//     | Node (value, left, right) ->
//         [ binaryTreePaths left; binaryTreePaths right ] |> Seq.concat |> Seq.map (fun lst -> value :: lst) |> Seq.toList

let rec binaryTreePaths tree =
    match tree with
    | Tip -> Seq.empty
    | Node (value, Tip, Tip) -> seq { [ value ] }
    | Node (value, left, right) ->
        seq {
            binaryTreePaths left
            binaryTreePaths right
        }
        |> Seq.concat
        |> Seq.map (fun lst -> value :: lst)


// let rec binaryTreePaths tree =
//     seq {
//         match tree with
//         | Tip -> ()
//         | Node (value, Tip, Tip) -> yield [ value ]
//         | Node (value, left, right) ->
//             for lst in [ binaryTreePaths left; binaryTreePaths right ] |> Seq.concat do
//                 yield value :: lst
//     }



// "[1,2,3,null,5]" |> parseTree |> binaryTreePaths |> Seq.toArray

// "[1,2,3,null,5]" |> parseTree |> binaryTreePaths === [ [ 1; 2; 5 ]; [ 1; 3 ] ]



"[1]" |> parseTree |> binaryTreePaths === [ [ 1 ] ]

bst |> binaryTreePaths
=== [ [ 10; 5; 2; 1 ]
      [ 10; 5; 2; 3 ]
      [ 10; 5; 7; 6 ]
      [ 10; 5; 7; 8 ]
      [ 10; 15; 13; 12 ]
      [ 10; 15; 13; 14 ]
      [ 10; 15; 20; 19 ]
      [ 10; 15; 20; 25 ] ]



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






// https://leetcode.com/problems/balanced-binary-tree/
// https://leetcode.com/problems/delete-node-in-a-bst/ (tylko moje usuwanie jest idetyczne jak w przykladach leecode bo usuwam od lewego)
