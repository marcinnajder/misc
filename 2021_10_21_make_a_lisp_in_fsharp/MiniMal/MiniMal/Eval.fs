module Eval

open Types
open Printer
open Env

let rec eval mal (env: Env) =
    let mal = macroExpand mal env
    match mal with
    | MalList (items, ListType.List) ->
        match items with
        | [] -> mal
        | Symbol ("def!") :: tail -> applyDef tail env
        | Symbol ("let*") :: tail -> applyLet tail env
        | Symbol ("do") :: tail -> applyDo tail env
        | Symbol ("if") :: tail -> applyIf tail env
        | Symbol ("fn*") :: tail -> applyFn tail env

        | Symbol ("quote") :: tail -> applyQuote tail env
        | Symbol ("quasiquoteexpand") :: tail -> applyQuasiquoteExpand tail env
        | Symbol ("quasiquote") :: tail -> applyQuasiquote tail env

        | Symbol ("macroexpand") :: tail -> applyMacroExpand tail env
        | Symbol ("defmacro!") :: tail -> applyDefMacro tail env

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


// (fn* (...) ...)
and internal applyFn items env =
    match items with
    | [ MalList (argNames, List); body ] ->
        Fn((fun argValues -> eval body (Env(Map(bindFunctionArguments argNames argValues), Some env))), false)
    | _ ->
        failwith
            $"'fn*' requires 2 arguments where the first one must be a 'list', but got '{joinWithSeparator items None}'"

and internal bindFunctionArguments names values =
    match (names, values) with
    | [], _ -> []
    | Symbol ("&") :: Symbol (argName) :: _, argValues -> [ argName, MalList(argValues, List) ]
    | Symbol (argName) :: restNames, argValue :: restValues ->
        (argName, argValue)
        :: bindFunctionArguments restNames restValues
    | _ ->
        failwith
            $"Cannot bind function arguments, names: '{joinWithSeparator names None}' , values: '{joinWithSeparator values None}'"


// (quote ...)
and internal applyQuote items env =
    match items with
    | [ first ] -> first
    | _ -> failwith $"'quote' requires one argument, but got '{joinWithSeparator items None}'"

// (quasiquote (...))
and internal applyQuasiquote items env =
    match items with
    | [ first ] ->
        let t = transformQuasiquote first
        eval t env
    | _ -> failwith $"'quasiquote' requires one argument, but got '{joinWithSeparator items None}'"

// (quasiquoteexpand (...))
and internal applyQuasiquoteExpand items env =
    match items with
    | [ first ] -> transformQuasiquote first
    | _ -> failwith $"'quasiquoteexpand' requires one argument, but got '{joinWithSeparator items None}'"

and internal transformQuasiquote mal =
    match mal with
    | MalList ([ Symbol ("unquote"); unquotedMal ], _) -> unquotedMal
    | MalList ([], _) -> MalList([], List)
    | MalList (head :: tail, _) ->
        let r =
            transformQuasiquote (MalList(tail, List))
        match head with
        | MalList ([ Symbol ("splice-unquote"); unquotedMal ], _) -> MalList([ Symbol("concat"); unquotedMal; r ], List)
        | _ -> MalList([ Symbol("cons"); transformQuasiquote head; r ], List)
    | MalMap (_)
    | Symbol (_) -> MalList([ Symbol("quote"); mal ], List)
    | _ -> mal

// (defmacro! ...)
and internal applyDefMacro items env =
    match items with
    | [ Symbol (varName); MalList (Symbol ("fn*") :: _, _) as varValue ] ->
        match (eval varValue env) with
        | Fn (func, _) as fn -> env.Set varName (Fn(func, true))
        | _ -> noWayIAmHere ()
    | _ ->
        failwith
            $"'defmacro!' requires 2 arguments where the first argument must be of type 'symbol' and the second of type 'fn', but got '{joinWithSeparator items None}'"

and internal isMacroCall mal (env: Env) =
    match mal with
    | MalList ([ Symbol (key) ], _) ->
        match (env.Get key) with
        | Fn (_, true) -> true
        | _ -> false
    | _ -> false


and internal macroExpand mal (env: Env) =
    match mal with
    | MalList ((Symbol (funcName)) :: args, _) ->
        let funcBody =
            try
                env.Get funcName
            with
            | _ -> Nil
        match funcBody with
        | Fn (funcCall, true) -> macroExpand (funcCall (args)) env
        | _ -> mal
    | _ -> mal

// (macroexpand ...)
and internal applyMacroExpand items env =
    match items with
    | [ mal ] -> macroExpand mal env
    | _ -> failwith $"'macroexpand' requires 1 argument, but got '{joinWithSeparator items None}'"
