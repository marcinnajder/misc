using System.Collections;

using static MonadsInCSharp.Function;
using static MonadsInCSharp.Monad;

namespace MonadsInCSharp;

public static class FunctionalConceptsSamples
{
    static int Add2(int a, int b) => a + b;
    static int Add3(int a, int b, int c) => a + b + c;
    static void WriteLine2(int a, int b) => Console.WriteLine(new { a, b });
    static void WriteLine3(int a, int b, int c) => Console.WriteLine(new { a, b, c });

    public static void Currying()
    {
        // C# nie radzi sobie z wnioskowaniem typow :( musimy jawnie podac typy generyczne
        Func<int, Func<int, int>> Add2C = Curry<int, int, int>(Add2);
        Console.WriteLine(Add2C(1)(2)); // -> 3

        // F#
        // let add a b = a + b;
        // "add 1 2"  == "(add 1) 2"

        Func<int, Action<int>> WriteLine2C = Curry<int, int>(WriteLine2);
        WriteLine2C(1)(2); // -> { a = 1, b = 2 }
    }

    public static void PartialFunctionApplication()
    {

        Func<int, int> Inc = Apply<int, int, int>(Add2, 1);
        Console.WriteLine(Inc(10)); // -> 1 + 10 = 11

        // F#
        // let add a b = a + b;
        // let inc = add 1;
        // List.map (fun x -> add 1 x) [1;2;3] 
        // List.map (add 1) [1;2;3]
        // List.map inc [1;2;3] 

        Func<int, int, int> Add10 = Apply<int, int, int, int>(Add3, 10);
        Console.WriteLine(Add10(1, 2)); // -> 10 + 1 + 2 = 13

        Func<int, int> Add10And100 = Apply<int, int, int, int>(Add3, 10, 100);
        Console.WriteLine(Add10And100(1)); // -> 10 + 100 + 1 = 111
    }

    static Optional<int> TryParseInt(string text) =>
        int.TryParse(text, out var result) ? new Optional<int>(result) : Optional<int>.None;

    static string OptionalToString<T>(Optional<T> optional) => !optional.HasValue ? "None" :
        $"Some({optional.Value switch { IEnumerable items => string.Join(",", items.OfType<object>()), var item => item }})";

    static Task<T> GetDataAsync<T>(T data) => Task.Delay(10).ContinueWith(_ => data);

    static IEnumerable<T> AsEnumerable<T>(params T[] items) => items;


    public static void EnumerableMonad()
    {
        IEnumerable<int> q =
            from x in new[] { 1, 2 }
            let z = x * 10
            from y in new[] { 5, 6, 7 }
            select z + y; // -> 15, 16, 17, 25, 26, 27

        // zamieniony jest na
        q = new[] { 1, 2 }
            .Select(x => x * 10)
            .SelectMany(z => new[] { 5, 6, 7 }, (z, y) => z + y);

        // co mozemy zapisac jako
        q = new[] { 1, 2 }
            .Select(x => x * 10)
            .SelectMany(z => new[] { 5, 6, 7 }.Select(y => z + y));

        foreach (var result in q)
        {
            Console.WriteLine(string.Join(",", result));
        }
    }

    public static void OptionalMonad()
    {
        Optional<int> optional =
            from x in TryParseInt("1")
            let z = x * 10
            from y in TryParseInt("2")
            select z + y;

        optional = TryParseInt("1")
            .Select(x => x * 10)
            .SelectMany(z => TryParseInt("2"), (z, y) => z + y);

        optional = TryParseInt("1")
            .Select(x => x * 10)
            .SelectMany(z => TryParseInt("2").Select(y => z + y));

        Console.WriteLine(OptionalToString(optional));
    }


    public static void TaskMonad()
    {
        Task<int> task =
            from x in GetDataAsync(1)
            let z = x * 10
            from y in GetDataAsync(2)
            select z + y;

        task = GetDataAsync(1)
            .Select(x => x * 10)
            .SelectMany(z => GetDataAsync(2).Select(y => z + y));

        task = GetDataAsync(1)
            .Select(x => x * 10)
            .SelectMany(z => GetDataAsync(2), (z, y) => z + y);

        Console.WriteLine(task.Result);
    }

