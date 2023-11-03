using System.Reflection;

namespace AlgorithmsAndDataStructures.Tests;

[TestClass]
public class LListTester
{
    private LList<int> intsEmpty = [];
    private LList<int> ints = [1, 2, 3];
    private bool throwsWhenEmpty = typeof(LList<>).GetField("head", BindingFlags.Instance | BindingFlags.NonPublic) != null;

    [TestMethod]
    public void Equals()
    {
        Assert.AreEqual<LList<int>>([], []);
        Assert.AreEqual<LList<int>>([1, 2], [1, 2]);

        Assert.AreNotEqual<LList<int>>([1, 2], [1, 2, 3]);
        Assert.AreNotEqual<LList<int>>([1, 2], [1, 3]);
    }

    [TestMethod]
    public void EmptyLists()
    {
        var empty1 = LList<string>.Empty;
        var empty2 = new LList<string>();
        LList<string> empty3 = [];

        foreach (var empty in new[] { empty1, empty2, empty3 })
        {
            Assert.AreEqual(0, empty.Length);
            Assert.AreEqual("[]", empty.ToString());
            if (throwsWhenEmpty)
            {
                Assert.ThrowsException<InvalidOperationException>(() => empty.Head);
                Assert.ThrowsException<InvalidOperationException>(() => empty.Tail);
            }
            else
            {
                Assert.AreEqual(default, empty.Head);
                Assert.AreEqual(default, empty.Tail);
            }
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => empty[0]);
        }
    }

    [TestMethod]
    public void NonEmptyLists()
    {
        LList<int> nonempty1 = [1, 2, 3];
        LList<int> nonempty2 = LList.Of(1, 2, 3);
        LList<int> nonempty3 = new LList<int>(1, new LList<int>(2, new LList<int>(3, new LList<int>())));

        foreach (var nonempty in new[] { nonempty1, nonempty2, nonempty3 })
        {
            Assert.AreEqual(3, nonempty.Length);
            Assert.AreEqual("[1, 2, 3]", nonempty.ToString());
            Assert.AreEqual(1, nonempty.Head);
            Assert.AreEqual(1, nonempty[0]);
            Assert.AreEqual(2, nonempty[1]);
        }
    }


    [TestMethod]
    public void Index()
    {
        Assert.AreEqual(1, ints[0]);
        Assert.AreEqual(3, ints[^1]);
    }

    [TestMethod]
    public void Slice()
    {
        Assert.AreEqual([1, 2, 3], ints.Slice(0, 3));
        Assert.AreSame(ints, ints.Slice(0, 3));
        Assert.AreEqual([2, 3], ints.Slice(1, 2));
        Assert.AreSame(ints.Tail, ints.Slice(1, 2));
        Assert.AreEqual([], ints.Slice(0, 0));

        Assert.AreEqual([1], ints.Slice(0, 1));
        Assert.AreEqual([1, 2], ints.Slice(0, 2));
        Assert.AreEqual([2], ints.Slice(1, 1));
        Assert.AreEqual([2, 3], ints.Slice(1, 2));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ints.Slice(0, 4));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ints.Slice(1, 3));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ints.Slice(-1, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ints.Slice(1, -1));
    }

    [TestMethod]
    public void PatternMatching()
    {
        Assert.AreEqual("[]", PrintUsingPatternMatching([]));
        Assert.AreEqual("[1]", PrintUsingPatternMatching([1]));
        Assert.AreEqual("[1,2]", PrintUsingPatternMatching([1, 2]));
        Assert.AreEqual("['2',2]", PrintUsingPatternMatching([2, 2]));
        Assert.AreEqual("[3,_,_,4]", PrintUsingPatternMatching([3, 55, 66, 4]));
        Assert.AreEqual("[55, 66]", PrintUsingPatternMatching([3, 55, 66, 100]));
        Assert.AreEqual("[55, 66]", PrintUsingPatternMatching([3, 55, 66, 100]));
        //Assert.AreEqual("head: 3 tail: [55, 66, 100, 1001]", PrintUsingPatternMatching([3, 55, 66, 100, 1001]));

        static string PrintUsingPatternMatching(LList<int> list) =>
            list switch
            {
            [] => "[]",
            [1] => "[1]",
            [1, 2] => "[1,2]",
            [var first, 2] => $"['{first}',2]",
            [3, _, _, 4] => $"[3,_,_,4]",
            [3, .. var middle, 100] => middle.ToString(),
                //(var head, var tail) => $"head: {head} tail: {tail}",
                _ => "other"
            };

    }

    [TestMethod]
    public void ToLList()
    {
        Assert.AreEqual([], Array.Empty<int>().ToLList());
        Assert.AreEqual([], ToEnumerable(Array.Empty<int>()).ToLList());

        Assert.AreEqual([1], new List<int>() { 1 }.ToLList());
        Assert.AreEqual([1], ToEnumerable(new List<int>() { 1 }).ToLList());

        Assert.AreEqual([1, 2], new List<int>() { 1, 2 }.ToLList());
        Assert.AreEqual([1, 2], ToEnumerable(new List<int>() { 1, 2 }).ToLList());
    }

    [TestMethod]
    public void CountLSelectLWhereL()
    {
        Assert.AreEqual(3, ints.CountL());
        Assert.AreEqual([10, 20, 30], ints.SelectL(x => x * 10));
        Assert.AreEqual([2], ints.WhereL(x => x % 2 == 0));
    }

    [TestMethod]
    public void ConcatLSkipLSkipWhileL()
    {
        Assert.AreEqual([1, 2, 3, 10, 20, 30], ints.ConcatL([10, 20, 30]));

        Assert.AreEqual([1, 2, 3], ints.SkipL(0));
        Assert.AreEqual([2, 3], ints.SkipL(1));
        Assert.AreEqual([3], ints.SkipL(2));
        Assert.AreEqual([], ints.SkipL(3));
        Assert.AreEqual([], ints.SkipL(4));

        Assert.AreEqual([], intsEmpty.SkipWhileL(x => x < 2));
        Assert.AreEqual([2, 3], ints.SkipWhileL(x => x < 2));
        Assert.AreSame(ints.Tail, ints.SkipWhileL(x => x < 2));
    }



    [TestMethod]
    public void CountFirstSingle()
    {
        Assert.AreEqual(0, intsEmpty.Count());
        Assert.AreEqual(3, ints.Count());

        Assert.ThrowsException<InvalidOperationException>(() => intsEmpty.First());
        Assert.AreEqual(1, ints.First());

        Assert.AreEqual(0, intsEmpty.FirstOrDefault());
        Assert.AreEqual(1, ints.FirstOrDefault());

        Assert.ThrowsException<InvalidOperationException>(() => intsEmpty.Single());
        Assert.ThrowsException<InvalidOperationException>(() => ints.Single());
        Assert.AreEqual(11, new LList<int>(11, LList<int>.Empty).Single());

        Assert.AreEqual(0, intsEmpty.SingleOrDefault());
        Assert.ThrowsException<InvalidOperationException>(() => ints.Single());
        Assert.AreEqual(11, new LList<int>(11, LList<int>.Empty).SingleOrDefault());
    }

    // [TestMethod]
    // public void ToLList2()
    // {
    //     // comparing list without ValueRef to list with ValueRef
    //     Assert.AreEqual([], Array.Empty<int>().ToLList2());
    //     Assert.AreEqual([], ToEnumerable(Array.Empty<int>()).ToLList2());

    //     Assert.AreEqual([1], new List<int>() { 1 }.ToLList2());
    //     Assert.AreEqual([1], ToEnumerable(new List<int>() { 1 }).ToLList2());

    //     Assert.AreEqual([1, 2], new List<int>() { 1, 2 }.ToLList2());
    //     Assert.AreEqual([1, 2], ToEnumerable(new List<int>() { 1, 2 }).ToLList2());

    //     // comparing list with ValueRef to list with ValueRef
    //     Assert.AreEqual(
    //         ToEnumerable(new List<int>() { 1, 2 }).ToLList2(),
    //         ToEnumerable(new List<int>() { 1, 2 }).ToLList2());

    //     // mixing list wittout ValueRef and list with ValueRef
    //     Assert.AreEqual([-1, 1, 2], new LList<int>(-1, ToEnumerable(new List<int>() { 1, 2 }).ToLList2()));
    // }

    private static IEnumerable<T> ToEnumerable<T>(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }
    }
}
