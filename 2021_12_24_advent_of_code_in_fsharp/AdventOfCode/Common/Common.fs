module Common

open System
open System.IO
open System.Linq
open System.Reflection
open System.Text.RegularExpressions

let ProjectFolderPath =
    Path.Combine [| FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName
                    ".."
                    ".."
                    ".." |]

let parseNumbers (separator: char) (strings: string) =
    strings.Split([| separator |], StringSplitOptions.RemoveEmptyEntries) |> Array.map Int32.Parse


let assertTrue value = if value then () else failwith "assert error"

let (===) actual expected = if actual = expected then () else failwithf "assertion failed: %A <> %A" actual expected

let matchesNumbers text =
    Regex.Matches(text, @"(\-?)\d+") |> Seq.cast<Match> |> Seq.map (fun m -> Int32.Parse(m.Value)) |> Seq.toArray


let matchesNumbers1 text =
    match matchesNumbers text with
    | [| a |] -> (a)
    | _ -> failwithf "wrong data format: %s" text

let matchesNumbers2 text =
    match matchesNumbers text with
    | [| a; b |] -> (a, b)
    | _ -> failwithf "wrong data format: %s" text

let matchesNumbers3 text =
    match matchesNumbers text with
    | [| a; b; c |] -> (a, b, c)
    | _ -> failwithf "wrong data format: %s" text

let matchesNumbers4 text =
    match matchesNumbers text with
    | [| a; b; c; d |] -> (a, b, c, d)
    | _ -> failwithf "wrong data format: %s" text
