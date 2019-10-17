import "../../src/useDevPowerfp";
import { tokenize, read_str } from "../../src/reader";
import { inspect } from "util";
import { Result_ok, Option_some } from "powerfp";
import { MalType } from "src/types";


console.log(tokenize("(+ 1  ;;; ( +    1 2 )"));

// console.log(tokenize(`( +   "a" "b")`));



// const text = "( +    1 2 )";
// //const text = ";";
// const { value: { value } } = read_str(text) as Result_ok<Option_some<MalType>>;
// console.log(inspect(value, false, 1000));



