using static MonadsInCSharp.Function;

namespace MonadsInCSharp;

public static class Monad
{
    // ** Return

    public static Optional<T> ReturnO<T>(this T value) => new Optional<T>(value);
    public static Task<T> ReturnT<T>(this T value) => Task.FromResult(value);
    public static T[] ReturnA<T>(this T value) => new[] { value };
    public static IEnumerable<T> ReturnE<T>(this T value) => new[] { value };
    public static IO<T> ReturnIO<T>(this T value) => () => value;
    public static TTask<T> ReturnTT<T>(this T value) => new TTask<T>(ReturnT(value));

    // ** Select, SelectMany, Apply

    // public static IEnumerable<R> Select<T, R>(this IEnumerable<T> enumerable, Func<T, R> f)
    // {
    //     foreach (var item in enumerable)
    //     {
    //         yield return f(item);
    //     }
    // }

    // public static IEnumerable<R> SelectMany<T, R>(this IEnumerable<T> enumerable, Func<T, IEnumerable<R>> f)
    // {
    //     foreach (var item in enumerable)
    //     {
    //         foreach (var subitem in f(item))
    //         {
    //             yield return subitem;
    //         }
    //     }
    // }

    public static Optional<R> Select<T, R>(this Optional<T> optional, Func<T, R> f)
        => optional.HasValue ? new Optional<R>(f(optional.Value)) : Optional<R>.None;

    public static Optional<R> SelectMany<T, R>(this Optional<T> optional, Func<T, Optional<R>> f)
        => optional.HasValue ? f(optional.Value) : Optional<R>.None;

    public static Optional<R> Apply<T, R>(this Optional<Func<T, R>> f, Optional<T> optional)
        => (optional, f) switch
        {
            ({ HasValue: true }, { HasValue: true }) => new Optional<R>(f.Value(optional.Value)),
            _ => new Optional<R>()
        };


    public static Task<R> Select<T, R>(this Task<T> task, Func<T, R> f)
            => task.ContinueWith(t => f(t.Result));

    public static Task<R> SelectMany<T, R>(this Task<T> task, Func<T, Task<R>> f)
        => task.ContinueWith(t => f(t.Result)).Unwrap();

    public static Task<R> Apply<T, R>(this Task<Func<T, R>> f, Task<T> task)
        => Task.WhenAll(task, f).ContinueWith(_ => f.Result(task.Result));



    public static R[] Select<T, R>(this T[] source, Func<T, R> f)
        => Array.ConvertAll<T, R>(source, x => f(x));

    public static R[] SelectMany<T, R>(this T[] source, Func<T, R[]> f)
        => source.Aggregate(Enumerable.Empty<R>(), (p, c) => p.Concat(f(c)), result => result.ToArray());

    public static R[] Apply<T, R>(this Func<T, R>[] f, T[] source) => f.AsEnumerable().Apply(source).ToArray();

    public static IEnumerable<R> Apply<T, R>(this IEnumerable<Func<T, R>> f, IEnumerable<T> source)
    {
        foreach (var ff in f)
        {
            foreach (var item in source)
            {
                yield return ff(item);
            }
        }
    }

    // Nullable<T> wymaga aby T byl value type a to jest bardzo ograniczajace, bo taki Select moze 
    // konwertowac do dowolnego typy lub taka metoda Apply powinna przykac Nullable<Func<T, R>> i kod sie nie kompiluje

    public static Nullable<R> Select<T, R>(this Nullable<T> optional, Func<T, R> f) where T : struct where R : struct
        => optional.HasValue ? new Nullable<R>(f(optional.Value)) : new Nullable<R>();

    public static Nullable<R> SelectMany<T, R>(this Nullable<T> optional, Func<T, Nullable<R>> f) where T : struct where R : struct
        => optional.HasValue ? f(optional.Value) : new Nullable<R>();

    // ** SelectMany ( ... , ...) -> generated

    public static IEnumerable<R> SelectMany<T, TT, R>(this IEnumerable<T> m, Func<T, IEnumerable<TT>> f, Func<T, TT, R> r)
        => m.SelectMany(v => f(v).Select(vv => r(v, vv)));

    public static Optional<R> SelectMany<T, TT, R>(this Optional<T> m, Func<T, Optional<TT>> f, Func<T, TT, R> r)
        => m.SelectMany(v => f(v).Select(vv => r(v, vv)));

