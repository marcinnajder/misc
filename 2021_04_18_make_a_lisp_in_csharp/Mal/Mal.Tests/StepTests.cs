
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

        [TestMethod]
        public void Step2()
        {
            MalStepsRunner.ExecuteTest("../../../MalSteps/step2_eval.mal", verbose: this.verbose, (text, env) => Reader.ReadText(text)
                .Pipe(mal => mal != null ? EvalM.Eval(mal!, EnvM.DefaultEnv) : null)
                .Pipe(mal => Printer.PrintStr(mal, true))
                , MalStepsRunner.Option.Deferrable, MalStepsRunner.Option.Optional
            );
        }

        [TestMethod]
        public void Step3()
        {
            MalStepsRunner.ExecuteTest("../../../MalSteps/step3_env.mal", verbose: this.verbose, (text, env) => Reader.ReadText(text)
                .Pipe(mal => mal != null ? EvalM.Eval(mal!, EnvM.DefaultEnv) : null)
                .Pipe(mal => Printer.PrintStr(mal, true))
                , MalStepsRunner.Option.Deferrable, MalStepsRunner.Option.Optional
            );
        }


        // [TestMethod]
        // public void Step2()
        // {
        //     MalStepsRunner.ExecuteTest("../../../MalSteps/step2_eval.mal", verbose: true,
        //         (text, env) => Reader.ReadText(text).Pipe(mal => Printer.PrintStr(mal!)));
        // }
    }
}

