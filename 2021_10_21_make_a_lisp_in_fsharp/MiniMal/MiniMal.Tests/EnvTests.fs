namespace MiniMal.Tests

open Microsoft.VisualStudio.TestTools.UnitTesting
open Types
open Env
open TestUtils

[<TestClass>]
type EnvTests() =

    [<TestMethod>]
    member this.EnvTest() =
        let env1 =
            Env(Map([ ("a", Str("a")); ("b", Str("b")) ]), None)
        let env2 = Env(Map([ ("c", Str("c")) ]), Some env1)
        env2.Set "d" (Str("d")) |> ignore

        Assert.AreSame(env1, (env2.Find "a").Value)
        Assert.AreSame(env1, (env2.Find "b").Value)
        Assert.AreSame(env2, (env2.Find "c").Value)
        Assert.AreSame(env2, (env2.Find "d").Value)
        Assert.IsTrue((env2.Find "e") = None)

        assertMalEquals (Str("a")) (env2.Get "a")
        assertMalEquals (Str("b")) (env2.Get "b")
        assertMalEquals (Str("c")) (env2.Get "c")
        assertMalEquals (Str("d")) (env2.Get "d")
        assertThrowsException (fun () -> env2.Get "e")
