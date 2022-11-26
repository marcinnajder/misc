using System.Reflection;

namespace LeetCode;

static class Common
{
    public static string ProjectFolderPath { get; } =
        Path.Combine(new[] { new FileInfo(Assembly.GetEntryAssembly()!.Location).DirectoryName!, "..", "..", ".." });

    public static IEnumerable<int> ParseNumbers(char separator, string strings) =>
        strings.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
}

public class NonExhaustivePatternMatchingException : Exception
{
    public NonExhaustivePatternMatchingException() : base("Non-exhaustive pattern matching") { }
}
