using static AdventOfCode.AdventOfCode2016.Day1.TurnDirection;
using static AdventOfCode.AdventOfCode2016.Day1.CardinalDirection;
using static System.Math;

namespace AdventOfCode.AdventOfCode2016.Day1;

public enum CardinalDirection { North = 0, East = 1, South = 2, West = 3 }
public enum TurnDirection { Left = -1, Right = 1 }

public record Point(int X, int Y);
public record Position(int X, int Y, CardinalDirection Dir) : Point(X, Y);
public record Turn(TurnDirection Dir, int Blocks);

public static class Day1
{
    public static IEnumerable<Turn> LoadData(string input)
    {
        return input
            .Split(",", StringSplitOptions.TrimEntries)
            .Select(t => new Turn(t[0] == 'R' ? Right : Left, int.Parse(t.Substring(1))));
    }

    public static IEnumerable<Position> Walk(IEnumerable<Turn> turns)
    {
        return turns.Scan(
            new Position(0, 0, North),
            (pos, turn) =>
            {
                var val = ((int)pos.Dir + (int)turn.Dir) % 4; // change position
                var nextDirection = (CardinalDirection)(val >= 0 ? val : 4 + val);

                return nextDirection switch
                {
                    East => pos with { X = pos.X + turn.Blocks, Dir = nextDirection },
                    West => pos with { X = pos.X - turn.Blocks, Dir = nextDirection },
                    North => pos with { Y = pos.Y + turn.Blocks, Dir = nextDirection },
                    South => pos with { Y = pos.Y - turn.Blocks, Dir = nextDirection },
                    _ => throw new Exception("unknown direction")
                };
            }
        );
    }

    public static string Puzzle1(string input)
    {
        var turns = LoadData(input);
        var postion = Walk(turns).Last();
        return (Math.Abs(postion.X) + Math.Abs(postion.Y)).ToString();
    }

    public static string Puzzle2(string input)
    {
        var turns = LoadData(input);
        var pos = new Point(0, 0);
        var visited = new HashSet<Point>() { pos };

        foreach (var next in Walk(turns))
        {
            var length = Abs(next.X - pos.X + next.Y - pos.Y);
            var xComp = next.X.CompareTo(pos.X);
            var yComp = next.Y.CompareTo(pos.Y);

            for (var i = 0; i < length; i++)
            {
                pos = pos with { X = pos.X + xComp, Y = pos.Y + yComp };
                if (!visited.Add(pos))
                {
                    return (Abs(pos.X) + Abs(pos.Y)).ToString();
                }
            }
        }

        throw new Exception("it should be unreachable code :) ");
    }
}



