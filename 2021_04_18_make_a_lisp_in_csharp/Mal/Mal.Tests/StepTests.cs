
using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerFP;
using static Mal.Program;

namespace Mal.Tests
{
    [TestClass]
    public class ExecuteTest
    {
        private bool verbose = false;

        [TestMethod]
        public void Step0()
        {
            // var steps = MalStepsRunner.ReadTestCases("../../../MalSteps/step0_repl.mal");
            // Console.WriteLine(string.Join(Environment.NewLine,
            //     steps.Select(t => (t.Input, string.Join(",", t.Output), string.Join(",", t.Options)))));

            MalStepsRunner.ExecuteTest("../../../MalSteps/step0_repl.mal", verbose: this.verbose, (text, env) => text);
        }

        [TestMethod]
        public void Step1()
        {
            MalStepsRunner.ExecuteTest("../../../MalSteps/step1_read_print.mal", verbose: this.verbose, (text, env) => Reader.ReadText(text)
                .Pipe(mal => Printer.PrintStr(mal, true))
                , MalStepsRunner.Option.Deferrable, MalStepsRunner.Option.Optional
                );
        }

        private void ExecuteFile(string fileName)
        {
            MalStepsRunner.ExecuteTest($"../../../MalSteps/{fileName}", verbose: this.verbose, (text, env) => Reader.ReadText(text)
                .Pipe(mal => mal != null ? EvalM.Eval(mal!, env) : null)
                .Pipe(mal => Printer.PrintStr(mal, true))
                , MalStepsRunner.Option.Deferrable, MalStepsRunner.Option.Optional
            );
        }


        [TestMethod]
        public void Step2() => ExecuteFile("step2_eval.mal");

        [TestMethod]
        public void Step3() => ExecuteFile("step3_env.mal");

        [TestMethod]
        public void Step4() => ExecuteFile("step4_if_fn_do.mal");

        [TestMethod]
        public void Step6() => ExecuteFile("step6_file.mal");

        [TestMethod]
        public void Step7() => ExecuteFile("step7_quote.mal");

        [TestMethod]
        public void Step8() => ExecuteFile("step8_macros.mal");

        [TestMethod]
        public void Step9() => ExecuteFile("step9_try.mal");

        [TestMethod]
        public void Step10() => ExecuteFile("stepA_mal.mal");
    }
}

