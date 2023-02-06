namespace AdventOfCode;

public record LList<T>(T Head, LList<T>? Tail) { }

public static class LListM
{
    public static IEnumerable<T> ToEnumerable<T>(this LList<T>? llist)
    {
        if (llist == null)
        {
            yield break;
        }

        var node = llist;
        while (node != null)
        {
            yield return node.Head;
            node = node.Tail;
        }
    }

    public static IEnumerable<T> ToEnumerableRec<T>(this LList<T>? llist)
    {
        if (llist == null)
        {
            yield break;
        }

        yield return llist.Head;

        foreach (var item in ToEnumerableRec(llist.Tail))
        {
            yield return item;
        }
    }

    public static LList<T>? ToLList<T>(this IEnumerable<T> llist)
    {
        return NextValue(llist.GetEnumerator());
        static LList<T>? NextValue(IEnumerator<T> e) => e.MoveNext() ? new(e.Current, NextValue(e)) : null;
    }

    public static LList<T>? LListFrom<T>(params T[] items) => items.Length == 0 ? null : items.ToLList();


    public static int Count<T>(this LList<T>? llist) =>
        llist switch
        {
            null => 0,
            (_, var Tail) => 1 + Count(Tail)
        };

    public static LList<R>? Select<T, R>(this LList<T>? llist, Func<T, R> f) =>
        llist switch
        {
            null => null,
            (var Head, var Tail) => new(f(Head), Select(Tail, f))
        };

    public static LList<T>? Where<T>(this LList<T>? llist, Func<T, bool> f) =>
        llist switch
        {
            null => null,
            (var Head, var Tail) => f(Head) ? llist with { Tail = Where(Tail, f) } : Where(Tail, f)
        };

    public static A Aggregate<T, A>(this LList<T>? llist, A seed, Func<A, T, A> f) =>
        llist switch
        {
            null => seed,
            (var Head, var Tail) => Aggregate(Tail, f(seed, Head), f)
        };

    public static T Aggregate<T>(this LList<T>? llist, Func<T, T, T> f) =>
        llist switch
        {
            null => throw new Exception("List contains no elements"),
            { Tail: null } => llist.Head,
            _ => Aggregate(llist.Tail, llist.Head, f)
        };

    public static LList<T>? Take<T>(this LList<T>? llist, int count) =>
        llist switch
        {
            null => null,
            (var Head, var Tail) => count <= 0 ? null : new(Head, Take(Tail, count - 1))
        };

    public static LList<T>? Skip<T>(this LList<T>? llist, int count) =>
        llist switch
        {
            null => null,
            (var Head, var Tail) => count <= 0 ? llist : Skip(Tail, count - 1)
        };



    public static LList<T>? Concat<T>(this LList<T>? llist1, LList<T>? llist2) =>
        (llist1, llist2) switch
        {
            (null, _) => llist2,
            ((var Head, var Tail), _) => new(Head, Concat(Tail, llist2))
        };


    public static LList<R>? Zip<T1, T2, R>(this LList<T1>? llist1, LList<T2>? llist2, Func<T1, T2, R> f) =>
        (llist1, llist2) switch
        {
            (null, _) or (_, null) => null,
            ((var Head1, var Tail1), (var Head2, var Tail2)) => new(f(Head1, Head2), Zip(Tail1, Tail2, f))
        };


    public static LList<TT>? SelectMany<T, TT>(this LList<T>? llist, Func<T, LList<TT>?> f) =>
        llist switch
        {
            null => null,
            (var Head, var Tail) => f(Head).Concat(SelectMany(Tail, f))
        };

    public static LList<R>? SelectMany<T, TT, R>(this LList<T>? llist, Func<T, LList<TT>?> f, Func<T, TT, R> r) =>
        llist switch
        {
            null => null,
            (var Head, var Tail) => f(Head).Select(tt => r(Head, tt)).Concat(SelectMany(Tail, f, r))
        };

    public static LList<T>? Reverse<T>(this LList<T>? llist)
    {
        return Reverse2(llist, null);

        static LList<T>? Reverse2(LList<T>? llist, LList<T>? result) =>
            llist switch
            {
                null => result,
                (var Head, var Tail) => Reverse2(Tail, new(Head, result)),
            };
    }

    public static bool All<T>(this LList<T>? llist, Func<T, bool> f) =>
        llist switch
        {
            null => true,
            (var Head, var Tail) => f(Head) && All(Tail, f)
        };

    public static bool Any<T>(this LList<T>? llist, Func<T, bool> f) =>
        llist switch
        {
            null => false,
            (var Head, var Tail) => f(Head) || Any(Tail, f)
        };

    public static T ElementAt<T>(this LList<T>? llist, int index) =>
        (llist, index) switch
        {
            (_, < 0) => throw new Exception("Index cannot be less than zero"),
            (null, _) => throw new Exception("Index out of bounds"),
            ((var Head, _), 0) => Head,
            ((var Head, var Tail), _) => ElementAt(Tail, index - 1)
        };

    public static bool SequenceEqual<T>(this LList<T>? llist1, LList<T>? llist2, Func<T, T, bool> equals) =>
        (llist1, llist2) switch
        {
            (null, null) => true,
            (null, _) or (_, null) => false,
            ((var Head1, var Tail1), (var Head2, var Tail2)) => equals(Head1, Head2) && SequenceEqual(Tail1, Tail2, equals)
        };

}

