module AdventOfCode2021.Day16

open System

let rec bin n =
    if n = 1 then
        [ [ '0' ]; [ '1' ] ] :> seq<_>
    else
        [ '0'; '1' ] |> Seq.collect (fun h -> bin (n - 1) |> Seq.map (fun t -> h :: t))

let bitsToInt64 bits = Seq.foldBack (fun bit (p, r) -> (2L * p, (if bit = '0' then r else p + r))) bits (1L, 0L) |> snd

let bitsToInt bits = bits |> bitsToInt64 |> int

let mapping = bin 4 |> Seq.map (fun bits -> Convert.ToString(bitsToInt bits, 16).[0] |> Char.ToUpper, bits) |> Map

let loadData (input: string) = input |> Seq.collect (fun c -> mapping.[c])

// let loadData' (input: string) =
//     input |> Seq.collect (fun c -> Convert.ToString(Convert.ToInt32(c |> string, 16), 2).PadLeft(4, '0'))
// loadData "D2FE28" |> Seq.toArray |> String


type Package =
    | ValueLiteral of version: int * typeId: int * value: int64
    | Operator of version: int * typeId: int * packages: Package list

let shareSequence (source: seq<char>) =
    let enumerator = source.GetEnumerator()
    seq {
        while enumerator.MoveNext() do
            enumerator.Current
    }

let readValue reader =
    seq {
        let mutable finish = false
        while not finish do
            let bit = reader |> Seq.head
            yield! reader |> Seq.take 4
            finish <- bit = '0'
    }
    |> bitsToInt64


let rec readOperator reader =
    let lengthTypeId = reader |> Seq.head
    if lengthTypeId = '0' then
        let bitsCount = reader |> Seq.take 15 |> bitsToInt
        let newReader = reader |> Seq.take bitsCount |> shareSequence // shareSequence !!!
        newReader |> readManyPackages |> Seq.toList
    else
        let packageCount = reader |> Seq.take 11 |> bitsToInt
        reader |> readManyPackages |> Seq.take packageCount |> Seq.toList

and tryReadPackage reader =
    let first3Bits = reader |> Seq.truncate 3 |> ResizeArray
    if first3Bits.Count = 3 then
        let version = first3Bits |> bitsToInt
        let typeId = reader |> Seq.take 3 |> bitsToInt
        if typeId = 4 then
            let value = readValue reader
            ValueLiteral(version, typeId, value) |> Some
        else
            let packages = readOperator reader
            Operator(version, typeId, packages) |> Some
    else
        None

and readManyPackages reader =
    seq {
        match (tryReadPackage reader) with
        | Some package ->
            yield package
            yield! readManyPackages reader
        | None -> ()
    }

let rec sumVersions packages =
    match packages with
    | ValueLiteral (version, _, _) -> version
    | Operator (version, _, packages) -> version + (packages |> Seq.sumBy sumVersions)




let rec calculate package =
    match package with
    | ValueLiteral (_, _, value) -> value
    | Operator (_, typeId, packages) ->
        let values = packages |> Seq.map calculate
        match typeId with
        | 0 -> values |> Seq.sum
        | 1 -> values |> Seq.fold (*) 1L
        | 2 -> values |> Seq.fold min Int64.MaxValue
        | 3 -> values |> Seq.fold max Int64.MinValue
        | 5 -> values |> Seq.pairwise |> Seq.head ||> (>) |> (fun g -> if g then 1L else 0L)
        | 6 -> values |> Seq.pairwise |> Seq.head ||> (<) |> (fun l -> if l then 1L else 0L)
        | 7 -> values |> Seq.pairwise |> Seq.head ||> (=) |> (fun e -> if e then 1L else 0L)
        | _ -> failwith $"Unknown package typeId={typeId}"


let puzzle (input: string) calculateResult =
    let data = loadData input
    let reader = data |> shareSequence
    let result = reader |> tryReadPackage |> Option.get |> calculateResult
    result |> string

let puzzle1 (input: string) = puzzle input sumVersions
let puzzle2 (input: string) = puzzle input calculate