    public static void AggregateMSelectMWhereM()
    {
        {
            Console.WriteLine("SelectM");

            // M<R[]> SelectM<T, R>(T[] ms, Func<T, M<R>> f);

            Optional<int[]> o1 = new[] { "1", "2", "3" }.SelectM(x => TryParseInt(x)); // Some([1,2,3])
            Console.WriteLine(OptionalToString(o1));

            Optional<int[]> o2 = new[] { "1", "2", "3a" }.SelectM(x => TryParseInt(x)); // None
            Console.WriteLine(OptionalToString(o2));

            Task<int[]> t = new[] { "1", "2", "3" }.SelectM(x => GetDataAsync(int.Parse(x))); // Task<[1,2,3]>
            Console.WriteLine(string.Join(",", t.Result));
        }

        {

            Console.WriteLine("WhereM");

            // M<T[]> WhereM<T>(T[] ms, Func<T, M<bool>> f);

            Optional<string[]> o1 = new[] { "1", "2", "3", "4" }.WhereM(x => TryParseInt(x).Select(xx => xx % 2 == 0)); // Some([2,4])
            Console.WriteLine(OptionalToString(o1));

            Optional<string[]> o2 = new[] { "1", "2", "3a", "4" }.WhereM(x => TryParseInt(x).Select(xx => xx % 2 == 0)); // None
            Console.WriteLine(OptionalToString(o2));

            Task<int[]> t = new[] { 1, 2, 3, 4 }.WhereM(x => GetDataAsync(x).Select(xx => xx % 2 == 0)); // Task<[2,4]>
            Console.WriteLine(string.Join(",", t.Result));
        }

        {
            Console.WriteLine("AggregateM");

            // M<A> AggregateM<T, A>(T[] ms, A seed, Func<A, T, M<A>> f);

            Optional<int> o1 = new[] { "1", "2", "3" }.AggregateM(0, (a, c) => TryParseInt(c).Select(v => a + v)); // Some(6)
            Console.WriteLine(OptionalToString(o1));

            Optional<int> o2 = new[] { "1", "2", "3a" }.AggregateM(0, (a, c) => TryParseInt(c).Select(v => a + v)); // None
            Console.WriteLine(OptionalToString(o2));

            Task<int> t = new[] { 1, 2, 3 }.AggregateM(0, (a, c) => GetDataAsync(c).Select(v => a + v));// Task<6>
            Console.WriteLine(t.Result);
        }
    }

    // Optional

    public static Func<Optional<T>, Optional<R>> MapO<T, R>(Func<T, R> f) => input => input.Select(f);
    public static Func<Optional<T>, Optional<R>> Bind<T, R>(Func<T, Optional<R>> f) => input => input.SelectMany(f);
    public static Func<Optional<T>, Optional<R>> Apply<T, R>(Optional<Func<T, R>> f) => input => f.Apply(input);

    // Task
    public static Func<Task<T>, Task<R>> MapT<T, R>(Func<T, R> f) => input => input.Select(f);
    public static Func<Task<T>, Task<R>> Bind<T, R>(Func<T, Task<R>> f) => input => input.SelectMany(f);
    public static Func<Task<T>, Task<R>> Apply<T, R>(Task<Func<T, R>> f) => input => f.Apply(input);

    public static Func<IEnumerable<T>, IEnumerable<R>> MapI<T, R>(Func<T, R> f) => input => input.Select(f);
    public static Func<IEnumerable<T>, IEnumerable<R>> Bind<T, R>(Func<T, IEnumerable<R>> f) => input => input.SelectMany(f);

    // w C# nie da sie zdefiniowac zmiennej/pola typu generycznego na zasadzie "Func<T,T> identity = Identity"
    // poniewa typ T musi byc dostarczony przez zewnetrzna klase/metode, dlatego nizej kazde wywolanie
    // funkcji od nowa wykonuje "Reverse -> Curry" dla Map. Teoretycznie aby tego nie robic za kazdym
    // razem mozna wprowadzic "static Delegate OptionMapCache"
    public static Func<Optional<T>, Optional<R>> Map2<T, R>(Func<T, R> f)
        => f.Pipe(Function.Reverse<Optional<T>, Func<T, R>, Optional<R>>(Monad.Select).Pipe(Function.Curry));


    public static void Apply()
    {
        {
            var optional1 = new Optional<Func<int, int>>(x => x + 1).Apply(new Optional<int>(10));
            Console.WriteLine(OptionalToString(optional1));

            var optional2 = new Optional<Func<int, int>>(x => x + 1).Apply(new Optional<int>());
            Console.WriteLine(OptionalToString(optional2));
        }

        {
            var task = Task.FromResult<Func<int, int>>(x => x + 1).Apply(Task.FromResult(10));
            Console.WriteLine(task.Result);
        }
    }



    public static void PipeAndCompose()
    {
        int value = 1                   // int              (1)
            .Pipe(Increment)            // int -> int       (2)
            .Pipe(IntToString)          // int -> string    ("2")
            .Pipe(s => s + s)           // string -> string ("22")
            .Pipe(int.Parse);           // string -> int    (22)

        Console.WriteLine(value);       // -> 22

        var func = Function
            .Compose<int, int, string>(Increment, IntToString)
            .Compose(s => s + s)
            .Compose(int.Parse);        // -> 22

        Console.WriteLine(func(1)); // -> 22

        // funkcje pomocnicze
        int Increment(int a) => a + 1;
        string IntToString(int a) => a.ToString();
        // int Add(int a, int b) => a + b;
    }

    public static void Lifters()
    {
        int n1 = 1                                      // int              (1)
            .Pipe(Increment)                            // int -> int       (2)
            .Pipe(IntToString)                          // int -> string    ("2")
            .Pipe(s => s + s)                           // string -> string ("22")
            .Pipe(int.Parse);                           // string -> int    (22)


        Optional<int> n2 = TryParseInt("1")             // Optional<int>
            .Pipe(MapO<int, int>(Increment))            // Optional<int> -> Optional<int>
            .Pipe(MapO<int, string>(IntToString))       // Optional<int> -> Optional<string>
            .Pipe(MapO<string, string>(s => s + s))     // Optional<string> -> Optional<string>
            .Pipe(Bind<string, int>(TryParseInt));      // Optional<string> -> Optional<int>

        Console.WriteLine(OptionalToString(n2));

        int Increment(int a) => a + 1;
        string IntToString(int a) => a.ToString();
        // int Add(int a, int b) => a + b;
    }
}
