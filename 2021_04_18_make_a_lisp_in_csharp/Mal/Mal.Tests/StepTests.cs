
using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerFP;
using static Mal.Types;


namespace Mal.Tests
{
    [TestClass]
    public class ExecuteTest
    {
        [TestMethod]
        public void Step0()
        {
            // var steps = MalStepsRunner.ReadTestCases("../../../MalSteps/step0_repl.mal");
            // Console.WriteLine(string.Join(Environment.NewLine,
            //     steps.Select(t => (t.Input, string.Join(",", t.Output), string.Join(",", t.Options)))));

            MalStepsRunner.ExecuteTest("../../../MalSteps/step0_repl.mal", verbose: false,
                (text, env) => text);
        }

        [TestMethod]
        public void Step1()
        {
            MalStepsRunner.ExecuteTest("../../../MalSteps/step1_read_print.mal", verbose: false,
                (text, env) => Reader.ReadText(text).Pipe(mal => Printer.PrintStr(mal!)));
        }

        // [TestMethod]
        // public void Step2()
        // {
        //     MalStepsRunner.ExecuteTest("../../../MalSteps/step2_eval.mal", verbose: true,
        //         (text, env) => Reader.ReadText(text).Pipe(mal => Printer.PrintStr(mal!)));
        // }
    }
}

