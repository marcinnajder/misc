import { ResultS, error, ok, Option, none, some, matchUnion } from "powerfp";
import { assertNever, parseMalType } from "./utils/common";
import { MalType, ListType, list2BracketMap, bracket2ListMap, list } from "./types";



class Reader {
  constructor(private tokens: string[], private position: number) {
  }
  next(): Option<string> {
    return this.position === this.tokens.length ? none : some(this.tokens[this.position++]);
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
      return read_atom(reader);
    }
  }
}

function read_list(reader: Reader, listType: ListType): ResultS<MalType> {
  const closingBracket = list2BracketMap[listType][1];
  const result = list([], listType);

  reader.next(); // skip first char '('

  while (true) {
    const malR = read_form(reader);

    switch (malR.type) {
      case "error": return error(malR.error);
      case "ok": {
        const malO = malR.value;
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
  "~@": "splice_unquote"
};

function read_atom(reader: Reader): ResultS<Option<MalType>> {
  const tokenO = reader.next();

  return matchUnion(tokenO, {
    "none": _ => ok(none),
    "some": ({ value: token }) => {
      switch (token) {
        case "'":
        case "`":
        case "~":
        case "~@": {
          return read_form(reader).map(o => o.map(v => ({ type: quoteMapping[token], mal: v } as MalType)));
        }
        default: {
          return parseMalType(token).map(some);
        }
      }
    }
  });
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
    if (match[0] === ';') {
      continue;
    }
    results.push(match);
  }
  return results;
}
