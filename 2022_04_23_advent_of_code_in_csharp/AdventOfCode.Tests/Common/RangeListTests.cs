using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Common;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class RangeListTests
{
    [TestMethod]
    public void MergeLines_Empty_AddsLine()
    {
        var result = RangeList.MergeLines(null, (6, 8));
        CollectionAssert.AreEqual(new[] { (6, 8) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_Single_AddsAfter()
    {
        var lines = LListM.LListFrom((10, 12));
        var result = RangeList.MergeLines(lines, (14, 16));
        CollectionAssert.AreEqual(new[] { (10, 12), (14, 16) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_InsertBetweenTwo()
    {
        var lines = LListM.LListFrom((10, 12), (18, 20));
        var result = RangeList.MergeLines(lines, (14, 16));
        CollectionAssert.AreEqual(new[] { (10, 12), (14, 16), (18, 20) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_MergeLeft()
    {
        var lines = LListM.LListFrom((10, 12), (16, 18));
        var result = RangeList.MergeLines(lines, (6, 9));
        CollectionAssert.AreEqual(new[] { (6, 12), (16, 18) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_ExtendRightOfFirst()
    {
        var lines = LListM.LListFrom((10, 12), (16, 18));
        var result = RangeList.MergeLines(lines, (13, 14));
        CollectionAssert.AreEqual(new[] { (10, 14), (16, 18) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_OverlapLeft()
    {
        var lines = LListM.LListFrom((10, 12), (16, 18));
        var result = RangeList.MergeLines(lines, (8, 11));
        CollectionAssert.AreEqual(new[] { (8, 12), (16, 18) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_OverlapAcrossBoundary()
    {
        var lines = LListM.LListFrom((10, 12), (16, 18));
        var result = RangeList.MergeLines(lines, (11, 13));
        CollectionAssert.AreEqual(new[] { (10, 13), (16, 18) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_FullMergeBetween()
    {
        var lines = LListM.LListFrom((10, 12), (16, 18));
        var result = RangeList.MergeLines(lines, (13, 15));
        CollectionAssert.AreEqual(new[] { (10, 18) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_SpanAndMerge()
    {
        var lines = LListM.LListFrom((10, 12), (16, 18));
        var result = RangeList.MergeLines(lines, (11, 17));
        CollectionAssert.AreEqual(new[] { (10, 18) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_MergeManyCoveringMiddle()
    {
        var lines = LListM.LListFrom((10, 12), (16, 18), (25, 27));
        var result = RangeList.MergeLines(lines, (7, 20));
        CollectionAssert.AreEqual(new[] { (7, 20), (25, 27) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_MergeToSingle()
    {
        var lines = LListM.LListFrom((10, 12), (16, 18), (25, 27));
        var result = RangeList.MergeLines(lines, (7, 30));
        CollectionAssert.AreEqual(new[] { (7, 30) }, result.ToEnumerable().ToArray());
    }

    [TestMethod]
    public void MergeLines_PrependsWhenBeforeAll()
    {
        var lines = LListM.LListFrom((10, 12));
        var result = RangeList.MergeLines(lines, (6, 8));
        CollectionAssert.AreEqual(new[] { (6, 8), (10, 12) }, result.ToEnumerable().ToArray());
    }
}