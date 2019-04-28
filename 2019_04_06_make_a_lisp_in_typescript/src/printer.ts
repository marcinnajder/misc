import { MalType, list2BracketMap } from "./types";

export type PrintLineType = (...s: string[]) => void;

export function pr_str(mal: MalType, print_readably: boolean): string {
  switch (mal.type) {
    case "number": return mal.value.toString();
    case "symbol": return mal.name;
    case "string": {
      if (mal.value[0] === '\u029e') {
        return ':' + mal.value.slice(1);
      } else if (print_readably) {
        return '"' + mal.value.replace(/\\/g, "\\\\")
          .replace(/"/g, '\\"')
          .replace(/\n/g, "\\n") + '"'; // string
      } else {
        return mal.value;
      }
    }
    case "keyword": return ":" + mal.name;
    case "true":
    case "false":
    case "nil": {
      return mal.type
    }
    case "quote":
    case "quasiquote":
    case "unquote":
    case "splice-unquote": {
      return `(${mal.type} ${pr_str(mal.mal, print_readably)})`;
    }
    case "list": return `${list2BracketMap[mal.listType][0]}${mal.items.map(m => pr_str(m, print_readably)).join(" ")}${list2BracketMap[mal.listType][1]}`;
    case "fn": {
      return "#<function>";
    }
  }
}

