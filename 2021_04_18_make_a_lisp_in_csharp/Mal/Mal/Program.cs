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
                Console.WriteLine(text);
            }
        }
    }
}
