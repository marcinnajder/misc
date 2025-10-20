using System.Reflection;
using System.Text.RegularExpressions;

namespace AdventOfCode;

static class Parser
{
    private static Regex intssRegex = new Regex(@"\d+");

    public static List<int> ParseInts(string text) =>
        intssRegex.Matches(text).Select(m => int.Parse(m.Value)).ToList();
}

