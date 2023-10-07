module AlgorithmsAndDataStructures.BinaryTreeDelete

open Utils
open BinaryTree

open System

// https://en.wikipedia.org/wiki/Binary_search_tree
// - tutaj opisu jest algorytm, takze czytalem w ksiazce ziomka 'cormen'
// - sam algorytm jest taki ze wskazujemy wezel/wartosc ktory chcemy usunac i
// -- jesli to jest lisc to usuwamy
// -- jesli ma jedno dziecko to usuwamy element przesuwajac wszystkie dzieci do gory
// -- jesli ma dwojke dzieci zawsze idziemy do prawego dziecka/podrzewa (takie jest tylko zalozenie bo mozna isc do lewego
// i wykonywac analogiczne operacje) i znajdujemy 'nastepnika' dla usuwanego elementu (to bedzie minimalny
// element w prawym podrzewie) i zastepujemy nim usuwany element
// - poniewaz to jest drzewo immutable to
// -- jak schodzimy rekurencyjnie w dol szukajace elementu usuwnego to musimy jednoczesnie budowac nowe drzewo
// -- nie pomozemy po prostu wyszukac sobie 'nastepnika', musimy jakby jedna oprecja usunac go idac od dolu jednoczesnie
// przebudowujac prawe poddrzewo i zwracajac go


// https://leetcode.com/problems/delete-node-in-a-bst/ (tylko moje usuwanie jest idetyczne jak w przykladach leecode bo usuwam od lewego)

let rec pullUpSuccessorOfRightSubtree tree =
    match tree with
    | Tip -> Unchecked.defaultof<_>, Tip
    | Node (v, Node (succValue, Tip, succLeft), right) -> succValue, Node(v, succLeft, right)
    | Node (v, left, right) ->
        let succValue, left' = pullUpSuccessorOfRightSubtree left
        succValue, Node(v, left', right)


"[10,5,15]" |> parseTree |> pullUpSuccessorOfRightSubtree === (5, Node(10, Tip, Node(15, Tip, Tip)))

"[10,5,15,2,7]" |> parseTree |> pullUpSuccessorOfRightSubtree
=== (2, Node(10, Node(5, Tip, Node(7, Tip, Tip)), Node(15, Tip, Tip)))

"[10,5,15,2,7,null,null,null,3]" |> parseTree |> pullUpSuccessorOfRightSubtree
=== (2, Node(10, Node(5, Node(3, Tip, Tip), Node(7, Tip, Tip)), Node(15, Tip, Tip)))


let rec delete value tree =
    match tree with
    | Tip -> Tip
    | Node (v, left, right) when v <> value ->
        if value <= v then Node(v, delete value left, right) else Node(v, left, delete value right)
    | Node (_, left, right) ->
        match left, right with
        | Tip, Tip -> Tip
        | _, Tip -> left
        | Tip, _ -> right
        | Node (lv, lleft, Tip), _ -> Node(lv, lleft, right)
        | _, Node (rv, Tip, rright) -> Node(rv, left, rright)
        | _ ->
            let succValue, right' = pullUpSuccessorOfRightSubtree right
            Node(succValue, left, right')


"[10,5,15]" |> parseTree |> delete 5 === Node(10, Tip, Node(15, Tip, Tip))
"[10,5,15]" |> parseTree |> delete 15 === Node(10, Node(5, Tip, Tip), Tip)
"[10,5,15]" |> parseTree |> delete 10 === Node(5, Tip, Node(15, Tip, Tip))
"[10,5,15,2]" |> parseTree |> delete 10 === Node(5, Node(2, Tip, Tip), Node(15, Tip, Tip))

"[10,5,15,2,7,null,20]" |> parseTree |> delete 10
=== Node(15, Node(5, Node(2, Tip, Tip), Node(7, Tip, Tip)), Node(20, Tip, Tip))

"[10,5,15,2,7,13,20]" |> parseTree |> delete 10
=== Node(13, Node(5, Node(2, Tip, Tip), Node(7, Tip, Tip)), Node(15, Tip, Node(20, Tip, Tip)))

let bst = "[10,5,15,2,7,13,20,1,3,6,8,12,14,19,25]" |> parseTree

bst |> delete 15
=== Node(
    10,
    Node(5, Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip)), Node(7, Node(6, Tip, Tip), Node(8, Tip, Tip))),
    Node(19, Node(13, Node(12, Tip, Tip), Node(14, Tip, Tip)), Node(20, Tip, Node(25, Tip, Tip)))
)

bst |> delete 13
=== Node(
    10,
    Node(5, Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip)), Node(7, Node(6, Tip, Tip), Node(8, Tip, Tip))),
    Node(15, Node(12, Tip, Node(14, Tip, Tip)), Node(20, Node(19, Tip, Tip), Node(25, Tip, Tip)))
)


bst |> delete 5
=== Node(
    10,
    Node(6, Node(2, Node(1, Tip, Tip), Node(3, Tip, Tip)), Node(7, Tip, Node(8, Tip, Tip))),
    Node(15, Node(13, Node(12, Tip, Tip), Node(14, Tip, Tip)), Node(20, Node(19, Tip, Tip), Node(25, Tip, Tip)))
)


let random = new Random()
let maxItem = 10000
let countOfItems = 100

let randomNumers =
    Seq.initInfinite (fun _ -> random.Next(maxItem)) |> Seq.distinct |> Seq.take countOfItems |> ResizeArray

let mutable randomTree = randomNumers |> Seq.fold (flip insert) Tip
isValid randomTree === true

for _ in { 0 .. (randomNumers.Count - 1) } do
    let index = random.Next(randomNumers.Count)
    randomTree <- delete randomNumers.[index] randomTree
    isValid randomTree === true
    randomNumers.RemoveAt(index)
