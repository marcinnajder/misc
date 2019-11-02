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
    additionalImports: [`import { ListType, MalFuncType, MapType } from "./types";`]
  },
] as AdtOptions[]);
