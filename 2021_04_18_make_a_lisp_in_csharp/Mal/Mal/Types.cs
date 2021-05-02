

// https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-fsharpresult-2.html - dokumentacja F#
// https://github.com/dotnet/fsharp/blob/main/src/fsharp/FSharp.Core/prim-types.fsi#L2497-2497 - podstawowe typy
// https://github.com/dotnet/fsharp/blob/main/src/fsharp/FSharp.Core/option.fs#L68-68 - modul Option


// https://github.com/marcinnajder/misc/blob/master/2019_04_06_make_a_lisp_in_typescript/src/types.ts

using System;
using System.Linq;
using System.Collections.Generic;
using PowerFP;
using System.Diagnostics.CodeAnalysis;

namespace Mal
{
    public static class Types
    {
        public enum ListType { List, Vector }
        public enum ListTypeAndMap { List = ListType.List, Vector = ListType.Vector, HashMap }

        public abstract record MalType { }
        public record Number(double Value) : MalType { };
        public record Symbol(string Name, MalType Meta) : MalType { };
        public record List(LList<MalType>? Items, ListType ListType, MalType Meta) : MalType { };
        public record Nil() : MalType { };
        public record True() : MalType { };
        public record False() : MalType { };
        public record Str(string Value) : MalType { };
        public record Keyword(string Name) : MalType { };
        public record Fn(Func<MalType[], MalType> Value, MalType Meta) : MalType { };
        public record Atom(MalType Mal) : MalType { };
        public record Map(Dictionary<MalType, MalType> Value, MalType Meta) : MalType { };

        public static True TrueV = new True();
        public static False FalseV = new False();
        public static Nil NilV = new Nil();


        public static readonly Dictionary<ListTypeAndMap, (string Left, string Right)> List2BracketMap = new()
        {
            { ListTypeAndMap.List, ("(", ")") },
            { ListTypeAndMap.Vector, ("[", "]") },
            { ListTypeAndMap.HashMap, ("{", "}") },
        };

        public static readonly Dictionary<string, ListTypeAndMap> Bracket2ListMap = List2BracketMap.ToDictionary(kv => kv.Value.Left, kv => kv.Key);

        public static List ListFrom(params MalType[] mals) => new List(mals.ToLList(), ListType.List, NilV);

        public static bool MalEqual(MalType mal1, MalType mal2) =>
            (mal1, mal2) switch
            {
                (Map map1, Map map2) => map1.Meta.Equals(map2.Meta) && map1.Value.SequenceEqual(map2.Value, KeyValueComparer.Instance),
                _ => mal1.Equals(mal2)
            };

        private class KeyValueComparer : IEqualityComparer<KeyValuePair<MalType, MalType>>
        {
            public static KeyValueComparer Instance = new KeyValueComparer();

            public bool Equals(KeyValuePair<MalType, MalType> x, KeyValuePair<MalType, MalType> y)
                => MalEqual(x.Key, y.Key) && MalEqual(x.Value, y.Value);

            public int GetHashCode([DisallowNull] KeyValuePair<MalType, MalType> obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}

#pragma warning disable CS8509
#pragma warning disable CS8509