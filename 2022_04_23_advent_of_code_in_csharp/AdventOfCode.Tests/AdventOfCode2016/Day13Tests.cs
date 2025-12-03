using AdventOfCode.AdventOfCode2016.Day13;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2016;

[TestClass]
public class Day13Tests
{
    [TestMethod]
    public void IsOpenSpaceTest()
    {
        // https://adventofcode.com/2016/day/13

        var board = """
.#.####.##
..#..#...#
#....##...
###.#.###.
.##..#..#.
..##....#.
#...##.###
""";

        var lines = board.Split(Environment.NewLine, StringSplitOptions.TrimEntries)!;
        var columnCount = lines[0].Length;
        for (int row = 0; row < lines.Length; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                Assert.AreEqual(lines[row][column] == '.', Day13.IsOpenSpace(((row, column), 10)));
            }
        }
    }
}