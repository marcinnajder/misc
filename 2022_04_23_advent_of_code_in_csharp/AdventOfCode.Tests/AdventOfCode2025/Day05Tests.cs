using AdventOfCode.AdventOfCode2025.Day5;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2025;

[TestClass]
public class Day05Tests
{
    [TestMethod]
    public void LoadDataTest()
    {
        var input = """
                    3-5
                    10-14
                    16-20
                    12-18

                    1
                    5
                    8
                    11
                    17
                    32
                    """;

        var data = Day5.LoadData(input);
        Assert.AreEqual(4, data.Ranges.Count);
        Assert.AreEqual(new IdRange(3, 5), data.Ranges[0]);
        CollectionAssert.AreEqual(new List<long>() { 1, 5, 8, 11, 17, 32 }, data.Ids);
    }
}