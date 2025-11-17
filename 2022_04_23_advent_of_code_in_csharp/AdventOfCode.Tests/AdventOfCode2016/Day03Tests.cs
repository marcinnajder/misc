using AdventOfCode.AdventOfCode2016.Day3;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests;

[TestClass]
public class Day03Tests
{

    [TestMethod]
    public void LoadDataTest()
    {
        var triangles = Day3.LoadData("5 25 10").ToList();
        Assert.AreEqual(new Triangle(5, 10, 25), triangles[0]);
    }

    [TestMethod]
    public void IsValidTest()
    {
        Assert.AreEqual(false, new Triangle(5, 10, 25).IsValid());
        Assert.AreEqual(true, new Triangle(3, 4, 5).IsValid());
    }
}