using System.Globalization;
using static System.Linq.Enumerable;

namespace AdventOfCode.AdventOfCode2025.Day2;

public record IdRange(string From, string To);

public static class Day2
{
    public static IEnumerable<IdRange> LoadData(string input)
    {
        return input
            .Split(',', StringSplitOptions.TrimEntries)
            .Select(t => t.Split('-').Pipe(parts => new IdRange(parts[0], parts[1])));
    }

    public static IEnumerable<string> FindInvalidIds(IdRange range)
    {
        // https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10/libraries#numeric-ordering-for-string-comparison new in .NET10        
        var comparer = StringComparer.Create(CultureInfo.CurrentCulture, CompareOptions.NumericOrdering); // 

        var start = range.From.Length % 2 == 0 // even number of chars
            ? range.From.Substring(0, range.From.Length / 2) // take first half of chars
            : "1" + new string('0', range.From.Length / 2); // generate half-long string "100..."

        for (int i = int.Parse(start); ; i++)
        {
            var iAsString = i.ToString();
            var pattern = string.Concat(iAsString, iAsString); // duplicate id

            if (comparer.Compare(pattern, range.To) > 0) // out of bounds
            {
                yield break; // stop searching process
            }

            if (comparer.Compare(pattern, range.From) >= 0) // inside the bounds
            {
                yield return pattern;
            }
        }
    }

    public static string Puzzle1(string input)
    {
        var ranges = LoadData(input);
        var result = ranges.SelectMany(FindInvalidIds).Sum(long.Parse);
        return result.ToString();
    }

    public static bool IsIdValid(string text)
    {
        for (int patternLength = 1; patternLength <= text.Length / 2; patternLength++)
        {
            if (text.Length % patternLength == 0) // length of text is divisible by length of pattern
            {
                var patten = text.AsSpan(0, patternLength);
                var patternFound = true;

                for (int i = patternLength; i < text.Length; i += patternLength)
                {
                    if (!patten.Equals(text.AsSpan(i, patternLength), StringComparison.Ordinal))
                    {
                        patternFound = false;
                        break;
                    }
                }

                if (patternFound)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static string Puzzle2(string input)
    {
        var ranges = LoadData(input);

        var result = ranges
            .SelectMany(r => Sequence(long.Parse(r.From), long.Parse(r.To), 1)) // generate ids in range
            .Sum(n => IsIdValid(n.ToString()) ? n : 0);

        return result.ToString();
    }
}
