
using static AdventOfCode.Extensions;

namespace AdventOfCode.AdventOfCode2016.Day3;

public record Triangle(int A, int B, int C)
{
    public static Triangle From(IEnumerable<int> abc, bool sort = true)
        => (sort ? abc.OrderBy(Identity) : abc).ToList() switch
        {
            [var a, var b, var c] => new Triangle(a, b, c),
            _ => throw new Exception("exactly 3 numbers are required")
        };
}

public static class Day3
{
    public static IEnumerable<Triangle> LoadData(string input, bool sort = true)
    {
        return input
            .Split(Environment.NewLine, StringSplitOptions.TrimEntries)
            .Select(line => line
                .Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .Pipe(abc => Triangle.From(abc, sort)));
    }


    public static bool IsValid(this Triangle triangle)
    {
        return triangle.A + triangle.B > triangle.C;
    }

    public static string Puzzle1(string input)
    {
        var triangles = LoadData(input);
        var count = triangles.Count(IsValid);
        return count.ToString();
    }

    public static string Puzzle2(string input)
    {
        var triangles = LoadData(input, sort: false);

        triangles = triangles
            .Chunk(3)
            .Where(chunk => chunk.Length == 3)
            .SelectMany(ts =>
                new Triangle[]
                {
                    Triangle.From(ts.Select(t=>t.A)),
                    Triangle.From(ts.Select(t=>t.B)),
                    Triangle.From(ts.Select(t=>t.C)),
                });

        var count = triangles.Count(IsValid);
        return count.ToString();
    }
}