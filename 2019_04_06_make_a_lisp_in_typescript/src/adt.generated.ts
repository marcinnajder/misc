// ** this code was generated automatically **
import { UnionChoice } from "powerfp";
import { MalType } from "./types";
import { ListType, MalFuncType } from "./types";

export type MalType_number_ = UnionChoice<MalType, "number_">;
export const number_ = (value: number) => ({ type: "number_", value }) as MalType_number_;
export type MalType_symbol = UnionChoice<MalType, "symbol">;
export const symbol = (name: string) => ({ type: "symbol", name }) as MalType_symbol;
export type MalType_list = UnionChoice<MalType, "list">;
export const list = (items: MalType[], listType: ListType) => ({ type: "list", items, listType }) as MalType_list;
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
export const fn = (fn: MalFuncType) => ({ type: "fn", fn }) as MalType_fn;
export type MalType_atom = UnionChoice<MalType, "atom">;
export const atom = (mal: MalType) => ({ type: "atom", mal }) as MalType_atom;
