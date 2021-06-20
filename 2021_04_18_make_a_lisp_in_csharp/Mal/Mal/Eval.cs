
using PowerFP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Mal.Printer;
using static Mal.Types;
using static Mal.EnvM;

namespace Mal
{
    public static class EvalM
    {
        public static MalType Eval(MalType mal, Env env)
            => MacroExpand(mal, env).Pipe(expandMal => expandMal switch
            {
                not List { ListType: ListType.List } => EvalAst(expandMal, env),
                List { Items: null } => expandMal,
                List { Items: (Symbol { Name: "def!" }, var Tail) } => ApplyDef(Tail, env),
                List { Items: (Symbol { Name: "let*" }, var Tail) } => ApplyLet(Tail, env),
                List { Items: (Symbol { Name: "do" }, var Tail) } => ApplyDo(Tail, env),
                List { Items: (Symbol { Name: "if" }, var Tail) } => ApplyIf(Tail, env),
                List { Items: (Symbol { Name: "fn*" }, var Tail) } => ApplyFn(Tail, env),

                List { Items: (Symbol { Name: "quote" }, var Tail) } => ApplyQuote(Tail, env),
                List { Items: (Symbol { Name: "quasiquoteexpand" }, var Tail) } => ApplyQuasiquoteExpand(Tail, env),
                List { Items: (Symbol { Name: "quasiquote" }, var Tail) } => ApplyQuasiquote(Tail, env),

                List { Items: (Symbol { Name: "defmacro!" }, var Tail) } => ApplyDefMacro(Tail, env),
                List { Items: (Symbol { Name: "macroexpand" }, var Tail) } => ApplyMacroExpand(Tail, env),

                List list => EvalAst(list, env) switch
                {
                    List { Items: (Fn fn, var Args) } => fn.Value(Args),
                    List { Items: { Head: var first } } => throw new Exception($"First element in a list should be 'fn' but it is '{first.PrintStr()}'"),
                    var m => throw new Exception($"Element type should be a 'list' but it is '{m}'")
                },
            });

        internal static MalType EvalAst(MalType mal, Env env) =>
            mal switch
            {
                Symbol symbol => env.Get(symbol),
                List list => list with { Items = list.Items.Select(m => Eval(m, env)) },
                Map map => map with { Value = MapM.MapFrom(map.Value.EntriesL().Select(kv => (kv.Key, Eval(kv.Value, env)))) },
                _ => mal
            };

        // (let* (a 1) a + 2) 
        internal static MalType ApplyLet(LList<MalType>? items, Env env)
            => items switch
            {
                (List { Items: var Bindings }, (var Expr, null)) =>
                    ApplyBindings(Bindings, new Env(MapM.Empty<Symbol, MalType>(), env)).Pipe(newEnv => Eval(Expr, newEnv)),
                _ => throw new Exception($"'let*' requires 2 arguments where the first is a list of bindings and the second is a Mal expression, but got '{items.JoinMalTypes()}'")
            };

        // ( a 1 b 3 ) 
        internal static Env ApplyBindings(LList<MalType>? items, Env env)
            => items switch
            {
                null => env,
                (Symbol Key, (var Value, var RestItems)) =>
                    Eval(Value, env).Pipe(value => { env.Set(Key, value); return ApplyBindings(RestItems, env); }),
                _ => throw new Exception($"Bindings argument in let* should contain an even number of elements where even element must be a 'symbol', but got '{items.JoinMalTypes()}'")
            };

        // (def! a 1) 
        internal static MalType ApplyDef(LList<MalType>? items, Env env)
            => items switch
            {
                (Symbol VarName, (var VarValue, null)) =>
                    Eval(VarValue, env).Pipe(varValue => env.Set(VarName, varValue)),
                _ => throw new Exception($"'def!' requires 2 arguments where the first argument must be 'symbol', but got '{items.JoinMalTypes()}'")
            };

        // (do (...) (...) (....) ) 
        internal static MalType ApplyDo(LList<MalType>? items, Env env)
            => items switch
            {
                (var Head, var Tail) => Eval(Head, env).Pipe(mal => Tail == null ? mal : ApplyDo(Tail, env)),
                null => throw new Exception($"'do' requires at least one argument")
            };

        // (if (...) (...) (...) )
        internal static MalType ApplyIf(LList<MalType>? items, Env env)
            => items switch
            {
                (var If, (var Then, (null or (_, null))) ThenElse) => Eval(If, env).Pipe(@if => @if switch
               {
                   not (Nil or False) => Eval(Then, env),
                   _ => ThenElse.Tail == null ? NilV : Eval(ThenElse.Tail.Head, env)
               }),
                _ => throw new Exception($"'if' requires 2 or 3 arguments but got '{items.JoinMalTypes()}'")
            };


