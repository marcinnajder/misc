using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PowerFP;
using static Mal.Types;

[assembly: InternalsVisibleTo("Mal.Tests")]


namespace Mal
{
    class Program
    {

        static void Main(string[] args)
        {
            while (true)
            {
                var text = Console.ReadLine();
                if (text != null)
                {
                    try
                    {
                        MalType? mal = Reader.ReadText(text);
                        if (mal == null)
                        {
                            Console.WriteLine("<empty>");
                        }
                        else
                        {
                            mal = EvalM.Eval(mal, EnvM.DefaultEnv);
                            Console.WriteLine(Printer.PrintStr(mal, true));
                        }
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
