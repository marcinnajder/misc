
using PowerFP;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Mal.Printer;
using static Mal.Types;

namespace Mal
{
    public static class Core
    {
        private class MalFunctionAttribute : Attribute
        {
            public string Name { get; set; }
            public MalFunctionAttribute(string name) => Name = name;
        }

        private static Map<Symbol, MalType>? ns = null;
        // property (instead of field because) order of static members initialization matters
        public static Map<Symbol, MalType> Ns => ns ?? (ns = MapM.MapFrom(
              GetMalFunctions().Concat(LListM.LListFrom(
                  (new Symbol("+", NilV), new Fn(args => ExecuteArithmeticFn(args, (a, b) => a + b), NilV) as MalType),
                  (new Symbol("-", NilV), new Fn(args => ExecuteArithmeticFn(args, (a, b) => a - b), NilV)),
                  (new Symbol("*", NilV), new Fn(args => ExecuteArithmeticFn(args, (a, b) => a * b), NilV)),
                  (new Symbol("/", NilV), new Fn(args => ExecuteArithmeticFn(args, (a, b) => a / b), NilV)),

                  (new Symbol("<", NilV), new Fn(args => ExecuteComparisonFn(args, (a, b) => a < b), NilV)),
                  (new Symbol("<=", NilV), new Fn(args => ExecuteComparisonFn(args, (a, b) => a <= b), NilV)),
                  (new Symbol(">", NilV), new Fn(args => ExecuteComparisonFn(args, (a, b) => a > b), NilV)),
                  (new Symbol(">=", NilV), new Fn(args => ExecuteComparisonFn(args, (a, b) => a >= b), NilV))
              ))));

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

        [MalFunction("list")]
        internal static FnDelegate ListFn = args => new List(args, ListType.List, NilV);

        [MalFunction("list?")]
        internal static FnDelegate IsListFn = args
            => args switch
            {
                (List { ListType: ListType.List }, null) => TrueV,
                (_, null) => FalseV,
                _ => ThrowError(args, "one argument"),
            };

        [MalFunction("empty?")]
        internal static FnDelegate IsEmptyFn = args
           => args switch
           {
               (List { Items: null }, null) => TrueV,
               (List { }, null) => FalseV,
               _ => ThrowError(args, "one argument of type 'list' or 'vector'")
           };

        [MalFunction("count")]
        internal static FnDelegate CountFn = args
            => args switch
            {
                (Nil, null) => new Number(0),
                (List { Items: var items }, null) => new Number(items.Count()),
                _ => ThrowError(args, "one argument of type 'list' or 'vector' or 'nil'")
            };

