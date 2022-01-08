namespace AdventOfCode2021

open System
open System.IO
open System.Reflection

module Common =

    let ProjectFolderPath =
        Path.Combine [| FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName
                        ".."
                        ".."
                        ".." |]

    let parseNumbers (separator: char) (strings: string) =
        strings.Split([| separator |], StringSplitOptions.RemoveEmptyEntries) |> Array.map Int32.Parse
