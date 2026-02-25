
// https://github.com/davidfowl/AspNetCoreDiagnosticScenarios/blob/master/AsyncGuidance.md#asynclocalt

class DisposableAsyncLocal
{
    public static void Run()
    {
        using (var thing = new DisposableThing() { Value = 7 })
        {
            // Make the disposable object available ambiently
            DisposableThing.Current = thing;

            Dispatch();

            // We're about to dispose the object so make sure nobody else captures this instance
            DisposableThing.Current = null;
        }

        void Dispatch()
        {
            // var txt = "inside new task/thread the current value is: " + DisposableThing.Current!.Value;
            // Console.WriteLine(txt);

            // Task.Run will capture the current execution context (which means async locals are captured in the callback)
            _ = Task.Run(async () =>
            {
                // Delay for a second then log
                await Task.Delay(1000);

                Log();
            });
        }

        void Log()
        {
            try
            {
                // Get the current value and make sure it's not null before reading the value
                var thing = DisposableThing.Current;
                if (thing is not null)
                {
                    try
                    {
                        Console.WriteLine($"Logging ambient value {thing.Value}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    class DisposableThing : IDisposable
    {
        private static readonly AsyncLocal<DisposableThing?> _current = new();

        private bool _disposed;

        public static DisposableThing? Current
        {
            get => _current.Value;
            set
            {
                _current.Value = value;
            }
        }

        private int _value;
        public int Value
        {
            set
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                _value = value;
            }
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _value;
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}