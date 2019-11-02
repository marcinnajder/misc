// ** this code was generated automatically **
import { UnionChoice } from "powerfp";
import { MalType } from "./types";
import { ListType, MalFuncType, MapType } from "./types";

export type MalType_number_ = UnionChoice<MalType, "number_">;
export const number_ = (value: number) => ({ type: "number_", value }) as MalType_number_;
export type MalType_symbol = UnionChoice<MalType, "symbol">;
export const symbol = (name: string, meta: MalType) => ({ type: "symbol", name, meta }) as MalType_symbol;
export type MalType_list = UnionChoice<MalType, "list">;
export const list = (items: MalType[], listType: ListType, meta: MalType) => ({ type: "list", items, listType, meta }) as MalType_list;
export type MalType_nil = UnionChoice<MalType, "nil">;
export const nil = { type: "nil" } as MalType_nil;
export type MalType_true_ = UnionChoice<MalType, "true_">;
export const true_ = { type: "true_" } as MalType_true_;
export type MalType_false_ = UnionChoice<MalType, "false_">;
export const false_ = { type: "false_" } as MalType_false_;
export type MalType_string_ = UnionChoice<MalType, "string_">;
export const string_ = (value: string) => ({ type: "string_", value }) as MalType_string_;
export type MalType_keyword = UnionChoice<MalType, "keyword">;
export const keyword = (name: string) => ({ type: "keyword", name }) as MalType_keyword;
export type MalType_fn = UnionChoice<MalType, "fn">;
export const fn = (fn: MalFuncType, meta: MalType) => ({ type: "fn", fn, meta }) as MalType_fn;
export type MalType_atom = UnionChoice<MalType, "atom">;
export const atom = (mal: MalType) => ({ type: "atom", mal }) as MalType_atom;
export type MalType_map = UnionChoice<MalType, "map">;
export const map = (map: MapType, meta: MalType) => ({ type: "map", map, meta }) as MalType_map;
