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
                        Console.WriteLine(mal == null ? "<empty>" : Printer.PrintStr(mal));
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
