
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Mal.Types;
using static Mal.Reader;
using static PowerFP.LListM;
using PowerFP;

namespace Mal.Tests
{
    [TestClass]
    public class EvalTests
    {
        [TestMethod]
        public void ExecuteArithmeticOperationTest()
        {
            Func<double, double, double> add = (a, b) => a + b;

            Assert.ThrowsException<Exception>(() => Evaluation.ExecuteArithmeticOperation(null, add));
            Assert.ThrowsException<Exception>(() => Evaluation.ExecuteArithmeticOperation(new(new Number(1), null), add));
            Assert.ThrowsException<Exception>(() => Evaluation.ExecuteArithmeticOperation(new(new Number(1), new(new Str("2"), null)), add));

            Assert.AreEqual(new Number(1 + 2 + 3), Evaluation.ExecuteArithmeticOperation(new Number[] { new(1), new(2), new(3) }.Cast<MalType>().ToLList(), add));
        }

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



    }
}