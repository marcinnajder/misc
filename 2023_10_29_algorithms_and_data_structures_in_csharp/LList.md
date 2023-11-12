# Immutable list in C#

Immutable linked list is a fundamental data structure used in all functional programming languages. List implementation is very simple and yet very powerful. Over the years non-functional languages like C#, Java and others copied many features from functional languages, for instance: lambdas, tuples, records, pattern matching, immutability, expression-based style and so on. Many developers are not aware how powerful immutable list is. We will present main features of that data structure by implementing it from scratch using modern C#.

Let's start by defining an immutable list in the simplest possible way using C# positional record.

```csharp
public record LList<T>(T Head, LList<T>? Tail);

var list1 = new LList<int>(1, new LList<int>(2, new LList<int>(3, null)));
LList<int> list2 = new(1, new(2, new(3, null)));

Console.WriteLine(list1.Head); // 1
Console.WriteLine(list1.Tail!.ToString()); // LList { Head = 2, Tail = LList { Head = 3, Tail =  } }
Console.WriteLine(list1 == list2); // True
```

Record in C# is just a class with some useful functionality builtin. Each node of singly linked list stores some value in `Head` property and the reference to next node in `Tail` property. Those properties provide only getters so the node object and the whole list are immutable so there can not be changed after creation. We can add a new item to the beginning of the list by creating a new node with `Tail` property set to an existing list. We can remove the first element just by returning `Tail` property of the first node. Immutable list is called "persistent data structure" because each "modification" creates a new instance of list where the structure in memory is shared.

Records in C# give us some methods for free. `GetHashCode` and `Equals` methods use all fields defined in the record type. In our case those fields are backing fields for properties `Head` and `Tail` so we can compare two lists by value. `ToString` method returns values of all properties, maybe it's not the perfect representation of list but still better that the default implementation of `ToString`. We will provide our own implementation of `ToString` later.

There is one small inconvenience in our implementation. The empty list is represented as a `null` value, let's change it.

```csharp
public record LList<T>
{
    private readonly int length;
    private readonly T head;
    private readonly LList<T> tail;

    public int Length => length;
    public T Head => EnsureNonEmpty(head);
    public LList<T> Tail => EnsureNonEmpty(tail);

    public LList(T head, LList<T> tail) => (this.head, this.tail, length) = (head, tail, tail.Length + 1);
    public LList() => (head, tail, length) = (default!, default!, 0);

    public bool IsEmpty => Length == 0;
    public static LList<T> Empty { get; } = new LList<T>();

    private R EnsureNonEmpty<R>(R result)
        => IsEmpty ? throw new InvalidOperationException("List is empty") : result;
}

LList<int> list3 = new(1, new(2, new(3, new())));
LList<int> list4 = new(1, new(2, new(3, LList<int>.Empty)));
```

New implementation is longer but the public API did not change so much. Now we can create an empty list using a new parameterless constructor or the static `Empty` property. Thanks to `Empty` property, we can avoid allocating a new memory for each instance of empty list.

Because our list is immutable, we can calculate the `Length` property only once during object creation. We increment by 1 the length of the `Tail` and store its value in additional readonly field `length`. Thanks to this tick access to length of list has a constant time and this will be crucial once we start working with indexes, ranges and pattern matching.

C#12 introduces a new feature called "collection expression" that provides unified way of creation collections of any type, including custom types like `LList<T>`. Our list is immutable so we have to implement a special factory method and use an attribute called `[CollectionBuilder]`.

```csharp
[CollectionBuilder(typeof(LList), nameof(LList.Create))]
public record LList<T> { /* ... */ }

public static class LList
{
    public static LList<T> Create<T>(ReadOnlySpan<T> items)
    {
        var result = LList<T>.Empty;
        for (int i = items.Length - 1; i >= 0; i--)
        {
            result = new(items[i], result);
        }
        return result;
    }

    public static LList<T> Of<T>(params T[] items) => Create(new ReadOnlySpan<T>(items));
}

var list5 = LList.Of(1, 2, 3);
var list6 = LList.Of<int>(); // empty list

// C#12 "collection expression"
LList<int> list7 = [1, 2, 3];
LList<int> list8 = []; // empty list
LList<int> list9 = [0, ..list7, 4, 5];  // spread operator
```