        // (fn* (...) ...)
        internal static MalType ApplyFn(LList<MalType>? items, Env env)
             => items switch
             {
                 (List { Items: var ArgNames }, (var Body, null)) =>
                    new Fn(argsValues => Eval(Body, new Env(MapM.MapFrom(BindFunctionArguments(ArgNames, argsValues).ToEnumerable()), env)), NilV),
                 _ => throw new Exception($"'fn*' requires 2 arguments where the first one must be a 'list', but got '{items.JoinMalTypes()}'")
             };


        internal static LList<(Symbol, MalType)>? BindFunctionArguments(LList<MalType>? names, LList<MalType>? values)
            => (names, values) switch
            {
                (null, _) => null,
                ((Symbol { Name: "&" }, (Symbol ArgName, null)), var ArgValue) => new((ArgName, new List(ArgValue, ListType.List, NilV)), null),
                ((Symbol ArgName, var RestArgNames), (var ArgValue, var RestArgValues)) =>
                    new((ArgName, ArgValue), BindFunctionArguments(RestArgNames, RestArgValues)),
                _ => throw new Exception($"Cannot bind function arguments, names: '{names.JoinMalTypes()}' , values: '{values.JoinMalTypes()}'")
            };

        // (quote ...)
        internal static MalType ApplyQuote(LList<MalType>? items, Env env)
             => items switch
             {
                 (var First, null) => First,
                 _ => throw new Exception($"'quote' requires one argument, but got '{items.JoinMalTypes()}'")
             };

        //(quasiquoteexpand (...))
        internal static MalType ApplyQuasiquoteExpand(LList<MalType>? items, Env env)
            => items switch
            {
                (var Mal, null) => TransformQuasiquote(Mal),
                _ => throw new Exception($"'quasiquoteexpand' requires one argument, but got '{items.JoinMalTypes()}'")
            };

        // (quasiquote (...))
        internal static MalType ApplyQuasiquote(LList<MalType>? items, Env env)
             => items switch
             {
                 (var Mal, null) => TransformQuasiquote(Mal).Pipe(mal => Eval(mal, env)),
                 _ => throw new Exception($"'quasiquote' requires one argument, but got '{items.JoinMalTypes()}'")
             };


        internal static MalType TransformQuasiquote(MalType mal)
            => mal switch
            {
                List { Items: (Symbol("unquote", _), (var UnquotedMal, null)) } => UnquotedMal,
                List { Items: null } => new List(null, ListType.List, NilV),
                List { Items: (var Head, var Tail) } => Head switch
                {
                    List { Items: (Symbol("splice-unquote", _), (var UnquotedMal, null)) } =>
                        MalListFrom(new Symbol("concat", NilV), UnquotedMal, TransformQuasiquote(new List(Tail, ListType.List, NilV))),
                    var Mal =>
                        MalListFrom(new Symbol("cons", NilV), TransformQuasiquote(Mal), TransformQuasiquote(new List(Tail, ListType.List, NilV))),
                },
                Map or Symbol => MalListFrom(new Symbol("quote", NilV), mal),
                _ => mal
            };

        // (defmacro! ...) 
        internal static MalType ApplyDefMacro(LList<MalType>? items, Env env)
            => items switch
            {
                (Symbol VarName, (List { Items: (Symbol { Name: "fn*" }, _) } VarValue, null)) =>
                    Eval(VarValue, env).Pipe(mal => (Fn)mal).Pipe(fn => env.Set(VarName, fn with { IsMacro = true })),
                _ => throw new Exception($"'defmacro!' requires 2 arguments where the first argument must be of type 'symbol' and the second of type 'fn', but got '{items.JoinMalTypes()}'")
            };


        internal static bool IsMacroCall(MalType mal, Env env)
            => mal switch
            {
                List { Items: (Symbol symbol, _) } => env.Get(symbol).Pipe(value => value is Fn { IsMacro: true }),
                _ => false
            };


        internal static MalType MacroExpand(MalType mal, Env env)
            => mal switch
            {
                List { Items: (Symbol FuncName, var Args) } => env
                    .Pipe(env_ => { try { return env_.Get(FuncName); } catch { return NilV; } })
                    .Pipe(funcBody => funcBody switch
                    {
                        Fn { Value: var FuncCall, IsMacro: true } => MacroExpand(FuncCall(Args), env),
                        _ => mal,
                    }),
                _ => mal
            };

        // (macroexpand ...) 
        internal static MalType ApplyMacroExpand(LList<MalType>? items, Env env)
            => items switch
            {
                (var Mal, null) => MacroExpand(Mal, env),
                _ => throw new Exception($"'macroexpand' requires 1 argument, but got '{items.JoinMalTypes()}'")
            };


    }
}