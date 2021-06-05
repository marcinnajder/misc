
using PowerFP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Mal.Printer;
using static Mal.Types;

namespace Mal
{
    public static class EnvM
    {
        public static readonly Env DefaultEnv = new Env(MapM.MapFrom(
            (new Symbol("+", NilV), new Fn(args => ExecuteArithmeticOperation(args, (a, b) => a + b), NilV) as MalType),
            (new Symbol("-", NilV), new Fn(args => ExecuteArithmeticOperation(args, (a, b) => a - b), NilV)),
            (new Symbol("*", NilV), new Fn(args => ExecuteArithmeticOperation(args, (a, b) => a * b), NilV)),
            (new Symbol("/", NilV), new Fn(args => ExecuteArithmeticOperation(args, (a, b) => a / b), NilV))
        ), null);

        public record Env
        {
            public Map<Symbol, MalType> Data { get; internal set; }
            public Env? Outer { get; init; }

            public Env(Map<Symbol, MalType> data, Env? outer)
            {
                Data = data;
                Outer = outer;
            }
        }

        public static MalType Set(this Env env, Symbol key, MalType value)
        {
            env.Data = env.Data.Add(key, value);
            return value;
        }


        public static Env? Find(this Env env, Symbol key)
            => FindEnvAndValue(env, key) switch
            {
                null => null,
                (var Env, _) => Env
            };

        public static MalType Get(this Env env, Symbol key)
            => FindEnvAndValue(env, key) switch
            {
                null => throw new Exception($"Cannot find symbol '{key}' in Env"),
                (_, var Value) => Value
            };

        private static (Env, MalType)? FindEnvAndValue(this Env env, Symbol key)
            => env.Data.TryFind(key) switch
            {
                (true, var value) => (env, value!),
                _ => env.Outer == null ? null : FindEnvAndValue(env.Outer, key)
            };

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
    }
}