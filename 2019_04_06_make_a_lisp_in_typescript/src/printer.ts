import { MalType, list2BracketMap, stringToKey } from "./types";
import { matchUnion } from "powerfp";

export type PrintLineType = (...s: string[]) => void;

export function pr_str(mal: MalType, print_readably: boolean): string {
  return matchUnion(mal, {
    number_: ({ value }) => value.toString(),
    symbol: ({ name }) => name,
    string_: ({ value }) => value[0] === '\u029e' ? `:${value.slice(1)}` :
      (print_readably ? `"${value.replace(/\\/g, "\\\\").replace(/"/g, '\\"').replace(/\n/g, "\\n")}"` : value),
    keyword: ({ name }) => `:${name}`,
    true_: ({ type }) => removeUnderscore(type),
    false_: ({ type }) => removeUnderscore(type),
    nil: ({ type }) => type,
    list: ({ listType, items }) =>
      `${list2BracketMap[listType][0]}${items.map(m => pr_str(m, print_readably)).join(" ")}${list2BracketMap[listType][1]}`,
    fn: _ => "#<function>",
    atom: ({ mal }) => `(atom ${pr_str(mal, print_readably)})`,
    map: ({ map }) => `{${Object.entries(map).map(([key, value]) => `${pr_str(stringToKey(key), print_readably)} ${pr_str(value, print_readably)}`).join(" ")}}`
  });



  function quote_str(mal: { type: string, mal: MalType }) {
    return `(${mal.type.replace("_", "-")} ${pr_str(mal.mal, print_readably)})`;
  }

  function removeUnderscore(text: string): string {
    return text.replace("_", "");
  }
}
