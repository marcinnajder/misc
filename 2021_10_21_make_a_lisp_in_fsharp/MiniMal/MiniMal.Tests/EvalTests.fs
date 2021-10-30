namespace MiniMal.Tests


open Microsoft.VisualStudio.TestTools.UnitTesting
open Types
open Eval
open TestUtils
open Printer

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


    [<TestMethod>]
    member this.ApplyDoTest() =
        assertThrowsException (fun () -> applyDo [] (emptyEnv None))
        let malResult =
            applyDo [ Nil; MalList([ Symbol("+"); Number(1.); Number(3.) ], List) ] (defaultEnv None)

        assertMalEquals (Number(4.)) malResult

    [<TestMethod>]
    member this.ApplyIfTest() =
        assertThrowsException (fun () -> applyIf [] (emptyEnv None))
        assertThrowsException (fun () -> applyIf [ Symbol("a") ] (emptyEnv None))
        assertThrowsException (fun () -> applyIf [ Symbol("a"); Symbol("a"); Symbol("a"); Symbol("a") ] (emptyEnv None))
        // falsy values
        assertMalEquals (Number(2.)) (applyIf [ False; Number(1.); Number(2.) ] (emptyEnv None))
        assertMalEquals (Number(2.)) (applyIf [ Nil; Number(1.); Number(2.) ] (emptyEnv None))
        assertMalEquals Nil (applyIf [ Nil; Number(1.) ] (emptyEnv None))
        // truthy values
        assertMalEquals (Number(1.)) (applyIf [ True; Number(1.); Number(2.) ] (emptyEnv None))
        assertMalEquals (Number(1.)) (applyIf [ Str(""); Number(1.); Number(2.) ] (emptyEnv None))
        assertMalEquals (Number(1.)) (applyIf [ MalList([], Vector); Number(1.); Number(2.) ] (emptyEnv None))

    [<TestMethod>]
    member this.BindFunctionArgumentsTest() =
        let one = Number(1.)
        let two = Number(2.)
        let three = Number(3.)
        let comparePair (name1: string, mal1) (name2, mal2) = name1 = name2 && malEquals mal1 mal2
        Assert.IsTrue(
            compareLists
                [ "a", one ]
                (bindFunctionArguments [ Symbol("a") ] [
                    one
                 ])
                comparePair
        )
        Assert.IsTrue(compareLists [] (bindFunctionArguments [] [ one ]) comparePair)
        Assert.IsTrue(
            compareLists
                [ "a", one ]
                (bindFunctionArguments [ Symbol("a") ] [
                    one
                    two
                 ])
                comparePair
        )
        assertThrowsException (fun () -> bindFunctionArguments [ Symbol("a") ] [])
        Assert.IsTrue(
            compareLists
                [ ("a", one); ("b", MalList([ two; three ], List)) ]
                (bindFunctionArguments [ Symbol("a"); Symbol("&"); Symbol("b") ] [
                    one
                    two
                    three
                 ])
                comparePair
        )
        Assert.IsTrue(
            compareLists
                [ ("a", one); ("b", MalList([], List)) ]
                (bindFunctionArguments [ Symbol("a"); Symbol("&"); Symbol("b") ] [
                    one
                 ])
                comparePair
        )
        assertThrowsException
            (fun () ->
                bindFunctionArguments [ Symbol("a"); Symbol("&") ] [
                    one
                ])

    [<TestMethod>]
    member this.ApplyFnTest() =
        assertThrowsException (fun () -> applyFn [] (emptyEnv None))
        assertThrowsException (fun () -> applyFn [ Number(1.); Number(2.) ] (emptyEnv None))
        assertThrowsException (fun () -> applyFn [ MalList([ Number(1.); Number(2.) ], List) ] (emptyEnv None))
        let a, b, plus = Symbol("a"), Symbol("b"), Symbol("+")
        let one, two = Number(1.), Number(2.)
        let fn =
            applyFn [ MalList([ a; b ], List); MalList([ plus; a; b ], List) ] (defaultEnv None)
        match fn with
        | Fn (func, false) -> assertMalEquals (Number(3.)) (func [ one; two ])
        | _ -> ()

    [<TestMethod>]
    member this.ApplyQuoteTest() =
        assertThrowsException (fun () -> applyQuote [] (emptyEnv None))
        assertThrowsException (fun () -> applyQuote [ Number(1.); Number(1.) ] (emptyEnv None))
        assertMalEquals (Number(1.)) (applyQuote [ Number(1.) ] (emptyEnv None))


    [<TestMethod>]
    member this.TransformQuasiquoteTest() =
        assertMalEquals (Number(1.)) (transformQuasiquote (Number(1.)))
        assertMalEquals (MalList([ Symbol("quote"); Symbol("abc") ], List)) (transformQuasiquote (Symbol("abc")))
        let mal1 =
            transformQuasiquote (MalList([ Number(1.); Number(2.) ], List))
        Assert.AreEqual("(cons 1 (cons 2 ()))", printStr mal1)
        let oneTwoList =
            MalList([ Number(1.); Number(2.) ], List)
        let mal2 =
            transformQuasiquote (
                MalList(
                    [ Number(1.)
                      MalList([ Symbol("unquote"); oneTwoList ], List)
                      Number(4.)
                      MalList([ Symbol("splice-unquote"); oneTwoList ], List) ],
                    List
                )
            )
        Assert.AreEqual("(cons 1 (cons (1 2) (cons 4 (concat (1 2) ()))))", printStr mal2)

    [<TestMethod>]
    member this.ApplyDefMacroTest() =
        let env1 = emptyEnv None
        assertThrowsException (fun () -> applyDefMacro [] env1)
        assertThrowsException (fun () -> applyDefMacro [ Symbol("a") ] env1)
        assertThrowsException (fun () -> applyDefMacro [ Symbol("a"); Number(1.) ] env1)

        let mal1 =
            applyDefMacro [ Symbol("x"); MalList([ Symbol("fn*"); MalList([], List); Nil ], List) ] env1
        Assert.IsTrue(
            match mal1 with
            | Fn (_, true) -> true
            | _ -> false
        )

    [<TestMethod>]
    member this.IsMacroCallTest() =
        let env1 = emptyEnv None
        env1.Set "number" (Number(1.)) |> ignore
        env1.Set "function" (Fn((fun _ -> Nil), false))
        |> ignore
        env1.Set "macro" (Fn((fun _ -> Nil), true))
        |> ignore

        assertThrowsException (fun () -> isMacroCall (MalList([ Symbol("number__") ], List)) env1)

        Assert.IsFalse(isMacroCall (MalList([ Symbol("number") ], List)) env1)
        Assert.IsFalse(isMacroCall (MalList([ Symbol("function") ], List)) env1)
        Assert.IsTrue(isMacroCall (MalList([ Symbol("macro") ], List)) env1)


