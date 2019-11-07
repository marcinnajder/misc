import { ResultS, error, ok, Option, none, some, matchUnion } from "powerfp";
import { MalType, ListType, list2BracketMap, bracket2ListMap, list, symbol, listToMap, nil, number_, true_, false_, string_, keyword } from "./types";

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


export function read_str(text: string): ResultS<Option<MalType>> {
  const tokens = tokenize(text);
  const reader = new Reader(tokens);
  return read_form(reader);
}

class Reader {
  private position = 0;
  constructor(private tokens: string[]) {
  }
  next(): Option<string> {
    return this.position === this.tokens.length ? none : some(this.tokens[this.position++]);
  }
  peek() {
    return this.tokens[this.position];
  }
}


const readerMacros: { [token: string]: string } = {
  "@": "deref",
  "'": "quote",
  "`": "quasiquote",
  "~": "unquote",
  "~@": "splice-unquote",
  // "^": "with-meta",
};

export function read_form(reader: Reader): ResultS<Option<MalType>> {
  const token = reader.peek();

  // reader macro
  if (token === "^") {
    reader.next();                          // skip current token
    return read_form(reader).bind(metaMalO => matchUnion(metaMalO, {
      some: ({ value }) => read_form(reader).map(malO => malO.map(mal => list([symbol("with-meta", nil), mal, value], "list", nil))),
      none: ok
    }));
  } else if (token in readerMacros) {       // reader macro
    reader.next();                          // skip current token
    return read_form(reader).map(malO => malO.map(mal => list([symbol(readerMacros[token], nil), mal], "list", nil)));
  } else if (token in bracket2ListMap) {   // list
    return read_list(reader, token).map(some);
  } else {
    return read_atom(reader);
  }
}

function read_list(reader: Reader, token: string): ResultS<MalType> {
  const listType = bracket2ListMap[token];
  const closingBracket = list2BracketMap[listType][1];
  const items: MalType[] = [];

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
                  if (closingBracket === "}") {
                    return listToMap(items);
                  } else {
                    return ok(list(items, listType as ListType, nil));
                  }
                }
              }
              default: {
                items.push(mal);
              }
            }
          }
        }
      }
    }
  }
}

function read_atom(reader: Reader): ResultS<Option<MalType>> {
  const tokenO = reader.next();

  return matchUnion(tokenO, {
    none: _ => ok(none),
    some: ({ value }) => parseMalType(value).map(some)
  });
}


function parseMalType(text: string): ResultS<MalType> {
  const n = Number(text);
  if (!Number.isNaN(n)) {
    return ok(number_(n));
  }
  if (text === "true") {
    return ok(true_);
  }
  if (text === "false") {
    return ok(false_);
  }
  if (text === "nil") {
    return ok(nil);
  }
  if (text[0] === '"') {
    if (text[text.length - 1] === '"') {
      return ok(string_(text.slice(1, text.length - 1).replace(/\\(.)/g, function (_, c) { return c === "n" ? "\n" : c })));
    }
    return error(`String value '${text}' in not closed`);
  }
  if (text[0] === ":") {
    return ok(keyword(text.substr(1)));
  }
  return ok(symbol(text, nil));
}
