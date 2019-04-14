import { MalType, list2BracketMap } from "./types";
import { type } from "os";

export function pr_str(mal: MalType): string {
  switch (mal.type) {
    case "number": return mal.value.toString();
    case "symbol": return mal.name;
    case "string": return mal.value;
    case "keyword": return mal.name;
    case "true":
    case "false":
    case "nil": {
      return mal.type
    }
    case "quote":
    case "quasiquote":
    case "unquote":
    case "splice-unquote": {
      return `(${mal.type} ${pr_str(mal.mal)})`;
    }
    case "list": return `${list2BracketMap[mal.listType][0]}${mal.items.map(pr_str).join(" ")}${list2BracketMap[mal.listType][1]}`;
    default: {
      const _exhaustiveCheck: never = mal;
      return "";
    }
  }
}
