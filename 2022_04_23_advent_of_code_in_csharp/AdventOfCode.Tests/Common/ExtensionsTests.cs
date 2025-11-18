using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AdventOfCode.Option;


namespace AdventOfCode.AdventOfCode2021.Tests;


[TestClass]
public class GraphTests
{
    [TestMethod]
    public void JoinToString()
    {
        Assert.AreEqual("", new int[] { }.JoinToString());
        Assert.AreEqual("1", new[] { 1 }.JoinToString());
        Assert.AreEqual("1,2", new[] { 1, 2 }.JoinToString());
        Assert.AreEqual("1-2-3", new[] { 1, 2, 3 }.JoinToString("-"));
    }


    [TestMethod]
    public void Pairwise()
    {
        var pairs = new[]
        {
            (Enumerable.Range(0, 0).Pairwise(),""),
            (Enumerable.Range(0, 1).Pairwise(),""),
            (Enumerable.Range(0, 2).Pairwise(),"0,1"),
            (Enumerable.Range(0, 3).Pairwise(),"0,1-1,2"),
            (Enumerable.Range(0, 4).Pairwise(),"0,1-1,2-2,3"),
        };

        foreach (var (windows, result) in pairs)
        {
            Assert.AreEqual(result, windows.Select(p => $"{p.Item1},{p.Item2}").JoinToString("-"));
        }
    }


    [TestMethod]
    public void Windowed()
    {
        var pairs = new[]
        {
            (Enumerable.Range(0, 5).Windowed(10),""),
            (Enumerable.Range(0, 5).Windowed(5),"0-1-2-3-4"),
            (Enumerable.Range(0, 5).Windowed(2),"0-1,1-2,2-3,3-4"),
            (Enumerable.Range(0, 5).Windowed(1),"0,1,2,3,4"),
        };

        foreach (var (windows, result) in pairs)
        {
            Assert.AreEqual(result, windows.Select(w => w.JoinToString("-")).JoinToString());
        }
    }


    [TestMethod]
    public void Choose()
    {
        Assert.AreEqual("10,20", new[] { 0, 1, 0, 2 }.Choose(x => x > 0 ? Some(x * 10) : None<int>()).JoinToString());
    }

    [TestMethod]
    public void Pick()
    {
        Assert.AreEqual(Option.Some(10), new[] { 0, 1, 0, 2 }.TryPick(x => x > 0 ? Some(x * 10) : None<int>()));
        Assert.AreEqual(Option.None<int>(), new[] { 0, 1, 0, 2 }.TryPick(x => x > 2 ? Some(x * 10) : None<int>()));
    }

    [TestMethod]
    public void AggregateBack()
    {
        Assert.AreEqual("", new char[] { }.AggregateBack("", (p, c) => p + c));
        Assert.AreEqual("a", new char[] { 'a' }.AggregateBack("", (p, c) => p + c));
        Assert.AreEqual("cba", new char[] { 'a', 'b', 'c' }.AggregateBack("", (p, c) => p + c));
    }


    [TestMethod]
    public void PartitionByTest()
    {

        var pairs = new[]
      {
            (new string[0].PartitionBy(IsSpace),""),
            (new [] { "a"}.PartitionBy(IsSpace),"a"),
            (new [] { "a", "b"}.PartitionBy(IsSpace),"a-b"),

            (new [] { "a", "b", " ", "c" ,"d"}.PartitionBy(IsSpace),"a-b, ,c-d"),
            (new [] { "a", "b", " ", " ", "c" ,"d"}.PartitionBy(IsSpace),"a-b, - ,c-d"),
            (new [] { "a", "b", " ", }.PartitionBy(IsSpace),"a-b, "),

            (new string?[] { "a", "b", null, null, "c"}.PartitionBy(IsNull),"a-b,-,c"),

            (new [] { "a", "b", "cc", "dd" ,"e"}.PartitionBy(GetLength),"a-b,cc-dd,e"),

        };

        foreach (var (packs, result) in pairs)
        {
            Assert.AreEqual(result, packs.Select(w => w.JoinToString("-")).JoinToString());
        }

        static bool IsSpace(string str) => str is " ";
        static bool IsNull(string? str) => str is null;
        static int GetLength(string str) => str.Length;

    }



}