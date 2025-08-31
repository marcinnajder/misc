
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MonadsInCSharp;

static class AsyncMethodBuilderTester
{
    static Optional<int> TryParseInt(string text) =>
        int.TryParse(text, out var result) ? new Optional<int>(result) : Optional<int>.None;


    public static void OptionalSpecificTest()
    {
        var a1 = ProcessText1("1", "2");
        Console.WriteLine(a1.Value);

        var a2 = ProcessText2("1", "2");
        Console.WriteLine(a2.Value);

        var a3 = ProcessText3("1", "2");
        Console.WriteLine(a3.Value);


        static Optional<string> ProcessText1(string text1, string text2)
        {
            Optional<int> number1 = TryParseInt(text1);
            if (!number1.HasValue)
            {
                return new Optional<string>();
            }

            Optional<int> number2 = TryParseInt(text2);
            if (!number2.HasValue)
            {
                return new Optional<string>();
            }

            return new Optional<string>((number1.Value + number2.Value).ToString());
        }

        // funkcja wykorzystujaca async/await dla typu Optional<T> :)
        static async Optional<string> ProcessText2(string text1, string text2)
        {
            int number1 = await TryParseInt(text1);
            int number2 = await TryParseInt(text2);

            return (number1 + number2).ToString();
        }

        static Optional<string> ProcessText3(string text1, string text2)
            => TryParseInt(text1).SelectMany(number1 =>
                TryParseInt(text2).Select(number2 => (number1 + number2).ToString()));
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

    public static void EnumerableWithTest()
    {
        // nie dziala to poprawnie dla monad ktore w SelectMany wieloktornie wolaja funkcje 'f' (IEnumerable, IObservable, ...),
        // objaw jest taki ze generalnie leci stack overflow, wyjasnienie w pliku 'opisane_problemy.cs'

        var result = Run();

        // stack overflow:)
        //var bb = aa.ToList(); 

        // dla okreslonej ilosci elementow dziala, zwraca tylko pierwszy element czyli 5,5,5,5,...
        var bb = result.Take(10).ToList();
        Console.WriteLine(string.Join(',', bb));

        [AsyncMethodBuilder(typeof(IEnumerableMethodBuilder<>))]
        async IEnumerable<string> Run()
        {
            IEnumerable<int> items = new[] { 5, 10, 15 };
            int item = await items;
            return item + " `zl";
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

        Console.ReadLine(); // wait few seconds to complete

        async TTask<string> RunAsync()
        {
            int a = await new TTask<int>(Task.Delay(1000).ContinueWith(_ => 1));
            int b = await new TTask<int>(Task.Delay(1000).ContinueWith(_ => 2));
            return (a + b).ToString();
        }
    }


    public static void TaskAsyncAwaitTest()
    {
        // dla wbudowanego Task nie dziala poniewaz domyslmy TaskAwaiter jest zawsze brany,
        // zwracany jest zawsze null zamist Task, wyjasnienie 'opisane_problemy.txt'

        var aa = RunAsync();
        //var aa = BrokenTask(); 

        Debugger.Break();

        [AsyncMethodBuilder(typeof(TaskMethodBuilder<>))]
        static async Task<string> RunAsync()
        {
            int a = await Task.Delay(1000).ContinueWith(_ => 1);
            int b = await Task.Delay(1000).ContinueWith(_ => 2);
            return (a + b).ToString();
            // return "mama"; // sam rezultat bez zadnego await dziala poprawnie
        }


        [AsyncMethodBuilder(typeof(TaskMethodBuilder<>))]
        static Task<string> BrokenTask()
        {
            BrokenTaskStateMachine stateMachine = new BrokenTaskStateMachine();
            stateMachine.builder = TaskMethodBuilder<string>.Create();
            stateMachine.state = -1;
            stateMachine.builder.Start(ref stateMachine);
            return stateMachine.builder.Task;
        }
    }




    public static void OptionalDisassembledTest()
    {
        var a1 = ProcessText3("1", "2");
        Console.WriteLine(a1.HasValue);
        Console.WriteLine(a1.Value);

        static Optional<string> ProcessText3(string text1, string text2)
        {
            ProcessText2StateMachine stateMachine = new ProcessText2StateMachine();
            stateMachine.builder = OptionalMethodBuilder<string>.Create();
            stateMachine.text1 = text1;
            stateMachine.text2 = text2;
            stateMachine.state = -1;
            stateMachine.builder.Start(ref stateMachine);
            return stateMachine.builder.Task;
        }
    }




    //  -------- -------- -------- -------- -------- -------- -------- -------- -------- -------- 

    // dekompilacja kodu dla wbudowanego Optional<T>
    // ponizej jest dessemblowana metoda asynchroniczna, aby na blog posta wkleic
    // https://sharplab.io/#v2:CYLg1APgAgTAjAWAFBQAwAIpwHQCUCuAdgC4CWAtgKbYDCA9uQA6kA2lATgMocBupAxpQDOAbmTIoADnQB5RmTqEAhiwA8WVAD50ABXZ1BQoQBVKAD2IwAFBvTFzxOABpMcDPYswAlMgDeydED0UhJ0QnxyACMOOHQAXkwATnRjdgBPHSV2IUoASRIrD0cvMSQg4NDwqI4YeKSU9MzsvIKi71KAoKgAdnQrKuj2WLAwiMHvbGM6TmJ2EIBzKxLkAF9xJDkFZTUQ4m1UjKyc/OIbNzsHL3jNTsDdycajykKHFzp8YnQeLPR2YXwWMQrgB+MKUADusnkpEUKlUu00Vj+QgBQPQIChWzhCOwADlFJQOkgJABmdCMLJkFSYWqbGHbVTGG5IfxlLpkyJ0OgsdAACSUQgAaip8JR0L50PNKMQROg1mzAlAycZ0MKWKLxZLpbL5eUlZj6Soltc+vyhSLKC41aKrgkrAAzFQ5FzASiO1HLBWYMl02EsKwq77qyi27RWM3Wy2qi22vqzUUuIM2ol6slYABsBr9jO0+MIYolUplcrq+chvoZTKWpXl6wA2gBBIRpQj8ACy0oAFnRgAAhfCsV3sQppRiUOj2qwVlQd4jdvsDlhD1SaLxeAC6pPJlNI1NgWcr2glcvW+ozNIPKgAohZKIQhPShH5bt7XJnpywG+ClKR7Owc+gADi0pfj+f45oUnakEIl5qEy6B0NCfo+EgACQ5RxNoZawaBv4cBBiFYiwnq1kguwcNsFR/o6gjoLkH64X+z5epy3J8gKkaakWOqrKeaa0kh2yMfh8EYrk+JkPaaT0EwbBbC49GCSownsMx5SMHM3z2LBAGEYaLClKm6CsTyuRCDJjByZQwAmnO0HYHpfrYBGFqGUEJl0Qx354ewzkcRaJqOdsfnmsGKbsjh3l/lOSlwdoQUqKGdhQUIDmxXUCUGesRkqsBxC4P8gLGphCGxdgkZuYqaYACyyIQFlWcAZy1PwihkOEShbFcEryqRW77h+s7zv2g4icyrJGeeg1dj2I1LmN6A0H8nXPCh6FBCV2HTXOs2LsuVaei+Z61TMlKMpwbaIn89opBd6BCMQK1tko/BQfmVzgp2HBisYd1iU2Lb8Kd9jPa9ISUMg62BCVD1PS9b3UG2dA8JQuIONW2URR+AFQJmhbauSmkrfdBO6hFUAndKBUokVKrIh6Jq46WEI6VW9OAodXrHegjEyPVDCWdK1mMg2Li/ZdSJuikDZJBwLjXbdbb3Y9IPw+DKHlOUn3fdL6J0RJpBSQ10r0ug2t/Ireu5ADrbA5QoMIy+E2a3cN02IkHDBDBilESpmoucGGKOiwORyhrLvii+EeYJmCRbbFEGetHZMu5QIcQ165TO9Hioe75fPG/YTWw6rYP5tgSMo2jFhJxHKcnsSaEAPRN+glBMMQaToFQO3AE+XM1eg3DEHbDvg1Y1vNrbKv22r+bK3DZchpq9fc8PN6CIJVgb5Qglt2Ym9dSvR2D4xACq95KPalAFwLjUi2LF1XVLxgy1Aefyy/d0l7PS/hy75sfoyzEktX8AgVAGyNnfE2igzZfQtuLK2NsgYzzHvPHqvFG5AA===

    sealed class ProcessText2StateMachine : IAsyncStateMachine
    {
        public int state;
        public OptionalMethodBuilder<string> builder;
        public string text1, text2;

        private int number1, number2;
        private object awaiterObj;

        private void MoveNext()
        {
            int num = state;
            string result;
            try
            {
                OptionalAwaiter<int> awaiter;
                OptionalAwaiter<int> awaiter2;
                if (num != 0)
                {
                    if (num == 1)
                    {
                        awaiter = (OptionalAwaiter<int>)awaiterObj;
                        awaiterObj = null;
                        num = (state = -1);
                        goto IL_00eb;
                    }
                    awaiter2 = TryParseInt(text1).GetAwaiter();
                    if (!awaiter2.IsCompleted)
                    {
                        num = (state = 0);
                        awaiterObj = awaiter2;
                        ProcessText2StateMachine stateMachine = this;
                        builder.AwaitOnCompleted(ref awaiter2, ref stateMachine);
                        return;
                    }
                }
                else
                {
                    awaiter2 = (OptionalAwaiter<int>)awaiterObj;
                    awaiterObj = null;
                    num = (state = -1);
                }
                number1 = awaiter2.GetResult();
                awaiter = TryParseInt(text2).GetAwaiter();
                if (!awaiter.IsCompleted)
                {
                    num = (state = 1);
                    awaiterObj = awaiter;
                    ProcessText2StateMachine stateMachine = this;
                    builder.AwaitOnCompleted(ref awaiter, ref stateMachine);
                    return;
                }
                goto IL_00eb;
            IL_00eb:
                number2 = awaiter.GetResult();
                result = (number1 + number2).ToString();
            }
            catch (Exception exception)
            {
                state = -2;
                builder.SetException(exception);
                return;
            }
            state = -2;
            builder.SetResult(result);
        }

        void IAsyncStateMachine.MoveNext()
        {
            this.MoveNext();
        }


        private void SetStateMachine(IAsyncStateMachine stateMachine) { }

        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
        {
            this.SetStateMachine(stateMachine);
        }
    }


    // dekompilacja kodu dla wbudowanego Task<T>, uzwany jest 'AwaitUnsafeOnCompleted' zamiast 'AwaitOnCompleted'
    private sealed class BrokenTaskStateMachine : IAsyncStateMachine
    {
        public int state;

        public TaskMethodBuilder<string> builder;

        private TaskAwaiter awaiter1;

        private void MoveNext()
        {
            int num = state;
            string result;
            try
            {
                TaskAwaiter awaiter;
                if (num != 0)
                {
                    awaiter = Task.Delay(1000).GetAwaiter();
                    if (!awaiter.IsCompleted)
                    {
                        num = (state = 0);
                        awaiter1 = awaiter;
                        BrokenTaskStateMachine stateMachine = this;
                        builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
                        return;
                    }
                }
                else
                {
                    awaiter = awaiter1;
                    awaiter1 = default(TaskAwaiter);
                    num = (state = -1);
                }
                awaiter.GetResult();
                result = "task";
            }
            catch (Exception exception)
            {
                state = -2;
                builder.SetException(exception);
                return;
            }
            state = -2;
            builder.SetResult(result);
        }

        void IAsyncStateMachine.MoveNext()
        {
            //ILSpy generated this explicit interface implementation from .override directive in MoveNext
            this.MoveNext();
        }

        [DebuggerHidden]
        private void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
        {
            //ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
            this.SetStateMachine(stateMachine);
        }
    }

}


