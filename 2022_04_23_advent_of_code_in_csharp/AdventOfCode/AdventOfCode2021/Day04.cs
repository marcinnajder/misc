using static AdventOfCode.Common;
using static AdventOfCode.Option;
using static System.Linq.Enumerable;

namespace AdventOfCode.AdventOfCode2021;

static class Day4
{
    private const int BoardSize = 5;

    public record Cell(int Value, bool IsChecked)
    {
        public bool IsChecked { get; set; } = IsChecked;
    }

    public record Board(Cell[,] Cells, bool IsChecked)
    {
        public bool IsChecked { get; set; } = IsChecked;
    }

    public record Data(int[] Numbers, Board[] Boards);

    private static T[,] Array2D<T>(IEnumerable<IEnumerable<T>> rows)
    {
        var array = rows.Select(row => row.ToArray()).ToArray();
        var rowCount = array.Length;
        var columnCount = array[0].Length;
        var result = new T[rowCount, columnCount];
        for (var r = 0; r < rowCount; ++r)
        {
            var row = array[r];
            for (var c = 0; c < columnCount; ++c)
            {
                result[r, c] = row[c];
            }
        }
        return result;
    }

    public static Data LoadData(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var boards =
            lines
            .Skip(2)
            .Where(line => line.Length > 0)
            .Chunk(BoardSize)
            .Select(chunk => new Board(chunk.Select(line => ParseNumbers(' ', line).Select(n => new Cell(n, false))).Pipe(Array2D), false))
            .ToArray();
        return new Data(ParseNumbers(',', lines[0]).ToArray(), boards);
    }

    static IEnumerable<Cell[]> GetRowsAndColumns(Board board) =>
        Range(0, BoardSize).SelectMany(x =>
            new[]
            {
                Range(0, BoardSize).Select(y => board.Cells[x,y]).ToArray(),
                Range(0, BoardSize).Select(y => board.Cells[y,x]).ToArray(),
            }
        );

    static bool CheckCell(int n, Cell cell) => cell.IsChecked = cell.Value == n;

    static bool checkBoard(int n, Board board) =>
        board.IsChecked = GetRowsAndColumns(board)
            .Select(line => line.Count(c => c.IsChecked || CheckCell(n, c)))
            .Any(count => count == BoardSize);


    static IEnumerable<(int, Board)> FindWinningBoards(Data data) =>
        data.Numbers.SelectMany(
            n => data.Boards.Where(b => !b.IsChecked && checkBoard(n, b)).Select(b => (n, b)));

    static int CountScoreForBoard(Board board) =>
        board.Cells
            .OfType<Cell>()
            .Choose(cell => cell switch
            {
                { IsChecked: true } => None<int>(),
                { Value: var value } => Some(value),
            })
            .Sum();


    public static string Puzzle1(string input)
    {
        var data = LoadData(input);
        var (n, b) = data.Pipe(FindWinningBoards).First();
        var result = n * CountScoreForBoard(b);
        return result.ToString();
    }

    public static string Puzzle2(string input)
    {
        var data = LoadData(input);
        var (n, b) = data.Pipe(FindWinningBoards).Last();
        var result = n * CountScoreForBoard(b);
        return result.ToString();
    }
}