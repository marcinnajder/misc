using AdventOfCode.AdventOfCode2016.Day1;
using AdventOfCode.AdventOfCode2016.Day12;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AdventOfCode.AdventOfCode2016.Day12.Registry;

namespace AdventOfCode.Tests.AdventOfCode2016;

[TestClass]
public class Day12Tests
{
    private string input = """
cpy 41 a
inc a
inc a
dec a
jnz a 2
dec a      
""";

    [TestMethod]
    public void LoadDataTest()
    {
        var instructions = Day12.LoadData(input).ToList();

        Assert.AreEqual(6, instructions.Count);

        CollectionAssert.AreEqual(
            new IInstruction[]
            {
                new Copy(new NumberValue(41), A),
                new Inc(A),
                new Inc(A),
                new Dec(A),
                new Jump(new RegistryValue(A), 2),
                new Dec(A),
            },
            instructions
        );
    }

    [TestMethod]
    public void Puzzle1Test()
    {
        var value = Day12.Puzzle1(input);
        Assert.AreEqual("42", value);
    }

}