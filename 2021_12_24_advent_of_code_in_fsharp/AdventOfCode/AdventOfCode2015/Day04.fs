module AdventOfCode2015.Day04

open System
open System.Security.Cryptography
open System.Text

let loadData (input: string) = input

let getHash (input: string) =
    use hashAlgorithm = MD5.Create()
    let hexs = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input)) |> Seq.map (fun b -> b.ToString("x2"))
    String.Concat(hexs)

let findNumber (hashPrefix: string) secret =
    { 1 .. Int32.MaxValue }
    |> Seq.map (fun i -> i, getHash (secret + i.ToString()))
    |> Seq.find (fun (i, hash) -> hash.StartsWith(hashPrefix))
    |> fst

let puzzle1 input = loadData input |> findNumber "00000" |> string

let puzzle2 input = loadData input |> findNumber "000000" |> string
