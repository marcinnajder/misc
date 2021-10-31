module Core

open Types
open Printer
open Reader

let private throwError functionName args message =
    failwith $"""'{functionName}' function requires {message}, but got {joinWithSeparator args (Some(","))}"""

let internal executeArithmeticFn args operation =
    match args with
    | _ :: _ :: _ ->
        List.reduce
            (fun totalMal nextMal ->
                match totalMal, nextMal with
                | (Number total, Number next) -> Number(operation total next)
                | _ ->
                    let m =
                        match totalMal with
                        | Number _ -> nextMal
                        | _ -> totalMal
                    failwith
                        $"""All arguments of arithmetic operations must be of the 'Number' type, but got an argument '{(printStr m)}' in {joinWithSeparator args (Some(","))}""")
            args
    | _ ->
        failwith
            $"""Arithmetic operation required at least two arguments, but got '{List.length args}', arguments: {joinWithSeparator args (Some(","))}"""


let internal executeComparisonFn args comparison =
    match args with
    | [ Number value1; Number value2 ] -> if comparison value1 value2 then True else False
    | _ ->
        failwith
            $"""Number comparison operation requires two arguments of type 'Number', but got {joinWithSeparator args (Some(","))}"""


let internal listFn args = MalList(args, List)

let internal vectorFn args = MalList(args, Vector)

let internal constFn args =
    match args with
    | [ firstArg; MalList (items, _) ] -> MalList(firstArg :: items, List)
    | _ -> throwError "cons" args "two arguments where the second one must be of type 'list'"

let rec internal concatFn args =
    match args with
    | [] -> MalList([], List)
    | MalList (items, _) :: restArgs ->
        match concatFn restArgs with
        | MalList (restItems, _) -> MalList(items @ restItems, List)
        | _ -> noWayIAmHere ()
    | _ -> throwError "concat" args "all arguments to be of type 'list'"

let internal conjFn args =
    match args with
    | MalList (items, List) :: newItems -> MalList((newItems |> List.fold (fun p c -> c :: p) items), List)
    | MalList (items, Vector) :: newItems -> MalList(items @ newItems, Vector)
    | _ -> throwError "conj" args "at least one argument of type 'list' or 'vector'"

let internal countFn args =
    match args with
    | [ Nil ] -> Number(0.)
    | [ MalList (items, _) ] -> Number(List.length items |> double)
    | _ -> throwError "count" args "one argument of type 'list' or 'vector' or 'nil'"

let internal firstFn args =
    match args with
    | [ Nil ] -> Nil
    | [ MalList (items, _) ] ->
        match items with
        | [] -> Nil
        | first :: _ -> first
    | _ -> throwError "first" args "one argument of type 'list' or 'vector'"


let internal restFn args =
    match args with
    | [ Nil ] -> MalList([], List)
    | [ MalList (items, _) ] ->
        match items with
        | [] -> MalList([], List)
        | _ :: tail -> MalList(tail, List)
    | _ -> throwError "rest" args "one argument of type 'list' or 'vector'"


let internal nthFn args =
    match args with
    | [ MalList (items, _); Number n ] -> List.item (int n) items
    | _ -> throwError "nth" args "two arguments where the first one is of type 'list' and the second of type 'number'"

let internal isEmptyFn args =
    match args with
    | [ MalList ([], _) ] -> True
    | [ MalList (_) ] -> False
    | _ -> throwError "empty?" args "one argument of type 'list' or 'vector'"

let internal isListFn args =
    match args with
    | [ MalList (_, List) ] -> True
    | [ _ ] -> False
    | _ -> throwError "list?" args "one argument"

let internal vecFn args =
    match args with
    | [ MalList (_, Vector) as vec ] -> vec
    | [ MalList (items, _) ] -> MalList(items, List)
    | _ -> throwError "vec" args "one arguments of type 'list' or 'vector'"

// ** map

let internal assocFn args =
    match args with
    | MalMap currentMap :: newItems ->
        let newMap =
            match listToMap newItems with
            | MalMap m -> m
            | _ -> noWayIAmHere ()
        newMap
        |> Map.fold (fun c key value -> Map.add key value c) currentMap
        |> MalMap
    | _ -> throwError "assoc" args "at least one argument of type 'map'"

let internal dissocFn args =
    match args with
    | MalMap currentMap :: deletedItems ->
        deletedItems
        |> List.fold
            (fun c keyMal ->
                match keyMal with
                | Str s -> Map.remove s c
                | _ -> failwith $"all keys passed to 'dissoc' must be of type 'Str', but got {(printStr keyMal)}")
            currentMap
        |> MalMap
    | _ -> throwError "dissoc" args "at least one argument of type 'map'"

let internal getFn args =
    match args with
    | [ Nil ] -> Nil
    | [ MalMap map; Str s ] ->
        match Map.tryFind s map with
        | Some value -> value
        | None -> Nil
    | _ ->
        throwError "get" args "two arguments where the first one must be of type 'map' and the second of type 'string'"

