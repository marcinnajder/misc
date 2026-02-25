using System;
using System.Threading;
using System.Threading.Tasks;

// https://learn.microsoft.com/pl-pl/dotnet/api/system.threading.asynclocal-1?view=net-10.0
class ThreadLocalVsAsyncLocal
{
    static AsyncLocal<string> _asyncLocalString = new AsyncLocal<string>();

    static ThreadLocal<string> _threadLocalString = new ThreadLocal<string>();

    [ThreadStatic]
    static string _threadLocalString2 = "_";

    static async Task AsyncMethodA()
    {
        // Start multiple async method calls, with different AsyncLocal values.
        // We also set ThreadLocal values, to demonstrate how the two mechanisms differ.
        _asyncLocalString.Value = "Value 1";
        _threadLocalString.Value = "Value 1";
        _threadLocalString2 = "Value 1";
        var t1 = AsyncMethodB("Value 1");

        // _asyncLocalString.Value = "Value 2";
        // _threadLocalString.Value = "Value 2";
        // _threadLocalString2 = "Value 2";
        // var t2 = AsyncMethodB("Value 2");

        // Await both calls
        await t1;
        //await t2;
    }

    static async Task AsyncMethodB(string expectedValue)
    {
        Console.WriteLine("Entering AsyncMethodB (thread id: {0})", Thread.CurrentThread.ManagedThreadId);
        Console.WriteLine("   Expected '{0}', AsyncLocal value is '{1}', ThreadLocal value is '{2}' '{3}'",
            expectedValue, _asyncLocalString.Value, _threadLocalString.Value, _threadLocalString2);

        await Task.Delay(100);

        Console.WriteLine("Exiting AsyncMethodB (thread id: {0})", Thread.CurrentThread.ManagedThreadId);
        Console.WriteLine("   Expected '{0}', got '{1}', ThreadLocal value is '{2}' '{3}'",
            expectedValue, _asyncLocalString.Value, _threadLocalString.Value, _threadLocalString2);
    }

    public static async Task Run()
    {
        await AsyncMethodA();
    }
}

// The example displays the following output:
//   Entering AsyncMethodB.
//      Expected 'Value 1', AsyncLocal value is 'Value 1', ThreadLocal value is 'Value 1'
//   Entering AsyncMethodB.
//      Expected 'Value 2', AsyncLocal value is 'Value 2', ThreadLocal value is 'Value 2'
//   Exiting AsyncMethodB.
//      Expected 'Value 2', got 'Value 2', ThreadLocal value is ''
//   Exiting AsyncMethodB.
//      Expected 'Value 1', got 'Value 1', ThreadLocal value is ''