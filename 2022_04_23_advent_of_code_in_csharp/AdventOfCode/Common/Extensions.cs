namespace AdventOfCode;

public static class Extensions
{
    public static IEnumerable<(T, T)> Pairwise<T>(this IEnumerable<T> items)
    {
        _ = items ?? throw new ArgumentException(nameof(items));

        return PairwiseImpl(items);

        static IEnumerable<(T, T)> PairwiseImpl(IEnumerable<T> items)
        {
            using var enumerator = items.GetEnumerator();

            if (enumerator.MoveNext())
            {
                T prev = enumerator.Current;
                while (enumerator.MoveNext())
                {
                    yield return (prev, enumerator.Current);
                    prev = enumerator.Current;
                }
            }
        }
    }

    public static IEnumerable<T[]> Windowed<T>(this IEnumerable<T> items, int size)
    {
        _ = items ?? throw new ArgumentException(nameof(items));

        return Windowed3Impl(items, size);

        static IEnumerable<T[]> Windowed3Impl(IEnumerable<T> items, int size)
        {
            T[] window = new T[size];

            using var enumerator = items.GetEnumerator();

            for (int j = 0; j < size; j++)
            {
                if (enumerator.MoveNext())
                {
                    window[j] = enumerator.Current;
                }
                else
                {
                    yield break;
                }
            }

            yield return window.ToArray();

            int mod = 0;

            while (enumerator.MoveNext())
            {
                window[mod] = enumerator.Current;

                mod = (mod + 1) % size;

                var array = new T[size];
                for (int i = 0; i < size; i++)
                {
                    array[i] = window[(mod + i) % size];
                }
                yield return array;
            }
        }
    }

    public static IEnumerable<R> Choose<T, R>(this IEnumerable<T> items, Func<T, Option<R>> chooser) =>
        items.Select(chooser).OfType<Some<R>>().Select(o => o.Value);

    public static Option<R> TryPick<T, R>(this IEnumerable<T> items, Func<T, Option<R>> chooser) =>
        items.Select(chooser).OfType<Some<R>>().FirstOrDefault(Option.None<R>());

    public static R Pick<T, R>(this IEnumerable<T> items, Func<T, Option<R>> chooser) =>
        items.TryPick(chooser) switch
        {
            Some<R>(var value) => value,
            _ => throw new Exception("Element not found")
        };


    public static IEnumerable<A> Scan<T, A>(this IEnumerable<T> items, A seed, Func<A, T, A> folder)
    {
        var state = seed;
        yield return state;

        foreach (var item in items)
        {
            state = folder(state, item);
            yield return state;
        }
    }

    public static IEnumerable<T> Unfold<T, A>(A seed, Func<A, Option<(T, A)>> generator)
    {
        var state = seed;
        while (true)
        {
            var stateO = generator(state);
            if (stateO is Some<(T, A)>(var (item, newState)))
            {
                state = newState;
                yield return item;
            }
            else
            {
                yield break;
            }
        }
    }

    public static A AggregateBack<T, A>(this IEnumerable<T> items, A seed, Func<A, T, A> func) =>
        items
            .Aggregate((LList<T>?)null, (tail, head) => new(head, tail))
            .ToEnumerable()
            .Aggregate(seed, func);


    public static IEnumerable<int> RangeFromTo(int from, int to) => Enumerable.Range(from, to - from + 1);

    public static string JoinToString<T>(this IEnumerable<T> items, string separator = ",") => string.Join(separator, items);

    public static string ConcatToString<T>(this IEnumerable<T> items) => string.Concat(items);

    public static R Pipe<T, R>(this T value, Func<T, R> func) => func(value);

    public static void Pipe<T>(this T value, Action<T> func) => func(value);


    // https://clojuredocs.org/clojure.core/partition-by
    public static IEnumerable<T[]> PartitionBy<T, P>(this IEnumerable<T> items, Func<T, P> func)
    {
        var pack = new List<T>();

        using var enumerator = items.GetEnumerator();

        if (enumerator.MoveNext())
        {
            var prevPartitioner = func(enumerator.Current);
            pack.Add(enumerator.Current);

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                var partitioner = func(item);
                var theSame = prevPartitioner == null ? partitioner == null : prevPartitioner.Equals(partitioner);
                prevPartitioner = partitioner;

                if (theSame)
                {
                    pack.Add(enumerator.Current);
                }
                else
                {
                    yield return pack.ToArray();
                    pack = new List<T>() { item };
                }
            }
        }

        if (pack.Count > 0)
        {
            yield return pack.ToArray();
        }
    }

    public static T Identity<T>(T value) => value;
}


