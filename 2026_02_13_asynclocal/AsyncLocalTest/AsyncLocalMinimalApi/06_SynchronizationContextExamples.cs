
using System;
using System.Threading;
using System.Threading.Tasks;

// https://devblogs.microsoft.com/dotnet/executioncontext-vs-synchronizationcontext/
// https://devblogs.microsoft.com/dotnet/how-async-await-really-works/
// https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md#warning-deadlocks

class MySynchronizationContext : SynchronizationContext
{
    public override void Post(SendOrPostCallback d, object? state)
    {
        Console.WriteLine("SynchronizationContext.Post (async) -> ");
        var capturedContext = this; // Capture the current context
        new Thread(obj =>
        {
            SynchronizationContext.SetSynchronizationContext(capturedContext);
            d(obj);
        }).Start(state);
        //d(state);
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        Console.WriteLine("SynchronizationContext.Send (sync) -> ");
        d(state);
    }
}

class SynchronizationContextExamples
{
    static AsyncLocal<string> _asyncLocalString = new AsyncLocal<string>();

    private static void Log(string text)
    {
        var currentState = new
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId,
            ExecContext = ExecutionContext.Capture()?.GetHashCode(),
            SyncContext = SynchronizationContext.Current?.GetHashCode(),
            AsyncLocalValue = _asyncLocalString.Value
        };
        Console.WriteLine($"{currentState} - {text}");
    }

    static void SyncMethod1()
    {
        _ = AsyncMethod1();
    }

    static async Task AsyncMethod1()
    {
        Log("before async method2");
        await AsyncMethod2();
        Log("after async method2");
    }


    static async Task AsyncMethod2()
    {
        Log("async method2 called");

        await Task.Delay(100);
        Log("async method2 called after delay 100");

        await Task.Delay(100).ConfigureAwait(false);
        Log("async method2 called after delay 100 with .ConfigureAwait(false)");

        await Task.Delay(100);
        Log("async method2 called after delay 100");
    }


    public static void Run()
    {
        SynchronizationContext.SetSynchronizationContext(new MySynchronizationContext());
        SyncMethod1();
    }
}
