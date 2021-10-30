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
                match (totalMal, nextMal) with
                | (Number (total), Number (next)) -> Number(operation total next)
                | _ ->
                    let m =
                        match totalMal with
                        | Number (_) -> nextMal
                        | _ -> totalMal
                    failwith
                        $"""All arguments of arithmetic operations must be of the 'Number' type, but got an argument '{(printStr m)}' in {joinWithSeparator args (Some(","))}""")
            args
    | _ ->
        failwith
            $"""Arithmetic operation required at least two arguments, but got '{List.length args}', arguments: {joinWithSeparator args (Some(","))}"""


let internal executeComparisonFn args comparison =
    match args with
    | [ Number (value1); Number (value2) ] -> if comparison value1 value2 then True else False
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
        match (concatFn restArgs) with
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
    | [ MalList (items, _); Number (n) ] -> List.item (int n) items
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
    | MalMap (currentMap) :: newItems ->
        let newMap =
            match (listToMap newItems) with
            | MalMap (m) -> m
            | _ -> noWayIAmHere ()
        newMap
        |> Map.fold (fun c key value -> Map.add key value c) currentMap
        |> MalMap
    | _ -> throwError "assoc" args "at least one argument of type 'map'"

let internal dissocFn args =
    match args with
    | MalMap (currentMap) :: deletedItems ->
        deletedItems
        |> List.fold
            (fun c keyMal ->
                match keyMal with
                | Str (s) -> Map.remove s c
                | _ -> failwith $"all keys passed to 'dissoc' must be of type 'Str', but got {(printStr keyMal)}")
            currentMap
        |> MalMap
    | _ -> throwError "dissoc" args "at least one argument of type 'map'"

let internal getFn args =
    match args with
    | [ Nil ] -> Nil
    | [ MalMap (map); Str (s) ] ->
        match (Map.tryFind s map) with
        | Some (value) -> value
        | None -> Nil
    | _ ->
        throwError "get" args "two arguments where the first one must be of type 'map' and the second of type 'string'"

let internal containsFn args =
    match args with
    | [ MalMap (map); Str (s) ] ->
        match (Map.tryFind s map) with
        | Some (_) -> True
        | None -> False
    | _ ->
        throwError
            "contains?"
            args
            "two arguments where the first one must be of type 'map' and the second of type 'string'"


let internal keysFn args =
    match args with
    | [ MalMap (map) ] ->
        MalList(
            (map
             |> Map.toList
             |> List.map (fun (key, _) -> Str(key))),
            List
        )
    | _ -> throwError "keys" args "one argument of type 'map'"


let internal valsFn args =
    match args with
    | [ MalMap (map) ] ->
        MalList(
            (map
             |> Map.toList
             |> List.map (fun (_, value) -> value)),
            List
        )
    | _ -> throwError "vals" args "one argument of type 'map'"


let internal hashMapFn args = listToMap args


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

          ("const", Fn(constFn, false))
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

           ]

// ** utils


//         // ** utils

//         internal static FnDelegate EqualsFn = args
//             => args switch
//             {
//                 (var Mal1, (var Mal2, null)) => Types.MalEqual(Mal1, Mal2) ? TrueV : FalseV,
//                 _ => ThrowError("=", args, "two arguments"),

//             };

//         internal static FnDelegate StrFn = args => new Str(args.JoinWithSeparator());

//         internal static FnDelegate PrintLnFn = args => args.JoinWithSeparator().Pipe(text =>
//         {
//             Console.WriteLine(text);
//             return NilV;
//         });

//         private static FnDelegate IsOfType<T>() => args => args is (T, null) ? TrueV : FalseV;

//         internal static FnDelegate IsVectorFn = args => args is (List { ListType: ListType.Vector }, null) ? TrueV : FalseV;

//         internal static FnDelegate IsSequentialFn = args => args is (List, null) ? TrueV : FalseV;

//         internal static FnDelegate IsFnFn = args => args is (Fn { IsMacro: false }, null) ? TrueV : FalseV;

//         internal static FnDelegate IsMacroFn = args => args is (Fn { IsMacro: true }, null) ? TrueV : FalseV;

//         // ** interpreter

//         internal static FnDelegate ReadStringFn = args
//             => args switch
//             {
//                 (Str { Value: var strValue }, null) => Reader.ReadText(strValue) ?? NilV,
//                 _ => ThrowError("read-string", args, "one argument of type 'string'")
//             };

//         internal static FnDelegate CreateEval(EnvM.Env env) =>
//             args => args switch
//             {
//                 (var Mal, null) => EvalM.Eval(Mal, env),
//                 _ => ThrowError("eval", args, "one argument")
//             };


//         // private

//         private static MalType ThrowError(string functionName, LList<MalType>? args, string message)
//         {
//             throw new Exception($"'{functionName}' function requires {message}, but got {args.JoinWithSeparator(",")}");
//         }
//     }
// }
