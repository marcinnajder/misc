// https://leetcode.com/problems/two-sum/

namespace LeetCode;

using static System.Linq.Enumerable;

class P0001_TwoSum
{
    static int[] TwoSum(int[] nums, int target)
    {
        return Range(0, nums.Length)
            .SelectMany(i => Range(0, i), (i, j) => new[] { j, i })
            .First(res => nums[res[0]] + nums[res[1]] == target)
            .ToArray();
    }

    public static void Run()
    {
        var inputs = new[]
        {
            (new []  {2, 7, 11, 15}, 9),
            (new []  {3, 2, 4}, 6),
            (new []  {3, 3}, 6)
        };

        foreach (var (nums, target) in inputs)
        {
            var output = TwoSum(nums, target);
            Console.WriteLine($" {nums.JoinToString()} - {target} -> {output.JoinToString()}");
        }
    }
}
