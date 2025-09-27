namespace AdventOfCode.AdventOfCode2016.Day6;

public static class Day6
{
    public static string[] LoadData(string input)
    {
        return input.Split(Environment.NewLine, StringSplitOptions.TrimEntries);
    }

    public static string Puzzle(string input, bool ascending)
    {
        var messages = LoadData(input);
        var width = messages[0].Length;

        var histograms = new Dictionary<char, int>[width];
        for (int i = 0; i < width; i++)
        {
            histograms[i] = new Dictionary<char, int>();
        }

        foreach (var message in messages)
        {
            for (int i = 0; i < width; i++)
            {
                char c = message[i];
                histograms[i][c] = histograms[i].GetValueOrDefault(c, 0) + 1;
            }
        }

        return histograms.Select(h => h.MaxBy(kv => ascending ? kv.Value : -kv.Value).Key).ConcatToString();
    }

    public static string Puzzle1(string input)
    {
        return Puzzle(input, true);
    }

    public static string Puzzle2(string input)
    {
        return Puzzle(input, false);
    }
}
