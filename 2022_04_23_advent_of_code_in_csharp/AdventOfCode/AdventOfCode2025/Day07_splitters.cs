using System.Data.Common;
using Line = (int From, int To);
using Point = (int Row, int Column);

namespace AdventOfCode.AdventOfCode2025.Day7;

public record Data(Dictionary<int, List<Line>> LinesByColumns, int StartColumn);

public static class Day7
{
    public static Data LoadData(string input)
    {
        var rows = input.Split(Environment.NewLine);
        var start = rows[0].IndexOf('S');
        var linesByColumns = new Dictionary<int, List<Line>>();

        for (var r = 1; r < rows.Length; r++) // skip first row
        {
            var row = rows[r];
            var c = 0;
            while ((c = row.IndexOf('^', c + 1)) >= 0) // find all splitters in a row
            {
                if (linesByColumns.TryGetValue(c, out var lines))
                {
                    lines.Add((lines[^1].To + 2, r - 1)); // add next line
                }
                else
                {
                    linesByColumns.Add(c, [(0, r - 1)]); // add first line
                }
            }
        }

        return new Data(linesByColumns, start);
    }

    private const int PointMarker = -1;
    private static IComparer<Line> LineComparer = Lines.CreateLineToPointComparer(PointMarker);

    private static long CountPaths(Data data, Point splitter, Dictionary<Point, long> cache)
    {
        if (cache.TryGetValue(splitter, out var count))
        {
            return count;
        }

        count = 0;
        var adjacentPoints = new[]
        {
            splitter with { Column = splitter.Column - 1 }, splitter with { Column = splitter.Column + 1 }
        }; // points on the left and right

        foreach (var p in adjacentPoints)
        {
            if (data.LinesByColumns.TryGetValue(p.Column, out var lines) &&
                lines.BinarySearch((p.Row, PointMarker), LineComparer) is var index and >= 0)
            {
                count += CountPaths(data, (lines[index].To + 1, p.Column), cache);
            }
            else
            {
                count++;
            }
        }

        cache[splitter] = count;
        return count;
    }

    public static string Puzzle1(string input)
    {
        var data = LoadData(input);
        var splitter = (data.LinesByColumns[data.StartColumn][0].To + 1, data.StartColumn);

        var cache = new Dictionary<Point, long>();
        CountPaths(data, splitter, cache);
        var result = cache.Keys.Count;

        return result.ToString();
    }

    public static string Puzzle2(string input)
    {
        var data = LoadData(input);
        var splitter = (data.LinesByColumns[data.StartColumn][0].To + 1, data.StartColumn);

        var result = CountPaths(data, splitter, new Dictionary<Point, long>());

        return result.ToString();
    }
}