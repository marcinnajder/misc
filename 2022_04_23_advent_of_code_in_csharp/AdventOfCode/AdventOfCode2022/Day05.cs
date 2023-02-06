
using System.Text.RegularExpressions;
using static AdventOfCode.Extensions;

namespace AdventOfCode.AdventOfCode2022;

static class Day5
{
    private static Regex NumbersRegex = new Regex(@"[\d]+");

    public record Move(int Quantity, int From, int To);

    public record Data(Map<int, LList<char>> Crates, Move[] Moves);


    public static Map<int, LList<char>> ParseCrates(string[] lines)
    {
        var charIndexes = NumbersRegex.Matches(lines.Last()).Select(m => m.Index).ToArray();

        return lines
            .Reverse()
            .Skip(1)
            .Aggregate(MapM.Empty<int, LList<char>>(), (crates, line) =>
                charIndexes
                    .Select((charIndex, i) => (Char: line[charIndex], Index: i))
                    .Where(t => t.Char != ' ')
                    .Aggregate(crates,
                        (crates_, t) => crates_.Change(t.Index, e => (true, new(t.Char, e is (true, var tail) ? tail : null))))
            );
    }

    public static Data LoadData(string input)
    {
        var parts = input
            .Split(Environment.NewLine)
            .PartitionBy(string.IsNullOrEmpty)
            .ToArray();

        var crates = ParseCrates(parts.First());
        var moves = parts.Last()
            .Select(line =>
            {
                var nums = NumbersRegex.Matches(line).Select(m => int.Parse(m.Value)).ToArray();
                return new Move(nums[0], nums[1] - 1, nums[2] - 1);
            })
            .ToArray();

        return new Data(crates, moves);
    }


    private static Map<int, LList<char>> MoveOneByOne(Map<int, LList<char>> crates, Move move)
    {
        return Enumerable
           .Range(0, move.Quantity)
           .Aggregate(crates, (crates_, _) =>
            {
                var movedCrate = crates_.Find(move.From).Head;
                return crates_
                    .Change(move.From, e => (true, e.Item2!.Tail))
                    .Change(move.To, e => (true, new(movedCrate, e.Item2)));
            });
    }

    private static Map<int, LList<char>> MoveAllAtOnce(Map<int, LList<char>> crates, Move move)
    {
        var (from, to) = MoveMany(crates.Find(move.From), crates.Find(move.To), move.Quantity);

        return crates.Change(move.From, e => (true, from)).Change(move.To, e => (true, to));

        static (LList<char>? From, LList<char>? To) MoveMany(LList<char>? from, LList<char>? to, int count)
            => count == 0
                ? (from, to)
                : MoveMany(from!.Tail, to, count - 1).Pipe(moved => (moved.From, new LList<char>(from.Head, moved.To)));
    }


    public static string Puzzle(string input, Func<Map<int, LList<char>>, Move, Map<int, LList<char>>> mover)
    {
        var data = LoadData(input);
        var crates = data.Moves.Aggregate(data.Crates, mover);
        var heads = MapM.Entries(crates).Select(t => t.Value.Head);
        return string.Join("", heads);
    }

    public static string Puzzle1(string input) => Puzzle(input, MoveOneByOne);
    public static string Puzzle2(string input) => Puzzle(input, MoveAllAtOnce);
}
