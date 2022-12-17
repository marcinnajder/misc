// https://leetcode.com/problems/is-subsequence/

namespace LeetCode;

using static System.Linq.Enumerable;

class P0392_IsSubsequence
{
    static bool IsSubsequence(string s, string t)
    {
        return s
            .Scan(-1, (i, c) => Range(i + 1, t.Length - (i + 1)).FirstOrDefault(j => t[j] == c, -1))
            .Skip(1)
            .All(i => i != -1);
    }

    static bool IsSubsequence2(string s, string t)
    {
        return t
            .Scan(0, (i, c) => s[i] == c ? i + 1 : i)
            .Any(i => i == s.Length);
    }

    public static void Run()
    {
        var inputs = new[] {
            ("abc", "eopyahbgdc"),
            ("axc", "ahbgdc"),
        };

        foreach (var (s, t) in inputs)
        {
            var output = IsSubsequence(s, t);
            Console.WriteLine($" {(s, t)} -> {output}");
        }
    }
}