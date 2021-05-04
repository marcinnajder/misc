using System;
using System.Linq;
using System.Collections.Generic;


namespace PowerFP
{
    // https://fsprojects.github.io/FSharpx.Collections/reference/fsharpx-collections-map.html

    // https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections.html
    // https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-mapmodule.html

    // https://github.com/dotnet/fsharp/blob/main/src/fsharp/FSharp.Core/map.fs -> Map w F# jako binarne zbalansowne drzewo

    // public record Map1<K, V>((K, V) Head, LList<(K, V)>? Tail) : LList<(K, V)>(Head, Tail) { }
    // public record Map2<K, V>(K Key, V Value, Map2<K, V>? Tail) { }

    public record Map<K, V>(LList<(K, V)> Items) { }


    public static class MapM
    {
        public static LList<(K, V)>? MapFrom<K, V>(params (K, V)[] items) =>
            items.Length == 0 ? null : items.Aggregate((LList<(K, V)>?)null, (m, kv) => m.Add(kv.Item1, kv.Item2));

        public static LList<(K, V)> Add<K, V>(this LList<(K, V)>? map, K key, V value)
            => (map, key) switch
            {

                (_, null) or (((null, _), _), _) => throw new Exception("'Key' value cannot be null."),
                (null, _) => new((key, value), null),
                ((var Head, var Tail), _) => Head switch
                {
                    (var Key, _) when key.Equals(Key) => new((key, value), Tail),
                    (var Key, _) when key.GetHashCode() < Key.GetHashCode() => new((key, value), map),
                    _ => new(Head, Add(Tail, key, value))
                },
            };

        public static (bool, V?) TryFind<K, V>(this LList<(K, V)>? map, K key)
            => (map, key) switch
            {
                (_, null) or (((null, _), _), _) => throw new Exception("'Key' value cannot be null."),
                (null, _) => (false, default(V)),
                ((var Head, var Tail), _) => Head switch
                {
                    (var Key, var Value) when key.Equals(Key) => (true, Value),
                    (var Key, _) when key.GetHashCode() < Key.GetHashCode() => (false, default(V)),
                    _ => TryFind(Tail, key)
                }
            };


        public static V Find<K, V>(this LList<(K, V)>? map, K key)
            => TryFind(map, key) is (true, var Value) ? Value! : throw new Exception($"Map does not contain '{key}' key");

        public static bool ContainsKey<K, V>(this LList<(K, V)>? map, K key)
            => TryFind(map, key) is (true, _) ? true : false;


        public static LList<(K, V)>? Remove<K, V>(this LList<(K, V)>? map, K key)
            => (map, key) switch
            {
                (_, null) or (((null, _), _), _) => throw new Exception("'Key' value cannot be null."),
                (null, _) => null,
                ((var Head, var Tail), _) => Head switch
                {
                    (var Key, _) when key.Equals(Key) => Tail,
                    (var Key, _) when key.GetHashCode() < Key.GetHashCode() => map,
                    _ => new(Head, Remove(Tail, key))
                },
            };

        public static LList<(K, V)>? Change<K, V>(this LList<(K, V)>? map, K key, Func<(bool, V?), (bool, V?)> f)
            => (map, key) switch
            {

                (_, null) or (((null, _), _), _) => throw new Exception("'Key' value cannot be null."),
                (null, _) => f((false, default(V)))
                    .Pipe(x => x.Item1 ? new LList<(K, V)>((key, x.Item2!), null) : null),
                ((var Head, var Tail), _) => Head switch
                {
                    (var Key, var Value) when key.Equals(Key) => f((true, Value))
                        .Pipe(x => x.Item1 ? new LList<(K, V)>((Key, x.Item2!), Tail) : Tail),
                    (var Key, _) when key.GetHashCode() < Key.GetHashCode() => f((false, default(V)))
                        .Pipe(x => x.Item1 ? new LList<(K, V)>((key, x.Item2!), map) : map),
                    _ => new(Head, Change(Tail, key, f))
                },
            };


        // public static LList<(K, V)> Add2<K, V>(this LList<(K, V)>? map, K key, V value)
        //     => map.Change(key, _ => (true, value))!;
    }
}