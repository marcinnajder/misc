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

//         [TestMethod]
//         public void BindFunctionArgumentsTest()
//         {
//             Symbol a = new Symbol("a"), b = new Symbol("b"), amp = new Symbol("&");
//             MalType one = new Number(1), two = new Number(2), three = new Number(3);

//             Assert.AreEqual(LListFrom((a, one)), EvalM.BindFunctionArguments(MalLListFrom(a), LListFrom(one)));
//             Assert.AreEqual(null, EvalM.BindFunctionArguments(MalLListFrom(), LListFrom(one)));
//             Assert.AreEqual(LListFrom((a, one)), EvalM.BindFunctionArguments(MalLListFrom(a), LListFrom(one, two)));
//             Assert.ThrowsException<Exception>(() => EvalM.BindFunctionArguments(MalLListFrom(a), MalLListFrom()));

//             Assert.AreEqual(
//                 LListFrom((a, one), (b, new List(LListFrom(two, three), ListType.List))),
//                 EvalM.BindFunctionArguments(MalLListFrom(a, amp, b), LListFrom(one, two, three)));
//             Assert.AreEqual(
//                 LListFrom((a, one), (b, new List(null, ListType.List))),
//                 EvalM.BindFunctionArguments(MalLListFrom(a, amp, b), LListFrom(one)));

//             Assert.ThrowsException<Exception>(() => Assert.AreEqual(
//                 LListFrom((a, one), (b, new List(null, ListType.List))),
//                 EvalM.BindFunctionArguments(MalLListFrom(a, amp), LListFrom(one))));
//         }

//         [TestMethod]
//         public void ApplyFnTest()
//         {
//             Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(MalLListFrom(), EmptyEnv()));
//             Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(MalLListFrom(new Number(1), new Number(1)), EmptyEnv()));
//             Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(MalLListFrom(MalListFrom(), new Number(1), new Number(1)), EmptyEnv()));


//             Symbol a = new Symbol("a"), b = new Symbol("b"), plus = new Symbol("+");
//             MalType one = new Number(1), two = new Number(2);

//             var fn = (Fn)EvalM.ApplyFn(MalLListFrom(MalListFrom(a, b), MalListFrom(plus, a, b)), DefaultEnv());

//             Assert.AreEqual(new Number(3), fn.Value(LListFrom(one, two)));
//         }
