module TrickWithEnv


type IBla =
    abstract func1: int -> bool


// https://fsharpforfunandprofit.com/posts/dependency-injection-1/
// https://www.bartoszsypytkowski.com/dealing-with-complex-dependency-injection-in-f/
// https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/generics/constraints
// https://fsharpforfunandprofit.com/posts/interfaces/
// https://www.compositional-it.com/news-blog/refactoring-dependencies-in-f/

open System

type IA =
    abstract funcA: 'a -> 'a

type IAP =
    abstract A: IA

type A() =
    interface IA with
        member this.funcA id =
            printfn "funcA %A: " id
            id

type IB =
    abstract funcB: 'a -> 'a

type IBP =
    abstract B: IB

type B() =
    interface IB with
        member this.funcB id =
            printfn "funcB %A: " id
            id


type IC =
    abstract funcC: 'a -> 'a

type ICP =
    abstract C: IC

type C() =
    interface IC with
        member this.funcC id =
            printfn "funcC %A: " id
            id

type IABCP =
    inherit IAP
    inherit IBP
    inherit ICP


let ABCP =
    { new IAP with
        member this.A = A()
      interface IBP with
          member this.B = B()
      interface ICP with
          member this.C = C() }

let ABCP' =
    { new IABCP with
        member this.A = A()
        member this.B = B()
        member this.C = C() }



let funcUsingA (env: #IAP) id = env.A.funcA id

// let funcUsingA'' (env: IAP) id = env.A.funcA id
// let funcUsingA' (env: 'a when 'a :> IAP) id = env.A.funcA id

let funcUsingB (env: #IBP) id = env.B.funcB id

let funcUsingAB env id =
    funcUsingA env id |> ignore
    funcUsingB env id |> ignore
    id

let funcUsingABC (env: 'e when 'e :> IAP and 'e :> IBP and 'e :> ICP) id =
    env.C.funcC id |> ignore
    funcUsingA env id |> ignore
    funcUsingB env id |> ignore
    id


let funcUsingABC'<'e when 'e :> IAP and 'e :> IBP and 'e :> ICP> (env: 'e) id =
    env.C.funcC id |> ignore
    funcUsingA env id |> ignore
    funcUsingB env id |> ignore
    id

let get<'a> (env: 'a) = env

let funcUsingABC'' env id =
    (env :> ICP).C.funcC id |> ignore
    funcUsingA env id |> ignore
    funcUsingB env id |> ignore
    id

// funcUsingABC ABCP "id"
funcUsingABC' ABCP' "id" |> ignore
funcUsingABC'' ABCP' "id" |> ignore


type Call<'env, 'out> = 'env -> 'out

let bindDI (fn: 'out -> Call<'env, 'out2>) (call: Call<'env, 'out>) : Call<'env, 'out2> =
    (fun env -> (fn (call env)) env)

type DIBuilder() =
    member inline __.Return value : Call<'env, 'out> = (fun env -> value)
    member inline __.Zero() : Call<'env, 'out> = (fun env -> Unchecked.defaultof<_>)
    member inline __.ReturnFrom(call: Call<'env, 'out>) = call
    member inline __.Bind(call: Call<'env, 'out>, fn) = bindDI fn call

let di = DIBuilder()






let bla (x: int) =
    di {
        let! value1 = (fun (env: #IAP) -> env.A.funcA "abc")
        let! value2 = (fun (env: #IBP) -> env.B.funcB (DateTime(2023, 7, 18)))
        return value1.Length + value2.Year + x
    }

let _ = bla 1000 ABCP' // 3026 = 1000 + 2023 + 3


// todo: lepszy przyklad ktory pokazuje partial function applicattion gdzie tym razem Env jest przekazywany jako ostatni


//https://github.com/fsharp/fslang-suggestions/issues/1036
// - niestety takie cos jak nizej nie dziala w F# :/ tzn trzeba jednak pisac te dodatkowe interfejsy "providerujace"
// IAP, IBP, ... jeden uniwersalny IP<T> nie wystarczy bo F# ma "problemy" z wnioskowaniem typow
// - generalnie jest zupelnie poprawne i dziala w C#
//      interface IP<T> { T GetService(); }
//      static void MyFunction<T>(T arg) where T : IP<string>, IP<int { }

// [<Interface>]


// type IP<'a> =
//     abstract member S: 'a


// let funcUsingA_ (env: #IP<IA>) id = env.S.funcA id

// let funcUsingB_ (env: #IP<IB>) id = env.S.funcB id

// let funcUsingAB_ env id =
//     funcUsingA_ env id |> ignore
//     funcUsingB_ env id |> ignore // ! The type 'IB' does not match the type 'IA'
//     id

// // !! Type constraint mismatch. The type  ''e' is not compatible with type 'IP<IB>'
// let funcUsingABC_ (env: 'e when 'e :> IP<IA> and 'e :> IP<IB> and 'e :> IP<IC>) id =
//     env.C.funcC id |> ignore
//     funcUsingA env id |> ignore
//     funcUsingB env id |> ignore
//     id
