using System;
using System.Linq;
using System.Collections.Generic;


namespace PowerFP
{
    public static class LListMapM
    {
        public static LList<(K, V)>? MapFrom<K, V>(params (K, V)[] items) where K : notnull =>
            items.Length == 0 ? null : MapFrom(items as IEnumerable<(K, V)>);

        public static LList<(K, V)>? MapFrom<K, V>(Dictionary<K, V> items) where K : notnull =>
            items.Count == 0 ? null : MapFrom(items.Select(kv => (kv.Key, kv.Value)));

        public static LList<(K, V)>? MapFrom<K, V>(LList<(K Key, V Value)>? items) where K : notnull =>
            items == null ? null : MapFrom(items.ToEnumerable());

        public static LList<(K, V)>? MapFrom<K, V>(IEnumerable<(K, V)> items) where K : notnull =>
            items.Aggregate((LList<(K, V)>?)null, (m, kv) => m.Add(kv.Item1, kv.Item2));

        public static LList<(K, V)> Add<K, V>(this LList<(K, V)>? map, K key, V value) where K : notnull
            => map switch
            {
                null => new((key, value), null),
                (var Head, var Tail) => Head switch
                {
                    (var Key, _) when key.Equals(Key) => new((key, value), Tail),
                    (var Key, _) when key.GetHashCode() < Key.GetHashCode() => new((key, value), map),
                    _ => new(Head, Add(Tail, key, value))
                },
            };


        public static (bool, V?) TryFind<K, V>(this LList<(K, V)>? map, K key) where K : notnull
            => map switch
            {
                null => (false, default(V)),
                (var Head, var Tail) => Head switch
                {
                    (var Key, var Value) when key.Equals(Key) => (true, Value),
                    (var Key, _) when key.GetHashCode() < Key.GetHashCode() => (false, default(V)),
                    _ => TryFind(Tail, key)
                }
            };

        public static V Find<K, V>(this LList<(K, V)>? map, K key) where K : notnull
            => TryFind(map, key) is (true, var Value) ? Value! : throw new Exception($"Map does not contain '{key}' key");

        public static bool ContainsKey<K, V>(this LList<(K, V)>? map, K key) where K : notnull
            => TryFind(map, key) is (true, _) ? true : false;


        public static LList<(K, V)>? Remove<K, V>(this LList<(K, V)>? map, K key) where K : notnull
            => map switch
            {
                null => null,
                (var Head, var Tail) => Head switch
                {
                    (var Key, _) when key.Equals(Key) => Tail,
                    (var Key, _) when key.GetHashCode() < Key.GetHashCode() => map,
                    _ => new(Head, Remove(Tail, key))
                },
            };

        public static LList<(K, V)>? Change<K, V>(this LList<(K, V)>? map, K key, Func<(bool, V?), (bool, V?)> f) where K : notnull
            => map switch
            {
                null => f((false, default(V)))
                    .Pipe(x => x.Item1 ? new LList<(K, V)>((key, x.Item2!), null) : null),
                (var Head, var Tail) => Head switch
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