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

New implementation is longer but the public API did not change so much. Now we can create an empty list using a new  parameterless constructor or the static `Empty` property. Thanks to `Empty` property, we can avoid allocating a new memory for each instance of empty list.

Because our list is immutable, we can calculate the `Length` property only once during object creation. We increment by 1 the length of the `Tail` and store its value in additional readonly field `length`. Thanks to this tick access to length of list has a constant time and this will be crucial once we start working with indexes, ranges and pattern matching.

C#12 introduces a new feature called "collection expression" that provides unified way of creation collections of any type, including custom types like `LList<T>`. Our list is immutable so we have to implement a special factory method and use an attribute called `[CollectionBuilder]`.

```csharp
[CollectionBuilder(typeof(LList), nameof(LList.Create))]
public record LList<T> { /* ... */ }

public static class LList
{
    public static LList<T> Create<T>(ReadOnlySpan<T> items) =>
        items.Length == 0 ? LList<T>.Empty : new(items[0], Create(items.Slice(1)));

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

Because type `LList<T>`  has property called`Length`, we can also specify indexes "from the end" using `^` operator. Thanks to `Slice` method ranges can be used. That method `Slice` was implemented in a particular way. If the passed range arguments include all items up to the end of the list, the returned list will be shared in memory as a part of the original list. Now we have all pieces necessary to support pattern matching working with collection data types.  

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
    {
        if (items is LList<T> lst)
        {
            return lst;
        }
        using var iterator = items.GetEnumerator();
        return Next(iterator);

        static LList<T> Next(IEnumerator<T> iter)
            => iter.MoveNext() ? new(iter.Current, Next(iter)) : LList<T>.Empty;
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

...