using Line = (long From, long To);

namespace AdventOfCode.AdventOfCode2025.Day5;

public record IdRange(long From, long To);

public record Data(List<IdRange> Ranges, List<long> Ids);

public static class Day5
{
    public static Data LoadData(string input)
    {
        IEnumerable<string> lines = input.Split(Environment.NewLine, StringSplitOptions.TrimEntries);
        var ranges = new List<IdRange>();
        var ids = new List<long>();
        using var enumerator = lines.GetEnumerator();

        while (enumerator.MoveNext() && enumerator.Current is not "")
        {
            var parts = enumerator.Current.Split('-');
            ranges.Add(new IdRange(long.Parse(parts[0]), long.Parse(parts[1])));
        }

        while (enumerator.MoveNext())
        {
            ids.Add(long.Parse(enumerator.Current));
        }

        return new Data(ranges, ids);
    }

    private static List<Line> MapToMergedAndSortedLines(List<IdRange> ranges)
    {
        var empty = LListM.LListFrom<Line>();
        var mergedAndSortedLines = ranges
            .Aggregate(empty, (list, r) => RangeList.MergeLines(list, (r.From, r.To)))
            .ToEnumerable()
            .ToList();
        return mergedAndSortedLines;
    }

    public static string Puzzle1(string input)
    {
        var data = LoadData(input);
        var mergedAndSortedLines = MapToMergedAndSortedLines(data.Ranges);

        // compare two lines where one represents the point instead of line
        const long marker = -1;
        var comparer = Comparer<Line>.Create((l1, l2) =>
        {
            var (point, line, sign) = l1.To == marker ? (l1.From, l2, -1) : (l2.From, l1, 1);
            return point < line.From ? sign : (point > line.To ? -sign : 0);
        });

        var result = data.Ids.Count(id => mergedAndSortedLines.BinarySearch((id, marker), comparer) >= 0);

        return result.ToString();
    }


    public static string Puzzle2(string input)
    {
        var data = LoadData(input);
        var mergedAndSortedLines = MapToMergedAndSortedLines(data.Ranges);
        var result = mergedAndSortedLines.Sum(l => l.To - l.From + 1);
        return result.ToString();
    }
}