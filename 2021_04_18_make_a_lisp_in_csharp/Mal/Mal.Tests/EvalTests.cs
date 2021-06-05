
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Mal.Types;
using static Mal.Reader;
using static Mal.EnvM;
using static PowerFP.LListM;
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
            var env1 = new Env(new(null), null);

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


            var env2 = new Env(new(null), EnvM.DefaultEnv);
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
                ), new Env(new(null), null)));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyBindings(LListFrom<MalType>(
                new Symbol("a", NilV),
                new Number(1),
                new Symbol("b", NilV)
                ), new Env(new(null), null)));

            var env1 = new Env(new(null), null);
            var envResult1 = EvalM.ApplyBindings(null, env1);
            Assert.AreSame(env1, envResult1);

            var env2 = new Env(new(null), null);
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
                ), new Env(new(null), null)));

            Assert.ThrowsException<Exception>(() => EvalM.ApplyLet(LListFrom<MalType>(
                new List(LListFrom<MalType>(new Symbol("a", NilV), new Number(1)), ListType.List, NilV)
                ), new Env(new(null), null)));

            var malResult = EvalM.ApplyLet(LListFrom<MalType>(
                new List(LListFrom<MalType>(new Symbol("a", NilV), new Number(1), new Symbol("b", NilV), new Number(3)), ListType.List, NilV),
                new List(LListFrom<MalType>(new Symbol("+", NilV), new Symbol("a", NilV), new Symbol("b", NilV)), ListType.List, NilV)
                ), EnvM.DefaultEnv);

            Assert.AreEqual(new Number(4), malResult);
        }
    }
}