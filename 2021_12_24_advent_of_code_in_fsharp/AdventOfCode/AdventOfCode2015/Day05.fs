module AdventOfCode2015.Day05

open System

let vowels = Set "aeiou"

let forbidden =
    Set [ ('a', 'b')
          ('c', 'd')
          ('p', 'q')
          ('x', 'y') ]

let isTextNice1 (text: string) =
    let last =
        Seq.append text [ '-' ]
        |> Seq.pairwise
        |> Seq.scan
            (fun (vowelsCount, (cond1, cond2, cond3)) pair ->
                let c1, c2 = pair
                let vowelsCount2 = if vowels |> Set.contains c1 then vowelsCount + 1 else vowelsCount
                vowelsCount2, (cond1 || vowelsCount2 = 3, cond2 || c1 = c2, cond3 || forbidden |> Set.contains pair))
            (0, (false, false, false))
        |> Seq.skip 1
        |> Seq.map snd
        |> Seq.takeWhile (fun (_, _, cond3) -> cond3 = false)
        |> Seq.indexed
        |> Seq.tryLast
    match last with
    | None -> false
    | Some (index, (cond1, cond2, _)) -> cond1 && cond2 && (index = text.Length - 1)


let isTextNice2 (text: string) =
    Seq.append text [ '-' ]
    |> Seq.windowed 3
    |> Seq.scan
        (fun (set, (cond1, cond2) as state) chars ->
            let c1, c2, c3 = chars.[0], chars.[1], chars.[2]
            if c1 = c2 && c1 = c3 then
                state
            else
                let set2 = if cond1 then set else set |> Set.add (c1, c2)
                set2, (cond1 || set |> Set.contains (c1, c2), cond2 || (c1 = c3 && c1 <> c2)))
        (Set.empty<char * char>, (false, false))
    |> Seq.skip 1
    |> Seq.map snd
    |> Seq.contains (true, true)


let loadData (input: string) = input.Split Environment.NewLine

let puzzle input isTextNice = input |> loadData |> Seq.filter isTextNice |> Seq.length

let puzzle1 input = puzzle input isTextNice1 |> string

let puzzle2 input = puzzle input isTextNice2 |> string
