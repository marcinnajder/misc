open System
open System.IO
open System.Linq
open FSharp.Reflection
open System.Reflection
open System.Diagnostics

open AdventOfCode2021

[<EntryPoint>]
let main argv =
    let years, days = { 2023..2023 }, { 1..22 }
    //let years, days = { 2015..2021 }, { 1..25 }


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
            let stopwatch = Stopwatch.StartNew()
            let result = (method.Invoke(null, [| input |]).ToString())
            let duration = stopwatch.ElapsedMilliseconds
            printfn "%s.%s:  %s (%d ms)" dayModule.FullName method.Name result duration
    0