        [MalFunction("=")]
        internal static FnDelegate EqualsFn = args
            => args switch
            {
                (var Mal1, (var Mal2, null)) => Types.MalEqual(Mal1, Mal2) ? TrueV : FalseV,
                _ => ThrowError(args, "two arguments"),

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

        [MalFunction("pr-str")]
        internal static FnDelegate PrStrFn = args => MalsToStr(args, " ", true);

        [MalFunction("str")]
        internal static FnDelegate StrFn = args => MalsToStr(args, "", false);

        [MalFunction("prn")]
        internal static FnDelegate PrnFn = args => MalsToStr(args, " ", true).Pipe(PrintLineFn);

        [MalFunction("println")]
        internal static FnDelegate PrintLnFn = args => MalsToStr(args, " ", false).Pipe(PrintLineFn);

        [MalFunction("read-string")]
        internal static FnDelegate ReadStringFn = args
            => args switch
            {
                (Str { Value: var strValue }, null) => Reader.ReadText(strValue) ?? NilV,
                _ => ThrowError(args, "one argument of type 'string'")
            };

        [MalFunction("slurp")]
        internal static FnDelegate Slurp = args
            => args switch
            {
                (Str { Value: var filePath }, null) => new Str(File.ReadAllText(filePath)),
                _ => ThrowError(args, "one argument of type 'string'")
            };





        [MalFunction("atom")]
        internal static FnDelegate AtomFn = args
            => args switch
            {
                (var Mal, null) => new Atom(Mal),
                _ => ThrowError(args, "one argument")
            };

        [MalFunction("atom?")]
        internal static FnDelegate IsAtomFn = args
            => args switch
            {
                (Atom, null) => TrueV,
                _ => FalseV
            };

        [MalFunction("deref")]
        internal static FnDelegate DerefFn = args
            => args switch
            {
                (Atom { Mal: var Mal }, null) => Mal,
                _ => ThrowError(args, "one argument of type 'atom'")
            };

        [MalFunction("reset!")]
        internal static FnDelegate ResetFn = args
            => args switch
            {
                (Atom atom, (var NewMal, null)) => atom.Mal = NewMal,
                _ => ThrowError(args, "two arguments of type 'atom' and any other type")
            };

        [MalFunction("swap!")]
        internal static FnDelegate SwapFn = args
            => args switch
            {
                (Atom { Mal: var Mal } atom, (Fn { Value: var Func }, var RestArgs)) =>
                    Func(new(Mal, RestArgs)).Pipe(result => atom.Mal = result),
                _ => ThrowError(args, "two or more arguments of following types 'atom', 'fn', other optional 'fn'")
            };



        [MalFunction("cons")]
        internal static FnDelegate ConsFn = args
            => args switch
            {
                (var FirstArg, (List { Items: var Items } ListArg, null)) => new List(new(FirstArg, Items), ListType.List, NilV),
                //(var FirstArg, (List { Items: var Items } ListArg, null)) => ListArg with { Items = new(FirstArg, Items) },
                _ => ThrowError(args, "two arguments where the second one must be of type 'list'")
            };

        [MalFunction("concat")]
        internal static FnDelegate ConcatFn = ConcatImplFn;
        [MalFunction("concat")]
        private static MalType ConcatImplFn(LList<MalType>? args)
            => args switch
            {
                null => new List(null, ListType.List, NilV),
                (List { Items: var Items }, var RestArguments) =>
                    new List(Items.Concat((ConcatImplFn(RestArguments) as List)!.Items), ListType.List, NilV),
                _ => ThrowError(args, "all arguments to be of type 'list'")
            };


        [MalFunction("vec")]
        internal static FnDelegate VecFn = args
            => args switch
            {
                (List { ListType: ListType.Vector } vector, null) => vector,
                (List list, null) => new List(list.Items, ListType.Vector, list.Meta),
                _ => ThrowError(args, "one arguments of type 'list' or 'vector'")
            };


        [MalFunction("nth")]
        internal static FnDelegate NthFn = args
            => args switch
            {
                (List { Items: var Items }, (Number { Value: var Index }, null)) => Items.ElementAt((int)Index),
                _ => ThrowError(args, "two arguments where the first one is of type 'list' and the second of type 'number'")
            };

        [MalFunction("first")]
        internal static FnDelegate FirstFn = args
            => args switch
            {
                (Nil, null) => NilV,
                (List { Items: var Items }, null) => Items == null ? NilV : Items.Head,
                _ => ThrowError(args, "one argument of type 'list' or 'vector'")
            };

        [MalFunction("rest")]
        internal static FnDelegate RestFn = args
            => args switch
            {
                (Nil, null) => new List(null, ListType.List, NilV),
                (List { Items: var Items }, null) => new List(Items?.Tail, ListType.List, NilV),
                _ => ThrowError(args, "one argument of type 'list' or 'vector'")
            };

        // private

        // 'Binding' is a property instead of a field because it is used during initialization of other static properties or fields
        private static BindingFlags Binding => BindingFlags.Static | BindingFlags.NonPublic;

        private static MalType ThrowError(LList<MalType>? args, string message, [CallerMemberName] string malFunctionName = "")
        {
            var methodInfo = typeof(Core).GetField(malFunctionName, Binding) as MemberInfo
                ?? typeof(Core).GetMethod(malFunctionName, Binding);
            var malFunction = (MalFunctionAttribute?)Attribute.GetCustomAttribute(methodInfo!, typeof(MalFunctionAttribute));
            throw new Exception($"'{malFunction!.Name}' function requires {message}, but got {args.JoinMalTypes(",")}");
        }

        internal static LList<(Symbol Name, MalType Fn)>? GetMalFunctions()
            =>
            (
                from field in typeof(Core).GetFields(Binding)
                where field.FieldType == typeof(FnDelegate)
                let attribute = Attribute.GetCustomAttribute(field, typeof(MalFunctionAttribute)) as MalFunctionAttribute
                where attribute != null
                let fn = field.GetValue(null) as FnDelegate
                select (new Symbol(attribute.Name, NilV), new Fn(fn, NilV) as MalType)
            ).ToLList();


    }
}