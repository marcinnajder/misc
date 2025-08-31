// this code is generated automatically from 'AsyncMethodBuilders.generated.tt', do not change it manually
#nullable enable

using System.Runtime.CompilerServices;

namespace MonadsInCSharp;

// ** Optional **
public static partial class AwaiterExtensions
{
    public static OptionalAwaiter<T> GetAwaiter<T>(this Optional<T> monad) => new OptionalAwaiter<T>(monad);
}

internal interface IOptionalAwaiter
{
    Optional<object> MoveNext(Action moveNext, Func<Optional<object>> getNextMonad);
}

public class OptionalAwaiter<T> : INotifyCompletion, IOptionalAwaiter
{
    public bool IsCompleted => false;
    public void OnCompleted(Action continuation) { }
    protected T result = default!;
    public T GetResult() => result;

    // ***********************************

    private Optional<T> monad;
    public OptionalAwaiter(Optional<T> monad) => this.monad = monad;

    Optional<object> IOptionalAwaiter.MoveNext(Action moveNext, Func<Optional<object>> getNextMonad)
    {
        return monad.SelectMany(t =>
        {
            result = t;
            moveNext();
            return getNextMonad();
        });
    }
}

public class OptionalMethodBuilder<T>
{
    public void Start<TSM>(ref TSM stateMachine) where TSM : IAsyncStateMachine => stateMachine.MoveNext();

    public void SetStateMachine(IAsyncStateMachine stateMachine) { }
    public void SetException(Exception exception) { }
    public void AwaitUnsafeOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : ICriticalNotifyCompletion where TSM : IAsyncStateMachine
    { }

    // ***********************************

    private bool isFirstCall = true;
    private Optional<object>? currentTask;
    public Optional<T>? Task { get; private set; }

    public static OptionalMethodBuilder<T> Create() => new OptionalMethodBuilder<T>();

    public void AwaitOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : INotifyCompletion where TSM : IAsyncStateMachine
    {
        if (awaiter is IOptionalAwaiter awaiterO)
        {
            var wasFirstCall = isFirstCall;
            isFirstCall = false;

            currentTask = awaiterO.MoveNext(stateMachine.MoveNext, () => currentTask!);

            if (wasFirstCall)
            {
                Task = currentTask.Select(v => (T)v);
            }
        }
    }

    public void SetResult(T result)
    {
        if (isFirstCall)
        {
            Task = Monad.ReturnO(result);
        }
        else
        {
            currentTask = Monad.ReturnO<object>(result!);
        }
    }
}
// ** TTask **
public static partial class AwaiterExtensions
{
    public static TTaskAwaiter<T> GetAwaiter<T>(this TTask<T> monad) => new TTaskAwaiter<T>(monad);
}

internal interface ITTaskAwaiter
{
    TTask<object> MoveNext(Action moveNext, Func<TTask<object>> getNextMonad);
}

public class TTaskAwaiter<T> : INotifyCompletion, ITTaskAwaiter
{
    public bool IsCompleted => false;
    public void OnCompleted(Action continuation) { }
    protected T result = default!;
    public T GetResult() => result;

    // ***********************************

    private TTask<T> monad;
    public TTaskAwaiter(TTask<T> monad) => this.monad = monad;

    TTask<object> ITTaskAwaiter.MoveNext(Action moveNext, Func<TTask<object>> getNextMonad)
    {
        return monad.SelectMany(t =>
        {
            result = t;
            moveNext();
            return getNextMonad();
        });
    }
}

public class TTaskMethodBuilder<T>
{
    public void Start<TSM>(ref TSM stateMachine) where TSM : IAsyncStateMachine => stateMachine.MoveNext();

    public void SetStateMachine(IAsyncStateMachine stateMachine) { }
    public void SetException(Exception exception) { }
    public void AwaitUnsafeOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : ICriticalNotifyCompletion where TSM : IAsyncStateMachine
    { }

