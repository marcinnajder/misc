import "../useDevPowerfp";
import { generateFile } from "powerfp/generators/generationUtils";
import { generateAdtCode, Options as AdtOptions } from "powerfp/generators/adtCodeGenerator";
import { parseUnionTypes } from "powerfp/generators/parser";
import { createSourceFile } from "typescript";


generateFile("./src/adt.generated.ts", generateAdtCode, [
  {
    typeImportsPath: "powerfp",
    typeName: "MalType", filePathName: "./types", unionTagName: "type",
    ...(parseUnionTypes(createSourceFile, "./src/types.ts", [{ typeName: "MalType", unionTagName: "type" }])["MalType"]),
    //   unions: {
    //     nil: null,
    //     true_: null,
    //     false_: null,

    //     number_: { fields: { value: "number" } },
    //     string_: { fields: { value: "string" } },
    //     symbol: { fields: { name: "string" } },
    //     keyword: { fields: { name: "string" } },

    //     quote: { fields: { mal: "MalType" } },
    //     quasiquote: { fields: { mal: "MalType" } },
    //     unquote: { fields: { mal: "MalType" } },
    //     splice_unquote: { fields: { mal: "MalType" } },

    //     list: { fields: { items: "MalType[]", listType: "ListType" } },
    //     fn: { fields: { fn: "MalFuncType" } }
    //   },
    additionalImports: [`import { ListType, MalFuncType } from "./types";`]
  },
] as AdtOptions[]);
