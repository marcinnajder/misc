using System;
using System.Collections.Generic;

namespace PowerFP
{
    public static class Function
    {
        public static R Pipe<T, R>(this T value, Func<T, R> func) => func(value);
        public static void Pipe<T>(this T value, Action<T> func) => func(value);
    }
}