    // ***********************************

    private bool isFirstCall = true;
    private TTask<object>? currentTask;
    public TTask<T>? Task { get; private set; }

    public static TTaskMethodBuilder<T> Create() => new TTaskMethodBuilder<T>();

    public void AwaitOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : INotifyCompletion where TSM : IAsyncStateMachine
    {
        if (awaiter is ITTaskAwaiter awaiterTT)
        {
            var wasFirstCall = isFirstCall;
            isFirstCall = false;

            currentTask = awaiterTT.MoveNext(stateMachine.MoveNext, () => currentTask!);

            if (wasFirstCall)
            {
                Task = currentTask.Select(v => (T)v);
            }
        }
    }

    public void SetResult(T result)
    {
        if (isFirstCall)
        {
            Task = Monad.ReturnTT(result);
        }
        else
        {
            currentTask = Monad.ReturnTT<object>(result!);
        }
    }
}
// ** IO **
public static partial class AwaiterExtensions
{
    public static IOAwaiter<T> GetAwaiter<T>(this IO<T> monad) => new IOAwaiter<T>(monad);
}

internal interface IIOAwaiter
{
    IO<object> MoveNext(Action moveNext, Func<IO<object>> getNextMonad);
}

public class IOAwaiter<T> : INotifyCompletion, IIOAwaiter
{
    public bool IsCompleted => false;
    public void OnCompleted(Action continuation) { }
    protected T result = default!;
    public T GetResult() => result;

    // ***********************************

    private IO<T> monad;
    public IOAwaiter(IO<T> monad) => this.monad = monad;

    IO<object> IIOAwaiter.MoveNext(Action moveNext, Func<IO<object>> getNextMonad)
    {
        return monad.SelectMany(t =>
        {
            result = t;
            moveNext();
            return getNextMonad();
        });
    }
}

public class IOMethodBuilder<T>
{
    public void Start<TSM>(ref TSM stateMachine) where TSM : IAsyncStateMachine => stateMachine.MoveNext();

    public void SetStateMachine(IAsyncStateMachine stateMachine) { }
    public void SetException(Exception exception) { }
    public void AwaitUnsafeOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : ICriticalNotifyCompletion where TSM : IAsyncStateMachine
    { }

    // ***********************************

    private bool isFirstCall = true;
    private IO<object>? currentTask;
    public IO<T>? Task { get; private set; }

    public static IOMethodBuilder<T> Create() => new IOMethodBuilder<T>();

    public void AwaitOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : INotifyCompletion where TSM : IAsyncStateMachine
    {
        if (awaiter is IIOAwaiter awaiterIO)
        {
            var wasFirstCall = isFirstCall;
            isFirstCall = false;

            currentTask = awaiterIO.MoveNext(stateMachine.MoveNext, () => currentTask!);

            if (wasFirstCall)
            {
                Task = currentTask.Select(v => (T)v);
            }
        }
    }

    public void SetResult(T result)
    {
        if (isFirstCall)
        {
            Task = Monad.ReturnIO(result);
        }
        else
        {
            currentTask = Monad.ReturnIO<object>(result!);
        }
    }
}
// ** IEnumerable **
public static partial class AwaiterExtensions
{
    public static IEnumerableAwaiter<T> GetAwaiter<T>(this IEnumerable<T> monad) => new IEnumerableAwaiter<T>(monad);
}

internal interface IIEnumerableAwaiter
{
    IEnumerable<object> MoveNext(Action moveNext, Func<IEnumerable<object>> getNextMonad);
}

public class IEnumerableAwaiter<T> : INotifyCompletion, IIEnumerableAwaiter
{
    public bool IsCompleted => false;
    public void OnCompleted(Action continuation) { }
    protected T result = default!;
    public T GetResult() => result;

    // ***********************************

    private IEnumerable<T> monad;
    public IEnumerableAwaiter(IEnumerable<T> monad) => this.monad = monad;

