open System
open System.IO
open System.Linq
open FSharp.Reflection
open System.Reflection

open AdventOfCode2021

[<EntryPoint>]
let main argv =
    // let years, days  = { 2021..2021 }, { 10..10 }
    let years, days = { 2015..2021 }, { 1..25 }


    let allDayModuleNames =
        Seq.allPairs years (days |> Seq.map (fun n -> n.ToString().PadLeft(2, '0')))
        |> Seq.map (fun (year, day) -> $"AdventOfCode{year}.Day{day}")
        |> Set

    let existingDayModules =
        Assembly.GetExecutingAssembly().GetTypes()
        |> Seq.filter (fun t -> Set.contains t.FullName allDayModuleNames)
        |> Seq.sortBy (fun t -> t.FullName)

    for dayModule in existingDayModules do
        let input =
            Path.Combine(Common.ProjectFolderPath, dayModule.FullName.Replace('.', '/') + ".txt") |> File.ReadAllText
        for i in { 1..2 } do
            let method = dayModule.GetMethod("puzzle" + i.ToString())
            printfn "%s.%s:  %s" dayModule.FullName method.Name (method.Invoke(null, [| input |]).ToString())
    0
