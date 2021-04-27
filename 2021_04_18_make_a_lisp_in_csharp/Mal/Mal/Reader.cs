
using PowerFP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Mal.Types;


namespace Mal
{

    public static class Reader
    {
        public static MalType? ReadText(string text) => Tokenize(text).Pipe(LListM.ToLList).Pipe(ReadForm).Mal;

        public static (LList<string>? Tokens, MalType? Mal) ReadForm(LList<string>? tokens)
        {
            return tokens switch
            {
                null => (null, null),
                LList<string>(var token, var restTokens) => token switch
                {
                    "^" => default, // todo 
                    _ when MacroTokensMap.TryGetValue(token, out var macroToken) => default, // todo 
                    _ when Bracket2ListMap.TryGetValue(token, out var listToken) => default, // todo 
                    _ => (restTokens, ReadAtom(token))
                }
            };
        }


        private static readonly Dictionary<string, string> MacroTokensMap = new()
        {
            { "@", "deref" },
            { "'", "quote" },
            { "`", "quasiquote" },
            { "~", "unquote" },
            { "~@", "splice-unquote" },
        };



        // export function read_form(reader: Reader): ResultS<Option<MalType>> {
        //   const token = reader.peek();

        //   // reader macro
        //   if (token === "^") {
        //     reader.next();                          // skip current token
        //     return read_form(reader).bind(metaMalO => matchUnion(metaMalO, {
        //       some: ({ value }) => read_form(reader).map(malO => malO.map(mal => list([symbol("with-meta", nil), mal, value], "list", nil))),
        //       none: ok
        //     }));
        //   } else if (token in readerMacros) {       // reader macro
        //     reader.next();                          // skip current token
        //     return read_form(reader).map(malO => malO.map(mal => list([symbol(readerMacros[token], nil), mal], "list", nil)));
        //   } else if (token in bracket2ListMap) {   // list
        //     return read_list(reader, token).map(some);
        //   } else {
        //     return read_atom(reader);
        //   }
        // }


        internal static MalType ReadAtom(string token) =>
            Double.TryParse(token, out var doubleValue) switch
            {
                true => new Number(doubleValue),
                _ => token switch
                {
                    "true" => TrueV,
                    "false" => FalseV,
                    "nil" => NilV,
                    _ when token.FirstOrDefault() == ':' => new Keyword(token.Substring(1)),
                    _ when token.FirstOrDefault() == '"' =>
                        token switch
                        {
                            _ when token.Length > 1 && token.LastOrDefault() == '"' => new Str(token[1..^1].CleanUpText()),
                            _ => throw new Exception($"String value '${token}' in not closed")
                        },
                    _ => new Symbol(token)
                }
            };


        // private

        private static IEnumerable<string> Tokenize(string str)
        {
            const string pattern = @"[\s ,]*(~@|[\[\]{}()'`~@]|""(?:[\\].|[^\\""])*""?|;.*|[^\s \[\]{}()'""`~@,;]*)";
            var regex = new Regex(pattern);

            foreach (Match match in regex.Matches(str))
            {
                var token = match.Groups[1].Value;
                if (!string.IsNullOrEmpty(token) && !(token[0] == ';'))
                {
                    yield return token;
                }
            }
        }


        // reader
        private record ReaderFP(string[] Tokens, int Position) { }
        private static (ReaderFP, string?) NextToken(ReaderFP readPosition)
        {
            if (readPosition.Position == readPosition.Tokens.Length)
            {
                return (readPosition, null);
            }

            var currentToken = readPosition.Tokens[readPosition.Position];
            var nextReadPositon = readPosition with { Position = readPosition.Position + 1 };
            return (nextReadPositon, currentToken);
        }
        private static string PeekToken(ReaderFP readPosition) => readPosition.Tokens[readPosition.Position];


        private class ReaderOOP
        {
            private readonly string[] _tokens;
            private int _positon = 0;

            public ReaderOOP(string[] tokens)
            {
                _tokens = tokens;
            }

            public string? Next() => _positon == _tokens.Length ? null : _tokens[_positon++];
            public string Peek() => _tokens[_positon];
        }


        private static string CleanUpText(this string text) =>
            text.Replace("\\\\", "\u029e").Replace("\\\"", "\"").Replace("\\n", "\n").Replace("\u029e", "\\");
    }
}