
namespace AdventOfCode.AdventOfCode2021;

static class Day10
{
    record State;
    record Processing(LList<char>? Items) : State;
    record Corrupted(char Item) : State;
    record Completed(LList<char>? Items) : State;

    static Dictionary<char, char> Close2OpenMap = new() { { ')', '(' }, { '>', '<' }, { ']', '[' }, { '}', '{' }, };

    static Dictionary<char, char> Open2CloseMap = Close2OpenMap.ToDictionary(kv => kv.Value, kv => kv.Key);

    static Dictionary<char, int> Points1 = new() { { ')', 3 }, { ']', 57 }, { '}', 1197 }, { '>', 25137 }, };

    static Dictionary<char, long> Points2 = new() { { ')', 1L }, { ']', 2L }, { '}', 3L }, { '>', 4L }, };

    public static string[] LoadData(string input) => input.Split(Environment.NewLine);

    static IEnumerable<State> ProcessText(string text) =>
        text.Concat("-").Scan(
            new Processing(null) as State,
            (state, c) => state switch
            {
                Processing(var items) =>
                    c switch
                    {
                        '-' => new Completed(items),
                        ')' or ']' or '}' or '>' =>
                            items switch
                            {
                                (var head, var rest) when Close2OpenMap[c] == head => new Processing(rest),
                                _ => new Corrupted(c)
                            },
                        _ => new Processing(new(c, items))
                    },
                _ => state
            });

    public static string Puzzle1(string input)
    {
        var data = LoadData(input);
        var result = data
            .Choose(text => ProcessText(text).TryPick(s => s is Corrupted(var c) ? Option.Some(c) : Option.None<char>()))
            .Select(c => Points1[c])
            .Sum();
        return result.ToString();
    }

    public static string Puzzle2(string input)
    {
        var data = LoadData(input);
        var points = data
            .Choose(text => ProcessText(text)
                .TryPick(s => s is Processing ? Option.None<State>() : Option.Some(s))
                .Bind(s => s is Completed(var items) ? Option.Some(items) : Option.None<LList<char>?>()))
            .Select(items => items.ToEnumerable().Aggregate(0L, (a, c) => (a * 5) + Points2[Open2CloseMap[c]]))
            .OrderBy(x => x)
            .ToArray();
        var result = points[(points.Length - 1) / 2];
        return result.ToString();
    }
}