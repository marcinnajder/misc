
using System.Runtime.CompilerServices;

namespace MonadsInCSharp;

[AsyncMethodBuilder(typeof(TTaskMethodBuilder<>))]
public class TTask<T>
{
    public Task<T> Task { get; }
    public TTask(Task<T> task) => Task = task;

    public TTask<R> Select<R>(Func<T, R> f) => new TTask<R>(Task.Select(f));
    public TTask<R> SelectMany<R>(Func<T, TTask<R>> f) => new TTask<R>(Task.SelectMany(x => f(x).Task));
}

// - starting with C#10 we can specify method builder directly above async method (not only above the type returned by async method),
// this allows to override the default "Task" type method builder
// - here "Task" type is wrapped into "TTask" type to show more explicite that general implementation of 
// method builder works collectly 

