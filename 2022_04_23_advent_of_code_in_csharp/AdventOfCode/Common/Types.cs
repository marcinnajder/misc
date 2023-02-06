namespace AdventOfCode;

public record Option<T>;
public record None<T> : Option<T>
{
    public static Option<T> Instance { get; } = new None<T>();
}
public record Some<T>(T Value) : Option<T>;

public static class Option
{
    public static Option<T> Some<T>(T value) => new Some<T>(value);
    public static Option<T> None<T>() => AdventOfCode.None<T>.Instance;

    public static Option<R> Bind<T, R>(this Option<T> option, Func<T, Option<R>> binder) =>
        option switch
        {
            Some<T>(var value) => binder(value),
            _ => Option.None<R>()
        };

    public static T Get<T>(this Option<T> option) =>
        option is Some<T>(var value) ? value : throw new Exception("Option value was None");
}


// public record LList<T>(T Head, LList<T>? Tail);

// public static class LList
// {
//     public static IEnumerable<T> ToEnumerable<T>(this LList<T>? llist)
//     {
//         if (llist == null)
//         {
//             yield break;
//         }

//         var node = llist;
//         while (node != null)
//         {
//             yield return node.Head;
//             node = node.Tail;
//         }
//     }
// }