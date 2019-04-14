import * as path from "path";
import * as fs from "fs";
import * as os from "os";
import assert = require("assert");
import { pipe, map, toarray, filter, buffer, scan, intersect, count, reduce } from "powerseq";
import { ResultS } from "../src/utils/result";
import { step1_read_print, step0_repl, step2_eval } from "../src/steps";
import { stringify } from "querystring";

type TestFunc = (text: string) => ResultS<string>;
interface TestCase {
  input: string;
  output: string;
  options: TestOptions;
}
type TestOptions = ("deferrable" | "optional")[];
const repoPath = path.join(__dirname, "../../");
const testsPath = path.join(repoPath, "tests");

runAllTests();

function runAllTests() {
  // executeTest("step0_repl.mal", step0_repl);
  // executeTest("step1_read_print.mal", step1_read_print, ["deferrable", "optional"]);
  executeTest("step2_eval.mal", step2_eval, ["deferrable", "optional"]);
}



function executeTest(fileName: string, testFunc: TestFunc, options: TestOptions = []) {
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

      const result = testFunc(testCase.input);
      console.log(testCase.input);

      switch (result.type) {
        case "error": {
          if (testCase.output.startsWith(';/.')) {
            console.log(result.error, "~", testCase.output);
          } else {
            assert.fail(result.error);
          }
          break;
        }
        case "ok": {
          testCase.output = testCase.output.substr(";=>".length);
          console.log(result.value);
          assert.equal(result.value, testCase.output);
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
    scan<string, { text: string | null; options: TestOptions }>((agg, l) => {
      if (l === ";>>> optional=True") return { text: null, options: [...agg.options, "optional"] };
      if (l === ";>>> deferrable=True") return { text: null, options: [...agg.options, "deferrable"] };
      return { text: l, options: agg.options };
    }, { text: null, options: [] }),
    filter(o => o.text !== null),
    buffer(2),
    map(([input, output]) => ({ input: input.text, output: output.text!, options: input.options }) as TestCase),
    toarray()
  )

  return tests;
}







