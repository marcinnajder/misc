
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Mal.Types;
using static Mal.Reader;
using static Mal.EnvM;
using static Mal.Core;
using static PowerFP.LListM;
using static Mal.Program;
using PowerFP;

namespace Mal.Tests
{
    [TestClass]
    public class EvalTests
    {
        // [TestMethod]
        // public void EvalFuncTest()
        // {
        //     //var mal = Reader.ReadText("(+ 5 (* 2 3))");
        //     //var mal = Reader.ReadText("(* 2 3)");
        //     var mal = Reader.ReadText("{\"a\" (+ 7 8)}");
        //     var aa = Evaluation.Eval(mal!, Evaluation.Env);
        //     Console.WriteLine(mal.PrintStr(true));
        //     Console.WriteLine(aa.PrintStr(true));
        // }

        [TestMethod]
        public void ApplyDefTest()
        {
            var env1 = EmptyEnv();

            Assert.ThrowsException<Exception>(() => EvalM.ApplyDef(MalLListFrom(), env1));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyDef(MalLListFrom(new Symbol("a", NilV)), env1));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyDef(MalLListFrom(
                new Symbol("a", NilV), new Number(1), new Number(1)), env1));

            var mal1 = EvalM.ApplyDef(MalLListFrom(new Symbol("x", NilV), new Number(1)), env1);

            Assert.AreEqual(new Number(1), mal1);
            Assert.AreEqual(new Number(1), env1.Get(new Symbol("x", NilV)));


            var env2 = new Env(MapM.Empty<Symbol, MalType>(), DefaultEnv());
            var mal2 = EvalM.ApplyDef(MalLListFrom(
                new Symbol("y", NilV),
                MalListFrom(new Symbol("+", NilV), new Number(1), new Number(2))
                ), env2);

