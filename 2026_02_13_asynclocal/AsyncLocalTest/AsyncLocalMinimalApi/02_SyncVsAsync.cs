
// https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md#avoid-setting-asynclocalt-values-outside-of-async-methods

class SyncVsAsync
{
    public static void RunSync()
    {
        var local = new AsyncLocal<int>();
        MethodA();
        Console.WriteLine(local.Value);
        Console.WriteLine();

        void MethodA()
        {
            local.Value = 1;
            MethodB();
            Console.WriteLine(local.Value);
        }

        void MethodB()
        {
            local.Value = 2;
            Console.WriteLine(local.Value);
        }
    }

    public static async Task RunAsync()
    {
        var local = new AsyncLocal<int>();
        await MethodA();
        Console.WriteLine(local.Value);
        Console.WriteLine();

        async Task MethodA()
        {
            local.Value = 1;
            await MethodB();
            Console.WriteLine(local.Value);
        }

        async Task MethodB()
        {
            local.Value = 2;
            Console.WriteLine(local.Value);
        }
    }

}
