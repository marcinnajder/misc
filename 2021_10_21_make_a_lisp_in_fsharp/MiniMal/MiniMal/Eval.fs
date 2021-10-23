module Eval

open Types
open Printer
open Env

let rec eval mal (env: Env) =
    match mal with
    | MalList (items, ListType.List) ->
        match items with
        | [] -> mal
        | Symbol ("def!") :: tail -> applyDef tail env
        | Symbol ("let*") :: tail -> applyLet tail env
        | _ ->
            match (evalAst mal env) with
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

// (let* (a 1) a + 2)
and internal applyLet items env =
    match items with
    | [ MalList (bindings, _); expr ] ->
        applyBindings bindings (Env(Map [], Some env))
        |> eval expr
    | _ ->
        failwith
            $"'def!' requires 2 arguments where the first argument must be 'symbol', but got '{joinWithSeparator items None}'"

// ( a 1 b 3 )
and internal applyBindings items env =
    match items with
    | [] -> env
    | Symbol (key) :: value :: restItems ->
        eval value env
        |> env.Set key
        |> (fun _ -> applyBindings restItems env)
    | _ ->
        failwith
            $"Bindings argument in let* should contain an even number of elements where even element must be a 'symbol', but got '{joinWithSeparator items None}'"
