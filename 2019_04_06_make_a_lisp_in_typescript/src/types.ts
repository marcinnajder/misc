import { ResultS } from "powerfp";
import { MalType_number_, MalType_symbol, MalType_string_, MalType_keyword, MalType_list } from "./adt.generated";
import { assertNever } from "./utils/common";
export * from "./adt.generated";
export type ListType = "list" | "vector" | "hash-map";

export const list2BracketMap: { [key in ListType]: [string, string] } = {
  "list": ["(", ")"],
  "vector": ["[", "]"],
  "hash-map": ["{", "}"]
};
export const bracket2ListMap: { [key: string]: ListType } = {
  "(": "list",
  "[": "vector",
  "{": "hash-map"
};


export type MalFuncType = (args: MalType[]) => ResultS<MalType>;

// ast

export type MalType =
  | { type: "number_", value: number }
  | { type: "symbol", name: string }
  | { type: "list", items: MalType[], listType: ListType }

  | { type: "nil" }
  | { type: "true_" }
  | { type: "false_" }
  | { type: "string_", value: string }

  | { type: "quote", mal: MalType }
  | { type: "quasiquote", mal: MalType }
  | { type: "unquote", mal: MalType }
  | { type: "splice_unquote", mal: MalType }

  | { type: "keyword", name: string }

  | { type: "fn", fn: MalFuncType }
  ;

export type ValueType = any;





export function malEqual(mal1: MalType, mal2: MalType): boolean {

  if (mal1.type !== mal2.type) {
    return false;
  } else {
    switch (mal1.type) {
      case "number_": return mal1.value == (mal2 as MalType_number_).value;
      case "symbol": return mal1.name == (mal2 as MalType_symbol).name;
      case "string_": return mal1.value == (mal2 as MalType_string_).value;
      case "keyword": return mal1.name == (mal2 as MalType_keyword).name;

      case "true_":
      case "false_":
      case "nil": return true;

      case "quote":
      case "quasiquote":
      case "unquote":
      case "splice_unquote": return malEqual(mal1, mal2);

      case "list": {
        const mal2_list = mal2 as MalType_list;
        if (mal1.items.length !== mal2_list.items.length) {
          return false;
        } else {
          for (let i = 0; i < mal1.items.length; i++) {
            if (!(malEqual(mal1.items[i], mal2_list.items[i]))) {
              return false;
            }
          }
          return true;
        }
      }
      case "fn": {
        return false;
      }
      default: {
        return assertNever(mal1);
      }
    }
  }
}





