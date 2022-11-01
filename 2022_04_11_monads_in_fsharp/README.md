Monady w F#
Option
Option - opcjonalna wartość
type MyOption<'T> =
| MNone
| MSome of 'T

let o1: MyOption<int> = MNone
let o2: MyOption<int> = MSome 12

let parseInt (str: string) =
let (success, value) = Int32.TryParse str
if success then MSome value else MNone
Option - funkcja “bind”
let parseTwoInts' str1 str2 =
let int1 = Int32.Parse str1
let int2 = Int32.Parse str2
int1 + int2

let parseTwoInts'' str1 str2 =
let int1O = parseInt str1
match int1O with
| MNone -> MNone
| MSome int1 ->
let int2O = parseInt str2
match int2O with
| MNone -> MNone
| MSome int2 -> MSome(int1 + int2)

// ('a -> Option'<'b>) -> Option'<'a> -> Option'<'b>
let bind binder option =
match option with
| MNone -> MNone
| MSome value -> binder value

let parseTwoInts''' str1 str2 =
parseInt str1
|> bind
(fun int1 ->
parseInt str2
|> bind (fun int2 -> MSome(int1 + int2)))
Option - funkcje “map” i “return”
// ('a -> 'b) -> Option'<'a> -> Option'<'b>
let map binder option =
match option with
| MNone -> MNone
| MSome value -> MSome(binder value)

let parseTwoInts'''` str1 str2 =
parseInt str1
|> bind
(fun int1 ->
parseInt str2
|> map (fun int2 -> int1 + int2))
  
// 'map' zaimplementowany za pomoca 'bind' oraz 'return'

//'a -> MyOption<'a>
let return' value = MSome value

let map' binder option = option |> bind (fun value -> return' (binder value))
Option - “computation expressions”
// string -> string -> int
let parseTwoInts' str1 str2 =
let int1 = Int32.Parse str1
let int2 = Int32.Parse str2
int1 + int2

// string -> string -> Option<int>
let parseTowInts''''' str1 str2 =
option {
let! int1 = parseInt str1
let! int2 = parseInt str2
return int1 + int2
}
Computation expressions
API
// Creating a New Type of Computation Expression
https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions#creating-a-new-type-of-computation-expression

type OptionBuilder() =
member this.Return(value) = ...
member this.ReturnFrom(value) = ...
member this.Zero() = ...
member this.Delay(f: DelayedO<_>) = ...
member this.Run(delayed: DelayedO<_>) = ...
member this.Bind(monad, binder) = .. .
member this.Combine(monad1, monad2: DelayedO<_>) = ...
member this.TryFinally(body: DelayedO<_>, finallyBody) = ...  
 member this.TryWith(body: DelayedO<_>, catchBody) = ...
member this.Using(res: #IDisposable, body) = ...
member this.While(guard, body: DelayedO<_>) = ...
member this.For(sequence: seq<\_>, body) = ...

let option = OptionBuilder()
Pomocnicze “DelayedO, returnO, bindO”
type DelayedO<'T> = unit -> Option<'T>

let returnO x = Some x // 'a -> Option<'a>
let bindO = Option.bind // ('a -> Option<'b>) -> Option<'a> -> Option<'b>

type OptionBuilder() =
member this.Return(value) = returnO value
member this.ReturnFrom(value) = value
...

let option = OptionBuilder()
.Delay, .Run, … .Return (return)
let returnTest () =
option {
return 1
}

let returnTest' () =
option.Run(
option.Delay(
fun () -> option.Return(1)
))

type OptionBuilder() =
// member this.Delay(f: DelayedO<_>) = f
member this.Delay(f: unit -> Option<_>) : DelayedO<_> = f  
 member this.Run(delayed: DelayedO<_>) = delayed ()

    member this.Return(value) = returnO value
    ...

.ReturnFrom (return!)
let returnFromTest () =
option {
return! Some 1
}

let returnFromTest' () =
option.Run(
option.Delay(
fun () -> option.ReturnFrom(1)
))

type OptionBuilder() =
member this.ReturnFrom(value) = value
...
.Bind (let!)
let bindTest () =
option {
let! v1 = Some 1
let! v2 = Some 2
return v1 + v2
}

let bindTest' () =
option.Run(
option.Delay(
fun () ->
option.Bind(Some 1, (fun v1 -> option.Bind(Some 2, (fun v2 -> option.Return(v1 + v2))))))
)

type OptionBuilder() =
member this.Bind(monad, binder) = bindO binder monad
...
.Bind (do!)
let doTest () =
option {
do! Some()
do! Some()
}

let doTest' () =
option.Run(
option.Delay(
fun () ->
option.Bind(Some(), (fun () -> option.Bind(Some(), (fun () -> option.Return(()))))))
)
.Zero
let zeroTest () =
option {
if false then
let! v1 = Some 1
return v1.ToString()
}

let zeroTest' () =
option.Run(
option.Delay
(fun () ->
if false then
option.Bind(Some 1, (fun v1 -> option.Return(v1.ToString())))
else
option.Zero<string>())
)

type OptionBuilder() =
member this.Zero() = returnO Unchecked.defaultof<\_>
...
.Combine, … .While (while)
let whileTest () =
option {
let mutable i = 0
while i < 3 do
let! v1 = Some 1
i <- i + v1
return i
}

let whileTest' () =
option.Run(
option.Delay
(fun () ->
let i = ref 0 // { contents = 0}
option.Combine(
option.While(
(fun () -> i.contents > 0),
option.Delay
(fun () ->
option.Bind(
Some 1,
(fun v1 ->
i.contents <- i.contents + v1
option.Zero<obj>())
))
),
option.Delay(fun () -> option.Return(i.contents))
))
)

type OptionBuilder() =
member this.While(guard, body: DelayedO<_>) =
if guard () then this.Run(body) |> bindO (fun _ -> this.While(guard, body)) else this.Zero()
...
.TryWith (try/with)
let tryCatchTest () =
option {
try
let! v1 = Some 1
return v1 / 0
with
| e ->
let! v2 = Some 1
return v2
}

let tryCatchTest' () =
option.Run(
option.Delay
(fun () ->
option.TryWith(
option.Delay(fun () -> option.Bind(Some 1, (fun v1 -> option.Return(v1 / 0)))),
fun e -> option.Bind(Some 1, (fun v2 -> option.Return(v2)))
))
)

type OptionBuilder() =
member this.TryWith(body: DelayedO<\_>, catchBody) =
try
this.Run(body)
with
| e -> catchBody e
...
.For (for)
let forInTest () =
option {
let mutable i = 0
for j in 1 .. 3 do
let! v1 = Some 1
i <- i + v1
return i
}

let forInTest' () =
option.Run(
option.Delay
(fun () ->
let i = ref 0
option.Combine(
option.For(
seq { 1 .. 3 },
(fun j ->
option.Bind(
Some 1,
(fun v1 ->
i.contents <- i.contents + v1
option.Zero<obj>())
))
),
option.Delay(fun () -> option.Return(i.contents))
))
)
.For, .Using, TryFinally
type OptionBuilder() =
member this.While(guard, body: DelayedO<_>) =
if guard () then this.Run(body) |> bindO (fun _ -> this.While(guard, body)) else this.Zero()

    member this.TryFinally(body: DelayedO<_>, finallyBody) =
        try
            this.Run(body)
        finally
            finallyBody ()

    member this.Using(res: #IDisposable, body) =
        this.TryFinally(
            this.Delay(
    	        fun _ -> body res), (fun () -> if not (isNull (box res)) then res.Dispose()))

    member this.For(sequence: seq<_>, body) =
        this.Using(
            sequence.GetEnumerator(),
            (fun iterator ->
                this.While((fun () -> iterator.MoveNext()), this.Delay(fun _ -> body iterator.Current)))
        )
    ...

Task
TaskBuilder
type DelayedT<'T> = unit -> Task<'T>

let returnT = Task.FromResult // 'a -> Task<'a>

// ('a -> Task<'b>) -> Task<'a> -> Task<'b>
let bindT<'a, 'r> (f: 'a -> Task<'r>) (task: Task<'a>) =
task.ContinueWith(fun (t: Task<'a>) -> f t.Result).Unwrap()

type TaskBuilder() =
member this.Return(value) = returnT value
member this.ReturnFrom(value) = value
member this.Zero() = returnT Unchecked.defaultof<\_>

    member this.Delay(f: DelayedT<_>) = f
    member this.Run(delayed: DelayedT<_>) = delayed ()

    member this.Bind(monad, binder) = bindT binder monad
    member this.Combine(monad1, monad2: DelayedT<_>) = monad1 |> bindT (fun _ -> this.Run(monad2))

    member this.While(guard, body: DelayedT<_>) =
        if guard () then this.Run(body) |> bindT (fun _ -> this.While(guard, body)) else this.Zero()

    member this_.TryFinally(body: DelayedT<_>, finallyBody) =
        try
            this_
                .Run(body)
                .ContinueWith(fun (t: Task<_>) ->
                    finallyBody ()
                    t.Result)
        with
        | e ->
            finallyBody ()
            Task.FromException<_>(e)

    member this_.TryWith(body: DelayedT<_>, catchBody) =
        try
            this_
                .Run(body)
                .ContinueWith(fun (t: Task<_>) ->
                    if t.IsFaulted then
    	                catchBody (t.Exception :> Exception)
                    else
    	                t)
                .Unwrap()
        with
        | e -> catchBody e

    member this.Using(res: #IDisposable, body) =
        this.TryFinally(
            this.Delay(fun _ -> body res),
            (fun () -> if not (isNull (box res)) then res.Dispose()))

    member this.For(sequence: seq<_>, body) =
        this.Using(
            sequence.GetEnumerator(),
            (fun iterator ->
                this.While(
    	            (fun () -> iterator.MoveNext()),
    	            this.Delay(fun _ -> body iterator.Current)))
        )

let task = TaskBuilder()
Option
// string -> Option<int>
let parseInt (str: string) =
let (success, value) = Int32.TryParse str
if success then Some value else None

let printM (o: Option<\_>) = printfn "%A" o

let xx = option
xx {
let mutable result = 0
let! v1 = parseInt "1"
result <- result + v1
return result
}
|> printM
Task
// string -> Task<int>
let parseInt (str: string) = Task.Delay(100).ContinueWith(fun \_ -> Int32.Parse str)

let printM (task: Task<_>) =
task.ContinueWith
(fun (t: Task<_>) ->
if t.IsFaulted then
printfn "error: %A" t.Exception.Message
else
printfn "result: %A" t.Result)
|> ignore

let xx = task
xx {
let mutable result = 0
let! v1 = parseInt "1"
result <- result + v1
return result
}
|> printM
Seq
Implementacja IEnumerable/IEnumerator
let newEnumerator moveNext current dispose =
{ new IEnumerator<'T> with
member this.Current = current ()
interface IEnumerator with
member this.MoveNext() = moveNext ()
member this.Current = current () :> obj
member this.Reset() = ()
interface IDisposable with
member this.Dispose() = dispose () }

let newEnumerable getEnumerator =
{ new IEnumerable<'T> with
member this.GetEnumerator() = getEnumerator ()
interface IEnumerable with
member this.GetEnumerator() = (this :?> IEnumerable<'T>).GetEnumerator() :> IEnumerator }

let ones = enumerable (fun _ -> enumerator (fun _ -> true) (fun \_ -> 1))
ones |> Seq.take 10 |> Seq.toArray
Implementacja IEnumerable/IEnumerator -> pomocnicze metody
let newEnumerableDForNext f =
newEnumerable
(fun () ->
let next, dispose = f ()
let mutable currentValue = None
let moveNext () =
currentValue <- next ()
Option.isSome currentValue
newEnumerator moveNext (fun () -> Option.get currentValue) dispose)

let newEnumerableForNext f = newEnumerableDForNext (fun () -> f (), ignore)

let a: seq<int> = enumerable' (fun () -> fun () -> Some 1)
let aaa = a |> Seq.truncate 10 |> Seq.toArray
tryFinally, tryCatch
let step (e: IEnumerator<\_>) = if e.MoveNext() then Some e.Current else None

let tryFinally (items: seq<\_>) (finallyBody: unit -> unit) =
newEnumerableDForNext
(fun () ->
let mutable enumerator = items.GetEnumerator()
let mutable isDisposed = false
let dispose () =
if not isDisposed then
isDisposed <- true
finallyBody ()
enumerator.Dispose()
(fun () ->
try
let res = step enumerator
if Option.isNone res then dispose ()
res
with
| e ->
dispose ()
raise e),
dispose)

let tryCatch (items: seq<_>) (catchBody: exn -> seq<_>) =
// todo: tutaj dispose takze powinien byc wspierany
newEnumerableForNext
(fun () ->
let mutable enumerators = items.GetEnumerator() |> Choice1Of2
let rec next () =
match enumerators with
| Choice1Of2 enumerator ->
try
step enumerator
with
| e ->
enumerators <- (catchBody e).GetEnumerator() |> Choice2Of2
next ()
| Choice2Of2 enumerator -> step enumerator
next)
SeqBuilder
type DelayedS<'T> = unit -> 'T seq
let returnS = Seq.singleton
let bindS = Seq.collect

type SeqBuilder() =
// member this.Return(value) = returnS value
// member this.ReturnFrom(value) = value
member this\_.Zero() = Seq.empty

    member this.Delay(f: DelayedS<_>) = f
    member this_.Run(delayed: DelayedS<_>) = newEnumerable (fun () -> (delayed ()).GetEnumerator())
      // seq { ... } must be lazy

    member this.Bind(monad, binder) = bindS binder monad

    member this_.Combine(monad1, monad2: DelayedS<_>) = Seq.append monad1 (this_.Run(monad2))

    member this_.While(guard, body: DelayedS<_>) =
        if guard () then
            this_.Combine(this_.Run(body), this_.Delay(fun () -> this_.While(guard, body)))
        else
            this_.Zero()

    member this_.Yield(value) = returnS value
    member this_.YieldFrom(value: seq<_>) = value

    member this_.TryFinally(body: DelayedS<_>, finallyBody) = tryFinally (this_.Run(body)) finallyBody
    member this_.TryWith(body: DelayedS<_>, catchBody) = tryCatch (this_.Run(body)) catchBody

    member this.Using(res: #IDisposable, body) =
        this.TryFinally(
            this.Delay(fun _ -> body res), (fun () -> if not (isNull (box res)) then res.Dispose()))

    member this.For(sequence: seq<_>, body) =
        this.Using(
            sequence.GetEnumerator(),
            (fun iterator ->
                this.While((fun () -> iterator.MoveNext()), this.Delay(fun _ -> body iterator.Current)))
        )

let seq2 = SeqBuilder()
SeqBuilder - przykłady
let printS (items: seq<\_>) = printfn "%A" (items |> Seq.toArray)

seq2 {
yield 1
yield 2
yield! [ 3; 4; 5 ]
let mutable i = 6
printfn "while"
while i < 8 do
printfn "while %d" i
yield i
i <- i + 1
printfn "for"
for j = 8 to 10 do
printfn "for %d" j
yield j
printfn "end"
}
|> Seq.truncate 10
|> printS

seq2 {
if true then
use dis = disposable
try
try
yield 1
yield 2 / 0
finally
printfn "finally"
with
| e -> yield 666
printfn "end"
}
|> Seq.truncate 1
|> printS

seq2 {
yield 1
yield 2
// bind :) -> dla wbudowanego seq { ... } leci blad kompilacji:
// Używanie ciągu „let! x = coll” w wyrażeniach sekwencji jest niedozwolone. Zamiast tego użyj ciągu „for x in coll”
let! x = seq { 3 .. 5 }
yield x \* 10
printfn "end"
}
|> printS
