using static AdventOfCode.Extensions;
using static System.Linq.Enumerable;

namespace AdventOfCode.AdventOfCode2022;

static class Day1
{
    public static LList<LList<int>?>? LoadData(string input)
    {
        return input.Split(Environment.NewLine)
            .Concat(new[] { "" })
            .Aggregate(
                ((LList<LList<int>?>? Lists, LList<int>? List))(null, null),
                (s, line) =>
                    line switch
                    {
                        "" => (new(s.List, s.Lists), null),
                        _ => (s.Lists, new(int.Parse(line), s.List))
                    })
            .Item1;
    }

    static LList<T>? InsertSorted<T>(LList<T>? xs, T x) where T : IComparable<T>
        => xs switch
        {
            null => new(x, null),
            (var Head, var Tail) => x.CompareTo(Head) < 0 ? new(x, xs) : new(Head, InsertSorted(Tail, x))
        };

    static LList<T>? InsertSortedPreservingLength<T>(LList<T>? xs, T x) where T : IComparable<T>
        => xs switch
        {
            null => null,
            (var Head, var Tail) => x.CompareTo(Head) < 0 ? xs : InsertSorted(xs, x)!.Tail
        };

    public static string Puzzle(string input, int topN)
        => LoadData(input)
            .ToEnumerable()
            .Select(l => l.ToEnumerable().Sum())
            .Aggregate(Repeat(0, topN).ToLList(), InsertSortedPreservingLength)
            .ToEnumerable()
            .Sum()
            .ToString();

    public static string Puzzle1(string input) => Puzzle(input, 1);

    public static string Puzzle2(string input) => Puzzle(input, 3);
}



static class Day1_
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
