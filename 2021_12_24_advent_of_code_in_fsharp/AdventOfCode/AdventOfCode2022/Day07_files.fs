module AdventOfCode2022.Day07

open System
open Common

let loadData (input: string) = input.Split Environment.NewLine

let linesToListOfFoldersWithSizes (lines: string seq) =
    let result =
        ({| Cwd = []; CwdAsString = ""; Paths = Map.empty |}, lines)
        ||> Seq.fold (fun state line ->
            if line.StartsWith("dir") then
                state
            else if line = "$ ls" then
                {| state with CwdAsString = String.Join('/', List.rev state.Cwd) |}
            else if line.StartsWith("$ cd ") then
                let dirName = line.Substring("$ cd ".Length)
                let cwd = if dirName = ".." then List.tail state.Cwd else dirName :: state.Cwd
                {| state with Cwd = cwd |}
            else
                let fileSize = matchesNumbers1 line
                let paths =
                    state.Paths |> Map.change state.CwdAsString (Option.defaultValue 0 >> ((+) fileSize) >> Some)
                {| state with Paths = paths |})
    result.Paths

type Folder = { Size: int; Folders: Map<string, Folder> }

let rec updateFolder segments size folder =
    match segments with
    | [] -> { folder with Size = size }
    | segment :: restSegments ->
        let folders =
            folder.Folders
            |> Map.change segment (function
                | None -> Some(updateFolder restSegments size { Size = 0; Folders = Map.empty })
                | Some f -> Some(updateFolder restSegments size f))
        { folder with Folders = folders }

let listToTree paths =
    paths
    |> Map.toSeq
    |> Seq.fold
        (fun folder (path: string, size) ->
            let segments = path.Split([| "/" |], StringSplitOptions.RemoveEmptyEntries) |> Array.toList
            updateFolder segments size folder)
        { Size = 0; Folders = Map.empty }

let rec treeToListOfSizes result folder =
    let result', sum' =
        folder.Folders
        |> Map.toSeq
        |> Seq.map snd
        |> Seq.fold
            (fun (lst, sum) subfolder ->
                let lst' = treeToListOfSizes lst subfolder
                lst', sum + (List.head lst'))
            (result, 0)
    (folder.Size + sum') :: result'

let linesToListOfSizes lines = lines |> linesToListOfFoldersWithSizes |> listToTree |> treeToListOfSizes []


let puzzle1 input =
    input |> loadData |> linesToListOfSizes |> Seq.filter (fun size -> size <= 100000) |> Seq.sum |> string

let puzzle2 input =
    let sizes = input |> loadData |> linesToListOfSizes
    let unusedSpace = 70000000 - List.head sizes
    let missingFreeSpace = 30000000 - unusedSpace
    sizes |> Seq.sort |> Seq.find (fun size -> size > missingFreeSpace) |> string