let internal containsFn args =
    match args with
    | [ MalMap map; Str s ] ->
        match Map.tryFind s map with
        | Some _ -> True
        | None -> False
    | _ ->
        throwError
            "contains?"
            args
            "two arguments where the first one must be of type 'map' and the second of type 'string'"


let internal keysFn args =
    match args with
    | [ MalMap map ] ->
        MalList(
            (map
             |> Map.toList
             |> List.map (fun (key, _) -> Str(key))),
            List
        )
    | _ -> throwError "keys" args "one argument of type 'map'"


let internal valsFn args =
    match args with
    | [ MalMap map ] ->
        MalList(
            (map
             |> Map.toList
             |> List.map (fun (_, value) -> value)),
            List
        )
    | _ -> throwError "vals" args "one argument of type 'map'"


let internal hashMapFn args = listToMap args

// ** utils

let internal equalsFn args =
    match args with
    | [ mal1; mal2 ] -> if malEquals mal1 mal2 then True else False
    | _ -> throwError "=" args "two arguments"

let internal strFn args = Str(joinWithSeparator args None)

let internal printLnFn args =
    let str = joinWithSeparator args None
    System.Console.WriteLine(str)
    Nil

let internal isVectorFn args =
    match args with
    | [ MalList (_, Vector) ] -> True
    | _ -> False

let internal isSequentialFn args =
    match args with
    | [ MalList (_) ] -> True
    | _ -> False

let internal isMapFn args =
    match args with
    | [ MalMap (_) ] -> True
    | _ -> False

let internal isFnFn args =
    match args with
    | [ Fn (_, false) ] -> True
    | _ -> False

let internal isMacroFn args =
    match args with
    | [ Fn (_, true) ] -> True
    | _ -> False

let internal isTrueFn args =
    match args with
    | [ True ] -> True
    | _ -> False

let internal isFalseFn args =
    match args with
    | [ False ] -> True
    | _ -> False

let internal isNilFn args =
    match args with
    | [ Nil ] -> True
    | _ -> False

let internal isSymbolFn args =
    match args with
    | [ Symbol _ ] -> True
    | _ -> False

let internal isStrFn args =
    match args with
    | [ Str _ ] -> True
    | _ -> False

let internal isNumberFn args =
    match args with
    | [ Number _ ] -> True
    | _ -> throwError "read-string" args "one argument of type 'string'"


// ** interpreter

let internal readStringFn args =
    match args with
    | [ Str (s) ] ->
        match (readText s) with
        | Some mal -> mal
        | None -> Nil
    | _ -> throwError "read-string" args "one argument of type 'string'"

let internal createEval env args =
    match args with
    | [ mal ] -> Eval.eval mal env
    | _ -> throwError "eval" args "one argument"


let ns =
    Map [ ("+", Fn((fun args -> executeArithmeticFn args (+)), false))
          ("-", Fn((fun args -> executeArithmeticFn args (-)), false))
          ("*", Fn((fun args -> executeArithmeticFn args (*)), false))
          ("/", Fn((fun args -> executeArithmeticFn args (/)), false))

          ("<", Fn((fun args -> executeComparisonFn args (<)), false))
          ("<=", Fn((fun args -> executeComparisonFn args (<=)), false))
          (">", Fn((fun args -> executeComparisonFn args (>)), false))
          (">=", Fn((fun args -> executeComparisonFn args (>=)), false))

          ("list", Fn(listFn, false))
          ("vector", Fn(vectorFn, false))

          ("cons", Fn(constFn, false))
          ("concat", Fn(concatFn, false))
          ("conj", Fn(conjFn, false))
          ("count", Fn(countFn, false))
          ("first", Fn(firstFn, false))
          ("rest", Fn(restFn, false))

          ("nth", Fn(nthFn, false))
          ("empty?", Fn(isEmptyFn, false))
          ("list?", Fn(isListFn, false))
          ("vec", Fn(vectorFn, false))

          ("assoc", Fn(assocFn, false))
          ("dissoc", Fn(dissocFn, false))
          ("get", Fn(getFn, false))
          ("contains?", Fn(containsFn, false))
          ("keys", Fn(keysFn, false))
          ("vals", Fn(valsFn, false))
          ("hash-map", Fn(hashMapFn, false))

          ("str", Fn(strFn, false))
          ("println", Fn(printLnFn, false))
          ("=", Fn(equalsFn, false))
          ("nil?", Fn(isNilFn, false))
          ("true?", Fn(isTrueFn, false))
          ("false?", Fn(isFalseFn, false))
          ("symbol?", Fn(isSymbolFn, false))
          ("number?", Fn(isNumberFn, false))
          ("string?", Fn(isStrFn, false))
          ("fn?", Fn(isFnFn, false))
          ("sequential?", Fn(isSequentialFn, false))
          ("map?", Fn(isMapFn, false))
          ("vector?", Fn(isVectorFn, false))
          ("read-string", Fn(readStringFn, false)) ]
