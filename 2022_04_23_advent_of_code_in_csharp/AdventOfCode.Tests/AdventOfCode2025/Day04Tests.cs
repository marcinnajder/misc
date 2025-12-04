using AdventOfCode.AdventOfCode2025.Day4;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2025;

[TestClass]
public class Day04Tests
{
    [TestMethod]
    public void LoadDataTest()
    {
        var input = """
                    ..@@.@@@@.
                    @@@.@.@.@@
                    @@@@@.@.@@
                    @.@@@@..@.
                    @@.@@@@.@@
                    .@@@@@@@.@
                    .@.@.@.@@@
                    @.@@@.@@@@
                    .@@@@@@@@.
                    @.@.@@@.@.
                    """;

        var data = Day4.LoadData(input);
        Assert.AreEqual(8 + 2, data.Size);
    }
}