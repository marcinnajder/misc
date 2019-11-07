import { ResultS, error, ok, isUnion } from "powerfp";
import { MalType_number_, MalType_symbol, MalType_string_, MalType_keyword, MalType_atom, keyword, MalType_map, string_, nil } from "./adt.generated";
import { toobject, flatmap, toarray, buffer, map as map_, pipe, find } from "powerseq";
import { map } from "./adt.generated";
export * from "./adt.generated";
export type ListType = "list" | "vector";
export type ListTypeAndMap = ListType | "hash-map";

export const list2BracketMap: { [key in ListTypeAndMap]: [string, string] } = {
  "list": ["(", ")"],
  "vector": ["[", "]"],
  "hash-map": ["{", "}"]
};

export const bracket2ListMap: { [key: string]: ListTypeAndMap } =
  toobject(Object.entries(list2BracketMap), ([, [opening]]) => opening, ([key]) => key) as any;


export type MalFuncType = (args: MalType[]) => ResultS<MalType>;
export type MapType = { [key: string]: MalType };
export type KeyType = MalType_keyword | MalType_string_;
export type MalType_with_meta = { meta: MalType };

// ast
export type MalType =
  | { type: "number_"; value: number }
  | { type: "symbol"; name: string; meta: MalType }
  | { type: "list"; items: MalType[]; listType: ListType; meta: MalType }
  | { type: "nil" }
  | { type: "true_" }
  | { type: "false_" }
  | { type: "string_"; value: string }
  | { type: "keyword"; name: string }
  | { type: "fn"; fn: MalFuncType; meta: MalType }
  | { type: "atom"; mal: MalType }
  | { type: "map"; map: MapType; meta: MalType }
  ;


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

      case "map":
      case "list": {
        const items1 = mal1.type === "list" ? mal1.items : mapToList(mal1.map);
        const items2 = mal2.type === "list" ? mal2.items : mapToList((mal2 as MalType_map).map);

        if (items1.length !== items2.length) {
          return false;
        } else {
          for (let i = 0; i < items1.length; i++) {
            if (!(malEqual(items1[i], items2[i]))) {
              return false;
            }
          }
          return true;
        }
      }

      case "fn": {
        return false;
      }
      case "atom": return malEqual(mal1.mal, (mal2 as MalType_atom).mal);
      default: {
        return assertNever(mal1);
      }
    }
  }
}

export function mapToList(map: MapType) {
  return pipe(Object.entries(map), flatmap(([key, value]) => [stringToKey(key), value]), toarray());
}

export function listToMap(mals: MalType[]) {
  return validateMap(mals).map(malss => map(toobject(buffer(malss, 2), kv => keyToString(kv[0] as any), kv => kv[1]), nil));
}

function validateMap(mals: MalType[]): ResultS<MalType[]> {
  if (mals.length % 2 !== 0) {
    return error(`map structure should contain even number of items, but it has ${mals.length} items`);
  }
  var item = find(mals, (item, index) => (index % 2 === 0) && ((item.type !== "keyword") && (item.type !== "string_")));
  if (item) {
    return error(`even item inside map structure should be 'keyword' or 'string', but it is '${item.type}'`);
  }
  return ok(mals);
}


export function keyToString(key: KeyType): string {
  return isUnion(key, "keyword") ? `k${key.name}` : `s${key.value}`;
}

export function stringToKey(str: string): KeyType {
  return str[0] === "k" ? keyword(str.slice(1)) : string_(str.slice(1));
}


export function assertNever(x: never): never {
  throw new Error("Unexpected object: " + x);
}
