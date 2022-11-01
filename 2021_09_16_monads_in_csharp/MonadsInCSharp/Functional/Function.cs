
namespace MonadsInCSharp;

public static class Function
{
    public static R Pipe<T, R>(this T value, Func<T, R> func) => func(value);
    public static void Pipe<T>(this T value, Action<T> func) => func(value);

    public static Func<T, R> Compose<T, TR, R>(this Func<T, TR> func1, Func<TR, R> func2) => t => func2(func1(t));
    public static Action<T> Compose<T, TR>(this Func<T, TR> func1, Action<TR> func2) => t => func2(func1(t));

    public static Func<T2, T1, R> Reverse<T1, T2, R>(this Func<T1, T2, R> f)
        => (arg1, arg2) => f(arg2, arg1);

    // * Curry

    public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(Func<T1, T2, R> f)
           => arg1 => arg2 => f(arg1, arg2);

    public static Func<T1, Func<T2, Func<T3, R>>> Curry<T1, T2, T3, R>(Func<T1, T2, T3, R> f)
           => arg1 => arg2 => arg3 => f(arg1, arg2, arg3);

    public static Func<T1, Func<T2, Func<T3, Func<T4, R>>>> Curry<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f)
        => arg1 => arg2 => arg3 => arg4 => f(arg1, arg2, arg3, arg4);


    public static Func<T1, Action<T2>> Curry<T1, T2>(Action<T1, T2> f)
           => arg1 => arg2 => f(arg1, arg2);

    public static Func<T1, Func<T2, Action<T3>>> Curry<T1, T2, T3>(Action<T1, T2, T3> f)
           => arg1 => arg2 => arg3 => f(arg1, arg2, arg3);

    public static Func<T1, Func<T2, Func<T3, Action<T4>>>> Curry<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f)
        => arg1 => arg2 => arg3 => arg4 => f(arg1, arg2, arg3, arg4);


    // * Apply

    // ** Func<T1, T2, R>, Action<T1, T2>

    public static Func<T2, R> Apply<T1, T2, R>(Func<T1, T2, R> f, T1 arg1)
        => arg2 => f(arg1, arg2);

    public static Action<T2> Apply<T1, T2>(Action<T1, T2> f, T1 arg1)
        => arg2 => f(arg1, arg2);

    // ** Func<T1, T2, T3, R>
    public static Func<T2, T3, R> Apply<T1, T2, T3, R>(Func<T1, T2, T3, R> f, T1 arg1)
        => (arg2, arg3) => f(arg1, arg2, arg3);

    public static Func<T3, R> Apply<T1, T2, T3, R>(Func<T1, T2, T3, R> f, T1 arg1, T2 arg2)
        => arg3 => f(arg1, arg2, arg3);

    public static Action<T2, T3> Apply<T1, T2, T3>(Action<T1, T2, T3> f, T1 arg1)
        => (arg2, arg3) => f(arg1, arg2, arg3);

    public static Action<T3> Apply<T1, T2, T3>(Action<T1, T2, T3> f, T1 arg1, T2 arg2)
        => arg3 => f(arg1, arg2, arg3);


    // ** Func<T1, T2, T3, T4, R>, Action<T1, T2, T3, T4>

    public static Func<T2, T3, T4, R> Apply<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f, T1 arg1)
        => (arg2, arg3, arg4) => f(arg1, arg2, arg3, arg4);

    public static Func<T3, T4, R> Apply<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f, T1 arg1, T2 arg2)
        => (arg3, arg4) => f(arg1, arg2, arg3, arg4);

    public static Func<T4, R> Apply<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> f, T1 arg1, T2 arg2, T3 arg3)
        => arg4 => f(arg1, arg2, arg3, arg4);

    public static Action<T2, T3, T4> Apply<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f, T1 arg1)
        => (arg2, arg3, arg4) => f(arg1, arg2, arg3, arg4);

    public static Action<T3, T4> Apply<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f, T1 arg1, T2 arg2)
        => (arg3, arg4) => f(arg1, arg2, arg3, arg4);

    public static Action<T4> Apply<T1, T2, T3, T4>(Action<T1, T2, T3, T4> f, T1 arg1, T2 arg2, T3 arg3)
        => arg4 => f(arg1, arg2, arg3, arg4);
}
