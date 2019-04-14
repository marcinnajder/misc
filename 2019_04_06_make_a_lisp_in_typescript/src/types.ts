
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

// ast

export type MalType_number = { type: "number", value: number };

export type MalType =
  //| { type: "number", value: number }
  MalType_number
  | { type: "symbol", name: string }
  | { type: "list", items: MalType[], listType: ListType }

  | { type: "nil" }
  | { type: "true" }
  | { type: "false" }
  | { type: "string", value: string }

  | { type: "quote", mal: MalType }
  | { type: "quasiquote", mal: MalType }
  | { type: "unquote", mal: MalType }
  | { type: "splice-unquote", mal: MalType }

  | { type: "keyword", name: string }
  ;


// apply

export type EnvironmentType = any;
export type ValueType = any;

export type MalTypeOrValue =
  | { type: "value", value: ValueType }
  | { type: "mal", mal: MalType }
  // | { type: "ast", mal: MalType }
  ;



