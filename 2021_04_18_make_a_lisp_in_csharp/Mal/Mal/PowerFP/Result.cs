
using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerFP
{
    // internal static MalType MacroExpand(MalType mal, Env env)
    // ma w kodzie try {} catch{} ... mozna tam Result wykorzystac

    public record Result<T, E> { }
    public record Ok<T, E>(T Value) : Result<T, E> { }
    public record Error<T, E>(E Err) : Result<T, E> { }

    public static class ResultM
    {
    }
}