//         [TestMethod]
//         public void TryCatchTest()
//         {
//             var emptyEnv = EmptyEnv();

//             Assert.ThrowsException<Exception>(() => EvalM.ApplyTryCatch(MalLListFrom(TrueV), emptyEnv));
//             Assert.ThrowsException<Exception>(() => EvalM.ApplyTryCatch(MalLListFrom(TrueV, MalListFrom(new Symbol("catch*"), new Symbol("err"), TrueV, FalseV)), emptyEnv));

//             Assert.AreEqual(TrueV, EvalM.ApplyTryCatch(
//                 MalLListFrom(TrueV, MalListFrom(new Symbol("catch*"), new Symbol("err"), FalseV)), emptyEnv));

//             Assert.AreEqual(FalseV, EvalM.ApplyTryCatch(
//                 MalLListFrom(new Symbol("xx"), MalListFrom(new Symbol("catch*"), new Symbol("err"), FalseV)), emptyEnv));

//             var defaultEnv = DefaultEnv();

//             Assert.AreEqual(new Number(666), EvalM.ApplyTryCatch(
//                 MalLListFrom(MalListFrom(new Symbol("throw"), new Number(666)), MalListFrom(new Symbol("catch*"), new Symbol("err"), new Symbol("err"))), defaultEnv));

//         }
