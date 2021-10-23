namespace MiniMal.Tests


open Microsoft.VisualStudio.TestTools.UnitTesting
open Types
open Eval
open TestUtils

[<TestClass>]
type EvalTests() =

    [<TestMethod>]
    member this.ApplyDefTest() =
        let env1 = emptyEnv None
        assertThrowsException (fun () -> applyDef [] env1)
        assertThrowsException (fun () -> applyDef [ Symbol("a") ] env1)
        assertThrowsException (fun () -> applyDef [ Symbol("a"); Number(1.); Number(1.) ] env1)
        let mal1 =
            applyDef [ Symbol("x"); Number(1.) ] env1
        assertMalEquals (Number(1.)) mal1
        assertMalEquals (Number(1.)) (env1.Get "x")


    [<TestMethod>]
    member this.ApplyBindingTest() =
        assertThrowsException (fun () -> applyBindings [ Symbol("a") ] (emptyEnv None))
        assertThrowsException (fun () -> applyBindings [ Symbol("a"); Number(1.); Symbol("b") ] (emptyEnv None))

        let env1 = emptyEnv None
        let envResult1 = applyBindings [] env1
        Assert.AreSame(env1, envResult1)

        let env2 = emptyEnv None
        let envResult2 =
            applyBindings [ Symbol("a"); Number(1.); Symbol("b"); Number(2.) ] env2
        Assert.AreSame(env2, envResult2)
        assertMalEquals (Number(1.)) (env2.Get "a")
        assertMalEquals (Number(2.)) (env2.Get "b")


    [<TestMethod>]
    member this.ApplyLetTest() =
        assertThrowsException (fun () -> applyLet [ Symbol("a") ] (emptyEnv None))
        assertThrowsException (fun () -> applyLet [ Symbol("a"); Number(1.) ] (emptyEnv None))

        let malResult =
            applyLet
                [ MalList([ Symbol("a"); Number(1.); Symbol("b"); Number(3.) ], List)
                  MalList([ Symbol("+"); Symbol("a"); Symbol("b") ], List) ]
                (defaultEnv None)

        assertMalEquals (Number(4.)) malResult
