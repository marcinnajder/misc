
using System;
using System.Collections.Generic;

namespace AdventOfCode;

public record Map<K, V>
    where K : notnull
{
    // properties instead of primary constructor because we want to 'internal' visibility
    internal LList<(K Key, V Value)>? Items { get; }
    internal Map(LList<(K Key, V Value)>? items) => Items = items;
}


public static class MapM
{
    public static Map<K, V> Empty<K, V>() where K : notnull => new(null);

    public static Map<K, V> MapFrom<K, V>(LList<(K Key, V Value)>? items) where K : notnull => new(LListMapM.MapFrom(items));
    public static Map<K, V> MapFrom<K, V>(params (K, V)[] items) where K : notnull => new(LListMapM.MapFrom(items));
    public static Map<K, V> MapFrom<K, V>(Dictionary<K, V> items) where K : notnull => new(LListMapM.MapFrom(items));
    public static Map<K, V> MapFrom<K, V>(IEnumerable<(K, V)> items) where K : notnull => new(LListMapM.MapFrom(items));

    public static Map<K, V> Add<K, V>(this Map<K, V> map, K key, V value) where K : notnull => new(LListMapM.Add(map.Items, key, value));
    public static (bool, V?) TryFind<K, V>(this Map<K, V> map, K key) where K : notnull => LListMapM.TryFind(map.Items, key);
    public static V Find<K, V>(this Map<K, V> map, K key) where K : notnull => LListMapM.Find(map.Items, key);
    public static bool ContainsKey<K, V>(this Map<K, V> map, K key) where K : notnull => LListMapM.ContainsKey(map.Items, key);
    public static Map<K, V> Remove<K, V>(this Map<K, V> map, K key) where K : notnull => new(LListMapM.Remove(map.Items, key));
    public static Map<K, V> Change<K, V>(this Map<K, V> map, K key, Func<(bool, V?), (bool, V?)> f) where K : notnull => new(LListMapM.Change(map.Items, key, f));


    public static bool TryFind<K, V>(this Map<K, V> map, K key, out V value)
        where K : notnull
    {
        var (found, foundValue) = LListMapM.TryFind(map.Items, key);
        value = (found ? foundValue : default)!;
        return found;
    }

    public static IEnumerable<(K Key, V Value)> Entries<K, V>(this Map<K, V> map) where K : notnull => map.Items.ToEnumerable();
    public static LList<(K Key, V Value)>? EntriesL<K, V>(this Map<K, V> map) where K : notnull => map.Items;
}
