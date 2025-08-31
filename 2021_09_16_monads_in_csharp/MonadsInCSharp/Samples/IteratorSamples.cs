using System.Collections;

using static MonadsInCSharp.Function;
using static MonadsInCSharp.Monad;

namespace MonadsInCSharp;

public static class IteratorSamples
{
    public static void ForEach()
    {
        var items = new[] { 5, 10, 15 };
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }

        // code above is translated into code below
        IEnumerable<int> iterable = items;
        using IEnumerator<int> iterator = iterable.GetEnumerator();
        while (iterator.MoveNext())
        {
            var item = iterator.Current;
            Console.WriteLine(item);
        }
    }
}