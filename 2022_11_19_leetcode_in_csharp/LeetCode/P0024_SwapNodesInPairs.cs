// https://leetcode.com/problems/swap-nodes-in-pairs/

namespace LeetCode;

using static LList;

class P0024_SwapNodesInPairs
{
    static LList<T>? SwapPairs<T>(LList<T>? list)
    {
        return list switch
        {
            (var head1, (var head2, var tail)) => new(head2, new(head1, SwapPairs(tail))),
            _ => list
        };
    }

    public static void Run()
    {
        var inputs = new[]
        {
            LListFrom(1,2,3,4),
            LListFrom(1),

        };

        foreach (var list in inputs)
        {
            var output = SwapPairs(list);
            Console.WriteLine($" {list.ToEnumerable().JoinToString()} -> {output.ToEnumerable().JoinToString()}");
        }
    }
}
