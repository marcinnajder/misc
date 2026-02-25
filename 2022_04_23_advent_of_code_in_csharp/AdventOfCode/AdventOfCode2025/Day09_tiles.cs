using static System.Math;

namespace AdventOfCode.AdventOfCode2025.Day9;

public record Point(int X, int Y);

// point in orthogonal polygon

public static class Day9
{
    public static Point[] LoadData(string input)
    {
        var lines = input.Split(Environment.NewLine);

        return lines.Select(l => l.Split(','))
            .Select(s => new Point(int.Parse(s[0]), int.Parse(s[1])))
            .ToArray();
    }

    public static string Puzzle1(string input)
    {
        var points = LoadData(input);

        long maxSize = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            var p1 = points[i];
            for (int j = i + 1; j < points.Length; j++)
            {
                var p2 = points[j];
                var size = (long)(Abs(p1.X - p2.X) + 1) * (Abs(p1.Y - p2.Y) + 1);
                maxSize = Max(maxSize, size);
            }
        }

        return maxSize.ToString();
    }

    public static string Puzzle2__(string input)
    {
        var points = LoadData(input);


        var range = points.Aggregate(
            (MinX: int.MaxValue, MinY: int.MaxValue, MaxX: int.MinValue, MaxY: int.MinValue),
            (s, p) => (Min(s.MinX, p.X), Min(s.MinY, p.Y), Max(s.MaxX, p.X), Max(s.MaxY, p.Y)));


        var set = new HashSet<Point>();

        for (int y = range.MinY; y <= range.MaxY; y++)
        {
            for (int x = range.MinX; x <= range.MaxX; x++)
            {
                //set.Add(new Point(x, y));
            }
        }

        Console.WriteLine(set);

        return set.Count.ToString();
    }


    public static SortedDictionary<int, List<(int, int)>> BuildLinesByColumnsFromPoints(Point[] points)
    {
        var result = new SortedDictionary<int, List<(int, int)>>();
        var pairs = points.Append(points[0]).Pairwise();

        foreach (var (p1, p2) in pairs)
        {
            if (p1.X == p2.X) // vertical line, the same column
            {
                var column = p1.X;
                var line = (Min(p1.Y, p2.Y), Max(p1.Y, p2.Y));

                if (result.TryGetValue(column, out var lines))
                {
                    lines.Add(line);
                }
                else
                {
                    result.Add(column, [line]);
                }
            }
        }

        // sort all lines in place
        foreach (var (_, lines) in result)
        {
            lines.Sort(comparer);
        }

        return result;
    }

    // compare two lines where one represents the point instead of line
    private const int pointMarker = -1;
    private static IComparer<(int From, int To)> comparer = Lines.CreateLineToPointComparer(pointMarker);

    public static bool IsInside(SortedDictionary<int, List<(int, int)>> linesByColumns, Point point)
    {
        var isPointInside = false;
        var lastXInside = -1;
        foreach (var (x, lines) in linesByColumns.TakeWhile(kv => kv.Key <= point.X)) // check columns going from left
        {
            if (lines.BinarySearch((point.Y, pointMarker), comparer) >= 0)
            {
                isPointInside = !isPointInside; // swap whether we are inside
                lastXInside = x; // but maybe we are on the line/column
            }
        }

        return lastXInside == point.X || isPointInside;
    }

    public static string Puzzle2(string input)
    {
        var points = LoadData(input);
        var linesByColumns = BuildLinesByColumnsFromPoints(points);
        var isInsideCache = new Dictionary<Point, bool>();

        long maxSize = 0;
        for (int i = 0; i < points.Length - 1; i++)
        {
            var p1 = points[i];
            for (int j = i + 1; j < points.Length; j++)
            {
                var p2 = points[j];
                var size = (long)(Abs(p1.X - p2.X) + 1) * (Abs(p1.Y - p2.Y) + 1);

                if (size > maxSize && AreAllPointsInside(p1, p2))
                {
                    maxSize = size;

                    var aaa = AreAllPointsInside(p1, p2);
                }
            }
        }

        return maxSize.ToString();

        bool AreAllPointsInside(Point pp1, Point pp2)
        {
            var a = new Point(Min(pp1.X, pp2.X), Min(pp1.Y, pp2.Y));
            var b = new Point(Max(pp1.X, pp2.X), Max(pp1.Y, pp2.Y));

            for (int x = a.X; x <= b.X; ++x)
            {
                for (int y = a.Y; y <= b.Y; ++y)
                {
                    var point = new Point(x, y);
                    if (!isInsideCache.TryGetValue(point, out var isInsideValue))
                    {
                        isInsideCache[point] = isInsideValue = IsInside(linesByColumns, point);
                    }

                    if (!isInsideValue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // bool AreAllPointsInside(Point pp1, Point pp2)
        // {
        //     var xStep = pp2.X >= pp1.X ? 1 : -1;
        //     var yStep = pp2.Y >= pp1.Y ? 1 : -1;
        //
        //     for (int x = pp1.X + xStep; x < pp2.X + (-1 * xStep); x += xStep)
        //     {
        //         for (int y = pp1.Y + yStep; y < pp2.Y + (-1 * yStep); y += yStep)
        //         {
        //             var point = new Point(x, y);
        //             if (!isInsideCache.TryGetValue(point, out var isInsideValue))
        //             {
        //                 isInsideCache[point] = isInsideValue = IsInside(linesByColumns, point);
        //             }
        //
        //             if (!isInsideValue)
        //             {
        //                 return false;
        //             }
        //         }
        //     }
        //
        //     return true;
        // }
    }
}