using AdventOfCode.AdventOfCode2025.Day2;
using AdventOfCode.Tests.AdventOfCode2016;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2025;

[TestClass]
public class Day02Tests
{
    [TestMethod]
    public void LoadDataTest()
    {
        var input = "11-22,95-115,998-1012";

        var ranges = Day2.LoadData(input).ToList();

        CollectionAssert.AreEqual(new IdRange[] { new("11", "22"), new("95", "115"), new("998", "1012") }, ranges);
    }


    [TestMethod]
    public void FindInvalidIdsTest()
    {
        CollectionAssert.AreEqual(new[] { "11", "22" }, Day2.FindInvalidIds(new("11", "22")).ToList());
        CollectionAssert.AreEqual(new[] { "99" }, Day2.FindInvalidIds(new("95", "115")).ToList());
        CollectionAssert.AreEqual(new[] { "1010" }, Day2.FindInvalidIds(new("998", "1012")).ToList());
        CollectionAssert.AreEqual(new[] { "1188511885" }, Day2.FindInvalidIds(new("1188511880", "1188511890")).ToList());

        CollectionAssert.AreEqual(new[] { "222222" }, Day2.FindInvalidIds(new("222220", "222224")).ToList());
        CollectionAssert.AreEqual(new string[] { }, Day2.FindInvalidIds(new("1698522", "1698528")).ToList());

        CollectionAssert.AreEqual(new[] { "446446" }, Day2.FindInvalidIds(new("446443", "446449")).ToList());

        CollectionAssert.AreEqual(new[] { "38593859" }, Day2.FindInvalidIds(new("38593856", "38593862")).ToList());

        CollectionAssert.AreEqual(new string[] { }, Day2.FindInvalidIds(new("565653", "565659")).ToList());

        CollectionAssert.AreEqual(new string[] { }, Day2.FindInvalidIds(new("565653", "565659")).ToList());
        CollectionAssert.AreEqual(new string[] { }, Day2.FindInvalidIds(new("824824821", "824824827")).ToList());
        CollectionAssert.AreEqual(new string[] { }, Day2.FindInvalidIds(new("2121212118", "2121212124")).ToList());
    }

    [TestMethod]
    public void IsIdValidTest()
    {
        Assert.IsTrue(Day2.IsIdValid("123123"));
        // Assert.IsTrue(Day2.IsIdValid2("121212"));
        // Assert.IsTrue(Day2.IsIdValid2("111111"));

        // Assert.IsFalse(Day2.IsIdValid2("123120"));
        // Assert.IsFalse(Day2.IsIdValid2("121210"));
        // Assert.IsFalse(Day2.IsIdValid2("111110"));
    }
}