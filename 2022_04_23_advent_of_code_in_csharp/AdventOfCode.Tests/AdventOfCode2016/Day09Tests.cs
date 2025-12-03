using System.Linq;
using AdventOfCode.AdventOfCode2016.Day9;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2016;

[TestClass]
public class Day09Tests
{
    [TestMethod]
    public void Puzzle1Test()
    {
        Assert.AreEqual("6", Day9.Puzzle1("ADVENT"));
        Assert.AreEqual("7", Day9.Puzzle1("A(1x5)BC"));
        Assert.AreEqual("9", Day9.Puzzle1("(3x3)XYZ"));
        Assert.AreEqual("11", Day9.Puzzle1("A(2x2)BCD(2x2)EFG"));
        Assert.AreEqual("6", Day9.Puzzle1("(6x1)(1x3)A"));
        Assert.AreEqual("18", Day9.Puzzle1("X(8x2)(3x3)ABCY"));
    }

    [TestMethod]
    public void Puzzle2Test()
    {
        Assert.AreEqual("9", Day9.Puzzle2("(3x3)XYZ"));
        Assert.AreEqual("20", Day9.Puzzle2("X(8x2)(3x3)ABCY"));
        Assert.AreEqual("241920", Day9.Puzzle2("(27x12)(20x12)(13x14)(7x10)(1x12)A"));
        Assert.AreEqual("445", Day9.Puzzle2("(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN"));
    }
}