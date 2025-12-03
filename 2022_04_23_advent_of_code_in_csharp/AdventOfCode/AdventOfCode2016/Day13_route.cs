using System.Numerics;

namespace AdventOfCode.AdventOfCode2016.Day13;

using Point = (int Row, int Column);

public static class Day13
{
    public static int LoadData(string input) => int.Parse(input);

    private static int CalculateFormula(int x, int y) => x * x + 3 * x + 2 * x * y + y + y * y;

    private static int CountOneBits(int number) => BitOperations.PopCount((uint)number);

    public static string Puzzle1(string input)
    {
        var favoriteNumber = LoadData(input);
        var getNeighbours = CreateGetNeighbours(favoriteNumber);

        var result = Graph.DijkstraShortestPath<Point>((1, 1), (Row: 39, Column: 31), getNeighbours);
        return result!.Value.ToString();
    }

    private static Func<Point, Link<Point>[]> CreateGetNeighbours(int favoriteNumber)
    {
        var isOpenSpace = IsOpenSpace; // temp variable necessary to infer the type of delegate
        var isOpenSpaceMemoized = Common.Memoize(isOpenSpace);

        return point =>
        {
            return Grid.GetNeighbours(point.Row, point.Column, int.MaxValue, int.MaxValue)
                .Where(p => isOpenSpaceMemoized((p, favoriteNumber)))
                .Select(p => new Link<Point>(p, 1))
                .ToArray();
        };
    }

    public static string Puzzle2(string input)
    {
        var favoriteNumber = LoadData(input);
        var getNeighbours = CreateGetNeighbours(favoriteNumber);

        var result = Graph.DijkstraTraverse((1, 1), getNeighbours)
            .TakeWhile(v => v.Weight < 51)
            .DistinctBy(v => v.To)
            .Count();

        return result.ToString();
    }


    // this methods takes only one argument to be easily memoized

    public static bool IsOpenSpace((Point Point, int FavoriteNumber) args)
    {
        var (point, favoriteNumber) = args;
        var sum = CalculateFormula(point.Column, point.Row) + favoriteNumber;
        return CountOneBits(sum) % 2 == 0;
    }
}