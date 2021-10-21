module Env

open Types

type Env(data: Map<string, MalType>, outer: Env option) =
    let mutable data = data
    let outer = outer

    let rec findEnvAndValue (env: Env) key =
        match Map.tryFind key env.Data with
        | Some (value) -> Some(env, value)
        | None ->
            env.Outer
            |> Option.bind (fun outerEnv -> findEnvAndValue outerEnv key)

    member private this.Data = data
    member private this.Outer = outer

    member this.Set key value =
        data <- Map.add key value data
        value

    member this.Find key =
        findEnvAndValue this key
        |> Option.map (fun (foundEnv, _) -> foundEnv)

    member this.Get key =
        match findEnvAndValue this key with
        | Some (_, foundMal) -> foundMal
        | None -> failwith $"Cannot find symbol '{key}' in Env"
