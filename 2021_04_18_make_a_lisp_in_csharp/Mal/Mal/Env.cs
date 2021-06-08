
using PowerFP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Mal.Printer;
using static Mal.Types;

namespace Mal
{
    public static class EnvM
    {
        public record Env
        {
            public Map<Symbol, MalType> Data { get; internal set; }
            public Env? Outer { get; init; }

            public Env(Map<Symbol, MalType> data, Env? outer)
            {
                Data = data;
                Outer = outer;
            }
        }

        public static MalType Set(this Env env, Symbol key, MalType value)
        {
            env.Data = env.Data.Add(key, value);
            return value;
        }


        public static Env? Find(this Env env, Symbol key)
            => FindEnvAndValue(env, key) switch
            {
                null => null,
                (var Env, _) => Env
            };

        public static MalType Get(this Env env, Symbol key)
            => FindEnvAndValue(env, key) switch
            {
                null => throw new Exception($"Cannot find symbol '{key}' in Env"),
                (_, var Value) => Value
            };

        private static (Env, MalType)? FindEnvAndValue(this Env env, Symbol key)
            => env.Data.TryFind(key) switch
            {
                (true, var value) => (env, value!),
                _ => env.Outer == null ? null : FindEnvAndValue(env.Outer, key)
            };
    }
}