using AdventOfCode.AdventOfCode2016.Day10;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.Tests.AdventOfCode2016;

[TestClass]
public class Day10Tests
{
    [TestMethod]
    public void LoadDataTest()
    {
        var input = """
value 5 goes to bot 2
bot 2 gives low to bot 1 and high to bot 0
value 3 goes to bot 1
bot 1 gives low to output 1 and high to bot 0
bot 0 gives low to output 2 and high to output 0
value 2 goes to bot 2        
""";

        var instructions = Day10.LoadData(input).ToList();

        Assert.AreEqual(6, instructions.Count);
        Assert.AreEqual(new BotValue(2, 5), instructions[0]);
        Assert.AreEqual(new BotPasses(2, 1, true, 0, true), instructions[1]);
        Assert.AreEqual(new BotPasses(1, 1, false, 0, true), instructions[3]);
    }
}