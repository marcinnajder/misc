
using PowerFP;
using System;
using System.Collections.Generic;
using System.Linq;
using static Mal.Types;

namespace Mal
{
    public static class Printer
    {
        private static (string Left, string Right) ToBracket(ListType listType) => List2BracketMap[(ListTypeAndMap)(int)listType];

        public static string PrintStr(this MalType mal, bool printReadable = false) =>
            mal switch
            {
                Number(var Value) => Value.ToString(),
                Symbol(var Name, _) => Name,
                Str(var Value) => PrintStringValue(Value, printReadable),
                Keyword(var Name) => $":{Name}",
                True => "true",
                False => "false",
                Nil => "nil",
                List l =>
                    $"{ToBracket(l.ListType).Left}{string.Join(' ', l.Items.ToEnumerable().Select(m => PrintStr(m, printReadable)))}{ToBracket(l.ListType).Right}",
                Fn => "#<function>",
                Atom(var Value) => $"(atom {PrintStr(Value, printReadable)})",
                Map(var Value, _) =>
                    $"{{{string.Join(' ', Value.Select(kv => $"{PrintStr(kv.Key, printReadable)} {PrintStr(kv.Value, printReadable)}")) }}}",

                //     list: ({ listType, items }) =>
                //       `${list2BracketMap[listType][0]}${items.map(m => pr_str(m, print_readably)).join(" ")}${list2BracketMap[listType][1]}`,
                //     fn: _ => "#<function>",
                //     atom: ({ mal }) => `(atom ${pr_str(mal, print_readably)})`,
                //     map: ({ map }) => `{${Object.entries(map).map(([key, value]) => `${pr_str(stringToKey(key), print_readably)} ${pr_str(value, print_readably)}`).join(" ")}}`

                _ => ""
            };

        private static string PrintStringValue(string str, bool printReadable) =>
            str.FirstOrDefault() == '\u029e'
                ? $":{str.Substring(1)}"
                : (printReadable
                    ? $"\"{str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n")}\""
                    : str);


        // jedynie w implementacji TS "string" byl kluczem w Map-ie wie sztycznie mial dodawany prefix
        // public static MalType StringToKey(this string name) => name[0] switch
        // {
        //     'k' => new Keyword(name.Substring(1)),
        //     's' => new Str(name.Substring(1)),
        //     _ => throw new Exception($"Key name '{name}' must to start with 'k' or 's'.")
        // };
        // public static string KeyToString(this MalType mal) => mal switch
        // {
        //     Keyword(var Value) => $"k{Value}",
        //     Str(var Value) => $"s{Value}",
        //     _ => throw new Exception($"Only 'Keyword' or 'Str' mal type can be treated as a key name but got '{mal.GetType()}' ")
        // };
    }
}

// export function pr_str(mal: MalType, print_readably: boolean): string {
//   return matchUnion(mal, {
//     number_: ({ value }) => value.toString(),
//     symbol: ({ name }) => name,
//     string_: ({ value }) => value[0] === '\u029e' ? `:${value.slice(1)}` :
//       (print_readably ? `"${value.replace(/\\/g, "\\\\").replace(/"/g, '\\"').replace(/\n/g, "\\n")}"` : value),
//     keyword: ({ name }) => `:${name}`,
//     true_: ({ type }) => removeUnderscore(type),
//     false_: ({ type }) => removeUnderscore(type),
//     nil: ({ type }) => type,
//     list: ({ listType, items }) =>
//       `${list2BracketMap[listType][0]}${items.map(m => pr_str(m, print_readably)).join(" ")}${list2BracketMap[listType][1]}`,
//     fn: _ => "#<function>",
//     atom: ({ mal }) => `(atom ${pr_str(mal, print_readably)})`,
//     map: ({ map }) => `{${Object.entries(map).map(([key, value]) => `${pr_str(stringToKey(key), print_readably)} ${pr_str(value, print_readably)}`).join(" ")}}`
//   });



//   function quote_str(mal: { type: string, mal: MalType }) {
//     return `(${mal.type.replace("_", "-")} ${pr_str(mal.mal, print_readably)})`;
//   }

//   function removeUnderscore(text: string): string {
//     return text.replace("_", "");
//   }
// }