In the next step we will implement C# indexer and `Slice` method.

```csharp
public record LList<T>
{
    // ...

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

LList<int> ints = [5, 10, 15, 20, 25];
Console.WriteLine($"{ints[0]}, {ints[1]}, ..., {ints[^2]}, {ints[^1]}"); // 5, 10, ..., 20, 25
Console.WriteLine(ints[1..3]); // [10, 15]
Console.WriteLine(ints[1..]); // [10, 15, 20, 25]
Console.WriteLine(ints[..^2]); // [5, 10, 15]
Console.WriteLine(Object.ReferenceEquals(ints[1..], ints.Tail)); // True !
```

Because type `LList<T>` has property called`Length`, we can also specify indexes "from the end" using `^` operator. Thanks to `Slice` method ranges can be used. That method `Slice` was implemented in a particular way. If the passed range arguments include all items up to the end of the list, the returned list will be shared in memory as a part of the original list. Now we have all pieces necessary to support pattern matching working with collection data types.

```csharp
static class LList
{
    public static int CountL<T>(this LList<T> list)
        => list switch
    {
        [] => 0,
        [_, .. var tail] => 1 + CountL(tail),
    };
}
```

This code looks like a canonical implementation of recursive function calculating the length of the list. The best thing about this pattern matching is that it's exhaustive and it works in an efficient way. If we remove any of those patterns, the code will not compile. Extracting `var tail` in this case is like calling `list.Slice(1)` and that is like calling `list.Tail` so there is no copying of memory at all. That function is equivalent to the following code

```csharp
static class LList
{
    public static int CountL<T>(this LList<T> list) => list.Length == 0 ? 0 : 1 + CountL(list.Tail);
}
```

Our linked list a perfect candidate for a sequence. It means we easily convert the list to `IEnumerable<T>` interface in both directions. All available LINQ operators will work fine with `LList<T>` type too.

```csharp
public record LList<T> : IEnumerable<T>
{
    // ...
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
}

public static class LList
{
    public static LList<T> ToLList<T>(this IEnumerable<T> items)
        => items switch
        {
            LList<T> llist => llist,
            T[] array => Of(array),
            IList<T> ilist => Enumerable.Range(1, ilist.Count).Aggregate(LList<T>.Empty, (list, i) => new(ilist[^i], list)),
            var ienumerable => FromSeqUsingRecursion(ienumerable)
        };

    private static LList<T> FromSeqUsingRecursion<T>(this IEnumerable<T> items)
    {
        using var iterator = items.GetEnumerator();
        return Next(iterator);
        
        static LList<T> Next(IEnumerator<T> iter) => iter.MoveNext() ? new(iter.Current, Next(iter)) : LList<T>.Empty;
    }

    // overridden operators
    public static T First<T>(this LList<T> list) => list.Head;
    public static T? FirstOrDefault<T>(this LList<T> list) => list.IsEmpty ? default : list.Head;
    public static T Single<T>(this LList<T> list) =>
        list switch
        {
            [var item] => item,
            _ => throw new InvalidOperationException("Sequence contains no elements or more than one element")
        };
    // ...
}

LList<int> ints = [5, 10, 15, 20, 25];
var q =
    from i in ints
    where i % 10 == 0
    select $"{i:.00}";
Console.WriteLine(q.ToLList()); // [10,00, 20,00]
```

LINQ provides around 50 extension methods for `IEnumerable<T>` interface. Knowing that the data structure is `LList<T>` we can provide better implementations for some of them, for instance `Count`, `First`, `Single`, ... . We can even implement all LINQ operators but there is a big difference between `IEnumerable<T>` and `LList<T>` in .net.  The first one is lazy, the second one is eager. In most cases where we execute few operators at once, the lazy approach is more preferable. But there is a set operators where the dedicated implementations could be very useful. Let's look at few of them.

