
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

        [MalFunction("throw")]
        internal static FnDelegate ThrowFn = args
            => args switch
            {
                (var Mal, null) => throw new MalException(Mal),
                _ => ThrowError(args, "one argument")
            };

        [MalFunction("apply")]
        internal static FnDelegate ApplyFn = args
            => args switch
            {
                (Fn Fn, var Args) => Fn.Value(Args.SelectMany(arg => arg is List list ? list.Items : LListM.LListFrom(arg))),
                _ => ThrowError(args, "at least one argument where the first must be of type 'fn'")
            };

        [MalFunction("map")]
        internal static FnDelegate MapFn = args
            => args switch
            {
                (Fn Fn, (List { Items: var Items }, null)) => new List(Items.Select(mal => Fn.Value(MalLListFrom(mal))), ListType.List, NilV),
                _ => ThrowError(args, "two arguments where the first one must be of type 'fn' and the second of 'list' or 'vector'")
            };


        private static FnDelegate IsOfType<T>() => args => args is (T, null) ? TrueV : FalseV;

        [MalFunction("nil?")] internal static FnDelegate IsNilFn = IsOfType<Nil>();
        [MalFunction("true?")] internal static FnDelegate IsTrueFn = IsOfType<True>();
        [MalFunction("false?")] internal static FnDelegate IsFalseFn = IsOfType<False>();
        [MalFunction("symbol?")] internal static FnDelegate IsSymbplFn = IsOfType<Symbol>();
        [MalFunction("keyword?")] internal static FnDelegate IsKeywordFn = IsOfType<Keyword>();
        [MalFunction("map?")] internal static FnDelegate IsMapFn = IsOfType<Map>();
        [MalFunction("string?")] internal static FnDelegate IsStringFn = IsOfType<Str>();
        [MalFunction("number?")] internal static FnDelegate IsNmberFn = IsOfType<Number>();

        [MalFunction("vector?")] internal static FnDelegate IsVectorFn = args => args is (List { ListType: ListType.Vector }, null) ? TrueV : FalseV;
        [MalFunction("sequential?")] internal static FnDelegate IsSequentialFn = args => args is (List, null) ? TrueV : FalseV;

        [MalFunction("fn?")]
        internal static FnDelegate IsFnFn = args => args is (Fn { IsMacro: false }, null) ? TrueV : FalseV;
        [MalFunction("macro?")]
        internal static FnDelegate IsMacroFn = args => args is (Fn { IsMacro: true }, null) ? TrueV : FalseV;

        [MalFunction("symbol")]
        internal static FnDelegate SymbolFn = args
            => args switch
            {
                (Str { Value: var StrValue }, null) => new Symbol(StrValue, NilV),
                _ => ThrowError(args, "one argument of type 'string'")
            };


        [MalFunction("keyword")]
        internal static FnDelegate KeywordFn = args
            => args switch
            {
                (Keyword keyword, null) => keyword,
                (Str { Value: var StrValue }, null) => new Keyword(StrValue),
                _ => ThrowError(args, "one argument of type 'string'")
            };


        [MalFunction("vector")]
        internal static FnDelegate VectorFn = args => new List(args, ListType.Vector, NilV);

        [MalFunction("hash-map")]
        internal static FnDelegate HashMapFn = args => Reader.MalsToMap(args);

        [MalFunction("assoc")]
        internal static FnDelegate AssocFn = args
            => args switch
            {
                (Map { Value: var MapItems }, var NewItems) => new Map(Reader.MalsToMap(NewItems).Pipe(mapWithNewItems
                    => mapWithNewItems.Value.EntriesL().Aggregate(MapItems, (map, kv) => map.Add(kv.Key, kv.Value))),
                    NilV),
                _ => ThrowError(args, "at least one argument of type 'map'")
            };

        [MalFunction("dissoc")]
        internal static FnDelegate DissocFn = args
            => args switch
            {
                (Map { Value: var MapItems }, var DeletedKeys) => new Map(
                    DeletedKeys.Aggregate(MapItems, (map, key) => map.Remove(key)),
                    NilV),
                _ => ThrowError(args, "at least one argument of type 'map'")
            };

        [MalFunction("get")]
        internal static FnDelegate GetFn = args
            => args switch
            {
                (Nil, _) => NilV,
                (Map { Value: var MapItems }, (var Key, null)) => MapItems.TryFind(Key, out var Value) ? Value : NilV,
                _ => ThrowError(args, "two arguments where the first one must be of type 'map' and the second of type 'keyword' oraz 'string'")
            };

        [MalFunction("contains?")]
        internal static FnDelegate ContainsFn = args
            => args switch
            {
                (Map { Value: var MapItems }, (var Key, null)) => MapItems.ContainsKey(Key) ? TrueV : FalseV,
                _ => ThrowError(args, "two arguments where the first one must be of type 'map' and the second of type 'keyword' oraz 'string'")
            };

        [MalFunction("keys")]
        internal static FnDelegate KeysFn = args
            => args switch
            {
                (Map { Value: var MapItems }, null) => new List(MapItems.EntriesL().Select(kv => kv.Key), ListType.List, NilV),
                _ => ThrowError(args, "one argument of type 'map'")
            };

        [MalFunction("vals")]
        internal static FnDelegate ValsFn = args
            => args switch
            {
                (Map { Value: var MapItems }, null) => new List(MapItems.EntriesL().Select(kv => kv.Value), ListType.List, NilV),
                _ => ThrowError(args, "one argument of type 'map'")
            };


        [MalFunction("time-ms")]
        internal static FnDelegate TimeMsFn = args => new Number(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

        [MalFunction("conj")]
        internal static FnDelegate ConjFn = args
            => args switch
            {
                (List { ListType: ListType.List, Items: var Items }, var NewItems) =>
                    new List(NewItems.Aggregate(Items, (agg, item) => new(item, agg)), ListType.List, NilV),
                (List { ListType: ListType.Vector, Items: var Items }, var NewItems) =>
                    new List(Items.Concat(NewItems), ListType.Vector, NilV),
                _ => ThrowError(args, "at least one argument of type 'list' or 'vector'")
            };


        [MalFunction("seq")]
        internal static FnDelegate SeqFn = args
            => args switch
            {
                (List { Items: null } or Str { Value: "" }, null) => NilV,
                (List { ListType: ListType.List } list, null) => list,
                (List { ListType: ListType.Vector } vector, null) => vector with { ListType = ListType.List },
                (Str { Value: var StrValue }, null) =>
                    new List(StrValue.Select(c => new Str(c.ToString()) as MalType).ToLList(), ListType.List, NilV),
                _ => ThrowError(args, "one argument of type 'list', 'vector' or 'string'")
            };


        [MalFunction("meta")]
        internal static FnDelegate MetaFn = args
            => args switch
            {
                (Fn { Meta: var Meta }, null) => Meta,
                (List { Meta: var Meta }, null) => Meta,
                (Map { Meta: var Meta }, null) => Meta,
                _ => ThrowError(args, "one argument of type 'fn', 'vector', 'list or 'map'")
            };

        [MalFunction("with-meta")]
        internal static FnDelegate WithMetaFn = args
            => args switch
            {
                (Fn Mal, (var Meta, null)) => Mal with { Meta = Meta },
                (List Mal, (var Meta, null)) => Mal with { Meta = Meta },
                (Map Mal, (var Meta, null)) => Mal with { Meta = Meta },
                _ => ThrowError(args, "two arguments where the first one must be of type 'fn', 'vector', 'list or 'map'")
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

        internal class MalException : Exception
        {
            public MalType Mal { get; }
            public MalException(MalType mal) => Mal = mal;
        }
    }
}