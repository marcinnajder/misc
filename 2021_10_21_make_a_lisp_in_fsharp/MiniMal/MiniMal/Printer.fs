module Printer

open Types
open System

let rec printStr mal =
    match mal with
    | Nil -> "nil"
    | True -> "true"
    | False -> "false"
    | Str value -> $"\"%s{value}\""
    | Number value -> value.ToString()
    | Symbol name -> name
    | MalList (items, listType) ->
        let opening, closing =
            if listType = List then "(", ")" else "[", "]"
        let content =
            String.Join(" ", items |> List.map printStr)
        $"{opening}{content}{closing}"
    | MalMap value ->
        let content =
            String.Join(
                " ",
                value
                |> Map.toList
                |> List.map (fun (key, value) -> $"\"{key}\" {printStr value}")
            )
        $"{{%s{content}}}"
    | Fn _ -> "#<function>"


let joinWithSeparator mals separator =
    String.Join(
        (match separator with
         | None -> ""
         | Some sep -> sep),
        mals |> List.map printStr
    )
