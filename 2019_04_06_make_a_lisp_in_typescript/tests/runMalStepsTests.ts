import "../src/useDevPowerfp";
import * as path from "path";
import * as fs from "fs";
import * as os from "os";
import assert = require("assert");
import { pipe, map, toarray, filter, scan, intersect, count, distinctuntilchanged } from "powerseq";
import { step_read_print, step_repl, step_eval, StepFunc } from "../src/steps";
import { defaultEnv, Env } from "../src/env";
import { assertNever } from "../src/types";
import { __printLine, ns } from "../src/core";
import { PrintLineType, pr_str } from "../src/printer";
import { initEnv } from "../src";
import { inspect } from "util";

interface TestCase {
  input: string;
  output: string[];
  options: TestOptions;
}
type TestOption = "deferrable" | "optional" | "soft";
type TestOptions = TestOption[];

type TestCaseLine =
  | { type: "option", option: TestOption }
  | { type: "input", text: string }
  | { type: "output", text: string };

const repoPath = path.join(__dirname, "../../");
const testsPath = path.join(repoPath, "tests");

runAllTests();

function runAllTests() {
  executeTest("malSteps/step0_repl.mal", step_repl);
  executeTest("malSteps/step1_read_print.mal", step_read_print, ["deferrable", "optional"]);
  executeTest("malSteps/step2_eval.mal", step_eval, ["deferrable", "optional"]);
  executeTest("malSteps/step3_env.mal", step_eval, ["deferrable", "optional"]);
  executeTest("malSteps/step4_if_fn_do.mal", step_eval, ["deferrable", "optional"]);
  //executeTest("malSteps/step5_tco.mal", step2_eval, ["deferrable", "optional"]);
  executeTest("malSteps/step6_file.mal", step_eval, ["deferrable", "optional", "soft"]);
  executeTest("malSteps/step7_quote.mal", step_eval, ["deferrable", "optional"]);
  executeTest("malSteps/step8_macros.mal", step_eval, ["deferrable", "optional"]);
  executeTest("malSteps/step9_try.mal", step_eval, ["deferrable", "optional"]);
  executeTest("malSteps/stepA_mal.mal", step_eval, ["deferrable", "optional", "soft"]);
}


function executeTest(fileName: string, step: StepFunc, options: TestOptions = []) {
  let consoleOutputs: string[] = [];
  const env = defaultEnv(ns);
  initEnv(step, env);
  //((ns as any)[__printLine] as PrintLineType) = (...s) => consoleOutputs.push(pr_str({ type: "string", value: s.join("") }, true));

  ((ns as any)[__printLine] as PrintLineType) = (...s) => consoleOutputs.push(s.join(""));


  console.log(`***********************************************************************************************`);
  console.log(`Runing file ${fileName} ...`);
  try {
    const testCases = readTestCases(fileName);

    //console.log(testCases);

    for (const testCase of testCases) {
      // skip test when option not match
      if (testCase.options.length > 0 && pipe(testCase.options, intersect(options), count()) < testCase.options.length) {
        continue;
      }

      consoleOutputs = []; // reset console outputs

      if (testCase.input.indexOf(`"`) !== -1) {
        debugger;
      }

      const result = step(testCase.input, env);

      console.log(testCase.input);

      switch (result.type) {
        case "error": {
          const errorOutput = testCase.output.find(o => o.startsWith(';/.'));
          if (errorOutput) {
            console.log(result.error, "~", testCase.output);
          } else {
            assert.fail(result.error);
          }
          break;
        }
        case "ok": {
          //testCase.output = testCase.output.substr(";=>".length);

          console.log(result.value);

          //assert.equal(result.value, testCase.output);

          assert.deepEqual(
            [...consoleOutputs, ...(result.value === "#<function>" ? [] : [result.value])],
            testCase.output
              .map(o => o.indexOf(";=>") === -1 ? o : o.substr(";=>".length))
              .map(o => o.indexOf(";/") === -1 ? o : o.substr(";/".length))
          );
        }
      }
    }
    console.log(" test succeeded")
  }
  catch (err) {
    console.error(" test failed")
    console.error(err);
  }
  console.log();
}


function readTestCases(fileName: string): TestCase[] {
  const result: TestCase[] = [];
  const testContent = fs.readFileSync(path.join(testsPath, fileName), "utf-8");
  const testLines = testContent.split(os.EOL);

  var tests = pipe(
    testLines,
    map(l => l.trim()),
    filter(l => l !== ""), // skip empty lines
    filter(l => !/^;;/.test(l)), // skip comments

    map<string, TestCaseLine>(l => {
      if (l === ";>>> optional=True") return { type: "option", option: "optional" };
      if (l === ";>>> deferrable=True") return { type: "option", option: "deferrable" };
      if (l === ";>>> soft=True") return { type: "option", option: "soft" };
      if (l.startsWith(";=>") || l.startsWith(";/.") || l.startsWith(";/")) return { type: "output", text: l };
      return { type: "input", text: l };
    }),

    scan<TestCaseLine, TestCase | null>((agg, l) => {
      if (agg != null) {
        switch (l.type) {
          case "input": return { input: l.text, output: [], options: [...agg.options] };
          case "output": {
            agg.output.push(l.text);
            return agg;
          }
          case "option": {
            agg.options.push(l.option)
            return agg;
          }
          default: return assertNever(l);
        }
      } else {
        switch (l.type) {
          case "input": return { input: l.text, output: [], options: [] };
          default: {
            throw `incorrect format of file '${fileName}' (input line should be first)`
          }
        }
      }
    }, null),
    filter(o => o !== null),    // skip first "null" item
    distinctuntilchanged(),     // for many outputs the same testcase is duplicated
    toarray()
  ) as any as TestCase[];

  return tests;
}




