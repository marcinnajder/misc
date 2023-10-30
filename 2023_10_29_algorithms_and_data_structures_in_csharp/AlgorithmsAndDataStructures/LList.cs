using System.Runtime.CompilerServices;
using System.Collections;
using System.Net.Http.Headers;

namespace AlgorithmsAndDataStructures;

public partial record LList<T>
{
    private readonly int length;
    private readonly T head;
    private readonly LList<T> tail;

    public int Length => length;
    public T Head => EnsureNonEmpty(head);
    public LList<T> Tail => EnsureNonEmpty(tail);

    public LList(T head, LList<T> tail) => (this.head, this.tail, length) = (head, tail, tail.Length + 1);
    public LList() => (head, tail, length) = (default!, default!, 0);

    // public void Deconstruct(out T Head, out LList<T> Tail) => (Head, Tail) = (this.Head, this.Tail);

    private R EnsureNonEmpty<R>(R result) => IsEmpty ? throw new InvalidOperationException("List is empty") : result;
}

[CollectionBuilder(typeof(LList), nameof(LList.Create))]
public partial record LList<T> : IEnumerable<T>
{
    public bool IsEmpty => Length == 0;

    public static LList<T> Empty { get; } = new LList<T>();

    public IEnumerator<T> GetEnumerator()
    {
        var node = this;
        while (!node.IsEmpty)
        {
            yield return node.Head;
            node = node.Tail;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => $"[{string.Join(", ", this)}]";

    public T this[int i] => i >= 0 && i < Length ? (i == 0 ? Head : Tail[i - 1]) : throw new ArgumentOutOfRangeException();

    public LList<T> Slice(int start, int length) =>
        length == 0 || (length > 0 && start >= 0 && start < Length && start + length - 1 < Length)
         ? SliceSafe(start, length) : throw new ArgumentOutOfRangeException();

    private LList<T> SliceSafe(int start, int length) =>
        (start, length) switch
        {
            (_, 0) => Empty,
            (0, _) => length == Length ? this : new(Head, Tail.SliceSafe(0, length - 1)),
            _ => Tail.SliceSafe(start - 1, length)
        };
}


public static class LList
{
    public static LList<T> Create<T>(ReadOnlySpan<T> items) =>
        items.Length == 0 ? LList<T>.Empty : new(items[0], Create(items.Slice(1)));

    public static LList<T> Of<T>(params T[] items) => Create(new ReadOnlySpan<T>(items));

    // public static LList<T> Empty<T>() => LList<T>.Empty;

    public static LList<T> ToLList<T>(this IEnumerable<T> items)
    {
        if (items is LList<T> lst)
        {
            return lst;
        }
        using var iterator = items.GetEnumerator();
        return Next(iterator);

        static LList<T> Next(IEnumerator<T> iter) => iter.MoveNext() ? new(iter.Current, Next(iter)) : LList<T>.Empty;
    }

    public static LList<T> Cons<T>(T head, LList<T> tail) => new(head, tail);


    public static int Count<T>(this LList<T> list) => list.Length;

    public static T First<T>(this LList<T> list) => list.Head;

    public static T? FirstOrDefault<T>(this LList<T> list) => list.IsEmpty ? default : list.Head;

    public static T Single<T>(this LList<T> list) =>
        list switch
        {
            ([var item]) => item,
            _ => throw new InvalidOperationException("Sequence contains no elements or more than one element")
        };

    public static T? SingleOrDefault<T>(this LList<T> list) =>
        list switch
        {
            ([var item]) => item,
            [] => default,
            _ => throw new InvalidOperationException("Sequence contains more than one element"),
        };

    public static int CountL<T>(this LList<T> list)
        => list switch
        {
            ([]) => 0,
            [_, .. var tail] => 1 + CountL(tail),
        };


    // public static int CountL<T>(this LList<T> list) => list.Length == 0 ? 0 : 1 + CountL(list.Tail);

    public static LList<R> SelectL<T, R>(this LList<T> list, Func<T, R> f)
        => list switch
        {
            ([]) => [],
            [var head, .. var tail] => new(f(head), SelectL(tail, f)),
        };

    public static LList<T> WhereL<T>(this LList<T> list, Func<T, bool> f)
        => list switch
        {
            ([]) => [],
            [var head, .. var tail] => f(head) ? new(head, WhereL(tail, f)) : WhereL(tail, f)
        };

    public static LList<T> ConcatL<T>(this LList<T> list1, LList<T> list2)
        => list1 switch
        {
            ([]) => list2,
            [var head, .. var tail] => new(head, ConcatL(tail, list2))
        };

    public static LList<T> SkipL<T>(this LList<T> list, int n)
        => (list, n) switch
        {
            ([], _) => [],
            (_, 0) => list,
            ([_, .. var tail], _) => tail.SkipL(n - 1)
        };

    //todo: SkipWhile

}



// public record LList2<T>(T Head, LList2<T>? Tail);

// public partial record LList<T>(T Head, LList<T> Tail)
// {
//     public T Head { get; } = Head; // can not use 'with {..}'
//     public LList<T> Tail { get; } = Tail; // can not use 'with {..}'
//     public int Length { get; } = Tail == default ? 0 : Tail.Length + 1;

//     public LList() : this(default!, default!) { }
// }