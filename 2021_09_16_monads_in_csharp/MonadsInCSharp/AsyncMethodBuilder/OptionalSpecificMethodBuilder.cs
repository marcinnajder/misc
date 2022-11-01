using System.Runtime.CompilerServices;

namespace MonadsInCSharp;

// we can choose method builder for Optional monad by

// -> uncommenting this code to use generated method builder
[AsyncMethodBuilder(typeof(OptionalMethodBuilder<>))]
public partial class Optional<T> { }

// -> or this code to use method builder working only with Optional type
// [AsyncMethodBuilder(typeof(OptionalSpecificMethodBuilder<>))]
// public partial class Optional<T>
// {
//     // using instance method instead of extension method 
//     public OptionalSpecificAwaiter<T> GetAwaiter() => new OptionalSpecificAwaiter<T>(this);
// }

/// ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- 

internal interface IOptionalSpecificAwaiter
{
    bool HasValue { get; }
}

public class OptionalSpecificAwaiter<T> : INotifyCompletion, IOptionalSpecificAwaiter
{
    public bool IsCompleted => this.optional.HasValue; // 'false' only in case of lack of value inside Optional
    public void OnCompleted(Action continuation) => throw new Exception("I should never be here");
    public T GetResult() => optional.Value;

    private Optional<T> optional;
    public OptionalSpecificAwaiter(Optional<T> optional) => this.optional = optional;

    bool IOptionalSpecificAwaiter.HasValue => optional.HasValue;
}

public class OptionalSpecificMethodBuilder<T>
{
    public static OptionalSpecificMethodBuilder<T> Create() => new OptionalSpecificMethodBuilder<T>();
    public void Start<TSM>(ref TSM stateMachine) where TSM : IAsyncStateMachine => stateMachine.MoveNext();

    public Optional<T>? Task { get; private set; }
    public void SetResult(T result) => Task = new Optional<T>(result);

    public void AwaitOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : INotifyCompletion where TSM : IAsyncStateMachine
    {
        if (awaiter is IOptionalSpecificAwaiter { HasValue: false })
        {
            Task = new Optional<T>();
        }
        else
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }
    }

    // empty methods
    public void SetStateMachine(IAsyncStateMachine stateMachine) { }
    public void SetException(Exception exception) { }
    public void AwaitUnsafeOnCompleted<TA, TSM>(ref TA awaiter, ref TSM stateMachine)
        where TA : ICriticalNotifyCompletion where TSM : IAsyncStateMachine
    { }
}


// https://devblogs.microsoft.com/premier-developer/extending-the-async-methods-in-c/

// - probowalem zaimplementowac 'MethodBuilder' w miare uniwesralnie aby dzialal dla dowolnej monady czyli 
// aby wykorzystywal metody Select/SelectMany ale nie udalo sie to
// - problem jest taki ze ze wykonujac Select<T,R>(f) lub SelectMany<T,R>(f) musimy znac 'R` nawet gdy 'f'
// nigdy nie zostala wywolana. W metodach asynchronicznych z async/await musimy wykonac kod po 'await'
// aby dodziec sie jaki bedzie rezultat (czyli typ 'R')
// - ... no nie wiem, moze to sie da jakos zrobic budujac wywolania
//  m.SelectMany<T,object>(t => MoveNext(t); return PopM().Select(tt => (object) tt)) )


// - osobny namespace, aby extension metody 'GetAwaiter' brane byly odpowidnion
// - ta implementacja jest prosta ale dziala jedynie dla typu Optional<T>, pozostale async method buildery
// w swojej implementacji korzystaja jedynie z metod moonady (Return,Select,SelectMany) dzieki temu 
// teoretycznie jest to przepis na wykorzystanie async/await z dowolna monada
// - nie do konca tak jest ; bo nie dziala monadami gdzie funkcja przekazana do Select/SelectMany
// bedzie wielokrotnie wolana, czyli IEnumerable<T>, IObservable<T>,.... 