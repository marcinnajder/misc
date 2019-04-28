import { ResultS, error, ok } from "./utils/result";
import { Option, none, some } from "./utils/option";
import { assertNever, parseMalType } from "./utils/common";
import { MalType, ListType, list2BracketMap, bracket2ListMap } from "./types";


class Reader {
  constructor(private tokens: string[], private position: number) {
  }
  next(): Option<string> {
    return this.position === this.tokens.length ? none() : some(this.tokens[this.position++]);
  }
  peek() {
    return this.tokens[this.position];
  }
}

export function read_str(text: string): ResultS<Option<MalType>> {
  const tokens = tokenize(text);
  const reader = new Reader(tokens, 0);
  return read_form(reader);
}



export function read_form(reader: Reader): ResultS<Option<MalType>> {
  const token = reader.peek();
  switch (token) {
    case list2BracketMap["list"][0]:
    case list2BracketMap["vector"][0]:
    case list2BracketMap["hash-map"][0]: {
      return read_list(reader, bracket2ListMap[token]).bind(v => ok(some(v)));
    }
    default: {
      const atomR = read_atom(reader);
      switch (atomR.type) {
        case "error": return error(atomR.error);
        case "ok": {
          const atomO = atomR.value;
          switch (atomO.type) {
            case "none": return ok(none());
            case "some": return ok(some(atomO.value));
            default: return assertNever(atomO);
          }
        }
        default: return assertNever(atomR);
      }
    }
  }
}


function read_list(reader: Reader, listType: ListType): ResultS<MalType> {
  const closingBracket = list2BracketMap[listType][1];
  const result: MalType = { type: "list", listType, items: [] };

  reader.next(); // skip first char '('

  while (true) {

    const malE = read_form(reader);

    switch (malE.type) {
      case "error": return error(malE.error);
      case "ok": {
        const malO = malE.value;
        switch (malO.type) {
          case "none": return error('List is not closed');
          case "some": {
            const mal = malO.value;
            switch (mal.type) {
              case "symbol": {
                if (mal.name === closingBracket) {
                  return ok(result);
                }
              }
              default: {
                result.items.push(mal);
              }
            }
          }
        }
      }
    }
  }
}

const quoteMapping = {
  "'": "quote",
  "`": "quasiquote",
  "~": "unquote",
  "~@": "splice-unquote"
};

function read_atom(reader: Reader): ResultS<Option<MalType>> {
  const tokenO = reader.next();
  switch (tokenO.type) {
    case "none": return ok(none());
    case "some": {
      const token = tokenO.value;
      switch (token) {
        case "'":
        case "`":
        case "~":
        case "~@": {
          return read_form(reader).bind(o => ok(o.bind(v => some({ type: quoteMapping[token], mal: v } as MalType))));
        }
        default: {
          const malTypeR = parseMalType(tokenO.value);
          switch (malTypeR.type) {
            case "error": return error(malTypeR.error);
            case "ok": return ok(some(malTypeR.value));
            default: return assertNever(malTypeR);
          }
        }
      }
    }
  }

}



export function tokenize(str: string): string[] {
  // const a = `[\s,]`;                    // Matches any number of whitespaces or commas
  // const b = `~@`;                       // Captures the special two-characters ~@
  // const c = "[\[\]{}()'`~^@]";          // Captures any special single character, one of []{}()'`~^@
  // const d = '"(?:\\.|[^\\"])*"?';       // Starts capturing at a double-quote and stops at the next double-quote unless it was preceded by a backslash in which case it includes it until the next double-quote
  // const e = ";.*";                      // Captures any sequence of characters starting with
  // const f = "[^\s\[\]{}('\"`,;)]*";     // Captures a sequence of zero or more non special characters (e.g. symbols, numbers, "true", "false", and "nil") and is sort of the inverse of the one above that captures special characters 
  var re = /[\s,]*(~@|[\[\]{}()'`~^@]|"(?:\\.|[^\\"])*"?|;.*|[^\s\[\]{}('"`,;)]*)/g;
  var results = [];
  var match;
  while ((match = re.exec(str)![1]) != '') {
    if (match[0] === ';') { continue; }
    results.push(match);
  }
  return results;
}
