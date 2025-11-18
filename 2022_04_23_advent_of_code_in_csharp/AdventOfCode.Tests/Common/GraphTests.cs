using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.AdventOfCode2021.Tests;



// let input =
//     """1163751742
// 1381373672
// 2136511328
// 3694931569
// 7463417111
// 1319128137
// 1359912421
// 3125421639
// 1293138521
// 2311944581"""

// let graph = input |> loadGridFromData |> loadGraphFromGrid
// dijkstraShortestPath graph (0, 0) (9, 9) |> ignore // 40

// // let input =
// //     System.IO.File.ReadAllText
// //         "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day15.txt"
// // dijkstraShortestPath graph (0,0) (99,99) // 540

[TestClass]
public class ExtensionsTests
{
    [TestMethod]
    public void DijkstraTraverseLazilyTest()
    {
        // https://adventofcode.com/2021/day/15
        var input = """
1163751742
1381373672
2136511328
3694931569
7463417111
1319128137
1359912421
3125421639
1293138521
2311944581
""";

        var edges = input.Pipe(Grid.LoadGridFromData).Pipe(Graph.LoadGraphFromGrid).ToArray();
        var shortestPath = Graph.DijkstraShortestPath((0, 0), (9, 9), edges);
        Assert.AreEqual(40, shortestPath);

        // input = File.ReadAllText("/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day15.txt");
        // var edges = input.Pipe(Graph.LoadGridFromData).Pipe(Graph.LoadGraphFromGrid).ToArray();
        // var shortestPath = Graph.DijkstraShortestPath((0, 0), (99, 99), edges);
        // Assert.AreEqual(540, shortestPath);
    }

}