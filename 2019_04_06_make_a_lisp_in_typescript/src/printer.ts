import { MalType, list2BracketMap } from "./types";
import { matchUnion } from "powerfp";

export type PrintLineType = (...s: string[]) => void;


export function pr_str(mal: MalType, print_readably: boolean): string {
  return matchUnion(mal, {
    number_: ({ value }) => value.toString(),
    symbol: ({ name }) => name,
    string_: ({ value }) => value[0] === '\u029e' ? `:${value.slice(1)}` :
      (print_readably ? `"${value.replace(/\\/g, "\\\\").replace(/"/g, '\\"').replace(/\n/g, "\\n")}"` : value)
    ,
    keyword: ({ name }) => `:${name}`,
    true_: ({ type }) => removeUnderscore(type),
    false_: ({ type }) => removeUnderscore(type),
    nil: ({ type }) => type,
    // quote: quote_str,
    // quasiquote: quote_str,
    // unquote: quote_str,
    // splice_unquote: quote_str,
    list: ({ listType, items }) =>
      `${list2BracketMap[listType][0]}${items.map(m => pr_str(m, print_readably)).join(" ")}${list2BracketMap[listType][1]}`,
    fn: _ => "#<function>",
    atom: ({ mal }) => `(atom ${pr_str(mal, print_readably)})`
  });

  function quote_str(mal: { type: string, mal: MalType }) {
    return `(${mal.type.replace("_", "-")} ${pr_str(mal.mal, print_readably)})`;
  }

  function removeUnderscore(text: string): string {
    return text.replace("_", "");
  }


  // switch (mal.type) {
  //   case "number_": return mal.value.toString();
  //   case "symbol": return mal.name;
  //   case "string_": {
  //     if (mal.value[0] === '\u029e') {
  //       return ':' + mal.value.slice(1);
  //     } else if (print_readably) {
  //       return; // string
  //     } else {
  //       return mal.value;
  //     }
  //   }
  //   case "keyword": return ":" + mal.name;
  //   case "true_":
  //   case "false_":
  //   case "nil": {
  //     return removeUnderscore(mal.type);
  //   }
  //   case "quote":
  //   case "quasiquote":
  //   case "unquote":
  //   case "splice_unquote": {
  //     return `(${mal.type.replace("_", "-")} ${pr_str(mal.mal, print_readably)})`;
  //   }
  //   case "list": return `${list2BracketMap[mal.listType][0]}${mal.items.map(m => pr_str(m, print_readably)).join(" ")}${list2BracketMap[mal.listType][1]}`;
  //   case "fn": {
  //     return "#<function>";
  //   }
  // }
}



