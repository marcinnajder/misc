using AdventOfCode.AdventOfCode2025.Day3;
using AdventOfCode.Tests.AdventOfCode2016;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2025;

[TestClass]
public class Day03Tests
{
    [TestMethod]
    public void FindJoltageTest()
    {
        Assert.AreEqual(98, Day3.FindJoltage("987654321111111", 2));
        Assert.AreEqual(89, Day3.FindJoltage("811111111111119", 2));
        Assert.AreEqual(78, Day3.FindJoltage("234234234234278", 2));
        Assert.AreEqual(92, Day3.FindJoltage("818181911112111", 2));

        Assert.AreEqual(987654321111, Day3.FindJoltage("987654321111111", 12));
        Assert.AreEqual(811111111119, Day3.FindJoltage("811111111111119", 12));
        Assert.AreEqual(434234234278, Day3.FindJoltage("234234234234278", 12));
        Assert.AreEqual(888911112111, Day3.FindJoltage("818181911112111", 12));
    }
}