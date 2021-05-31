using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerFP
{
    // https://fsprojects.github.io/FSharpx.Collections/reference/fsharpx-collections-map.html

    // https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections.html
    // https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-mapmodule.html

    // https://github.com/dotnet/fsharp/blob/main/src/fsharp/FSharp.Core/map.fs -> Map w F# jako binarne zbalansowne drzewo

    // public record Map1<K, V>((K, V) Head, LList<(K, V)>? Tail) : LList<(K, V)>(Head, Tail) { }
    // public record Map2<K, V>(K Key, V Value, Map2<K, V>? Tail) { }

    public record Map<K, V>(LList<(K Key, V Value)>? Items) { }

    public static class MapM
    {
        public static Map<K, V> MapFrom<K, V>(params (K, V)[] items) => new(LListMapM.MapFrom(items));
        public static Map<K, V> MapFrom<K, V>(Dictionary<K, V> items) where K : notnull => new(LListMapM.MapFrom(items));
        public static Map<K, V> MapFrom<K, V>(IEnumerable<(K, V)> items) => new(LListMapM.MapFrom(items));

        public static Map<K, V> Add<K, V>(this Map<K, V> map, K key, V value) => new(LListMapM.Add(map.Items, key, value));
        public static (bool, V?) TryFind<K, V>(this Map<K, V> map, K key) => LListMapM.TryFind(map.Items, key);
        public static V Find<K, V>(this Map<K, V> map, K key) => LListMapM.Find(map.Items, key);
        public static bool ContainsKey<K, V>(this Map<K, V> map, K key) => LListMapM.ContainsKey(map.Items, key);
        public static Map<K, V> Remove<K, V>(this Map<K, V> map, K key) => new(LListMapM.Remove(map.Items, key));
        public static Map<K, V> Change<K, V>(this Map<K, V> map, K key, Func<(bool, V?), (bool, V?)> f) => new(LListMapM.Change(map.Items, key, f));


        public static bool TryFind<K, V>(this Map<K, V> map, K key, out V value)
        {
            var (found, foundValue) = LListMapM.TryFind(map.Items, key);
            value = (found ? foundValue : default)!;
            return found;
        }

        public static IEnumerable<(K Key, V Value)> Entries<K, V>(this Map<K, V> map) => map.Items.ToEnumerable();
        public static LList<(K Key, V Value)>? EntriesL<K, V>(this Map<K, V> map) => map.Items;
    }
}