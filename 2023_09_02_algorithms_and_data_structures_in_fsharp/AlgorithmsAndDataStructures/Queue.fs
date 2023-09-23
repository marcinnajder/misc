module AlgorithmsAndDataStructures.Queue

open Utils

// https://www.youtube.com/watch?v=rCiBfZO67A4&t=228s Functional Queues | OCaml Programming | Chapter 5 Video 7


type Queue<'a> = { Front: 'a list; Back: 'a list }

let empty = { Front = []; Back = [] }

let peek =
    function
    | { Front = [] } -> None
    | { Front = x :: _ } -> Some x

peek empty === None


let enqueue x =
    function
    | { Front = [] } -> { Front = [ x ]; Back = [] }
    | q -> { q with Back = x :: q.Back }

let denqueue =
    function
    | { Front = [] } -> None
    | { Front = _ :: []; Back = back } -> Some { Front = List.rev back; Back = [] }
    | { Front = _ :: t; Back = back } -> Some { Front = t; Back = back }


denqueue empty === None

let q = empty |> enqueue 5 |> enqueue 10 |> enqueue 15

peek q === Some 5
q |> denqueue |> Option.bind peek === Some 10
q |> denqueue |> Option.bind denqueue |> Option.bind denqueue === Some empty







// https://www.lucc.pl/inf/struktury_danych_i_zlozonosc_obliczeniowa/cormen_-_wprowadzenie_do_algorytmow.pdf

// https://dahlan.unimal.ac.id/files/ebooks/2013%20Algorithms_Unlocked.pdf
// https://doc.lagout.org/programmation/Functional%20Programming/Chris_Okasaki-Purely_Functional_Data_Structures-Cambridge_University_Press%281998%29.pdf
// https://doc.lagout.org/programmation/

// https://cs3110.github.io/textbook/cover.html
