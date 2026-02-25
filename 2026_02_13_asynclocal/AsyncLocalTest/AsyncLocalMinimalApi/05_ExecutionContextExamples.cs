
using System;
using System.Threading;
using System.Threading.Tasks;

// https://devblogs.microsoft.com/dotnet/executioncontext-vs-synchronizationcontext/ś
class ExecutionContextExamples
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
        Log("starting");

        _asyncLocalString.Value = "abc";
        Log("async value changed");

        Task.Run(() =>
        {
            Log("new thread called");

            _asyncLocalString.Value = "def";
            Log("new thread called after async local changed");
        });

        Thread.Sleep(1000);

        Log("after new thread");

        AsyncMethod1().Wait();

        Log("after async method");
    }

    static void SyncMethod2()
    {
        Log("sync method called");
    }

    static async Task AsyncMethod1()
    {
        Log("async method called");

        await Task.Delay(100);
        Log("async method called after delay 100");

        _asyncLocalString.Value = "ghi";
        Log("async method called after async local changed");
    }

    public static void Run()
    {
        SyncMethod1();
    }
}
