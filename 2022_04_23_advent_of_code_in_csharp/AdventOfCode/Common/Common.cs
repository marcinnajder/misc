using System.Diagnostics;
using System.Reflection;

namespace AdventOfCode;

static class Common
{
    public static string ProjectFolderPath { get; } =
        Path.Combine(new[] { new FileInfo(Assembly.GetEntryAssembly()!.Location).DirectoryName!, "..", "..", ".." });

    public static IEnumerable<int> ParseNumbers(char separator, string strings) =>
        strings.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);


    public static Func<T, R> Memoize<T, R>(Func<T, R> func)
        where T : notnull
    {
        var cache = new Dictionary<T, R>();
        return arg => cache.GetOrAdd(arg, func);
    }

    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueFactory)
        where TKey : notnull
    {
        if (!dict.TryGetValue(key, out var value))
        {
            value = valueFactory(key);
            dict.Add(key, value);
        }
        return value;
    }
}

public class NonExhaustivePatternMatchingException : Exception
{
    public NonExhaustivePatternMatchingException() : base("Non-exhaustive pattern matching") { }
}
