// https://leetcode.com/problems/first-unique-character-in-a-string/

namespace LeetCode;

using static System.Linq.Enumerable;

class P0387_FirstUniqChar
{
    static int FirstUniqChar(string s)
    {
        return s
            .Select((c, i) => new { Char = c, Index = i })
            .FirstOrDefault(t => Range(0, s.Length).All(j => t.Index == j || t.Char != s[j]))
            ?.Index ?? -1;
    }
    public static void Run()
    {
        var inputs = new[] { "letmein", "lifeislovepoem", "aabb" };

        foreach (var input in inputs)
        {
            var output = FirstUniqChar(input);
            Console.WriteLine($" {input} -> {output}");
        }
    }
}
