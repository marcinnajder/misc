using AdventOfCode.AdventOfCode2016.Day7;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2016;

[TestClass]
public class Day07Tests
{
    [TestMethod]
    public void IsAbbaTest()
    {
        Assert.IsTrue(Day7.IsAbba("abba"));
        Assert.IsTrue(Day7.IsAbba("ioxxoj"));
        Assert.IsFalse(Day7.IsAbba("aaaa"));
    }

    [TestMethod]
    public void FindAllAbaTest()
    {
        CollectionAssert.AreEqual(new[] { ('a', 'b', 'a') }, Day7.FindAllAba("aba").ToArray());
        CollectionAssert.AreEqual(new[] { ('z', 'a', 'z'), ('z', 'b', 'z') }, Day7.FindAllAba("zazbz").ToArray());
        CollectionAssert.AreEqual(new (char, char, char)[] { }, Day7.FindAllAba("aaa").ToArray());
    }
}