module Eval

open Types
open Printer
open Env

let rec eval mal (env: Env) =
    match mal with
    | MalList (items, ListType.List) ->
        match (evalAst mal env) with
        | MalList ([], _) -> mal
        | MalList (Fn (fn, _) :: args, _) -> fn args
        | MalList (first :: _, _) -> failwith $"First element in a list should be 'fn' but it is '{printStr first}'"
        | m -> failwith $"Element type should be a 'list' but it is '{m}'"
    | _ -> evalAst mal env

and evalAst mal (env: Env) =
    match mal with
    | Symbol (name) -> env.Get name
    | MalList (items, listType) -> MalList(items |> List.map (fun m -> eval m env), listType)
    | MalMap (map) ->
        MalMap(
            map
            |> Map.toSeq
            |> Seq.map (fun (key, value) -> key, eval value env)
            |> Map.ofSeq
        )
    | m -> m

// (def! a 1)
and internal applyDef items env =
    match items with
    | [ Symbol (varName); varValue ] -> eval varValue env |> env.Set varName
    | _ ->
        failwith
            $"'def!' requires 2 arguments where the first argument must be 'symbol', but got '{joinWithSeparator items None}'"
