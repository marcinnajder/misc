
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Mal.Types;


namespace Mal.Tests
{
    public static class MalStepsRunner
    {
        private enum Option { Deferrable, Optional, Soft }

        private record TestCaseLine { }
        private record OptionLine(Option Option) : TestCaseLine { }
        private record InputLine(string Text) : TestCaseLine { }
        private record OutputLine(string Text) : TestCaseLine { }

        private record TestCase(string Input, List<string> Output, List<Option> Options) { }




        //"../../../Steps/step0_repl.mal"
        private static TestCase[] readTestCase(string testFilePath) =>
            File.ReadAllLines(testFilePath)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l))
                .Where(l => !l.StartsWith(";;")) // skip comments
                .Select(l => l switch
               {
                   ";>>> optional=True" => new OptionLine(Option.Optional) as TestCaseLine,
                   ";>>> deferrable=True" => new OptionLine(Option.Deferrable),
                   ";>>> soft=True" => new OptionLine(Option.Soft),
                   _ when l.StartsWith(";=>") && l.StartsWith(";/.") && l.StartsWith(";/") => new OutputLine(l),
                   _ => new OutputLine(l)
               })
               .Aggregate(new List<TestCase>(), (agg, line) =>
               {
                   switch (line)
                   {
                       case InputLine(var Text):
                           {
                               agg.Add(new TestCase(Text, new(), new()));
                               break;
                           }
                       case OutputLine(var Text):
                           {
                               agg.Last().Output.Add(Text);
                               break;
                           }
                       case OptionLine(var Option):

                           {
                               agg.Last().Options.Add(Option);
                               break;
                           }
                   }
                   return agg;
               })
               .ToArray();





        // function readTestCases(fileName: string): TestCase[] {
        //   const result: TestCase[] = [];
        //   const testContent = fs.readFileSync(path.join(testsPath, fileName), "utf-8");
        //   const testLines = testContent.split(os.EOL);

        //   var tests = pipe(
        //     testLines,
        //     map(l => l.trim()),
        //     filter(l => l !== ""), // skip empty lines
        //     filter(l => !/^;;/.test(l)), // skip comments

        //     map<string, TestCaseLine>(l => {
        //       if (l === ";>>> optional=True") return { type: "option", option: "optional" };
        //       if (l === ";>>> deferrable=True") return { type: "option", option: "deferrable" };
        //       if (l === ";>>> soft=True") return { type: "option", option: "soft" };
        //       if (l.startsWith(";=>") || l.startsWith(";/.") || l.startsWith(";/")) return { type: "output", text: l };
        //       return { type: "input", text: l };
        //     }),

        //     scan<TestCaseLine, TestCase | null>((agg, l) => {
        //       if (agg != null) {
        //         switch (l.type) {
        //           case "input": return { input: l.text, output: [], options: [...agg.options] };
        //           case "output": {
        //             agg.output.push(l.text);
        //             return agg;
        //           }
        //           case "option": {
        //             agg.options.push(l.option)
        //             return agg;
        //           }
        //           default: return assertNever(l);
        //         }
        //       } else {
        //         switch (l.type) {
        //           case "input": return { input: l.text, output: [], options: [] };
        //           default: {
        //             throw `incorrect format of file '${fileName}' (input line should be first)`
        //           }
        //         }
        //       }
        //     }, null),
        //     filter(o => o !== null),    // skip first "null" item
        //     distinctuntilchanged(),     // for many outputs the same testcase is duplicated
        //     toarray()
        //   ) as any as TestCase[];

        //   return tests;
        // }


    }
}

