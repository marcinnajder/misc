using AdventOfCode.AdventOfCode2016.Day1;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests;

[TestClass]
public class Day01Tests
{
    [TestMethod]
    public void LoadDataTest()
    {
        var directions = Day1.LoadData("R2, L3").ToList();

        CollectionAssert.AreEqual(new Turn[] { new(TurnDirection.Right, 2), new(TurnDirection.Left, 3) }, directions);
    }

    [TestMethod]
    public void Puzzle1Test()
    {
        Assert.AreEqual("5", Day1.Puzzle1("R2, L3"));
        Assert.AreEqual("2", Day1.Puzzle1("R2, R2, R2"));
        Assert.AreEqual("12", Day1.Puzzle1("R5, L5, R5, R3"));
    }

    [TestMethod]
    public void Puzzle2Test()
    {
        Assert.AreEqual("4", Day1.Puzzle2("R8, R4, R4, R8"));
    }
}