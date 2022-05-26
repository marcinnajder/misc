using System.Collections.Generic;
using System.Linq;
using AdventOfCode.AdventOfCode2021;

var puzzles = new (Func<string, string>, Func<string, string>)[]
{
    (Day1.Puzzle1,Day1.Puzzle2),
    (Day2.Puzzle1,Day2.Puzzle2),
    (Day4.Puzzle1,Day4.Puzzle2),
    (Day10.Puzzle1,Day10.Puzzle2),
    (Day16.Puzzle1,Day16.Puzzle2),
    (Day17.Puzzle1,Day17.Puzzle2),
};

foreach (var (puzzle1, puzzle2) in puzzles)
{
    var day = puzzle1.Method.DeclaringType!.Name.Substring("Day".Length).Pipe(int.Parse);
    var dayStr = $"{(day < 10 ? "0" : "")}{day}";
    var input = Path.Combine(Common.ProjectFolderPath, $"AdventOfCode2021/Day{dayStr}.txt").Pipe(File.ReadAllText);
    Console.WriteLine($"2021/Day{dayStr}/Puzzle01:  {puzzle1(input)}");
    Console.WriteLine($"2021/Day{dayStr}/Puzzle02:  {puzzle2(input)}");
}