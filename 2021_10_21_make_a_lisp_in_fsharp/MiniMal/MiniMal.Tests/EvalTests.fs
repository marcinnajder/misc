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
