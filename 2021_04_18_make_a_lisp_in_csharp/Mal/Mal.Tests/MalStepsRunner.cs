
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerFP;

namespace Mal.Tests
{
    public static class MalStepsRunner
    {
        public enum Option { Deferrable, Optional, Soft }
        public record TestCase(string Input, List<string> Output, List<Option> Options) { }

        private static Dictionary<string, Option> OptionLines = Enum.GetValues<Option>()
            .ToDictionary(option => $";>>> {option.ToString().ToLower()}=True");

        public static TestCase[] ReadTestCases(string testFilePath) =>
            File.ReadAllLines(testFilePath)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l))
                .Where(l => !l.StartsWith(";;")) // skip comments
                .Aggregate(new List<TestCase>(), (agg, l) =>
               {
                   if (OptionLines.TryGetValue(l, out var option))
                   {
                       agg.Last().Options.Add(option);
                   }
                   else if (l.StartsWith(";=>") || l.StartsWith(";/.") || l.StartsWith(";/"))
                   {
                       agg.Last().Output.Add(l);
                   }
                   else
                   {
                       agg.Add(new TestCase(l, new(), new()));
                   }
                   return agg;
               })
               .ToArray();

        public static void ExecuteTest(string fileName, bool verbose, Func<string, object?, string> stepFunc, params Option[] options)
        {

            Action<string> Log = verbose ? Console.WriteLine : _ => { };

            Log("******************************");
            Log($"Runing file '{fileName}' ... ");

            try
            {
                var testCases = ReadTestCases(fileName);
                foreach (var testCase in testCases)
                {
                    // skip test when option not match
                    if (testCase.Options.Count > 0 && testCase.Options.Intersect(options).Count() < testCase.Options.Count)
                    {
                        continue;
                    }

                    try
                    {
                        var consoleOutputs = new List<string>();

                        Log(testCase.Input);
                        var result = stepFunc(testCase.Input, null/*env*/);
                        Log("-> " + result);

                        var expected = testCase.Output
                            .Select(l => new[] { ";=>", ";/" }.Aggregate(l, (ll, prefix) => ll.StartsWith(prefix) ? ll.Substring(prefix.Length) : ll))
                            .ToLList();

                        var actual = consoleOutputs.Concat(new[] { result }.Where(l => l != "#<function>"))
                            .ToLList();

                        Assert.AreEqual(expected, actual);

                        Log(" test succeeded");
                    }
                    catch (Exception stepException)
                    {
                        var errorOutputLine = testCase.Output.FirstOrDefault(l => l.StartsWith(";/."));
                        if (errorOutputLine != null)
                        {
                            Log($"{stepException.Message} ~ {errorOutputLine}");
                        }
                        else
                        {
                            Assert.Fail(stepException.Message);
                        }
                        break;
                    }
                }
            }
            catch (Exception exception)
            {
                Log(exception.Message);
                Log(" test failed");
            }
        }
    }
}