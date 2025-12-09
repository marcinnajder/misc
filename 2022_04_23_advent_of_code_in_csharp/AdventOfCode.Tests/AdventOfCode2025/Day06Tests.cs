using AdventOfCode.AdventOfCode2025.Day5;
using AdventOfCode.AdventOfCode2025.Day6;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2025;

[TestClass]
public class Day06Tests
{
    [TestMethod]
    public void Transpose_BasicSquareMatrix_ReturnsExpected()
    {
        var input = new[] { "ab", "cd" };
        var result = Day6.Transpose(input);
        CollectionAssert.AreEqual(new[] { "ac", "bd" }, result);
    }
}