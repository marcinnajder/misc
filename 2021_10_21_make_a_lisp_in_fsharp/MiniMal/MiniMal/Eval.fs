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
        | Symbol ("do") :: tail -> applyDo tail env
        | Symbol ("if") :: tail -> applyIf tail env
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
            $"'let*' requires 2 arguments where the first is a list of bindings and the second is a Mal expression, but got '{joinWithSeparator items None}'"

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

// (do (...) (...) (....) )
and internal applyDo items env =
    match items with
    | [ head ] -> eval head env
    | head :: tail ->
        eval head env |> ignore
        applyDo tail env
    | [] -> failwith $"'do' requires at least one argument"

// (if (...) (...) (...) )
and internal applyIf items env =
    match items with
    | [ if_; then_; _ ]
    | [ if_; then_ ] ->
        match (eval if_ env) with
        | False
        | Nil ->
            match items with
            | [ _; _; else_ ] -> eval else_ env
            | _ -> Nil
        | _ -> eval then_ env
    | _ -> failwith $"'if' requires 2 or 3 arguments but got '{joinWithSeparator items None}'"


// // (if (...) (...) (...) )
// internal static MalType ApplyIf(LList<MalType>? items, Env env)
//     => items switch
//     {
//         (var If, (var Then, (null or (_, null))) ThenElse) => Eval(If, env).Pipe(@if => @if switch
//        {
//            not (Nil or False) => Eval(Then, env),
//            _ => ThenElse.Tail == null ? NilV : Eval(ThenElse.Tail.Head, env)
//        }),
//         _ => throw new Exception($"'if' requires 2 or 3 arguments but got '{items.JoinWithSeparator()}'")
//     };

// // (fn* (...) ...)
// internal static MalType ApplyFn(LList<MalType>? items, Env env)
//      => items switch
//      {
//          (List { Items: var ArgNames }, (var Body, null)) =>
//             new Fn(argsValues => Eval(Body, new Env(MapM.MapFrom(BindFunctionArguments(ArgNames, argsValues)), env))),
//          _ => throw new Exception($"'fn*' requires 2 arguments where the first one must be a 'list', but got '{items.JoinWithSeparator()}'")
//      };
