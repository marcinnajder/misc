using Point = (int Row, int Column);

namespace AdventOfCode.AdventOfCode2025.Day4;

public record Data(char[][] Board, int Size);

public static class Day4
{
    public static Data LoadData(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var size = lines.Length;
        var board = Array.ConvertAll(lines, line => line.ToCharArray());
        return new Data(board, size);
    }

    public static IEnumerable<Point> FindRollsToRemove(Data data, Func<Point, bool> isRoll)
    {
        for (int r = 0; r < data.Size; r++)
        {
            for (int c = 0; c < data.Size; c++)
            {
                var point = (r, c);
                if (!isRoll(point))
                {
                    continue; // skip non-rolls
                }

                var fourOrMore = Grid.GetNeighboursAll(r, c, data.Size, data.Size).Where(isRoll).Skip(3).Any();
                if (!fourOrMore)
                {
                    yield return point;
                }
            }
        }
    }

    public static string Puzzle1(string input)
    {
        var data = LoadData(input);
        var result = FindRollsToRemove(data, p => data.Board[p.Row][p.Column] == '@').Count();
        return result.ToString();
    }

    public static string Puzzle2(string input)
    {
        var data = LoadData(input);
        var removed = new HashSet<Point>();

        while (true)
        {
            var points = FindRollsToRemove(data, p => !removed.Contains(p) && data.Board[p.Row][p.Column] == '@');
            using var enumerator = points.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                return removed.Count.ToString();
            }

            do
            {
                removed.Add(enumerator.Current);
            } while (enumerator.MoveNext());
        }
    }
}