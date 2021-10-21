module Reader

open System
open System.Globalization
open System.Text.RegularExpressions
open Types
open Printer

let private numberFormat =
    NumberFormatInfo(NumberGroupSeparator = ".", NumberDecimalSeparator = ",")

let rec internal malsToKeyValuesPairs mals =
    match mals with
    | [] -> []
    | Str (key) :: value :: rest -> (key, value) :: malsToKeyValuesPairs rest
    | _ -> failwith $"Invalid Map '{joinWithSeparator mals None}', odd number of elements or key is not a 'Str' type"

let internal listToMap mals = MalMap(Map(malsToKeyValuesPairs mals))


type internal ReadResult =
    { Result: MalType option
      RestTokens: string list }


type internal ReadListResult =
    { Result: MalType list
      RestTokens: string list }


let rec internal readForm tokens : ReadResult =
    match tokens with
    | [] -> { Result = None; RestTokens = [] }
    | token :: restTokens ->
        match token with
        | "("
        | "[" ->
            let closing, listType =
                if token = "(" then ")", List else "]", Vector
            let result = readList restTokens closing
            { Result = Some(MalList(result.Result, listType))
              RestTokens = result.RestTokens }
        | "{" ->
            let result = readList restTokens "}"
            { Result = Some(listToMap result.Result)
              RestTokens = result.RestTokens }
        | atomToken ->
            { Result = Some(readAtom atomToken)
              RestTokens = restTokens }

and internal readList tokens endOfListToken : ReadListResult =
    match (readForm tokens) with
    | { Result = None } -> failwith $"List is not closed"
    | { Result = Some (Symbol (name))
        RestTokens = restTokens } when name = endOfListToken -> { Result = []; RestTokens = restTokens }
    | { Result = Some (mal)
        RestTokens = restTokens } ->
        let r = readList restTokens endOfListToken
        { r with Result = mal :: r.Result }

and internal readAtom token =
    match token with
    | "true" -> True
    | "false" -> False
    | "nil" -> Nil
    | number when
        let (success, x) =
            Double.TryParse(number, NumberStyles.Any, numberFormat)
        success
        ->
        let doubleValue =
            Double.Parse(token, NumberStyles.Any, numberFormat)
        Number(doubleValue)
    | str when str.[0] = '"' ->
        if str.Length > 1 && str.[str.Length - 1] = '"' then
            Str(str.[1..(str.Length - 2)])
        else
            failwith $"String value '{token}' in not closed"
    | symbolValue -> Symbol(symbolValue)


let private tokenize str =
    seq {
        let pattern =
            @"[\s ,]*(~@|[\[\]{}()'`~@]|""(?:[\\].|[^\\""])*""?|;.*|[^\s \[\]{}()'""`~@,;]*)"
        let regex = Regex(pattern)
        for m in regex.Matches(str) do
            let token = m.Groups.[1].Value
            if
                not (String.IsNullOrEmpty(token))
                && token.[0] <> ';'
            then
                yield token
    }

let readText text =
    tokenize text
    |> List.ofSeq
    |> readForm
    |> (fun r -> r.Result)
