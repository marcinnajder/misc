using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PowerFP;
using static Mal.Types;
using static Mal.EnvM;
using static Mal.EvalM;
using static Mal.Core;
using static Mal.Printer;
using static Mal.Reader;

[assembly: InternalsVisibleTo("Mal.Tests")]

namespace Mal
{
    public delegate string? StepFunction(string text, Env env);

    public static class Program
    {
        // creates a new Env when necessary, remember Env is mutated
        public static Env EmptyEnv() => new Env(MapM.Empty<Symbol, MalType>(), null);
        public static Env DefaultEnv() => new Env(Ns, null);

        public static StepFunction ReplStep = (text, env) =>
            ReadText(text)?.Pipe(mal => Eval(mal, env)).Pipe(mal => Printer.PrintStr(mal, true));

        public static void InitEnv(StepFunction step, Env env, string[]? cmdArgs = null)
        {
            env.Set(new Symbol("*ARGV*", NilV), new List((cmdArgs ?? new string[0]).Skip(1).Select(arg => new Str(arg) as MalType).ToLList(), ListType.List, NilV));

            env.Set(new Symbol("eval", NilV), new Fn(args => args switch
            {
                (var Mal, null) => Eval(Mal, env),
                _ => throw new Exception($"'eval' function requires one argument, but got {args.JoinMalTypes(", ")}")
            }, NilV));

            //e.set("eval", fn(([ast]: MalType[]) => eval_(ast, e), nil));

            step("(def! not (fn* (a) (if a false true)))", env);
            step("(def! load-file (fn* (f) (eval (read-string (str \"(do \" (slurp f) \"\\nnil)\")))))", env);
        }

        static void Main(string[] args)
        {
            var env = DefaultEnv();
            InitEnv(ReplStep, env, args);

            if (args is { Length: > 0 })
            {
                try
                {
                    var text = $"(load-file \"{args[0]}\"){Environment.NewLine}";
                    Console.WriteLine(ReplStep(text, env) ?? "<empty>");
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error: {exception.Message}");
                }
                return;
            }

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
