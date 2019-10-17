import { MalType_number_, MalFuncType, MalType, malEqual, string_, nil, list, true_, false_ } from "./types";
import { ok, error } from "powerfp";
import { pr_str, PrintLineType } from "./printer";
import { number_ } from "./adt.generated";

// type NumberFuncType = (args: MalType_number[]) => ResultS<MalType_number>;
export const __printLine = "__printLine";
export type Ns = { [symbol: string]: MalFuncType };

export const ns = {
  "+": (args: MalType_number_[]) => ok(args.reduce(({ value: a }, { value: e }) => number_(a + e))),
  "-": (args: MalType_number_[]) => ok(args.reduce(({ value: a }, { value: e }) => number_(a - e))),
  "*": (args: MalType_number_[]) => ok(args.reduce(({ value: a }, { value: e }) => number_(a * e))),
  "/": (args: MalType_number_[]) => ok(args.reduce(({ value: a }, { value: e }) => number_(a / e))),

  // ??
  "pr-str": (args: MalType[]) => {
    return ok(string_(pr_strEachThenJoin(args, true, " ")));
  },


  "str": (args: MalType[]) => ok(string_(pr_strEachThenJoin(args, false, ""))),

  // ??
  "prn": (args: MalType[]) => !ns[__printLine] ? error("___printLine is not set on env") :
    //(args.forEach(exp => (ns[__printLine] as any as PrintLineType)(pr_str(exp, true))), ok({ type: "nil" } as MalType))
    ((ns[__printLine] as any as PrintLineType)(...args.map(exp => pr_str(exp, true))), ok(nil))
  ,

  "println": (args: MalType[]) => !ns[__printLine] ? error("___printLine is not set on env") :
    //(args.forEach(exp => (ns[__printLine] as any as PrintLineType)(pr_str(exp, false))), ok({ type: "nil" } as MalType)),
    ((ns[__printLine] as any as PrintLineType)(...args.map(exp => pr_str(exp, false))), ok(nil)),

  "list": (args: MalType[]) => ok(list(args, "list")),
  "list?": ([first]: MalType[]) => ok(first.type === "list" && first.listType === "list" ? true_ : false_),


  "empty?": ([first]: MalType[]) => first && first.type === "list" ? ok(first.items.length === 0 ? true_ : false_)
    : error(`'empty?' requires 'list' as a first argument`),

  // "count": ([first]: MalType[]) => first && first.type === "list" ? ok(number_(first.items.length))
  //   : error(`'count' requires 'list' as a first argument`),
  // according to test case "(count nil)" should return "0"
  "count": ([first]: MalType[]) => ok(number_(first.type === "list" ? first.items.length : 0)),


  "=": ([first, second]: MalType[]) => ok(malEqual(first, second) ? true_ : false_),

  "<": ([first, second]: MalType_number_[]) => ok(first.value < second.value ? true_ : false_),
  "<=": ([first, second]: MalType_number_[]) => ok(first.value <= second.value ? true_ : false_),
  ">": ([first, second]: MalType_number_[]) => ok(first.value > second.value ? true_ : false_),
  ">=": ([first, second]: MalType_number_[]) => ok(first.value >= second.value ? true_ : false_)

} as Ns;



function pr_strEachThenJoin(args: MalType[], print_readably: boolean, separator: string) {
  return args.map(mal => pr_str(mal, print_readably)).join(separator)
};