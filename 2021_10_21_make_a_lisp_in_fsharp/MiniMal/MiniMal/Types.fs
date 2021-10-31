module Types

type ListType =
    | List
    | Vector

type MalType =
    | Nil
    | True
    | False
    | Number of value: double
    | Str of value: string
    | Symbol of name: string
    | MalList of items: MalType list * listType: ListType
    | MalMap of value: Map<string, MalType>
    | Fn of value: FnDelegate * isMacro: bool

and FnDelegate = MalType list -> MalType

let rec internal compareLists l1 l2 f =
    match l1, l2 with
    | [], [] -> true
    | x :: xs, y :: ys -> f x y && compareLists xs ys f
    | _ -> false

let rec malEquals mal1 mal2 =
    match mal1, mal2 with
    | MalList (list1, _), MalList (list2, _) -> compareLists list1 list2 malEquals
    | MalMap value1, MalMap value2 ->
        compareLists (Map.toList value1) (Map.toList value2) (fun (k1, v1) (k2, v2) -> k1 = k2 && malEquals v1 v2)
    | Fn _, Fn _ -> false
    | Nil, Nil
    | True, True
    | False, False -> true
    | Str s1, Str s2 -> s1 = s2
    | Number s1, Number s2 -> s1 = s2
    | Symbol s1, Symbol s2 -> s1 = s2
    | _ -> false

let internal noWayIAmHere () =
    failwith "no way this code is being executed, this pattern should never be met"
