
using PowerFP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Mal.Printer;
using static Mal.Types;

namespace Mal
{
    public static class Evaluation
    {
        public static readonly Map<Symbol, MalType> Env = MapM.MapFrom(
            (new Symbol("+", NilV), new Fn(args => ExecuteArithmeticOperation(args, (a, b) => a + b), NilV) as MalType),
            (new Symbol("-", NilV), new Fn(args => ExecuteArithmeticOperation(args, (a, b) => a - b), NilV)),
            (new Symbol("*", NilV), new Fn(args => ExecuteArithmeticOperation(args, (a, b) => a * b), NilV)),
            (new Symbol("/", NilV), new Fn(args => ExecuteArithmeticOperation(args, (a, b) => a / b), NilV))
        );

        internal static MalType ExecuteArithmeticOperation(LList<MalType>? args, Func<double, double, double> operation)
                => args switch
                {
                    { Tail: { } } => args.Aggregate((totalMal, nextMal) => (totalMal, nextMal) switch
                        {
                            (Number(var total), Number(var next)) => new Number(operation(total, next)),
                            _ => throw new Exception($"All arguments of arithmetic operations must be of the 'Number' type, but got an argument '{(totalMal is not Number ? totalMal : nextMal)}' in {args.JoinMalTypes(",")}")
                        }),
                    _ => throw new Exception($"Arithmetic operation required at least two arguments, but got '{args.Count()}', arguments: {args.JoinMalTypes(",")}"),
                };


        public static MalType Eval(MalType mal, Map<Symbol, MalType> env)
            => mal switch
            {
                not List { ListType: ListType.List } => EvalAst(mal, env),
                List { Items: null } => mal,
                List list => EvalAst(list, env) switch
                {
                    List { Items: (Fn fn, var Args) } => fn.Value(Args),
                    List { Items: { Head: var first } } => throw new Exception($"First element in a list should be 'fn' but it is '{first}'"),
                    var m => throw new Exception($"Element type should be a 'list' but it is '{m}'")
                },
            };

        internal static MalType EvalAst(MalType mal, Map<Symbol, MalType> env) =>
            mal switch
            {
                Symbol symbol when env.TryFind(symbol, out var foundMal) => foundMal,
                Symbol { Name: var name } => throw new Exception($"Cannot find symbol '{name}' in Environment"),
                List list => list with { Items = list.Items.Select(m => Eval(m, env)) },
                Map map => map with { Value = new(map.Value.Items.Select(kv => (kv.Key, Eval(kv.Value, env)))) },
                _ => mal
            };

    }
}