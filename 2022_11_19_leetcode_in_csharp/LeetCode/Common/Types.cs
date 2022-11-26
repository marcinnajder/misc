namespace LeetCode;



public record Option<T>;
public record None<T> : Option<T>
{
    public static Option<T> Instance { get; } = new None<T>();
}
public record Some<T>(T Value) : Option<T>;

public static class Option
{
    public static Option<T> Some<T>(T value) => new Some<T>(value);
    public static Option<T> None<T>() => LeetCode.None<T>.Instance;

    public static Option<R> Bind<T, R>(this Option<T> option, Func<T, Option<R>> binder) =>
        option switch
        {
            Some<T>(var value) => binder(value),
            _ => Option.None<R>()
        };

    public static T Get<T>(this Option<T> option) =>
        option is Some<T>(var value) ? value : throw new Exception("Option value was None");
}


public record LList<T>(T Head, LList<T>? Tail);

public static class LList
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
}