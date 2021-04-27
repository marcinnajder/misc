using System;
using System.Runtime.CompilerServices;
using PowerFP;
using static Mal.Types;

[assembly: InternalsVisibleTo("Mal.Tests")]

namespace Mal
{
    class Program
    {

        private static string CleanUpText(string text) =>
         text;
        //text.Replace("\\\\", "\u029e").Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\u029e", "\\");
        static void Main(string[] args)
        {
            Console.WriteLine(CleanUpText("asada \\\\ sd \\n asd"));

            while (true)
            {
                var text = Console.ReadLine();
                Console.WriteLine(text);
            }
        }
    }
}
