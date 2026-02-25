using AdventOfCode.AdventOfCode2025.Day9;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2025;

[TestClass]
public class Day09Tests
{
    private static SortedDictionary<int, List<(int, int)>> linesByColumns = new()
    {
        { 0, [(4, 7)] }, // column 0, rows 4..7 included
        { 3, [(0, 4)] }, // column 3, rows 0..4 included
        { 6, [(0, 5)] },
        { 9, [(0, 5)] },
        { 8, [(7, 10)] },
        { 10, [(0, 10)] },
    };

    [TestMethod]
    public void BuildLinesByColumnsFromPointsTest()
    {
        Point[] points =
        [
            new(0, 7), new(0, 4), new(3, 4), new(3, 0),
            new(6, 0), new(6, 5), new(9, 5), new(9, 0),
            new(10, 0), new(10, 10), new(8, 10), new(8, 7),
        ];

        SortedDictionary<int, List<(int, int)>> linesByColumnsFromPoints = Day9.BuildLinesByColumnsFromPoints(points);

        Assert.AreEqual(linesByColumns.Count, linesByColumnsFromPoints.Count);

        foreach (var (x, lines) in linesByColumnsFromPoints)
        {
            CollectionAssert.AreEqual(linesByColumns[x], lines);
        }
    }

    [TestMethod]
    public void IsInsideTest()
    {
        var rows = new Dictionary<int, List<int>>
        {
            [0] = [0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1],
            [1] = [0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1],
            [2] = [0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1],
            [3] = [0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1],
            //[4] = [1, 1, 1, 1, 0, 0, 1, 0, 0, 1, 1],
            [4] = [1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1], // 4,4 wysypuje sie 
            [5] = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
            [6] = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
            [7] = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1],
            [9] = [0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1],
            [10] = [0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1],
        };

        foreach (var (x, y) in rows.SelectMany(kv => Enumerable.Range(0, kv.Value.Count), (kv, x) => (x, y: kv.Key)))
        {
            Assert.AreEqual(rows[y][x] == 1, Day9.IsInside(linesByColumns, new Point(x, y)));
        }
    }
}