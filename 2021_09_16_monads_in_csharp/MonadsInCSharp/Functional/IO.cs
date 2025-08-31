using System.Runtime.CompilerServices;

namespace MonadsInCSharp;

[AsyncMethodBuilder(typeof(IOMethodBuilder<>))]
public delegate T IO<T>();

public class Unit
{
    public static Unit V { get; } = new Unit();
}


public static class IOOperators
{
    // monad function
    public static IO<R> Select<T, R>(this IO<T> io, Func<T, R> f) => () => f(io());
    public static IO<R> SelectMany<T, R>(this IO<T> io, Func<T, IO<R>> f) => () => f(io())();

    public static void Run<T>(this IO<T> io) => io();

    // io operations
    public static IO<string> ReadLine() => () =>
        Console.ReadLine()!;

    public static IO<Unit> WriteLine(string text) => () =>
    {
        Console.WriteLine(text);
        return Unit.V;
    };
}

