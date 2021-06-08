using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PowerFP;
using static Mal.Types;
using static Mal.EnvM;
using static Mal.Core;

[assembly: InternalsVisibleTo("Mal.Tests")]

namespace Mal
{
    public delegate string? StepFunction(string text, Env env);

    public static class Program
    {
        // creates a new Env when necessary, remember Env is mutated
        public static Env EmptyEnv() => new Env(MapM.Empty<Symbol, MalType>(), null);
        public static Env DefaultEnv() => new Env(Ns, null);

        public static StepFunction ReplStep = (string text, Env env) =>
            Reader.ReadText(text)?.Pipe(mal => EvalM.Eval(mal, env)).Pipe(mal => Printer.PrintStr(mal, true));

        public static void InitEnv(StepFunction step, Env env)
        {
            step("(def! not (fn* (a) (if a false true)))", env);
        }

        static void Main(string[] args)
        {
            var env = DefaultEnv();
            InitEnv(ReplStep, env);

            while (true)
            {
                var text = Console.ReadLine();
                if (text != null)
                {
                    try
                    {
                        Console.WriteLine(ReplStep(text, env) ?? "<empty>");
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"Error: {exception.Message}");
                    }
                }
            }
        }
    }
}
