
using PowerFP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Mal.Printer;
using static Mal.Types;

namespace Mal
{
    public static class Core
    {
        public static readonly Map<Symbol, MalType> Ns = MapM.MapFrom(
            (new Symbol("+", NilV), new Fn(args => ExecuteArithmeticFn(args, (a, b) => a + b), NilV) as MalType),
            (new Symbol("-", NilV), new Fn(args => ExecuteArithmeticFn(args, (a, b) => a - b), NilV)),
            (new Symbol("*", NilV), new Fn(args => ExecuteArithmeticFn(args, (a, b) => a * b), NilV)),
            (new Symbol("/", NilV), new Fn(args => ExecuteArithmeticFn(args, (a, b) => a / b), NilV)),

            (new Symbol("list", NilV), new Fn(ListFn, NilV)),
            (new Symbol("list?", NilV), new Fn(IsListFn, NilV)),
            (new Symbol("empty?", NilV), new Fn(IsEmptyFn, NilV)),
            (new Symbol("count", NilV), new Fn(CountFn, NilV)),
            (new Symbol("=", NilV), new Fn(EqualsFn, NilV)),

            (new Symbol("<", NilV), new Fn(args => ExecuteComparisonFn(args, (a, b) => a < b), NilV)),
            (new Symbol("<=", NilV), new Fn(args => ExecuteComparisonFn(args, (a, b) => a <= b), NilV)),
            (new Symbol(">", NilV), new Fn(args => ExecuteComparisonFn(args, (a, b) => a > b), NilV)),
            (new Symbol(">=", NilV), new Fn(args => ExecuteComparisonFn(args, (a, b) => a >= b), NilV)),

            (new Symbol("pr-str", NilV), new Fn(PrStrFn, NilV)),
            (new Symbol("str", NilV), new Fn(StrFn, NilV)),
            (new Symbol("prn", NilV), new Fn(PrnFn, NilV)),
            (new Symbol("println", NilV), new Fn(PrintLnFn, NilV)),


            (new Symbol("list123", NilV), new Fn(ListFn, NilV))
            );

        internal static MalType ExecuteArithmeticFn(LList<MalType>? args, Func<double, double, double> operation)
            => args switch
            {
                { Tail: { } } => args.Aggregate((totalMal, nextMal) => (totalMal, nextMal) switch
                    {
                        (Number(var total), Number(var next)) => new Number(operation(total, next)),
                        _ => throw new Exception($"All arguments of arithmetic operations must be of the 'Number' type, but got an argument '{(totalMal is not Number ? totalMal : nextMal)}' in {args.JoinMalTypes(",")}")
                    }),
                _ => throw new Exception($"Arithmetic operation required at least two arguments, but got '{args.Count()}', arguments: {args.JoinMalTypes(",")}"),
            };

        internal static MalType ListFn(LList<MalType>? args) => new List(args, ListType.List, NilV);

        internal static MalType IsListFn(LList<MalType>? args)
            => args switch
            {
                (List { ListType: ListType.List }, null) => TrueV,
                (_, null) => FalseV,
                _ => throw new Exception($"'list?' function requires one argument, but got {args.JoinMalTypes(",")}"),
            };

        internal static MalType IsEmptyFn(LList<MalType>? args)
            => args switch
            {
                (List { Items: null }, null) => TrueV,
                (List { }, null) => FalseV,
                _ => throw new Exception($"'empty?' function requires one argument of type 'list' or 'vector', but got {args.JoinMalTypes(",")}"),
            };

        internal static MalType CountFn(LList<MalType>? args)
            => args switch
            {
                (Nil, null) => new Number(0),
                (List { Items: var items }, null) => new Number(items.Count()),
                _ => throw new Exception($"'count' function requires one argument of type 'list' or 'vector' or 'nil', but got {args.JoinMalTypes(",")}"),
            };

        internal static MalType EqualsFn(LList<MalType>? args)
            => args switch
            {
                (var Mal1, (var Mal2, null)) => Types.MalEqual(Mal1, Mal2) ? TrueV : FalseV,
                _ => throw new Exception($"'=' function requires two arguments, but got {args.JoinMalTypes(",")}"),
            };

        internal static MalType ExecuteComparisonFn(LList<MalType>? args, Func<double, double, bool> comparison)
            => args switch
            {
                (Number { Value: var value1 }, (Number { Value: var value2 }, null)) => comparison(value1, value2) ? TrueV : FalseV,
                _ => throw new Exception($"Number comparison operation requires two arguments of type 'Number', but got {args.JoinMalTypes(",")}"),
            };

        internal static Action<string> PrintLine = Console.WriteLine;

        private static MalType PrintLineFn(MalType mal)
            => mal switch
            {
                Str { Value: var argValue } => argValue.Pipe(arg => { PrintLine(arg); return NilV; }),
                _ => throw new Exception($"PrintLine requires one argument of type 'Str', but got {Printer.PrintStr(mal)}"),
            };

        private static MalType MalsToStr(LList<MalType>? args, string separator, bool printReadable)
            => new Str(string.Join(separator, args.ToEnumerable().Select(mal => Printer.PrintStr(mal, printReadable))));

        internal static MalType PrStrFn(LList<MalType>? args) => MalsToStr(args, " ", true);
        internal static MalType StrFn(LList<MalType>? args) => MalsToStr(args, "", false);
        internal static MalType PrnFn(LList<MalType>? args) => MalsToStr(args, " ", true).Pipe(PrintLineFn);
        internal static MalType PrintLnFn(LList<MalType>? args) => MalsToStr(args, " ", false).Pipe(PrintLineFn);
    }
}