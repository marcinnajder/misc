// https://leetcode.com/problems/how-many-numbers-are-smaller-than-the-current-number/

namespace LeetCode;

using static System.Linq.Enumerable;

class P1365SmallerNumbersThanCurrent
{
    static int[] SmallerNumbersThanCurrent(int[] numbers)
    {
        return numbers
            .Select((n, index) => Range(0, numbers.Length).Count(j => j != index && numbers[j] < n))
            .ToArray();
    }

    public static void Run()
    {
        var inputs = new[]
        {
            new []  {8, 1, 2, 2, 3},
            new []  {6, 5, 4, 8},
            new []  {7, 7, 7, 7},
        };

        foreach (var input in inputs)
        {
            var output = SmallerNumbersThanCurrent(input);
            Console.WriteLine($" {input.JoinToString()} -> {output.JoinToString()}");
        }
    }
}
