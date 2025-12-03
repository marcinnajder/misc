using AdventOfCode.AdventOfCode2016.Day14;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2016;

[TestClass]
public class Day14Tests
{
    [TestMethod]
    public void FindOrCheckSeries2Test()
    {
        Assert.IsNull(Day14.FindOrCheckSeries("abcdef", 2));
        Assert.IsNull(Day14.FindOrCheckSeries("abbef", 3));

        Assert.AreEqual('b', Day14.FindOrCheckSeries("abbbef", 3));
        Assert.AreEqual('b', Day14.FindOrCheckSeries("abbb", 3));
        Assert.AreEqual('b', Day14.FindOrCheckSeries("bbbef", 3));
        Assert.AreEqual('b', Day14.FindOrCheckSeries("abbbef", 2));

        Assert.AreEqual('b', Day14.FindOrCheckSeries("abbbef", 3, 'b'));
        Assert.AreEqual('b', Day14.FindOrCheckSeries("acccbbbef", 3, 'b'));

        Assert.IsNull(Day14.FindOrCheckSeries("abbeef", 3, 'b'));
    }
}