    IEnumerable<object> IIEnumerableAwaiter.MoveNext(Action moveNext, Func<IEnumerable<object>> getNextMonad)
    {
        return monad.SelectMany(t =>
        {
            result = t;
            moveNext();
            return getNextMonad();
        });
    }
}

public class IEnumerableMethodBuilder<T>
{
    public void Start<TSM>(ref TSM stateMachine) where TSM : IAsyncStateMachine => stateMachine.MoveNext();

    public void SetStateMachine(IAsyncStateMachine stateMachine) { }
    public void SetException(Exception exception) { }
    public void AwaitUnsafeOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : ICriticalNotifyCompletion where TSM : IAsyncStateMachine
    { }

    // ***********************************

    private bool isFirstCall = true;
    private IEnumerable<object>? currentTask;
    public IEnumerable<T>? Task { get; private set; }

    public static IEnumerableMethodBuilder<T> Create() => new IEnumerableMethodBuilder<T>();

    public void AwaitOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : INotifyCompletion where TSM : IAsyncStateMachine
    {
        if (awaiter is IIEnumerableAwaiter awaiterE)
        {
            var wasFirstCall = isFirstCall;
            isFirstCall = false;

            currentTask = awaiterE.MoveNext(stateMachine.MoveNext, () => currentTask!);

            if (wasFirstCall)
            {
                Task = currentTask.Select(v => (T)v);
            }
        }
    }

    public void SetResult(T result)
    {
        if (isFirstCall)
        {
            Task = Monad.ReturnE(result);
        }
        else
        {
            currentTask = Monad.ReturnE<object>(result!);
        }
    }
}
// ** Task **
public static partial class AwaiterExtensions
{
    public static TaskAwaiter<T> GetAwaiter<T>(this Task<T> monad) => new TaskAwaiter<T>(monad);
}

internal interface ITaskAwaiter
{
    Task<object> MoveNext(Action moveNext, Func<Task<object>> getNextMonad);
}

public class TaskAwaiter<T> : INotifyCompletion, ITaskAwaiter
{
    public bool IsCompleted => false;
    public void OnCompleted(Action continuation) { }
    protected T result = default!;
    public T GetResult() => result;

    // ***********************************

    private Task<T> monad;
    public TaskAwaiter(Task<T> monad) => this.monad = monad;

    Task<object> ITaskAwaiter.MoveNext(Action moveNext, Func<Task<object>> getNextMonad)
    {
        return monad.SelectMany(t =>
        {
            result = t;
            moveNext();
            return getNextMonad();
        });
    }
}

public class TaskMethodBuilder<T>
{
    public void Start<TSM>(ref TSM stateMachine) where TSM : IAsyncStateMachine => stateMachine.MoveNext();

    public void SetStateMachine(IAsyncStateMachine stateMachine) { }
    public void SetException(Exception exception) { }
    public void AwaitUnsafeOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : ICriticalNotifyCompletion where TSM : IAsyncStateMachine
    {

    }

    // ***********************************

    private bool isFirstCall = true;
    private Task<object>? currentTask;
    public Task<T>? Task { get; private set; }

    public static TaskMethodBuilder<T> Create() => new TaskMethodBuilder<T>();

    public void AwaitOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : INotifyCompletion where TSM : IAsyncStateMachine
    {
        if (awaiter is ITaskAwaiter awaiterT)
        {
            var wasFirstCall = isFirstCall;
            isFirstCall = false;

            currentTask = awaiterT.MoveNext(stateMachine.MoveNext, () => currentTask!);

            if (wasFirstCall)
            {
                Task = currentTask.Select(v => (T)v);
            }
        }
    }

    public void SetResult(T result)
    {
        if (isFirstCall)
        {
            Task = Monad.ReturnT(result);
        }
        else
        {
            currentTask = Monad.ReturnT<object>(result!);
        }
    }
}
