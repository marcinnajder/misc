module CodeSnippets.GetExternalGroupIdFromEnvMap

open Utils

// original Java implementation

// private Optional<String> getExternalGroupIdFromEnvMap(String internalGroupId){
// 	return getEnvValue() // Optional<String>
// 			.map(e -> Splitter.on(GROUPS_SEPARATOR).withKeyValueSeparator(GROUP_KEY_VALUE_SEPARATOR).split(e)) // Optional<Map<...>>
// 			.map(f -> f.entrySet().stream().filter(v -> internalGroupId.toString().equals(v.getValue())).map(Map.Entry::getValue)) // Optional<String<...>>
// 			.flatMap(Stream::findFirst);
// }


let splitEnvs (envs: string) = envs.Split([| ';'; '=' |]) |> Seq.chunkBySize 2 |> Seq.map (fun a -> a[0], a[1]) |> Map

let envs = Some "USER=mn;PWD=abc"

let getExternalGroupIdFromEnvMap1 internalGroupId =
    envs
    |> Option.map (fun e -> splitEnvs e)
    |> Option.map (fun m -> m |> Seq.filter (fun kv -> kv.Key = internalGroupId) |> Seq.map (fun kv -> kv.Value))
    |> Option.bind (Seq.tryHead)

let getExternalGroupIdFromEnvMap2 internalGroupId =
    envs
    |> Option.map (fun e -> splitEnvs e)
    |> Option.bind (fun m ->
        m |> Seq.filter (fun kv -> kv.Key = internalGroupId) |> Seq.map (fun kv -> kv.Value) |> Seq.tryHead)

let getExternalGroupIdFromEnvMap3 internalGroupId =
    envs
    |> Option.map (fun e -> splitEnvs e)
    |> Option.bind (fun m ->
        m |> Seq.choose (fun kv -> if kv.Key = internalGroupId then Some kv.Value else None) |> Seq.tryHead)

let getExternalGroupIdFromEnvMap4 internalGroupId =
    envs
    |> Option.map (fun e -> splitEnvs e)
    |> Option.bind (fun m -> m |> Seq.tryPick (fun kv -> if kv.Key = internalGroupId then Some kv.Value else None))

let getExternalGroupIdFromEnvMap5 internalGroupId =
    envs
    |> Option.map splitEnvs
    |> Option.bind (Seq.tryPick (fun kv -> if kv.Key = internalGroupId then Some kv.Value else None))

let getExternalGroupIdFromEnvMap6 internalGroupId =
    envs |> Option.bind (splitEnvs >> Seq.tryPick (fun kv -> if kv.Key = internalGroupId then Some kv.Value else None))


getExternalGroupIdFromEnvMap6 "USER" === Some "mn"
getExternalGroupIdFromEnvMap6 "USER_" === None
