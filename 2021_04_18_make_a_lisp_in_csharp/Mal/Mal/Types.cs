

// https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-fsharpresult-2.html - dokumentacja F#
// https://github.com/dotnet/fsharp/blob/main/src/fsharp/FSharp.Core/prim-types.fsi#L2497-2497 - podstawowe typy
// https://github.com/dotnet/fsharp/blob/main/src/fsharp/FSharp.Core/option.fs#L68-68 - modul Option


// https://github.com/marcinnajder/misc/blob/master/2019_04_06_make_a_lisp_in_typescript/src/types.ts

using System;
using System.Linq;
using System.Collections.Generic;
using PowerFP;

namespace Mal
{
    public static class Types
    {
        public enum ListType { List, Vector }
        public enum ListTypeAndMap { List = ListType.List, Vector = ListType.Vector, HashMap }

        public abstract record MalType { }
        public record Number(double Value) : MalType { };
        public record Symbol(string Name, MalType? Meta = null) : MalType { };
        public record List(LList<MalType>? Items, ListType ListType, MalType? Meta = null) : MalType { };
        public record Nil() : MalType { };
        public record True() : MalType { };
        public record False() : MalType { };
        public record Str(string Value) : MalType { };
        public record Keyword(string Name) : MalType { };
        public record Fn(Func<MalType[], MalType> Value, MalType? Meta = null) : MalType { };
        public record Atom(MalType Mal) : MalType { };
        public record Map(Dictionary<MalType, MalType> Value, MalType? Meta = null) : MalType { };

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
    }
}

#pragma warning disable CS8509
#pragma warning disable CS8509
