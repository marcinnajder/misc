module AdventOfCode2022.Day09

open System
open System.Linq
open System.Text.RegularExpressions
open Common


let input =
    System.IO.File.ReadAllText
        "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2022/Day08.txt"


let assertTrue value = if value then () else failwith "assert error"



type LinkedList =
    | Empty // []
    | Cons of int * LinkedList // ::

let l = Cons(1, Cons(2, Empty))

let rec len l =
    match l with
    | Empty -> 0
    | Cons (_, tail) -> 1 + len tail


let rec len2 l =
    match l with
    | [] -> 0
    | head :: tail -> 1 + len2 tail



len2 [ 1; 2; 4; 5; 6 ] = 5 |> assertTrue

// len2 ([]: int list) = 0 |> assertTrue

// let a = []

// val length : xs:'a list -> int
// val map : f:('a -> 'b) -> xs:'a list -> 'b list
// val filter : f:('a -> bool) -> xs:'a list -> 'a list
// val fold : f:('a -> 'b -> 'a) -> state:'a -> xs:'b list -> 'a
// val reduce : f:('a -> 'a -> 'a) -> xs:'a list -> 'a
// val take : n:int -> xs:'a list -> 'a list
// val skip : n:int -> xs:'a list -> 'a list
// val concat : list1:'a list -> list2:'a list -> 'a list
// val zip : f:('a -> 'b -> 'c) -> list1:'a list -> list2:'b list -> 'c list
// val collect : f:('a -> 'b list) -> xs:'a list -> 'b list
// val reverse : xs:'a list -> 'a list
// val forall : f:('a -> bool) -> xs:'a list -> bool
// val exists : f:('a -> bool) -> xs:'a list -> bool
// val nth : n:int -> xs:'a list -> 'a
// val sequenceEqual : f:('a -> 'b -> bool) -> list1:'a list -> list2:'b list -> bool



































let rec length xs =
    match xs with
    | [] -> 0
    | _ :: tail -> 1 + length tail


length [] = 0 |> assertTrue
length [ 1 ] = 1 |> assertTrue
length [ 1; 2 ] = 2 |> assertTrue

let rec map f xs =
    match xs with
    | [] -> []
    | head :: tail -> f head :: map f tail

map ((+) 1) [ 1; 2; 3 ] = [ 2; 3; 4 ] |> assertTrue
map ((+) 1) ([]: int list) = [] |> assertTrue


let rec filter f xs =
    match xs with
    | [] -> []
    | head :: tail -> if f head then head :: filter f tail else filter f tail

filter (fun x -> x > 5) [ 0; 5; 10; 20; 0 ] = [ 10; 20 ] |> assertTrue

let rec fold f state xs =
    match xs with
    | [] -> state
    | head :: tail -> fold f (f state head) tail

fold (+) 0 [ 1; 2; 3 ] = 6 |> assertTrue
fold max 0 [ 1; 2; 3; 1 ] = 3 |> assertTrue

let rec reduce f xs =
    match xs with
    | [] -> failwith "list can not be empty"
    | head :: tail -> fold f head tail

reduce (+) [ 1; 2; 3 ] = 6 |> assertTrue


let rec take n xs =
    match xs, n with
    | [], _
    | _, 0 -> []
    | head :: tail, _ -> head :: take (n - 1) tail


take 2 ([]: int list) = [] |> assertTrue
take 0 [ 1; 2; 3 ] = [] |> assertTrue
take 1 [ 1; 2; 3 ] = [ 1 ] |> assertTrue
take 2 [ 1; 2; 3 ] = [ 1; 2 ] |> assertTrue
take 3 [ 1; 2; 3 ] = [ 1; 2; 3 ] |> assertTrue
take 4 [ 1; 2; 3 ] = [ 1; 2; 3 ] |> assertTrue

let rec skip n xs =
    match xs, n with
    | [], _ -> []
    | _, 0 -> xs
    | _ :: tail, _ -> skip (n - 1) tail


skip 2 ([]: int list) = [] |> assertTrue
skip 0 [ 1; 2; 3 ] = [ 1; 2; 3 ] |> assertTrue
skip 1 [ 1; 2; 3 ] = [ 2; 3 ] |> assertTrue
skip 2 [ 1; 2; 3 ] = [ 3 ] |> assertTrue
skip 3 [ 1; 2; 3 ] = [] |> assertTrue
skip 4 [ 1; 2; 3 ] = [] |> assertTrue

let rec concat list1 list2 =
    match list1, list2 with
    | [], _ -> list2
    | _, [] -> list1
    | head :: tail, _ -> head :: concat tail list2

concat [ 1; 2 ] [] = [ 1; 2 ] |> assertTrue
concat [] [ 1; 2 ] = [ 1; 2 ] |> assertTrue
concat [ 1; 2 ] [ 3; 4 ] = [ 1; 2; 3; 4 ] |> assertTrue

let rec zip f list1 list2 =
    match list1, list2 with
    | [], _
    | _, [] -> []
    | head1 :: tail1, head2 :: tail2 -> f head1 head2 :: zip f tail1 tail2


zip (+) [ 10; 100; 1000 ] [ 3; 7; 9 ] = [ 13; 107; 1009 ] |> assertTrue
zip (+) [ 10; 100 ] [ 3; 7; 9 ] = [ 13; 107 ] |> assertTrue
zip (+) [ 10; 100; 1000 ] [ 3; 7 ] = [ 13; 107 ] |> assertTrue


let rec collect f xs =
    match xs with
    | [] -> []
    | head :: tail -> concat (f head) (collect f tail)

collect (fun x -> [ x; x ]) [ 1; 2 ] = [ 1; 1; 2; 2 ] |> assertTrue


let rec reverse xs =
    let rec rev lst state =
        match lst with
        | [] -> state
        | head :: tail -> rev tail (head :: state)
    rev xs []


reverse [ 1; 2; 3 ] = [ 3; 2; 1 ] |> assertTrue

let rec forall f xs =
    match xs with
    | [] -> true
    | head :: tail -> f head && forall f tail

forall (fun x -> x > 0) [ 5; 10; 15 ] = true |> assertTrue
forall (fun x -> x < 15) [ 5; 10; 15 ] = false |> assertTrue

let rec exists f xs =
    match xs with
    | [] -> false
    | head :: tail -> f head || exists f tail


exists (fun x -> x > 10) [ 5; 10; 15 ] = true |> assertTrue
exists (fun x -> x > 15) [ 5; 10; 15 ] = false |> assertTrue


let rec nth n xs =
    match xs, n with
    | [], _ -> failwith "n out of bounds"
    | head :: _, 0 -> head
    | _ :: tail, n' -> nth (n' - 1) tail

nth 0 [ 11; 22; 33 ] = 11 |> assertTrue
nth 1 [ 11; 22; 33 ] = 22 |> assertTrue
// nth 4 [ 11; 22; 33 ]

let rec sequenceEqual f list1 list2 =
    match list1, list2 with
    | [], [] -> true
    | [], _
    | _, [] -> false
    | head1 :: tail1, head2 :: tail2 -> f head1 head2 && sequenceEqual f tail1 tail2


sequenceEqual (=) [ 1; 2; 3 ] [ 1; 2; 3 ] = true |> assertTrue
sequenceEqual (<) [ 1; 2; 3 ] [ 10; 20; 30 ] = true |> assertTrue
sequenceEqual (<) [ 1; 2; 3 ] [ 10; 2; 30 ] = false |> assertTrue
sequenceEqual (=) [ 1; 2; 3 ] [ 1; 2 ] = false |> assertTrue
