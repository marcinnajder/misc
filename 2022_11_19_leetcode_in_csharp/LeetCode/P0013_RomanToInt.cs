// https://leetcode.com/problems/plus-one/

namespace LeetCode;

using static System.Linq.Enumerable;

class P0013_RomanToInt
{
    static (int?, char[]) DigitsToInt1(char[] digits)
    {
        return digits switch
        {
            ['I', 'I', 'I', .. var rest] => (3, rest),
            ['I', 'I', .. var rest] => (2, rest),
            ['I', 'V', .. var rest] => (4, rest),
            ['I', 'X', .. var rest] => (9, rest),
            ['I', .. var rest] => (1, rest),
            ['V', 'I', 'I', 'I', .. var rest] => (8, rest),
            ['V', 'I', 'I', .. var rest] => (7, rest),
            ['V', 'I', .. var rest] => (6, rest),
            ['V', .. var rest] => (5, rest),
            _ => (null, digits)
        };
    }

    static (int?, char[]) DigitsToInt2(char[] digits)
    {
        return digits switch
        {
            ['I', .. var rest1] => rest1 switch
            {
                ['I', .. var rest2] => rest2 switch
                {
                    ['I', .. var rest3] => (3, rest3),
                    _ => (2, rest2)
                },
                ['V', .. var rest2] => (4, rest2),
                ['X', .. var rest2] => (9, rest2),
                _ => (1, rest1)
            },
            ['V', .. var rest0] => rest0 switch
            {
                ['I', .. var rest1] => rest1 switch
                {
                    ['I', .. var rest2] => rest2 switch
                    {
                        ['I', .. var rest3] => (8, rest3),
                        _ => (7, rest2)
                    },
                    _ => (6, rest1)
                },
                _ => (5, rest0)
            },
            _ => (null, digits)
        };
    }


    static Func<char[], (int?, char[])> DigitsToInt(char I, char V, char X, int t)
    {
        return digits => digits switch
        {
            [var d1, .. var rest1] when d1 == I => rest1 switch
            {
                [var d2, .. var rest2] when d2 == I => rest2 switch
                {
                    [var d3, .. var rest3] when d3 == I => (3 * t, rest3),
                    _ => (2 * t, rest2)
                },
                [var d2, .. var rest2] when d2 == V => (4 * t, rest2),
                [var d2, .. var rest2] when d2 == X => (9 * t, rest2),
                _ => (1 * t, rest1)
            },
            [var d0, .. var rest0] when d0 == V => rest0 switch
            {
                [var d1, .. var rest1] when d1 == I => rest1 switch
                {
                    [var d2, .. var rest2] when d2 == I => rest2 switch
                    {
                        [var d3, .. var rest3] when d3 == I => (8 * t, rest3),
                        _ => (7 * t, rest2)
                    },
                    _ => (6 * t, rest1)
                },
                _ => (5 * t, rest0)
            },
            _ => (null, digits)
        };
    }


    static int RomanToInt(string s)
    {
        return new[]
            {
                DigitsToInt('M','?','?',1000),
                DigitsToInt('C','D','M',100),
                DigitsToInt('X','L','C',10),
                DigitsToInt('I','V','X',1),
            }
            .Scan(
                (digits: s.ToArray(), total: 0),
                (state, func) =>
                    func(state.digits) switch
                    {
                        (int number, var restDigits) => (restDigits, state.total + number),
                        _ => state
                    })
            .First(state => state.digits.Length == 0)
            .total;
    }

    public static void Run()
    {
        var inputs = new[] { "III", "LVIII", "MCMXCIV" };

        foreach (var input in inputs)
        {
            var output = RomanToInt2(input);
            // var output = RomanToInt__(input);
            Console.WriteLine($" {input} -> {output}");
        }
    }


    // ** using Span<T> instead of Array
    // - Span<T> is a 'ref struct' thus can't be set as a generic type parameter
    // - so fo instance can't be used with ValueType<...>, Func<...>, .Scan<...>

    ref struct Result
    {
        public int? Number { get; }
        public ReadOnlySpan<char> Digits { get; }

        public Result(int? number, ReadOnlySpan<char> digits)
        {
            Number = number;
            Digits = digits;
        }

        public void Deconstruct(out int? number, out ReadOnlySpan<char> digits)
        {
            number = Number;
            digits = Digits;
        }
    }

    delegate Result DigitsToIntDelegate(ReadOnlySpan<char> digits);

    static DigitsToIntDelegate DigitsToInt__(char I, char V, char X, int t)
    {
        return digits => digits switch
        {
            [var d1, .. var rest1] when d1 == I => rest1 switch
            {
                [var d2, .. var rest2] when d2 == I => rest2 switch
                {
                    [var d3, .. var rest3] when d3 == I => new(3 * t, rest3),
                    _ => new(2 * t, rest2)
                },
                [var d2, .. var rest2] when d2 == V => new(4 * t, rest2),
                [var d2, .. var rest2] when d2 == X => new(9 * t, rest2),
                _ => new(1 * t, rest1)
            },
            [var d0, .. var rest0] when d0 == V => rest0 switch
            {
                [var d1, .. var rest1] when d1 == I => rest1 switch
                {
                    [var d2, .. var rest2] when d2 == I => rest2 switch
                    {
                        [var d3, .. var rest3] when d3 == I => new(8 * t, rest3),
                        _ => new(7 * t, rest2)
                    },
                    _ => new(6 * t, rest1)
                },
                _ => new(5 * t, rest0)
            },
            _ => new(null, digits)
        };
    }



    static int RomanToInt__(string s)
    {
        var result = new Result(0, s.AsSpan());

        var funcs = new[]
        {
            DigitsToInt__('M','?','?',1000),
            DigitsToInt__('C','D','M',100),
            DigitsToInt__('X','L','C',10),
            DigitsToInt__('I','V','X',1),
        };

        foreach (var func in funcs)
        {
            if (func(result.Digits) is (int number, var digits))
            {
                result = new Result(result.Number + number, digits);
                if (digits.Length == 0)
                {
                    return result.Number!.Value;
                }
            }
        }

        return result.Number!.Value;
    }

    public static int RomanToInt2(string s)
    {
        var m = new Dictionary<char, int> { { 'I', 1 }, { 'V', 5 }, { 'X', 10 }, { 'L', 50 }, { 'C', 100 }, { 'D', 500 }, { 'M', 1000 } };

        return Enumerable.Range(0, s.Length)
            .Select(i => m[s[^(i + 1)]])
            .Aggregate((Total: 0, Prev: 0), (s, next) => (s.Total + ((next < s.Prev ? -1 : 1) * next), next))
            .Total;
    }
}
