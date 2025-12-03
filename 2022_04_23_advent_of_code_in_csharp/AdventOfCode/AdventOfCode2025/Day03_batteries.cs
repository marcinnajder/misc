using static System.Linq.Enumerable;

namespace AdventOfCode.AdventOfCode2025.Day3;

record Digit(int Index, char Value);

public static class Day3
{
    public static IEnumerable<string> LoadData(string input)
    {
        return input.Split(Environment.NewLine);
    }

    public static long FindJoltage(string digits, int digitsCount)
    {
        var values = Range(0, digitsCount)
            .Scan(0, (startIndex, n) =>
            {
                var start = n == 0 ? 0 : startIndex + 1;
                var count = digits.Length - (digitsCount - 1) - start + n;
                return Range(start, count).MaxBy(i => digits[i]);
            })
            .Skip(1)
            .Select(i => digits[i]);

        return long.Parse(string.Concat(values));
    }

    public static string Puzzle(string input, int digitsCount)
    {
        var batteries = LoadData(input);
        var result = batteries.Sum(b => FindJoltage(b, digitsCount));
        return result.ToString();
    }

    public static string Puzzle1(string input) => Puzzle(input, 2);

    public static string Puzzle2(string input) => Puzzle(input, 12);
}
