// https://leetcode.com/problems/plus-one/

namespace LeetCode;

using static System.Linq.Enumerable;

class P0066_PlusOne
{
    static int[] PlusOne(int[] digits)
    {
        return new[] { 0 }
            .Concat(digits)
            .Reverse()
            .Scan(
                new { NextDigit = 0, Carry = 1 },
                (s, digit) => (s.Carry, digit) switch
                {
                    (0, _) => new { NextDigit = digit, Carry = 0 },
                    (_, 9) => new { NextDigit = 0, Carry = 1 },
                    _ => new { NextDigit = digit + 1, Carry = 0 }
                }
            )
            .Skip(1)
            .Select(s => s.NextDigit)
            .Reverse()
            .SkipWhile(d => d == 0)
            .ToArray();
    }

    public static void Run()
    {
        var inputs = new[]
        {
            new [] { 4, 3, 2, 2},
            new [] { 1, 2, 3},
            new [] { 9},
        };

        foreach (var input in inputs)
        {
            var output = PlusOne(input);
            Console.WriteLine($" {input.JoinToString()} -> {output.JoinToString()}");
        }
    }
}
