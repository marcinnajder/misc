// https://leetcode.com/problems/merge-two-sorted-lists/

namespace LeetCode;

using static LList;

class P0021_MergeTwoSortedLists
{
    static LList<int>? MergeTwoLists(LList<int>? list1, LList<int>? list2)
    {
        return (list1, list2) switch
        {
            (var list, null) => list,
            (null, var list) => list,
            ((var head1, var tail1), (var head2, var tail2)) =>
                head1 == head2
                    ? new(head1, new(head2, MergeTwoLists(tail1, tail2)))
                    : (head1 < head2
                        ? new(head1, MergeTwoLists(tail1, list2))
                        : new(head2, MergeTwoLists(list1, tail2)))
        };
    }

    public static void Run()
    {
        var inputs = new[]
        {
            (LListFrom(1,2,4),LListFrom(1,3,4)),
            (LListFrom<int>(),LListFrom<int>()),
        };

        foreach (var (list1, list2) in inputs)
        {
            var output = MergeTwoLists(list1, list2);
            Console.WriteLine($" {list1.ToEnumerable().JoinToString()} - {list2.ToEnumerable().JoinToString()} -> {output.ToEnumerable().JoinToString()}");
        }
    }
}
