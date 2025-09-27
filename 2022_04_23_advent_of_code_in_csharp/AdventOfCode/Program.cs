using System.Collections.Generic;
using System.Linq;
using AdventOfCode;
using AdventOfCode.AdventOfCode2016;
using AdventOfCode.AdventOfCode2021;
using AdventOfCode.AdventOfCode2022;

// dotnet watch test  -l "console;verbosity=detailed"

var puzzles = new (Func<string, string>, Func<string, string>)[]
{

    (AdventOfCode.AdventOfCode2016.Day1.Day1.Puzzle1, AdventOfCode.AdventOfCode2016.Day1.Day1.Puzzle2),
    (AdventOfCode.AdventOfCode2016.Day2.Day2.Puzzle1, AdventOfCode.AdventOfCode2016.Day2.Day2.Puzzle2),
    (AdventOfCode.AdventOfCode2016.Day3.Day3.Puzzle1, AdventOfCode.AdventOfCode2016.Day3.Day3.Puzzle2),
    (AdventOfCode.AdventOfCode2016.Day4.Day4.Puzzle1, AdventOfCode.AdventOfCode2016.Day4.Day4.Puzzle2),
    (AdventOfCode.AdventOfCode2016.Day6.Day6.Puzzle1, AdventOfCode.AdventOfCode2016.Day6.Day6.Puzzle2),
    //(AdventOfCode.AdventOfCode2016.Day3.Day3.Puzzle1, AdventOfCode.AdventOfCode2016.Day3.Day3.Puzzle1),

    // (AdventOfCode.AdventOfCode2021.Day1.Puzzle1, AdventOfCode.AdventOfCode2021.Day1.Puzzle2),
    // (Day2.Puzzle1,Day2.Puzzle2),
    // (Day4.Puzzle1,Day4.Puzzle2),
    // (Day10.Puzzle1,Day10.Puzzle2),
    // (Day16.Puzzle1,Day16.Puzzle2),
    // (Day17.Puzzle1,Day17.Puzzle2),

    // (AdventOfCode.AdventOfCode2022.Day1.Puzzle1, AdventOfCode.AdventOfCode2022.Day1.Puzzle2),
    // (AdventOfCode.AdventOfCode2022.Day5.Puzzle1, AdventOfCode.AdventOfCode2022.Day5.Puzzle2),
};



foreach (var (puzzle1, puzzle2) in puzzles)
{
    var declaringType = puzzle1.Method.DeclaringType!;
    var dayNumber = declaringType.Name.Substring("Day".Length).Pipe(int.Parse);
    var dayStr = $"{(dayNumber < 10 ? "0" : "")}{dayNumber}";
    var yearStr = declaringType.Namespace!.Split(".")[1][^4..];
    var input = Path.Combine(Common.ProjectFolderPath, $"AdventOfCode{yearStr}/Day{dayStr}.txt").Pipe(File.ReadAllText);
    Console.WriteLine($"{yearStr}/Day{dayStr}/Puzzle01:  {puzzle1(input)}");
    Console.WriteLine($"{yearStr}/Day{dayStr}/Puzzle02:  {puzzle2(input)}");
}