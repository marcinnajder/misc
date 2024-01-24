import { MalType_number_, MalFuncType, MalType, malEqual, string_, nil, list, true_, false_, atom, MalType_fn, fn, MalType_false_, listToMap, MalType_map, mapToList, map, keyToString, KeyType, stringToKey, MalType_with_meta } from "./types";
import { flatmap, toobject } from "powerseq";
import { ok, error, matchUnion, resultMapM, ResultS, isUnion, TypedObj, UnionChoice, Result_ok } from "powerfp";
import * as fs from "fs";
import { pr_str, PrintLineType } from "./printer";
import { read_str } from "./reader";
import { number_, MalType_string_, MalType_atom, MalType_list, MalType_symbol, symbol, MalType_true_, MalType_keyword, keyword } from "./adt.generated";

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

  "list": (args: MalType[]) => ok(list(args, "list", nil)),
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
  ">=": ([first, second]: MalType_number_[]) => ok(first.value >= second.value ? true_ : false_),

  "read-string": ([text]: MalType_string_[]) => read_str(text.value).map(malOp => malOp.type === "some" ? malOp.value : nil),
  //"read-string": ([text]: MalType_string_[]) => read_str(text.value).map(malOp => malOp.type === "some" ? malOp.value : list([], "list")),

  "slurp": ([fileName]: MalType_string_[]) => {
    try { return ok(string_(fs.readFileSync(fileName.value, "utf8"))); } catch (err) { return error((err as any).toString()); }
  },

  "atom": ([mal]: MalType[]) => ok(atom(mal)),
  "atom?": ([mal]: MalType[]) => ok(mal.type === "atom" ? true_ : false_),
  "deref": ([atom]: MalType_atom[]) => ok(atom.mal),
  "reset!": ([atom, mal]: MalType_atom[]) => {
    atom.mal = mal;
    return ok(mal);
  },
  "swap!": (mals: MalType[]) => {
    const [atom, fn, ...args]: [MalType_atom, MalType_fn, MalType] = mals as any;
    return fn.fn([atom.mal, ...args]).map(result => {
      atom.mal = result;
      return result;
    });
  },

  "cons": (mals: MalType[]) => {
    const [item, itemList]: [MalType, MalType_list] = mals as any;
    return ok(list([item, ...itemList.items], "list", nil));
  },
  "concat": (mals: MalType_list[]) => {
    return ok(list([...flatmap(mals, l => l.items)], "list", nil));
  },


  "nth": (mals: MalType[]) => {
    const [{ items }, { value: index }]: [MalType_list, MalType_number_] = mals as any;
    return index < items.length ? ok(items[index]) : error(`index '${index}' out of bounds '0..${items.length - 1}' inside 'nth' function`);
  },
  "first": ([mal]: MalType_list[]) => {
    return mal && mal.type === "list" && mal.items.length > 0 ? ok(mal.items[0]) : ok(nil);
  },
  "rest": ([mal]: MalType_list[]) => {
    return ok(list((mal && mal.type === "list" && mal.items.slice(1)) || [], "list", nil));
  },

  "throw": ([mal]: MalType[]) => {
    // JS exception needs to be thrown because ResultS<T> is returned so only string type as error is allowed
    throw mal;
  },
  "apply": (mals: MalType[]) => {
    const [fn, ...args]: [MalType_fn, MalType] = mals as any;
    if (fn.type !== "fn") {
      return error("first argument to 'apply' must be a fn");
    }
    const items = args.reduce((all, mal) => [...all, ...matchUnion(mal, { "list": ({ items }) => items, "_": (m: MalType) => [m] })], [] as MalType[]);
    return fn.fn(items);
  },
  "map": (mals: MalType[]) => {
    const [fn, collection]: [MalType_fn, MalType_list] = mals as any;
    if (fn.type !== "fn") {
      return error("first argument to 'map' must be a fn");
    }
    return resultMapM(collection.items, (item: MalType) => fn.fn([item])).bind(items => ok(list(items, "list", nil)));
  },

  "nil?": ([mal]: MalType[]) => isMalType(mal, "nil"),
  "true?": ([mal]: MalType[]) => isMalType(mal, "true_"),
  "false?": ([mal]: MalType[]) => isMalType(mal, "false_"),
  "symbol?": ([mal]: MalType[]) => isMalType(mal, "symbol"),

  "symbol": ([stringMal]: MalType_string_[]) => ok(symbol(stringMal.value, nil)),
  "keyword": ([keywordMal]: (MalType_keyword | MalType_string_)[]) => keywordMal.type === "keyword" ? ok(keywordMal) : ok(keyword(keywordMal.value)),
  "keyword?": ([mal]: MalType[]) => isMalType(mal, "keyword"),
  "vector": (mals: MalType[]) => ok(list(mals, "vector", nil)),
  "vector?": ([mal]: MalType[]) => isMalType(mal, "list", ({ listType }) => listType === "vector"),

  "sequential?": ([mal]: MalType[]) => isMalType(mal, "list"),

  "hash-map": (mals: MalType[]) => listToMap(mals),
  "map?": ([mal]: MalType[]) => isMalType(mal, "map"),
  "assoc": (mals: MalType[]) => {
    const [mapMal, ...keysValues]: [MalType_map, MalType] = mals as any;
    return listToMap([...mapToList(mapMal.map), ...keysValues]);
  },
  "dissoc": (mals: MalType[]) => {
    const [mapMal, ...keys]: [MalType_map, KeyType?] = mals as any;
    if (keys.length === 0) {
      return ok(mapMal);
    }
    const keysToBeDeleted = new Set(keys.map(x => keyToString(x!)));
    const mapEntries = Object.entries(mapMal.map);
    const itemsLeft = mapEntries.filter(kv => !keysToBeDeleted.has(kv[0]));
    return ok(itemsLeft.length === mapEntries.length ? mapMal : map(toobject(itemsLeft, kv => kv[0], kv => kv[1]), nil));
  },
  "get": (mals: MalType[]) => {
    const [mapMal, key]: [MalType_map, KeyType] = mals as any;
    return mapMal.type === "map" ? ok(mapMal.map[keyToString(key)] || nil) : ok(mapMal);
  },
  "contains?": (mals: MalType[]) => {
    const [mapMal, key]: [MalType_map, KeyType] = mals as any;
    return ok(keyToString(key) in mapMal.map ? true_ : false_);
  },
  "keys": ([mapMal]: MalType_map[]) => {
    return ok(list(Object.keys(mapMal.map).map(stringToKey), "list", nil));
  },
  "vals": ([mapMal]: MalType_map[]) => {
    return ok(list(Object.values(mapMal.map), "list", nil));
  },


  "time-ms": (mals: MalType[]) => {
    return ok(number_(new Date().getTime()));
  },
  "conj": (mals: MalType[]) => {
    const [collection, ...args]: [MalType_list, MalType?] = mals as any;
    if (args.length === 0) {
      return ok(collection);
    }
    return ok(list(collection.listType === "list" ?
      [...(args.reverse()) as any, ...collection.items] : [...collection.items, ...args], collection.listType, nil));
  },

  "meta": ([mal]: MalType[]) => ok((mal as MalType_with_meta).meta),
  "with-meta": (mals: MalType[]) => {
    const [mal, metaMal]: [MalType, MalType] = mals as any;
    return ok(matchUnion(mal, {
      "list": (listMal) => list(listMal.items, listMal.listType, metaMal),
      "map": (mapMal) => map(mapMal.map, metaMal),
      "symbol": (symbolMal) => symbol(symbolMal.name, metaMal),
      "fn": (fnMal) => fn(fnMal.fn, metaMal),
      "_": () => mal,
    }));
  },

  "string?": ([mal]: MalType[]) => isMalType(mal, "string_"),
  "number?": ([mal]: MalType[]) => isMalType(mal, "number_"),
  "fn?": ([mal]: MalType_fn[]) => isMalType(mal, "fn").map(v => v.type === "true_" && !malEqual(mal.meta, isMacroMeta) ? true_ : false_),
  "macro?": ([mal]: MalType_fn[]) => isMalType(mal, "fn").map(v => v.type === "true_" && malEqual(mal.meta, isMacroMeta) ? true_ : false_),
  "seq": ([mal]: MalType[]) => ok(matchUnion(mal, {
    "list": (listMal) => listMal.items.length === 0 ? nil :
      (listMal.listType === "list" ? listMal : list(listMal.items, "list", nil)) as any,
    "string_": (strMal) => strMal.value.length === 0 ? nil : list([...strMal.value].map(string_), "list", nil) as any,
    "_": (x: MalType) => x
  })),

} as Ns;


export const isMacroMeta = (listToMap([string_("is_macro"), true_]) as Result_ok<MalType>).value;

function isMalType<Type extends MalType["type"]>(mal: MalType, malType: Type, predicate: (m: UnionChoice<MalType, Type>) => boolean = () => true): ResultS<MalType_true_ | MalType_false_> {
  return ok(mal && isUnion(mal, malType) && predicate(mal) ? true_ : false_);
}

function pr_strEachThenJoin(args: MalType[], print_readably: boolean, separator: string) {
  return args.map(mal => pr_str(mal, print_readably)).join(separator)
}
