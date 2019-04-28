import { readFileSync } from "fs";
import { pipe, takewhile, scan, toarray, last, map } from "powerseq";
import { EOL } from "os";
import { join } from "path";

test();

function test() {
  const jsonContent = readFileSync(join(__dirname, "../../../tests/misc/sample.json"), "utf-8");

  try {
    const json = JSON.parse(jsonContent)
  }
  catch (err) {

    //Unexpected end of JSON input 
    const prefix = "in JSON at position ";
    const prefixIndex = err.message.indexOf(prefix);
    console.log({ prefixIndex });

    if (prefixIndex !== -1) {
      console.log(err.message);
      const charNumber = parseInt(err.message.substr(prefixIndex + prefix.length));
      console.log({ charNumber });
      findLineAndColumn(jsonContent, charNumber);
    } else {
      console.log(err);
    }
  }
}

function findLineAndColumn(jsonContent: string, charNumber: number) {

  const lines = jsonContent.split(EOL);
  const a = pipe(
    lines,
    scan((agg, line) => agg + line.length + 1, 0),
    takewhile(lastCharInLineNumber => lastCharInLineNumber < charNumber),
    map((lastCharInLineNumber, lineNumber) => ({ lineNumber: lineNumber, lastCharInLineNumber })),
    last()
    // toarray()
  );

  console.log(a);


  const result = typeof a === "undefined" ? { row: 1, column: charNumber + 1 } : { row: a.lineNumber + 1 + 1, column: (charNumber - a.lastCharInLineNumber) + 1 };
  console.log(result);
  console.log(`sample.json:${result.row}:${result.column}`);

}