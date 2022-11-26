open System.IO
open System

let folderPath = __SOURCE_DIRECTORY__

let parseFile filePath =
    let map =
        File.ReadLines(filePath)
        |> Seq.takeWhile (fun line -> line.StartsWith("//"))
        |> Seq.choose (fun line ->
            match line.IndexOf(":") with
            | -1 -> None
            | i -> Some(line.Substring(0, i).TrimStart('/').TrimStart(), line.Substring(i + 1).Trim()))
        |> Map
    let url = map |> Map.tryFind "url" |> Option.defaultValue ""
    let tags = (map |> Map.tryFind "tags" |> Option.defaultValue "").Split(',')
    let examples = map |> Map.tryFind "examples" |> Option.map (fun v -> $"`{v}`") |> Option.defaultValue ""
    {| Url = url; Tags = tags; Examples = examples |}

let lines, allTagsSeq =
    Directory.EnumerateFiles(folderPath)
    |> Seq.choose (fun filePath ->
        let fileName = Path.GetFileNameWithoutExtension(filePath)
        if fileName.Contains("_") && fileName.StartsWith("P") && not (fileName.EndsWith('_')) then
            Some(fileName, parseFile filePath)
        else
            None)
    |> Seq.sortBy fst
    |> Seq.mapi (fun i (name, info) ->
        ($"{i + 1}. [{name}]({info.Url}) {info.Examples} {String.Join(',', info.Tags)}{Environment.NewLine}"), info.Tags)
    |> Seq.mapFold (fun allTags (line, tags) -> (line, Seq.append allTags tags)) (Seq.empty)

let tagsString =
    allTagsSeq
    |> Seq.groupBy id
    |> Seq.map (fun (key, values) -> key, Seq.length values)
    |> Seq.sortByDescending snd
    |> Seq.map (fun (key, count) -> $"{key} ({count}) ")
    |> System.String.Concat


let docsContent =
    $"""
{tagsString}

{System.String.Concat lines}
"""

let docsFilePath = Path.Combine(folderPath, "docs.md")
File.WriteAllText(docsFilePath, docsContent)
printfn "wygenerowany został plik: %s" docsFilePath
