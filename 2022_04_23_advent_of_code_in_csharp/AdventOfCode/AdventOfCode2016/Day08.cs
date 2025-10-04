
using static System.StringSplitOptions;

namespace AdventOfCode.AdventOfCode2016.Day8;

public interface IOp;

public record Rect(int X, int Y) : IOp;
public record RotateRow(int Index, int By) : IOp;
public record RotateColumn(int Index, int By) : IOp;

public static class Day8
{
    public static IEnumerable<IOp> LoadData(string input)
    {
        var ops = new (string Prefix, string Sep, Func<int, int, IOp> Create)[]
        {
            ("rect ", "x", (a,b) => new Rect(a,b)),
            ("rotate row y=", "by", (a,b) => new RotateRow(a,b)),
            ("rotate column x=", "by", (a,b) => new RotateColumn(a,b)),
        };

        foreach (var line in input.Split(Environment.NewLine, TrimEntries))
        {
            var op = ops.First(o => line.StartsWith(o.Prefix));
            var args = line[op.Prefix.Length..].Split(op.Sep, TrimEntries);
            yield return op.Create(int.Parse(args[0]), int.Parse(args[1]));
        }
    }

    public static void SetBoardValues(char[,] board, int ySize, int xSize, char value)
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                board[y, x] = value;
            }
        }
    }

    public static char[,] ProcessOperations(string input, int ySize, int xSize)
    {
        var tempColumn = new char[ySize];
        var tempRow = new char[xSize];
        char[,] board = new char[ySize, xSize];

        var ops = LoadData(input);

        SetBoardValues(board, ySize, xSize, '.');

        foreach (var o in ops)
        {
            switch (o)
            {
                case Rect op:
                    SetBoardValues(board, op.Y, op.X, '#');
                    break;

                case RotateColumn op:
                    for (var y = 0; y < ySize; y++)
                    {
                        tempColumn[(y + op.By) % ySize] = board[y, op.Index];
                    }
                    for (var y = 0; y < ySize; y++)
                    {
                        board[y, op.Index] = tempColumn[y];
                    }
                    break;

                case RotateRow op:
                    for (var x = 0; x < xSize; x++)
                    {
                        tempRow[(x + op.By) % xSize] = board[op.Index, x];
                    }
                    for (var x = 0; x < xSize; x++)
                    {
                        board[op.Index, x] = tempRow[x];
                    }
                    break;

                default:
                    break;
            }
        }
        return board;
    }

    public static string Puzzle1(string input)
    {
        var board = ProcessOperations(input, xSize: 50, ySize: 6);
        return board.OfType<char>().Count(c => c == '#').ToString();
    }

    public static void PrintBoard(char[,] board)
    {
        Console.WriteLine();
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                Console.Write(board[y, x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public static string Puzzle2(string input)
    {
        // var board = ProcessOperations(input, xSize: 50, ySize: 6);
        // PrintBoard(board);
        return "RURUCEOEIL";
    }
}