    public static Task<R> SelectMany<T, TT, R>(this Task<T> m, Func<T, Task<TT>> f, Func<T, TT, R> r)
        => m.SelectMany(v => f(v).Select(vv => r(v, vv)));

    public static R[] SelectMany<T, TT, R>(this T[] m, Func<T, TT[]> f, Func<T, TT, R> r)
        => m.SelectMany(v => f(v).Select(vv => r(v, vv)));


    // ** AggregateM, SelectM, FilterM -> generated*

    public static Optional<A> AggregateM<T, A>(this IEnumerable<T> ms, A seed, Func<A, T, Optional<A>> f)
        => ms.Aggregate(ReturnO(seed), (a, c) => a.SelectMany(v => f(v, c)));

    public static Optional<R[]> SelectM<T, R>(this IEnumerable<T> ms, Func<T, Optional<R>> f)
        => ms.AggregateM(Enumerable.Empty<R>(), (a, c) => f(c).Select(v => a.Concat(new[] { v })))
                .Select(v => v.ToArray());

    public static Optional<T[]> WhereM<T>(this IEnumerable<T> ms, Func<T, Optional<bool>> f)
        => ms.AggregateM(Enumerable.Empty<T>(), (a, c) => f(c).Select(v => v ? a.Concat(new[] { c }) : a))
                .Select(v => v.ToArray());


    public static Task<A> AggregateM<T, A>(this IEnumerable<T> ms, A seed, Func<A, T, Task<A>> f)
        => ms.Aggregate(ReturnT(seed), (a, c) => a.SelectMany(v => f(v, c)));

    public static Task<R[]> SelectM<T, R>(this IEnumerable<T> ms, Func<T, Task<R>> f)
        => ms.AggregateM(Enumerable.Empty<R>(), (a, c) => f(c).Select(v => a.Concat(new[] { v })))
                .Select(v => v.ToArray());

    public static Task<T[]> WhereM<T>(this IEnumerable<T> ms, Func<T, Task<bool>> f)
        => ms.AggregateM(Enumerable.Empty<T>(), (a, c) => f(c).Select(v => v ? a.Concat(new[] { c }) : a))
                .Select(v => v.ToArray());



    /// LiftA - generated*

    public static Optional<R> LiftA<T1, T2, R>(Func<T1, T2, R> f, Optional<T1> arg1, Optional<T2> arg2)
        => arg1.Select(Curry<T1, T2, R>(f)).Apply(arg2);

    public static Optional<R> LiftA<T1, T2, T3, R>(Func<T1, T2, T3, R> f, Optional<T1> arg1, Optional<T2> arg2, Optional<T3> arg3)
        => arg1.Select(Curry<T1, T2, T3, R>(f)).Apply(arg2).Apply(arg3);

    public static Task<R> LiftA<T1, T2, R>(Func<T1, T2, R> f, Task<T1> arg1, Task<T2> arg2)
    => arg1.Select(Curry<T1, T2, R>(f)).Apply(arg2);

    public static Task<R> LiftA<T1, T2, T3, R>(Func<T1, T2, T3, R> f, Task<T1> arg1, Task<T2> arg2, Task<T3> arg3)
        => arg1.Select(Curry<T1, T2, T3, R>(f)).Apply(arg2).Apply(arg3);

    /// LiftM - - generated*

    public static Optional<R> LiftM<T1, T2, R>(Func<T1, T2, R> f, Optional<T1> arg1, Optional<T2> arg2)
        => arg1.SelectMany(val1 => arg2.Select(val2 => f(val1, val2)));

    public static Optional<R> LiftM<T1, T2, T3, R>(Func<T1, T2, T3, R> f, Optional<T1> arg1, Optional<T2> arg2, Optional<T3> arg3)
        => arg1.SelectMany(val1 => arg2.SelectMany(val2 => arg3.Select(val3 => f(val1, val2, val3))));

    public static Task<R> LiftM<T1, T2, R>(Func<T1, T2, R> f, Task<T1> arg1, Task<T2> arg2)
        => arg1.SelectMany(val1 => arg2.Select(val2 => f(val1, val2)));

    public static Task<R> LiftM<T1, T2, T3, R>(Func<T1, T2, T3, R> f, Task<T1> arg1, Task<T2> arg2, Task<T3> arg3)
        => arg1.SelectMany(val1 => arg2.SelectMany(val2 => arg3.Select(val3 => f(val1, val2, val3))));

}
