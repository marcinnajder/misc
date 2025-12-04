namespace AdventOfCode;

public class Grid
{
    public static IEnumerable<(int r, int c)> GetNeighbours(int row, int column, int rowCount, int columnCount)
    {
        if (row > 0) yield return (row - 1, column);
        if (row < rowCount) yield return (row + 1, column);
        if (column > 0) yield return (row, column - 1);
        if (column < columnCount) yield return (row, column + 1);
    }

    public static IEnumerable<(int Row, int Column)> GetNeighboursAll(int row, int column, int rowCount,
        int columnCount)
    {
        for (int r = Math.Max(row - 1, 0); r < Math.Min(row + 2, rowCount); r++)
        {
            for (int c = Math.Max(column - 1, 0); c < Math.Min(column + 2, columnCount); c++)
            {
                if (!(c == column && r == row))
                {
                    yield return (r, c);
                }
            }
        }
    }


    public static int[][] LoadGridOfIntsFromData(string input)
    {
        var lines = input.Split([Environment.NewLine], StringSplitOptions.TrimEntries);
        return Array.ConvertAll(lines, l => l.ToCharArray().Select(ch => (int)char.GetNumericValue(ch)).ToArray());
    }

    public static char[][] LoadGridOfCharsFromData(string input)
    {
        var lines = input.Split([Environment.NewLine], StringSplitOptions.TrimEntries);
        return Array.ConvertAll(lines, l => l.ToCharArray());
    }
}