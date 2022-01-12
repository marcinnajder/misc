open System
open System.IO
open AdventOfCode2021

[<EntryPoint>]
let main argv =
    let puzzles =
        [ (Day1.puzzle1, Day1.puzzle2)
          (Day2.puzzle1, Day2.puzzle2)
          (Day3.puzzle1, Day3.puzzle2)
          (Day4.puzzle1, Day4.puzzle2)
          (Day5.puzzle1, Day5.puzzle2)
          (Day6.puzzle1, Day6.puzzle2)
          (Day7.puzzle1, Day7.puzzle2)
          (Day8.puzzle1, Day8.puzzle2)
          (Day9.puzzle1, Day9.puzzle2)
          (Day10.puzzle1, Day10.puzzle2)
          (Day11.puzzle1, Day11.puzzle2)
          (Day12.puzzle1, Day12.puzzle2) ]

    puzzles
    |> Seq.iteri
        (fun i puzzle ->
            let day = i + 1
            let dayStr = $"""{if day < 10 then "0" else ""}{day}"""
            let input = Path.Combine(Common.ProjectFolderPath, $"AdventOfCode2021/Day{dayStr}.txt") |> File.ReadAllText
            let puzle1, puzle2 = puzzle
            printfn "2021/Day%s/Puzzle01:  %s" dayStr (puzle1 input)
            printfn "2021/Day%s/Puzzle02:  %s" dayStr (puzle2 input)
            ())
    0
