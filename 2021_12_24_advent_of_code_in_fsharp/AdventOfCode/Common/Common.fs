module Common

open System
open System.IO
open System.Reflection

let ProjectFolderPath =
    Path.Combine [| FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName
                    ".."
                    ".."
                    ".." |]

let parseNumbers (separator: char) (strings: string) =
    strings.Split([| separator |], StringSplitOptions.RemoveEmptyEntries) |> Array.map Int32.Parse
