// https://leetcode.com/problems/number-of-steps-to-reduce-a-number-to-zero/

namespace LeetCode;

using static System.Linq.Enumerable;

class P1342_NumberOfSteps
{
    static int NumberOfSteps(int num)
    {
        return new[] { num }
            .Expand(n => new[] { (n % 2 == 0 ? n / 2 : n - 1) })
            .TakeWhile(n => n != 0)
            .Count();
    }

    public static void Run()
    {
        var inputs = new[] { 14, 8, 123, 8, 123 };

        foreach (var input in inputs)
        {
            var output = NumberOfSteps(input);
            Console.WriteLine($" {input} -> {output}");
        }
    }
}