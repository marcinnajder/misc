open System
open System.IO
open System.Text.RegularExpressions
open System.Xml.Linq

let slnFilePath = "\n\n...... put sln file path here ......\n\n"

type Project = { Path: string; ProjectRefs: string []; PackageRefs: string [] }

let adjustPath (path: string) =
    let segments = path.Split([| '/'; '\\' |], StringSplitOptions.RemoveEmptyEntries)
    Path.Combine segments

let joinPath folderPath filePath = Path.GetFullPath(Path.Combine(folderPath, adjustPath (filePath)))

let parseSolutionLine line =
    Regex.Matches(line, "\"(.+?)\"")
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Value.Trim('"'))
    |> Seq.tryPick (fun s -> if s.EndsWith(".fsproj") || s.EndsWith(".csproj") then Some s else None)

let parseSolutionFile (solutionFilePath: string) =
    let solutionFolderPath = Path.GetDirectoryName(solutionFilePath)
    File.ReadAllLines(solutionFilePath)
    |> Seq.choose parseSolutionLine
    |> Seq.map (joinPath solutionFolderPath)
    |> Seq.toArray

let parsePojectFile (projectFilePath: string) =
    let projectFolderPath = Path.GetDirectoryName(projectFilePath)
    let xmlRoot = XElement.Load(projectFilePath)
    let projectRefs =
        xmlRoot.Descendants("ProjectReference")
        |> Seq.map (fun xel -> xel.Attribute("Include").Value)
        |> Seq.map (joinPath projectFolderPath)
        |> Seq.toArray
    let packageRefs =
        xmlRoot.Descendants("PackageReference")
        |> Seq.map (fun xel -> String.Join("@", [ "Include"; "Version" ] |> Seq.map (fun a -> xel.Attribute(a).Value)))
        |> Seq.toArray
    { Path = projectFilePath; ProjectRefs = projectRefs; PackageRefs = packageRefs }

let orderProjectsByRefs projects =
    let rec loop projs paths =
        match projs with
        | [] -> []
        | _ ->
            let referenced, referencing =
                List.partition (fun p -> Seq.forall (fun n -> Set.contains n paths) p.ProjectRefs) projs
            referenced @ loop referencing (referenced |> Seq.fold (fun names' p -> Set.add p.Path names') paths)
    loop projects Set.empty


let joinStrings (sep: string) (items: seq<_>) = String.Join(sep, items)

let findPrefix (paths: string list) =
    match paths with
    | []
    | _ :: [] -> ""
    | firstPath :: restPaths ->
        let segments = firstPath.Split('.')
        { segments.Length - 1 .. -1 .. 1 }
        |> Seq.map (fun n -> segments |> Seq.take n |> joinStrings ".")
        |> Seq.tryFind (fun path -> restPaths |> Seq.forall (fun p -> p.Length <> path.Length && p.StartsWith(path)))
        |> Option.defaultValue ""

let projects = parseSolutionFile slnFilePath |> Seq.map parsePojectFile |> Seq.toList |> orderProjectsByRefs

let getProjectFullName (path: string) = Path.GetFileNameWithoutExtension(path)

let prefix = projects |> List.map (fun p -> getProjectFullName p.Path) |> findPrefix

let getProjectShortName (path: string) =
    if prefix = "" then getProjectFullName path else (getProjectFullName path).Substring(prefix.Length + 1)

let projectsContent =
    projects
    |> Seq.map (fun p ->
        $"""{getProjectFullName p.Path} - {p.ProjectRefs |> Seq.map getProjectShortName |> joinStrings ", "}""")
    |> joinStrings Environment.NewLine

let formatPackagesContent p =
    $"""- {getProjectFullName p.Path}
{p.PackageRefs |> Seq.map (fun p -> " - " + p) |> joinStrings Environment.NewLine}"""

let packagesContent =
    projects |> Seq.map formatPackagesContent |> joinStrings (Environment.NewLine + Environment.NewLine)

let content =
    $"""
Solution file: {slnFilePath}

-- Projects
{projectsContent}

-- Packages
{packagesContent}
"""

printfn "%s" content
