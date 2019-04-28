import { MalType_number, MalFuncType, MalType, MalType_list, malEqual } from "./types";
import { ResultS, ok, error } from "./utils/result";
import { pr_str, PrintLineType } from "./printer";

// type NumberFuncType = (args: MalType_number[]) => ResultS<MalType_number>;
export const __printLine = "__printLine";
export type Ns = { [symbol: string]: MalFuncType };

export const ns = {
  "+": (args: MalType_number[]) => ok(args.reduce(({ value: a }, { value: e }) => ({ type: "number", value: a + e }))),
  "-": (args: MalType_number[]) => ok(args.reduce(({ value: a }, { value: e }) => ({ type: "number", value: a - e }))),
  "*": (args: MalType_number[]) => ok(args.reduce(({ value: a }, { value: e }) => ({ type: "number", value: a * e }))),
  "/": (args: MalType_number[]) => ok(args.reduce(({ value: a }, { value: e }) => ({ type: "number", value: a / e }))),

  // ??
  "pr-str": (args: MalType[]) => {
    return ok({ type: "string", value: pr_strEachThenJoin(args, true, " ") } as MalType)

  },


  "str": (args: MalType[]) => ok({ type: "string", value: pr_strEachThenJoin(args, false, "") } as MalType),

  // ??
  "prn": (args: MalType[]) => !ns[__printLine] ? error("___printLine is not set on env") :
    //(args.forEach(exp => (ns[__printLine] as any as PrintLineType)(pr_str(exp, true))), ok({ type: "nil" } as MalType))
    ((ns[__printLine] as any as PrintLineType)(...args.map(exp => pr_str(exp, true))), ok({ type: "nil" } as MalType))
  ,

  "println": (args: MalType[]) => !ns[__printLine] ? error("___printLine is not set on env") :
    //(args.forEach(exp => (ns[__printLine] as any as PrintLineType)(pr_str(exp, false))), ok({ type: "nil" } as MalType)),
    ((ns[__printLine] as any as PrintLineType)(...args.map(exp => pr_str(exp, false))), ok({ type: "nil" } as MalType)),

  "list": (args: MalType[]) => ok({ type: "list", listType: "list", items: args } as MalType),
  "list?": ([first]: MalType[]) => ok({ type: first.type === "list" && first.listType === "list" ? "true" : "false" } as MalType),


  "empty?": ([first]: MalType[]) => first.type === "list" ? ok({ type: first.items.length === 0 ? "true" : "false" } as MalType)
    : error(`'empty?' requires 'list' as a first argument`),
  "count": ([first]: MalType[]) => ok({ type: "number", value: first.type === "list" ? first.items.length : 0 } as MalType),


  "=": ([first, second]: MalType[]) => ok({ type: malEqual(first, second) ? "true" : "false" }),

  "<": ([first, second]: MalType_number[]) => ok({ type: first.value < second.value ? "true" : "false" }),
  "<=": ([first, second]: MalType_number[]) => ok({ type: first.value <= second.value ? "true" : "false" }),
  ">": ([first, second]: MalType_number[]) => ok({ type: first.value > second.value ? "true" : "false" }),
  ">=": ([first, second]: MalType_number[]) => ok({ type: first.value >= second.value ? "true" : "false" })

} as Ns;



function pr_strEachThenJoin(args: MalType[], print_readably: boolean, separator: string) {
  return args.map(mal => pr_str(mal, print_readably)).join(separator)
};