
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
        private static readonly Dictionary<string, string> MacroTokensMap = new()
        {
            { "@", "deref" },
            { "'", "quote" },
            { "`", "quasiquote" },
            { "~", "unquote" },
            { "~@", "splice-unquote" },
        };

        public static MalType? ReadText(string text) => Tokenize(text).Pipe(LListM.ToLList).Pipe(ReadForm).Mal;

        public static FormReader ReadForm(LList<string>? tokens) =>
            tokens switch
            {
                null => new(null, null),
                (var Token, var RestTokens) => Token switch
                {
                    "^" => ReadMeta(RestTokens),
                    _ when MacroTokensMap.TryGetValue(Token, out var macroToken) => ReadMacro(RestTokens, macroToken),
                    _ when Bracket2ListMap.TryGetValue(Token, out var ListType) =>
                        ReadList(RestTokens, Types.List2BracketMap[ListType].Right).Pipe(r =>
                            new FormReader(r.Tokens, ListType == ListTypeAndMap.HashMap ? MalsToMap(r.Mals) : new List(r.Mals, (ListType)ListType, NilV))
                        ),
                    _ => new(RestTokens, ReadAtom(Token))
                }
            };


        public record FormReader(LList<string>? Tokens, MalType? Mal) { }
        public record ListReader(LList<string>? Tokens, LList<MalType>? Mals) { }


        public static ListReader ReadList(LList<string>? tokens, string eolToken)
        {
            return ReadForm(tokens) switch
            {
                { Mal: null } => throw new Exception("List is not closed"),
                { Mal: Symbol symbol } fr when symbol.Name == eolToken => new(fr.Tokens, null),
                (var RestTokens, var Mal) => ReadList(RestTokens, eolToken).Pipe(r => r with { Mals = new(Mal!, r.Mals) })
            };
        }

        public static FormReader ReadMeta(LList<string>? tokens) =>
            ReadForm(tokens) switch
            {
                { Mal: null } r => r,
                (var RestTokens1, var MetaMal) => ReadForm(RestTokens1) switch
                {
                    { Mal: null } r => r,
                    (var RestTokens2, var ValueMal) => new(RestTokens2, ListFrom(new Symbol("with-meta", NilV), ValueMal!, MetaMal!))
                }
            };

        public static FormReader ReadMacro(LList<string>? tokens, string macroToken) =>
            ReadForm(tokens) switch
            {
                { Mal: null } r => r,
                (var RestTokens, var Mal) => new(RestTokens, ListFrom(new Symbol(macroToken, NilV), Mal!))
            };

        internal static Map MalsToMap(LList<MalType>? mals) =>
            new(MalsToKeyValuePairs(mals).ToEnumerable().ToDictionary(kv => kv.Key, kv => kv.Value), NilV);

        internal static LList<(MalType Key, MalType Value)>? MalsToKeyValuePairs(LList<MalType>? mals)
        {
            return mals switch
            {
                null => null,
                { Head: Keyword or Str, Tail: { Head: { } value } } => new((mals.Head, value), MalsToKeyValuePairs(mals.Tail.Tail)),
                _ => throw new Exception(string.Join(" ", mals.ToEnumerable().Select(m => Printer.PrintStr(m))).Pipe(str =>
                        $"Invalid Map '{str}'. Valid Map requiries even number of items where each even item is 'keyword' or 'string'.")),
            };
        }

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
                    _ => new Symbol(token, NilV)
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