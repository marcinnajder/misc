// module Generator

// dotnet fsi ./PuzzlesSummary/Generator.fsx

open System
open System.IO
open System

let miscFolderPath = Path.Join(__SOURCE_DIRECTORY__, "../..")
let eol = Environment.NewLine
let githubUrl = "https://github.com/marcinnajder/misc/tree/master"

type FolderSettings = { Name: String; Ext: string; Path: string }

let aocSettings =
    [ { Name = "F#"; Ext = "fs"; Path = "./2021_12_24_advent_of_code_in_fsharp/AdventOfCode" }
      { Name = "C#"; Ext = "cs"; Path = "./2022_04_23_advent_of_code_in_csharp/AdventOfCode" }
      { Name = "JS"; Ext = "js"; Path = "./2023_06_01_advent_of_code_in_javascript/src" }
      { Name = "Clojure"; Ext = "clj"; Path = "./2022_12_15_advent_of_code_in_clojure/src" }
      { Name = "Kotlin"
        Ext = "kt"
        Path = "./2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin" } ]

let langsOrder = aocSettings |> Seq.mapi (fun i lang -> lang.Name, i) |> Map

let join xs ys pred = xs |> Seq.choose (fun x -> ys |> Seq.tryPick (fun y -> if pred x y then Some(x, y) else None))

let joinStrings (separator: string) (xs: seq<_>) = String.Join(separator, xs)

let findFiles folderPath ext =
    Directory.GetFiles(folderPath, $"*.{ext}") |> Array.map (fun path -> {| Name = FileInfo(path).Name; Path = path |})

let tryExtractName (fileName: string) =
    let parts = (fileName.Split('.')[0]).Split([| '-'; '_' |])
    if parts.Length > 1 then Some($"{Seq.last parts}") else None

// **** advent of code ****

let years = [ 2015 .. DateTime.Now.Year ] |> List.map (fun year -> year.ToString())
let days = [ 1..25 ] |> List.map (fun day -> day.ToString().PadLeft(2, '0'))

type AocPuzzle = { Year: String; Day: string; Lang: FolderSettings; FilePath: string; FileName: string }

let findAocPuzzles (lang: FolderSettings) =
    let yearsPaths =
        Directory.GetDirectories(Path.Join(miscFolderPath, lang.Path))
        |> Array.map (fun path -> {| Name = DirectoryInfo(path).Name; Path = path |})
    join years yearsPaths (fun year yearPath -> yearPath.Name.Contains(year))
    |> Seq.collect (fun (year, yearPath) ->
        let daysPaths =
            findFiles yearPath.Path lang.Ext |> Seq.filter (fun file -> not (file.Name.EndsWith("_." + lang.Ext))) // remove commented files
        join days daysPaths (fun day dayPath -> dayPath.Name.Contains(day))

        |> Seq.map (fun (day, dayPath) ->
            { Year = year; Day = day; FilePath = dayPath.Path; Lang = lang; FileName = dayPath.Name }))

let aocPuzzlesByYear =
    aocSettings
    |> Seq.collect findAocPuzzles
    |> Seq.groupBy (fun p -> p.Year)
    |> Seq.map (fun (year, yearPuzzles) ->
        let puzzles =
            yearPuzzles
            |> Seq.groupBy (fun p -> p.Day)
            |> Seq.map (fun (day, dayPuzzles) ->
                let puzzles = dayPuzzles |> Seq.sortBy (fun p -> langsOrder[p.Lang.Name]) |> Seq.toArray
                day, puzzles)
            |> Seq.sortBy fst
            |> Seq.toArray
        year, puzzles)
    |> Seq.sortByDescending fst
    |> Seq.toArray


let formatAocPuzzle year ((day, dayPuzzles): string * AocPuzzle []) =
    let aocUrlFormat = "https://adventofcode.com/{0}/day/{1}"
    let puzzleName = dayPuzzles |> Seq.tryPick (fun p -> tryExtractName p.FileName) |> Option.defaultValue ""
    let langs =
        dayPuzzles
        |> Seq.map (fun p -> $"[{p.Lang.Name}]({githubUrl}{p.FilePath.Substring(p.FilePath.IndexOf(p.Lang.Path) + 1)})")
    $""" - [{day} {puzzleName}]({String.Format(aocUrlFormat, year, Int32.Parse(day))}) - {joinStrings ", " langs}"""



