import { ResultS } from "./utils/result";

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

export type MalType_number = { type: "number", value: number };
export type MalType_symbol = { type: "symbol", name: string };
export type MalType_string = { type: "string", value: string };
export type MalType_keyword = { type: "keyword", name: string };


export type MalType_list = { type: "list", items: MalType[], listType: ListType };
export type MalType_fn = { type: "fn", fn: MalFuncType };


export type MalType =
  | MalType_number
  | MalType_symbol
  | MalType_list

  | { type: "nil" }
  | { type: "true" }
  | { type: "false" }
  | MalType_string

  | { type: "quote", mal: MalType }
  | { type: "quasiquote", mal: MalType }
  | { type: "unquote", mal: MalType }
  | { type: "splice-unquote", mal: MalType }

  | MalType_keyword

  | MalType_fn
  ;


// apply

// export type EnvironmentType = any;
export type ValueType = any;

// export type MalTypeOrValue =
//   | { type: "value", value: ValueType }
//   | { type: "mal", mal: MalType }
//   // | { type: "ast", mal: MalType }
//   ;


export function malEqual(mal1: MalType, mal2: MalType): boolean {

  if (mal1.type !== mal2.type) {
    return false;
  } else {
    switch (mal1.type) {
      case "number": return mal1.value == (mal2 as MalType_number).value;
      case "symbol": return mal1.name == (mal2 as MalType_symbol).name;
      case "string": return mal1.value == (mal2 as MalType_string).value;
      case "keyword": return mal1.name == (mal2 as MalType_keyword).name;

      case "true":
      case "false":
      case "nil": return true;

      case "quote":
      case "quasiquote":
      case "unquote":
      case "splice-unquote": return malEqual(mal1, mal2);

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
    }
    return false; // to make compiler happy
  }
}





