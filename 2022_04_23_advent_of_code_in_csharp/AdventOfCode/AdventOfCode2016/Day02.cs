
using static AdventOfCode.AdventOfCode2016.Day2.Direction;
using static System.StringSplitOptions;

namespace AdventOfCode.AdventOfCode2016.Day2;


public record Point(int X, int Y);
public enum Direction { L, R, U, D };

public static class Day2
{
    public static IEnumerable<IEnumerable<Direction>> LoadData(string input)
    {
        return input
            .Split(Environment.NewLine, TrimEntries)
            .Select(line => line.Select(dir => Enum.Parse<Direction>(dir.ToString())));
    }

    public static Point Move(Point point, Direction dir, char[,] keypad)
    {
        var maxIndex = keypad.GetLength(0) - 1;
        var newPoint = (dir, point) switch
        {
            (U, { Y: not 0 }) => point with { Y = point.Y - 1 },
            (D, { Y: var y }) when y < maxIndex => point with { Y = point.Y + 1 },
            (L, { X: not 0 }) => point with { X = point.X - 1 },
            (R, { X: var x }) when x < maxIndex => point with { X = point.X + 1 },
            _ => point,
        };

        return keypad[newPoint.Y, newPoint.X] != ' ' ? newPoint : point;
    }

    public static string Puzzle(string input, Point start, char[,] keypad)
    {
        var lines = LoadData(input);

        var digits = lines
            .Scan(start, (point, line) => line.Aggregate(point, (p, dir) => Move(p, dir, keypad)))
            .Skip(1)
            .Select(p => keypad[p.Y, p.X]);

        return string.Concat(digits);
    }

    public static string Puzzle1(string input)
    {
        var keypad = new char[,]
        {
            { '1', '2', '3' },
            { '4', '5', '6' },
            { '7', '8', '9' },
        };

        return Puzzle(input, new Point(1, 1), keypad);
    }

    public static string Puzzle2(string input)
    {
        var keypad = new char[,]
        {
            { ' ', ' ', '1' , ' ', ' ' },
            { ' ', '2', '3' , '4', ' ' },
            { '5', '6', '7' , '8', '9' },
            { ' ', 'A', 'B' , 'C', ' ' },
            { ' ', ' ', 'D' , ' ', ' ' },
        };

        return Puzzle(input, new Point(0, 2), keypad);
    }
}