using static AdventOfCode.Extensions;

namespace AdventOfCode.AdventOfCode2022;

static class Day1
{
    public static IEnumerable<int[]> LoadData(string input)
        => input
            .Split(Environment.NewLine)
            .PartitionBy(string.IsNullOrEmpty)
            .Where(lines => lines is not [""])
            .Select(lines => Array.ConvertAll(lines, int.Parse));

    public static string Puzzle1(string input)
        => LoadData(input).Max(Enumerable.Sum).ToString();

    public static string Puzzle2(string input)
        => LoadData(input).Select(Enumerable.Sum).OrderByDescending(Identity).Take(3).Sum().ToString();
}