let aocPuzzlesContent =
    aocPuzzlesByYear
    |> Seq.map (fun (year, yearPuzzles) ->
        let puzzlesStr = yearPuzzles |> Seq.map (fun ps -> formatAocPuzzle year ps) |> joinStrings eol
        $"{eol}#### Advent Of Code {year}{eol}{eol}{puzzlesStr}")
    |> joinStrings eol


// **** leet code ****

let leetCodeLangs =
    [ { Name = "F#"; Ext = "fs"; Path = "./2022_11_03_leetcode_in_fsharp/LeetCode" }
      { Name = "C#"; Ext = "cs"; Path = "./2022_11_19_leetcode_in_csharp/LeetCode" }
      { Name = "JS"; Ext = "js"; Path = "./2022_10_22_leetcode_in_javascript" } ]

let tryExtractNumber (fileName: string) =
    let result =
        fileName
        |> Seq.choose (fun c -> if Char.IsDigit c then Some(c.ToString()) else None)
        |> joinStrings ""
        |> Int32.TryParse
    match result with
    | true, value -> Some(value)
    | false, _ -> None

type LeetCodePuzzle = { Lang: FolderSettings; FilePath: string; FileName: string; Number: int }

let findLeetCodePuzzles (lang: FolderSettings) =
    findFiles (Path.Join(miscFolderPath, lang.Path)) lang.Ext
    |> Seq.filter (fun file -> not (file.Name.EndsWith("_." + lang.Ext))) // remove commented files
    |> Seq.choose (fun file -> tryExtractNumber file.Name |> Option.map (fun number -> file, number))
    |> Seq.map (fun (file, number) -> { Lang = lang; FileName = file.Name; FilePath = file.Path; Number = number })

let leetCodePuzzles =
    leetCodeLangs
    |> Seq.collect findLeetCodePuzzles
    |> Seq.groupBy (fun p -> p.Number)
    |> Seq.map (fun (number, puzzles) -> number, puzzles |> Seq.sortBy (fun p -> langsOrder[p.Lang.Name]) |> Seq.toArray)
    |> Seq.sortBy fst
    |> Seq.toArray


let formatLeetCodePuzzle number (puzzles: LeetCodePuzzle []) =
    let puzzleName = puzzles |> Seq.tryPick (fun p -> tryExtractName p.FileName) |> Option.defaultValue ""
    let url =
        puzzles
        |> Seq.tryPick (fun p ->
            if p.Lang.Name = "F#" then
                File.ReadAllLines(p.FilePath)
                |> Seq.tryHead
                |> Option.bind (fun line -> if line.Contains("url:") then Some((line.Split("url:")[1]).Trim()) else None)
            else
                None)
        |> Option.defaultValue "https://leetcode.com/problems"
    let langs =
        puzzles
        |> Seq.map (fun p -> $"[{p.Lang.Name}]({githubUrl}{p.FilePath.Substring(p.FilePath.IndexOf(p.Lang.Path) + 1)})")
    $""" - [{number} {puzzleName}]({url}) - {joinStrings ", " langs}"""


let leetCodePuzzlesContent =
    let content =
        leetCodePuzzles |> Seq.map (fun (number, puzzles) -> formatLeetCodePuzzle number puzzles) |> joinStrings eol
    $"{eol}#### Leet Code{eol}{content}"

// **** ****

let fullContent =
    $"""---
layout: page
title: puzzles
# permalink: /puzzles/
---

{aocPuzzlesContent}{eol}{eol}{leetCodePuzzlesContent}"""

let filePath = Path.Join(miscFolderPath, "../marcinnajder.github.io/puzzles.markdown")
File.WriteAllText(filePath, fullContent)
printfn "nadpisano plik: %s" filePath
