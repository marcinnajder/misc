namespace AdventOfCode2021

module Common =

    open System

    let ProjectFolderPath = "/Volumes/data/bitbucket/fsharp/net/AdventOfCode/"

    let parseNumbers (separator: char) (strings: string) =
        strings.Split([| separator |], StringSplitOptions.RemoveEmptyEntries) |> Array.map Int32.Parse
