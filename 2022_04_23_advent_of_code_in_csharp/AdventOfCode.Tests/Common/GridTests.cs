using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Common;

[TestClass]
public class GridTests
{
    [TestMethod]
    public void GridTest()
    {
        CollectionAssert.AreEqual(
            new (int, int)[] { (0, 1), (1, 0), (1, 1) },
            Grid.GetNeighboursAll(0, 0, 10, 10).ToArray());


        CollectionAssert.AreEqual(
            new (int, int)[] { (8, 8), (8, 9), (9, 8) },
            Grid.GetNeighboursAll(9, 9, 10, 10).ToArray());

        CollectionAssert.AreEqual(
            new (int, int)[] { (0, 0), (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1), (2, 2) },
            Grid.GetNeighboursAll(1, 1, 10, 10).ToArray());
    }
}