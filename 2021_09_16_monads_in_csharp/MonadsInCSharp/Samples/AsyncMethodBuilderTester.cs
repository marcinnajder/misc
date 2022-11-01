
namespace MonadsInCSharp;

static class AsyncMethodBuilderTester
{
    static Optional<int> TryParseInt(string text) =>
        int.TryParse(text, out var result) ? new Optional<int>(result) : Optional<int>.None;


    public static void OptionalSpecificTest()
    {
        var a1 = ProcessText1("1", "2");
        Console.WriteLine(a1.HasValue);

        var a2 = ProcessText2("1", "2");
        Console.WriteLine(a2.Value);


        static Optional<string> ProcessText1(string text1, string text2)
        {
            Optional<int> number1 = TryParseInt(text1);
            if (!number1.HasValue) return new Optional<string>();

            Optional<int> number2 = TryParseInt(text2);
            if (!number2.HasValue) return new Optional<string>();

            return new Optional<string>((number1.Value + number2.Value).ToString());
        }

        // funkcja wykorzystujaca async/await dla typu Optional<T> :)
        static async Optional<string> ProcessText2(string text1, string text2)
        {
            int number1 = await TryParseInt(text1);
            int number2 = await TryParseInt(text2);

            return (number1 + number2).ToString();
        }
    }

    public static void OptionalTest()
    {
        var aa = RunAsync();

        Console.WriteLine(new { aa.Value, aa.HasValue });

        async Optional<string> RunAsync()
        {
            int a = await new Optional<int>(1);
            int b = await new Optional<int>(2);
            //int b = await new Optional<int>();
            return (a + b).ToString();
        }
    }

    public static void IOlWithAsyncAwaitTest()
    {
        var aa = RunAsync();

        Console.WriteLine(aa());

        async IO<string> RunAsync()
        {
            int a = await new IO<int>(() => 1);
            int b = await new IO<int>(() => 2);
            return (a + b).ToString();
        }
    }

    public static void TTaskAsyncAwaitTest()
    {
        RunAsync().Task.ContinueWith(tt => Console.WriteLine(tt.Result));

        // Console.ReadLine(); // wait few seconds to complete

        async TTask<string> RunAsync()
        {
            int a = await new TTask<int>(Task.Delay(1000).ContinueWith(_ => 1));
            int b = await new TTask<int>(Task.Delay(1000).ContinueWith(_ => 2));
            return (a + b).ToString();
        }
    }
}