```csharp
public static class LList
{
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

    public static LList<T> SkipWhileL<T>(this LList<T> list, Func<T, bool> f)
        => list switch
        {
            ([]) => [],
            [var head, .. var tail] => f(head) ? SkipWhileL(tail, f) : list
        };
}
```

Functions above return `LList<T>` instead of `IEnumerable<T>`, they are eager so running them return the results immediately. But we can take advantage of the linked list structure, the list is just a pair of a `Head` and a `Tail`. Once we reach some point during the process of iterating the items, we can use the `Tail` property. Calling one operator (`list1.ConcatL(list2)`) will work better than calling two LINQ operators ( `list1.ConcatL(list2).ToLList()`).

There is one small thing. Let's look at the implementation of function `ToLList` converting sequence of values into immutable list.

```csharp
public static class LList
{
    private static LList<T> FromSeqUsingRecursion<T>(this IEnumerable<T> items)
    {
        using var iterator = items.GetEnumerator();
        return Next(iterator);
        
        static LList<T> Next(IEnumerator<T> iter) => iter.MoveNext() ? new(iter.Current, Next(iter)) : LList<T>.Empty;
    }
}
```

For a very long sequences we can encounter the stack overflow problem. We call recursively `Next` helper function for each element in the sequence and we have to wait for result, only then we can create next node in the list. It's because our list is immutable. We can implement this function differently.

```csharp
public static class LList
{
    private static LList<T> FromSeqUsingListReversion<T>(IEnumerable<T> items)
    {
        return ToReversedList(ToReversedList(items));

        static LList<T> ToReversedList(IEnumerable<T> xs) => xs.Aggregate(LList<T>.Empty, (list, item) => new(item, list));
    }
}
```

But here, we have to iterate twice over all items. We can use one trick to avoid this problem, our list can be immutable for the outside world and mutable for internal code. This way we could change some fields of already created nodes. Unfortunately, we have yet another problem. We wanted to have an access to the length of the list in constant time. Each node has `Length` property so we would have to iterate all items twice. First time to create a linked list and count the number of items and the second time to set `Length` for each node in the list. Finally I have found not the prettiest solution, it works at least .

```csharp
public record LList<T>
{
    private readonly LengthValue lengthValue;
    private readonly T head;
    private LList<T> tail;

    public int Length => lengthValue.Value;

    // ...

    private LList(T head, LList<T> tail, LengthValue lengthValue) => (this.head, this.tail, this.lengthValue) = (head, tail, lengthValue);

    internal static LList<T> ToLList(IEnumerable<T> items)
    {
        using var iterator = items.GetEnumerator();

        if (!iterator.MoveNext())
        {
            return Empty;
        }

        var valueRef = new ValueRef<int>();
        var i = 0;
        var first = new LList<T>(iterator.Current, Empty, new LengthValue(i, valueRef));
        var last = first;

        while (iterator.MoveNext())
        {
            last = last.tail = new LList<T>(iterator.Current, Empty, new LengthValue(++i, valueRef));
        }

        valueRef.Value = i + 1;

        return first;
    }

    private record class ValueRef<V>
    {
        public V? Value;
    }

    private class LengthValue
    {
        private int indexOrLength;
        private ValueRef<int>? lengthRef;

        public int Value => lengthRef == null ? indexOrLength : lengthRef.Value - indexOrLength;

        public LengthValue(int length) => indexOrLength = length;

        public LengthValue(int index, ValueRef<int> lengthRef) => (indexOrLength, this.lengthRef) = (index, lengthRef);

        public override bool Equals(object? obj) => obj is LengthValue l ? Value.Equals(l.Value) : false;

        public override int GetHashCode() => Value.GetHashCode();
    }
}
```

Similar solution has been used in [F#](https://github.com/dotnet/fsharp/blob/main/src/FSharp.Core/local.fs#L531) and [Scala](https://youtu.be/7mTNZeiIK7E?t=1439) languages.
