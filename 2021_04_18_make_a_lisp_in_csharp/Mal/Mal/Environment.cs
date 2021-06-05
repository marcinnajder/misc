
using PowerFP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Mal.Printer;
using static Mal.Types;

namespace Mal
{
    public static class Environment
    {
        public record Env(Map<Symbol, MalType> Data, Env? OuterEnv) { }

        // public static Env Set(this Env env, Symbol key, MalType value)
        //  => env.Data.Add
    }
}