namespace AlgorithmsAndDataStructures;

// monads + generic math C#11 .NET7 2022
class Monads
{
    public static void Test()
    {
        Optional<int>[] monads = CreateMonadsFromValues<Optional<int>, int>(1, 2, 3);
        Console.WriteLine(monads);
    }

    static M[] CreateMonadsFromValues<M, T>(params T[] values)
        where M : IMonad<M, T>
    {
        return Array.ConvertAll(values, M.Return);
    }
}

interface IMonad<TSelf, T>
    where TSelf : IMonad<TSelf, T>
{
    static abstract TSelf Return(T value);
    // abstract TSelf<R> SelectMany<R>(Func<T, TSelf<R>> f); // this code is incorrect
}

class Optional<T> : IMonad<Optional<T>, T>
{
    public Optional(T value) { }

    public static Optional<T> Return(T value) => new(value);
}