            Assert.AreEqual(new Number(3), mal2);
            Assert.AreEqual(new Number(3), env2.Get(new Symbol("y", NilV)));
        }

        [TestMethod]
        public void ApplyBindingTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyBindings(MalLListFrom(new Symbol("a", NilV)), EmptyEnv()));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyBindings(
                MalLListFrom(new Symbol("a", NilV), new Number(1), new Symbol("b", NilV)), EmptyEnv()));

            var env1 = EmptyEnv();
            var envResult1 = EvalM.ApplyBindings(null, env1);
            Assert.AreSame(env1, envResult1);

            var env2 = EmptyEnv();
            var envResult2 = EvalM.ApplyBindings(MalLListFrom(
                new Symbol("a", NilV),
                new Number(1),
                new Symbol("b", NilV),
                new Number(2)
                ), env2);

            Assert.AreSame(env2, envResult2);
            Assert.AreEqual(new Number(1), env2.Get(new Symbol("a", NilV)));
            Assert.AreEqual(new Number(2), env2.Get(new Symbol("b", NilV)));
        }



        [TestMethod]
        public void ApplyLetTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyLet(MalLListFrom(new Symbol("a", NilV)), EmptyEnv()));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyLet(MalLListFrom(MalListFrom(new Symbol("a", NilV), new Number(1))), EmptyEnv()));

            var malResult = EvalM.ApplyLet(MalLListFrom(
                MalListFrom(new Symbol("a", NilV), new Number(1), new Symbol("b", NilV), new Number(3)),
                MalListFrom(new Symbol("+", NilV), new Symbol("a", NilV), new Symbol("b", NilV))
                ), DefaultEnv());

            Assert.AreEqual(new Number(4), malResult);
        }


        [TestMethod]
        public void ApplyDoTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyDo(MalLListFrom(), EmptyEnv()));
            var malResult = EvalM.ApplyDo(MalLListFrom(NilV, MalListFrom(new Symbol("+", NilV), new Number(1), new Number(3))), DefaultEnv());
            Assert.AreEqual(new Number(4), malResult);
        }

        [TestMethod]
        public void ApplyIfTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyIf(MalLListFrom(
                ), EmptyEnv()));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyIf(MalLListFrom(
                new Symbol("a", NilV)
                ), EmptyEnv()));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyIf(MalLListFrom(
                new Symbol("a", NilV),
                new Symbol("a", NilV),
                new Symbol("a", NilV),
                new Symbol("a", NilV)
                ), EmptyEnv()));

            // falsy values
            Assert.AreEqual(new Number(2),
                EvalM.ApplyIf(MalLListFrom(FalseV, new Number(1), new Number(2)), EmptyEnv()));
            Assert.AreEqual(new Number(2),
                EvalM.ApplyIf(MalLListFrom(NilV, new Number(1), new Number(2)), EmptyEnv()));
            Assert.AreEqual(NilV,
                EvalM.ApplyIf(MalLListFrom(NilV, new Number(1)), EmptyEnv()));

            //truthy values
            Assert.AreEqual(new Number(1),
                EvalM.ApplyIf(MalLListFrom(TrueV, new Number(1), new Number(2)), EmptyEnv()));
            Assert.AreEqual(new Number(1),
                EvalM.ApplyIf(MalLListFrom(new Str(""), new Number(1), new Number(2)), EmptyEnv()));
            Assert.AreEqual(new Number(1),
                EvalM.ApplyIf(MalLListFrom(new List(null, ListType.Vector, NilV), new Number(1), new Number(2)), EmptyEnv()));
        }

        [TestMethod]
        public void BindFunctionArgumentsTest()
        {
            Symbol a = new Symbol("a", NilV), b = new Symbol("b", NilV), amp = new Symbol("&", NilV);
            MalType one = new Number(1), two = new Number(2), three = new Number(3);

            Assert.AreEqual(LListFrom((a, one)), EvalM.BindFunctionArguments(MalLListFrom(a), LListFrom(one)));
            Assert.AreEqual(null, EvalM.BindFunctionArguments(MalLListFrom(), LListFrom(one)));
            Assert.AreEqual(LListFrom((a, one)), EvalM.BindFunctionArguments(MalLListFrom(a), LListFrom(one, two)));
            Assert.ThrowsException<Exception>(() => EvalM.BindFunctionArguments(MalLListFrom(a), MalLListFrom()));

            Assert.AreEqual(
                LListFrom((a, one), (b, new List(LListFrom(two, three), ListType.List, NilV))),
                EvalM.BindFunctionArguments(MalLListFrom(a, amp, b), LListFrom(one, two, three)));
            Assert.AreEqual(
                LListFrom((a, one), (b, new List(null, ListType.List, NilV))),
                EvalM.BindFunctionArguments(MalLListFrom(a, amp, b), LListFrom(one)));

            Assert.ThrowsException<Exception>(() => Assert.AreEqual(
                LListFrom((a, one), (b, new List(null, ListType.List, NilV))),
                EvalM.BindFunctionArguments(MalLListFrom(a, amp), LListFrom(one))));
        }

        [TestMethod]
        public void ApplyFnTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(MalLListFrom(), EmptyEnv()));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(MalLListFrom(new Number(1), new Number(1)), EmptyEnv()));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(MalLListFrom(MalListFrom(), new Number(1), new Number(1)), EmptyEnv()));


            Symbol a = new Symbol("a", NilV), b = new Symbol("b", NilV), plus = new Symbol("+", NilV);
            MalType one = new Number(1), two = new Number(2);

            var fn = (Fn)EvalM.ApplyFn(MalLListFrom(MalListFrom(a, b), MalListFrom(plus, a, b)), DefaultEnv());

            Assert.AreEqual(new Number(3), fn.Value(LListFrom(one, two)));
        }

        [TestMethod]
        public void ApplyQuoteTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyQuote(MalLListFrom(), EmptyEnv()));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(MalLListFrom(new Number(1), new Number(1)), EmptyEnv()));

            Assert.AreEqual(new Number(1), EvalM.ApplyQuote(MalLListFrom(new Number(1)), EmptyEnv()));
        }

        [TestMethod]
        public void TransformQuasiquoteTest()
        {
            Assert.AreEqual(new Number(1), EvalM.TransformQuasiquote(new Number(1)));
            Assert.AreEqual(
                MalListFrom(new Symbol("quote", NilV), new Symbol("abc", NilV)),
                EvalM.TransformQuasiquote(new Symbol("abc", NilV)));

            Assert.AreEqual("(cons 1 (cons 2 ()))", Printer.PrintStr(EvalM.TransformQuasiquote(
                MalListFrom(new Number(1), new Number(2)))));


            var oneTwoList = MalListFrom(new Number(1), new Number(2));

            var mal = EvalM.TransformQuasiquote(
                MalListFrom(
                    new Number(1),
                    MalListFrom(new Symbol("unquote", NilV), oneTwoList),
                    new Number(4),
                    MalListFrom(new Symbol("splice-unquote", NilV), oneTwoList)
                    ));

            Assert.AreEqual("(cons 1 (cons (1 2) (cons 4 (concat (1 2) ()))))", Printer.PrintStr(mal));
        }

        [TestMethod]
        public void ApplyDefMacroTest()
        {
            var env1 = EmptyEnv();

            Assert.ThrowsException<Exception>(() => EvalM.ApplyDefMacro(MalLListFrom(), env1));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyDefMacro(MalLListFrom(new Symbol("a", NilV)), env1));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyDefMacro(MalLListFrom(new Symbol("a", NilV), new Number(1)), env1));

            var mal1 = EvalM.ApplyDefMacro(MalLListFrom(
                new Symbol("x", NilV),
                MalListFrom(new Symbol("fn*", NilV), MalListFrom(), NilV)
                ), env1);

            Assert.IsTrue(mal1 is Fn { IsMacro: true });
        }


        [TestMethod]
        public void IsMacroCallTest()
        {

            var env1 = EmptyEnv();
            env1.Set(new Symbol("number", NilV), new Number(1));
            env1.Set(new Symbol("function", NilV), new Fn(args => NilV, NilV, false));
            env1.Set(new Symbol("macro", NilV), new Fn(args => NilV, NilV, true));

            Assert.ThrowsException<Exception>(() => EvalM.IsMacroCall(MalListFrom(new Symbol("number__", NilV)), env1));

            Assert.IsFalse(EvalM.IsMacroCall(MalListFrom(new Symbol("number", NilV)), env1));
            Assert.IsFalse(EvalM.IsMacroCall(MalListFrom(new Symbol("function", NilV)), env1));
            Assert.IsTrue(EvalM.IsMacroCall(MalListFrom(new Symbol("macro", NilV)), env1));

        }


        [TestMethod]
        public void TryCatchTest()
        {
            var emptyEnv = EmptyEnv();

            Assert.ThrowsException<Exception>(() => EvalM.ApplyTryCatch(MalLListFrom(TrueV), emptyEnv));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyTryCatch(MalLListFrom(TrueV, MalListFrom(new Symbol("catch*", NilV), new Symbol("err", NilV), TrueV, FalseV)), emptyEnv));

            Assert.AreEqual(TrueV, EvalM.ApplyTryCatch(
                MalLListFrom(TrueV, MalListFrom(new Symbol("catch*", NilV), new Symbol("err", NilV), FalseV)), emptyEnv));

            Assert.AreEqual(FalseV, EvalM.ApplyTryCatch(
                MalLListFrom(new Symbol("xx", NilV), MalListFrom(new Symbol("catch*", NilV), new Symbol("err", NilV), FalseV)), emptyEnv));

            var defaultEnv = DefaultEnv();

            Assert.AreEqual(new Number(666), EvalM.ApplyTryCatch(
                MalLListFrom(MalListFrom(new Symbol("throw", NilV), new Number(666)), MalListFrom(new Symbol("catch*", NilV), new Symbol("err", NilV), new Symbol("err", NilV))), defaultEnv));

        }
    }
}