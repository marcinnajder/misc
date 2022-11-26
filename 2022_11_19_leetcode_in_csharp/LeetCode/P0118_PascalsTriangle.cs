// https://leetcode.com/problems/pascals-triangle/

namespace LeetCode;

using static System.Linq.Enumerable;

class P0118_PascalsTriangle
{
    static int[][] PascalsTriangle(int rowNumber)
    {
        var one = new[] { 1 };

        return new[] { one }
            .Expand(prev => new[] { one.Concat(prev.Pairwise().Select(t => t.Item1 + t.Item2)).Concat(one).ToArray() })
            .Take(rowNumber)
            .ToArray();
    }

    public static void Run()
    {
        var inputs = new[] { 0, 1, 2, 3, 4, 5 };

        foreach (var input in inputs)
        {
            var output = PascalsTriangle(input);
            Console.WriteLine($" {input} -> {output.Select(row => row.JoinToString()).JoinToString(" ")}");
        }
    }
}

