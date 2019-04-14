import { MalType, EnvironmentType, MalType_number } from "./types";
import { ResultS, error, ok } from "./utils/result";
import { mapRS, liftRS } from "./utils/monadicFunctions";


type NumberFuncType = (...args: MalType_number[]) => MalType_number;

export const envNumbers: { [symbol: string]: NumberFuncType } = {
  "+": (...args: MalType_number[]) => args.reduce(({ value: a }, { value: e }) => ({ type: "number", value: a + e })),
  "-": (...args: MalType_number[]) => args.reduce(({ value: a }, { value: e }) => ({ type: "number", value: a - e })),
  "*": (...args: MalType_number[]) => args.reduce(({ value: a }, { value: e }) => ({ type: "number", value: a * e })),
  "/": (...args: MalType_number[]) => args.reduce(({ value: a }, { value: e }) => ({ type: "number", value: a / e })),
};


export function eval_(mal: MalType, env: EnvironmentType): ResultS<MalType> {
  switch (mal.type) {
    case "list": {
      if (mal.listType === "list") {
        if (mal.items.length === 0) {
          return ok(mal);
        }
        return eval_ast(mal, env).bind(({ items: [fn, ...args] }: any) => ok(fn(...args)));
      }
    }
  }
  return eval_ast(mal, env);
}

function eval_ast(mal: MalType, env: EnvironmentType): ResultS<MalType> {
  switch (mal.type) {
    case "symbol": {
      const symbolAction = env[mal.name];
      return typeof symbolAction === "undefined" ? error(`Unknown symbol ${mal.name}`) : ok(symbolAction); // funkcja zamiast MalType
    }
    case "list": {
      const itemsValues = mapRS(mal.items, m => eval_(m, env));
      const mapItems = (items: MalType[]) => ({ ...mal, items: items } as MalType);
      return liftRS(mapItems)(itemsValues);
    }
    default: {
      return ok(mal);
    }
  }
}
