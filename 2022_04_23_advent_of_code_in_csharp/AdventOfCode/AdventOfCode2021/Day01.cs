namespace AdventOfCode.AdventOfCode2021;

static class Day1
{
    public static IEnumerable<int> LoadData(string input) =>
        input.Split(Environment.NewLine).Select(Int32.Parse);

    private static int CountIncreases(IEnumerable<int> numbers) =>
        numbers.Pairwise().Count(pair => pair.Item2 > pair.Item1);

    public static string Puzzle1(string input)
    {
        var numers = LoadData(input);
        var result = CountIncreases(numers);
        return result.ToString();
    }
    //=> input.Pipe(LoadData).Pipe(CountIncreases).ToString();

    public static string Puzzle2(string input)
    {
        var numers = LoadData(input);
        var addedNumbers = numers.Windowed(3).Select(Enumerable.Sum);
        var result = CountIncreases(addedNumbers);
        return result.ToString();
    }
}

