
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
            => mal switch
            {
                not List { ListType: ListType.List } => EvalAst(mal, env),
                List { Items: null } => mal,
                List { Items: (Symbol { Name: "def!" }, var Tail) } => ApplyDef(Tail, env),
                List { Items: (Symbol { Name: "let*" }, var Tail) } => ApplyLet(Tail, env),
                List { Items: (Symbol { Name: "do" }, var Tail) } => ApplyDo(Tail, env),
                List { Items: (Symbol { Name: "if" }, var Tail) } => ApplyIf(Tail, env),
                List { Items: (Symbol { Name: "fn*" }, var Tail) } => ApplyFn(Tail, env),
                List list => EvalAst(list, env) switch
                {
                    List { Items: (Fn fn, var Args) } => fn.Value(Args),
                    List { Items: { Head: var first } } => throw new Exception($"First element in a list should be 'fn' but it is '{first.PrintStr()}'"),
                    var m => throw new Exception($"Element type should be a 'list' but it is '{m}'")
                },
            };

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
    }
}