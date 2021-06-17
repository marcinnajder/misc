
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

            Assert.ThrowsException<Exception>(() => EvalM.ApplyDef(null, env1));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyDef(LListFrom<MalType>(
                new Symbol("a", NilV)
                ), env1));
            Assert.ThrowsException<Exception>(() => EvalM.ApplyDef(LListFrom<MalType>(
                new Symbol("a", NilV),
                new Number(1),
                new Number(1)
                ), env1));

            var mal1 = EvalM.ApplyDef(LListFrom<MalType>(
                new Symbol("x", NilV),
                new Number(1)
                ), env1);

            Assert.AreEqual(new Number(1), mal1);
            Assert.AreEqual(new Number(1), env1.Get(new Symbol("x", NilV)));


            var env2 = new Env(MapM.Empty<Symbol, MalType>(), DefaultEnv());
            var mal2 = EvalM.ApplyDef(LListFrom<MalType>(
                new Symbol("y", NilV),
                new List(LListFrom<MalType>(new Symbol("+", NilV), new Number(1), new Number(2)), ListType.List, NilV)
                ), env2);

            Assert.AreEqual(new Number(3), mal2);
            Assert.AreEqual(new Number(3), env2.Get(new Symbol("y", NilV)));
        }

        [TestMethod]
        public void ApplyBindingTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyBindings(LListFrom<MalType>(
                new Symbol("a", NilV)
                ), EmptyEnv()));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyBindings(LListFrom<MalType>(
                new Symbol("a", NilV),
                new Number(1),
                new Symbol("b", NilV)
                ), EmptyEnv()));

            var env1 = EmptyEnv();
            var envResult1 = EvalM.ApplyBindings(null, env1);
            Assert.AreSame(env1, envResult1);

            var env2 = EmptyEnv();
            var envResult2 = EvalM.ApplyBindings(LListFrom<MalType>(
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
            Assert.ThrowsException<Exception>(() => EvalM.ApplyLet(LListFrom<MalType>(
                new Symbol("a", NilV)
                ), EmptyEnv()));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyLet(LListFrom<MalType>(
                new List(LListFrom<MalType>(new Symbol("a", NilV), new Number(1)), ListType.List, NilV)
                ), EmptyEnv()));

            var malResult = EvalM.ApplyLet(LListFrom<MalType>(
                new List(LListFrom<MalType>(new Symbol("a", NilV), new Number(1), new Symbol("b", NilV), new Number(3)), ListType.List, NilV),
                new List(LListFrom<MalType>(new Symbol("+", NilV), new Symbol("a", NilV), new Symbol("b", NilV)), ListType.List, NilV)
                ), DefaultEnv());

            Assert.AreEqual(new Number(4), malResult);
        }


        [TestMethod]
        public void ApplyDoTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyDo(LListFrom<MalType>(
                // new Symbol("a", NilV)
                ), EmptyEnv()));

            var malResult = EvalM.ApplyDo(LListFrom<MalType>(
                  NilV,
                  new List(LListFrom<MalType>(new Symbol("+", NilV), new Number(1), new Number(3)), ListType.List, NilV)
                  ), DefaultEnv());

            Assert.AreEqual(new Number(4), malResult);
        }

        [TestMethod]
        public void ApplyIfTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyIf(LListFrom<MalType>(
                ), EmptyEnv()));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyIf(LListFrom<MalType>(
                new Symbol("a", NilV)
                ), EmptyEnv()));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyIf(LListFrom<MalType>(
                new Symbol("a", NilV),
                new Symbol("a", NilV),
                new Symbol("a", NilV),
                new Symbol("a", NilV)
                ), EmptyEnv()));

            // falsy values
            Assert.AreEqual(new Number(2),
                EvalM.ApplyIf(LListFrom<MalType>(FalseV, new Number(1), new Number(2)), EmptyEnv()));
            Assert.AreEqual(new Number(2),
                EvalM.ApplyIf(LListFrom<MalType>(NilV, new Number(1), new Number(2)), EmptyEnv()));
            Assert.AreEqual(NilV,
                EvalM.ApplyIf(LListFrom<MalType>(NilV, new Number(1)), EmptyEnv()));

            //truthy values
            Assert.AreEqual(new Number(1),
                EvalM.ApplyIf(LListFrom<MalType>(TrueV, new Number(1), new Number(2)), EmptyEnv()));
            Assert.AreEqual(new Number(1),
                EvalM.ApplyIf(LListFrom<MalType>(new Str(""), new Number(1), new Number(2)), EmptyEnv()));
            Assert.AreEqual(new Number(1),
                EvalM.ApplyIf(LListFrom<MalType>(new List(null, ListType.Vector, NilV), new Number(1), new Number(2)), EmptyEnv()));
        }

        [TestMethod]
        public void BindFunctionArgumentsTest()
        {
            Symbol a = new Symbol("a", NilV), b = new Symbol("b", NilV), amp = new Symbol("&", NilV);
            MalType one = new Number(1), two = new Number(2), three = new Number(3);

            Assert.AreEqual(LListFrom((a, one)), EvalM.BindFunctionArguments(LListFrom<MalType>(a), LListFrom(one)));
            Assert.AreEqual(null, EvalM.BindFunctionArguments(LListFrom<MalType>(), LListFrom(one)));
            Assert.AreEqual(LListFrom((a, one)), EvalM.BindFunctionArguments(LListFrom<MalType>(a), LListFrom(one, two)));
            Assert.ThrowsException<Exception>(() => EvalM.BindFunctionArguments(LListFrom<MalType>(a), LListFrom<MalType>()));

            Assert.AreEqual(
                LListFrom((a, one), (b, new List(LListFrom(two, three), ListType.List, NilV))),
                EvalM.BindFunctionArguments(LListFrom<MalType>(a, amp, b), LListFrom(one, two, three)));
            Assert.AreEqual(
                LListFrom((a, one), (b, new List(null, ListType.List, NilV))),
                EvalM.BindFunctionArguments(LListFrom<MalType>(a, amp, b), LListFrom(one)));

            Assert.ThrowsException<Exception>(() => Assert.AreEqual(
                LListFrom((a, one), (b, new List(null, ListType.List, NilV))),
                EvalM.BindFunctionArguments(LListFrom<MalType>(a, amp), LListFrom(one))));
        }

        [TestMethod]
        public void ApplyFnTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(LListFrom<MalType>(
                ), EmptyEnv()));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(LListFrom<MalType>(
                new Number(1),
                new Number(1)
                ), EmptyEnv()));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(LListFrom<MalType>(
                new List(null, ListType.List, NilV),
                new Number(1),
                new Number(1)
                ), EmptyEnv()));


            Symbol a = new Symbol("a", NilV), b = new Symbol("b", NilV), plus = new Symbol("+", NilV);
            MalType one = new Number(1), two = new Number(2);

            var fn = (Fn)EvalM.ApplyFn(LListFrom<MalType>(
                new List(LListFrom<MalType>(a, b), ListType.List, NilV),
                new List(LListFrom<MalType>(plus, a, b), ListType.List, NilV)
                ), DefaultEnv());

            Assert.AreEqual(new Number(3), fn.Value(LListFrom(one, two)));
        }

        [TestMethod]
        public void ApplyQuoteTest()
        {
            Assert.ThrowsException<Exception>(() => EvalM.ApplyQuote(LListFrom<MalType>(
                ), EmptyEnv()));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyFn(LListFrom<MalType>(
                new Number(1),
                new Number(1)
                ), EmptyEnv()));

            Assert.AreEqual(new Number(1), EvalM.ApplyQuote(LListFrom<MalType>(new Number(1)), EmptyEnv()));
        }

        [TestMethod]
        public void TransformQuasiquoteTest()
        {
            Assert.AreEqual(new Number(1), EvalM.TransformQuasiquote(new Number(1)));
            Assert.AreEqual(
                new List(LListFrom<MalType>(new Symbol("quote", NilV), new Symbol("abc", NilV)), ListType.List, NilV),
                EvalM.TransformQuasiquote(new Symbol("abc", NilV)));

            Assert.AreEqual("(cons 1 (cons 2 ()))", Printer.PrintStr(EvalM.TransformQuasiquote(new List(
                LListFrom<MalType>(new Number(1), new Number(2)), ListType.List, NilV))));


            var oneTwoList = new List(LListM.LListFrom<MalType>(new Number(1), new Number(2)), ListType.List, NilV);

            var mal = EvalM.TransformQuasiquote(new List(
                LListFrom<MalType>(
                    new Number(1),
                    new List(LListM.LListFrom<MalType>(new Symbol("unquote", NilV), oneTwoList), ListType.List, NilV),
                    new Number(4),
                    new List(LListM.LListFrom<MalType>(new Symbol("splice-unquote", NilV), oneTwoList), ListType.List, NilV)
                    )
                , ListType.List, NilV));

            Assert.AreEqual("(cons 1 (cons (1 2) (cons 4 (concat (1 2) ()))))", Printer.PrintStr(mal));
        }
    }
}