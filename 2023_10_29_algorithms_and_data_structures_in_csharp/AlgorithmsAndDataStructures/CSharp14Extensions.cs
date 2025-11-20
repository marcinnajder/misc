namespace AlgorithmsAndDataStructures;

using static EnumerableExtensions;
using System.Numerics;

// https://devblogs.microsoft.com/dotnet/introducing-csharp-14/
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-14.0/extensions
// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-14.0/extension-operators

public static class EnumerableExtensions
{
    public static void Run()
    {
        var items1 = new List<string> { "a", "b", "c" };
        var items2 = new List<string> { "d", "e", "f" };

        // ** plus operator

        IEnumerable<string> items3 = items1 + items2;
        Console.WriteLine(new { JoinAll = items3.JoinAll(), IsEmpty = items3.IsEmpty });


        // statuc
        var items33 = EnumerableExtensions.op_Addition(items1, items2);
        var aa = EnumerableExtensions.get_IsEmpty(items1); // property


        string a = JoinAll(new[] { 1, 2, 3 });
        var items333 = op_Addition(items1, items2); // extension
    }

    extension<T>(IEnumerable<T> source)
    {
        public bool IsEmpty => !source.Any();

        public string JoinAll() => string.Join(",", source);

        public static IEnumerable<T> Identity => Enumerable.Empty<T>();

        public static IEnumerable<T> operator +(IEnumerable<T> left, IEnumerable<T> right) => left.Concat(right);
    }
}

public static class PipeExtensions
{
    public static Func<IEnumerable<T>, IEnumerable<R>> Select<T, R>(Func<T, R> func) => items => items.Select(func);

    public static Func<IEnumerable<T>, IEnumerable<T>> Where<T, R>(Func<T, bool> func) => items => items.Where(func);

    public static void Run()
    {
        // ** pipe
        string valPipe = 10 | Inc | Inc | ToString;
        Console.WriteLine(new { valPipe });

        var ignore = "marcin najder"
            | Where<char, bool>(x => x != ' ')
            | Select<char, char>(char.ToUpper)
            | (e => string.Join(",", e))
            | WriteLine;

        // ** compose
        Func<int, int> inc = Inc;

        //var func0 = Inc >> Inc; // Operator '>>' cannot be applied to operands of type 'method group' and 'method group'
        var func1 = inc >> inc;
        var func2 = inc >> Inc;

        var valCompose = func1(10);
        Console.WriteLine(new { valCompose });
    }


    private static int Inc(int i) => i + 1;

    private static string? ToString<T>(T value) => value?.ToString();

    private static object? WriteLine<T>(T value)
    {
        Console.WriteLine(value);
        return null;
    }

    extension<T, R>(T)
    {
        public static R operator |(T t, Func<T, R> func)
        {
            return func(t);
        }
    }

    extension<T1, T2, T3>(Func<T1, T2>)
    {
        public static Func<T1, T3> operator >>(Func<T1, T2> func1, Func<T2, T3> func2)
        {
            return arg => func2(func1(arg));
        }
    }
}

public static class OtherExtension
{
    public static void Run()
    {
        var action = () => 1;
        var aaa = action.Call();

        // var aaa2 = (() => 132).Call();
        var aaa3 = CallS(() => 132);

        var fullPath = "part1" / "part2" / "test.txt"; // "part1/part2/test.txt"
        Console.WriteLine(new { fullPath });

    }

    extension<R>(Func<R> func)
    {
        public R Call() => func();
    }
    extension<R>(Func<R>)
    {
        public static R CallS(Func<R> func) => func();
    }

    // https://andrewlock.net/exploring-dotnet-10-preview-features-3-csharp-14-extensions-members/
    extension(string)
    {
        public static string operator /(string left, string right) => Path.Combine(left, right);
    }
}