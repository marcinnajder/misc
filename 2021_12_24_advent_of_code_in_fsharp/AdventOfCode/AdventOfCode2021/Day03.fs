module AdventOfCode2021.Day3

open System


let loadData (input: string) = input.Split Environment.NewLine

let binaryStringToIntArray str = str |> Seq.map (fun c -> if c = '1' then 1 else -1) |> Seq.toArray

let puzzle1 (input: string) =
    let binaryStrings = loadData input
    let sum =
        binaryStrings
        |> Seq.map binaryStringToIntArray
        |> Seq.reduce (fun p c -> Seq.zip p c |> Seq.map (fun (x, y) -> x + y) |> Seq.toArray)
    let bitsToNumber f = Convert.ToInt32(sum |> Array.map (fun n -> if f n then '1' else '0') |> String, 2)
    let gamma = bitsToNumber ((>) 0)
    let epsilon = bitsToNumber ((<) 0)
    let result = gamma * epsilon
    result |> string

let puzzle2 (input: string) =
    let mapTuple f (a, b) = f a, f b
    let binaryStrings = loadData input
    let rec sieve (items: string []) index bit =
        let groupedByBit = items |> Array.groupBy (fun binaryString -> binaryString.[index] = '1') |> Map.ofArray
        let oneItems, zeroItems =
            (true, false) |> mapTuple (fun x -> groupedByBit |> Map.tryFind x |> Option.defaultValue [||])
        let foundStrings =
            if oneItems.Length = zeroItems.Length then
                if bit then oneItems else zeroItems
            else if oneItems.Length > zeroItems.Length = bit then
                oneItems
            else
                zeroItems
        if foundStrings.Length = 1 then Array.head foundStrings else sieve foundStrings (index + 1) bit
    let oxygen = Convert.ToInt32(sieve binaryStrings 0 true, 2)
    let co2 = Convert.ToInt32(sieve binaryStrings 0 false, 2)
    let result = oxygen * co2
    result |> string
