using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdventOfCode.AdventOfCode2016.Day2;
using static AdventOfCode.AdventOfCode2016.Day2.Direction;

namespace AdventOfCode.AdventOfCode2015.Tests;

[TestClass]
public class Day02Tests
{
    const string input =
        """
        ULL
        RRDDD
        LURDL
        UUUUD
        """;

    [TestMethod]
    public void LoadDataTest()
    {
        var lines = Day2.LoadData(input).ToList();

        CollectionAssert.AreEqual(new Direction[] { U, L, L }, lines[0].ToList());
    }

    [TestMethod]
    public void MoveTest()
    {

        var keypad = new char[,]
        {
            { ' ', ' ', '1' , ' ', ' ' },
            { ' ', '2', '3' , '4', ' ' },
            { '5', '6', '7' , '8', '9' },
            { ' ', 'A', 'B' , 'C', ' ' },
            { ' ', ' ', 'D' , ' ', ' ' },
        };


        // without any move
        Assert.AreEqual(new Point(0, 0), Day2.Move2(new Point(0, 0), L, keypad));
        Assert.AreEqual(new Point(0, 0), Day2.Move2(new Point(0, 0), U, keypad));
        Assert.AreEqual(new Point(4, 4), Day2.Move2(new Point(4, 4), R, keypad));
        Assert.AreEqual(new Point(4, 4), Day2.Move2(new Point(4, 4), D, keypad));

        Assert.AreEqual(new Point(2, 0), Day2.Move2(new Point(2, 0), U, keypad));
        Assert.AreEqual(new Point(2, 0), Day2.Move2(new Point(2, 0), L, keypad));
        Assert.AreEqual(new Point(2, 0), Day2.Move2(new Point(2, 0), R, keypad));
        Assert.AreEqual(new Point(2, 1), Day2.Move2(new Point(2, 0), D, keypad));
    }

    [TestMethod]
    public void Puzzle1Test()
    {
        var result = Day2.Puzzle1(input);
        Assert.AreEqual("1985", result);
    }

    [TestMethod]
    public void Puzzle2Test()
    {
        var result = Day2.Puzzle2(input);
        Assert.AreEqual("5DB3", result);
    }
}