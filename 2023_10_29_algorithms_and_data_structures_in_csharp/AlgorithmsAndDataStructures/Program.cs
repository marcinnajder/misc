using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Numerics;
using System.Reflection;
using System.Runtime;
using System.Security.Cryptography;
using AlgorithmsAndDataStructures;


EnumerableExtensions.Run();
PipeExtensions.Run();
OtherExtension.Run();

// Cre1.Run();
// Cre2.Run();

//Monads.Test();
// ExpressionTreesPatternMatching.ExpressionTrees();
// ExpressionTreesPatternMatching.VisitorTest();

// class BindingFlags{
//     public MyStruct a;
// }




// if (int.TryParse("123", out var liczba) == true)
// {
//     Console.WriteLine(liczba);
// }


// Console.WriteLine(liczba);

// int? TryParseInt(string text) => throw new AsnReader()




// var add = (int a, int b) => a + b;
// var add2 = (int a) => (int b) => a + b;

// var inc = add2(1);
// var aaaaa = inc(100);


// cuurried function








// List<int> ints = [1, 2, 3, 1];
// List<string> strings = ["a", "b", "ab", "abcdef"];

// var q = ints.GroupJoin(strings, i => i, s => s.Length, (i, ss) => new { i, ss = String.Join(",", ss) });

// Console.WriteLine(string.Join(",", q)); // { i = 1, ss = a,b },{ i = 2, ss = ab },{ i = 3, ss =  }



// // System.Linq.Enumerable.Union()


// //var t = string.Join("-", new[] { 1, 2, 3, 4 }.Chunk(3).Select(c => string.Join(",", c)));

// //Console.WriteLine(t);

// //System.Linq.Enumerable.Chunk()

// static void PrintAllMethods(Type type, bool inherited = false)
// {
//     IEnumerable<Type> types = inherited ? GetParents(type) : [type];

//     var names = types
//         .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
//         .Select(m => m.Name)
//         .Distinct();


//     Console.WriteLine($"Methods of : {type.Name}");
//     Console.WriteLine(String.Join(Environment.NewLine, names));
//     Console.WriteLine();

//     static IEnumerable<Type> GetParents(Type type)
//     {
//         foreach (var t in type.GetInterfaces().Concat([type.BaseType]).Where(t => t != null).SelectMany(GetParents))
//         {
//             yield return t;
//         }
//         yield return type;
//     }
// }

// PrintAllMethods2(typeof(IList<>), inherited: true);

// PrintAllMethods<IList<int>>(inherited: true);
// PrintAllMethods<List<int>>();


// Console.WriteLine("IList");
// Console.WriteLine(String.Join(Environment.NewLine, typeof(IList<>).GetMethods(System.Reflection.BindingFlags.Instance).Select(m => m.Name)));

// Console.WriteLine();
// Console.WriteLine("List");
// Console.WriteLine(String.Join(Environment.NewLine, typeof(List<>).GetMethods().Select(m => m.Name)));