open System
open Env
open Core
open Eval

[<assembly: System.Runtime.CompilerServices.InternalsVisibleTo("MiniMal.Tests")>]
do ()

[<EntryPoint>]
let main argv =
    let env = Env(Core.ns, None)
    while true do
        try
            let inputText = Console.ReadLine()
            let malO = Reader.readText inputText

            malO
            |> Option.map (fun mal -> eval mal env)
            |> Option.map Printer.printStr
            |> Option.map (fun outputText -> Console.WriteLine(outputText))
            |> ignore

        with
        | ex -> Console.WriteLine("Error: " + ex.Message)
    0

//     EnvM.Env env = new EnvM.Env(Core.Ns, null);
//     env.Set(new Types.Symbol("eval"), new Types.Fn(Core.CreateEval(env)));
