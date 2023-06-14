module AdventOfCode2022.Day11

open System
open Common

type Monkey =
    { OpOperation: int64 -> int64 -> int64
      OpValue: int64 option
      IfTest: int64
      IfTrue: int
      IfFalse: int
      WorryLevels: int64 seq
      InspectsCount: int64 }


let loadData (input: string) =
    input.Split Environment.NewLine
    |> Seq.chunkBySize 7
    |> Seq.map (fun lines ->
        { OpOperation = if lines.[2].Contains("+") then (+) else (*)
          OpValue = matchesNumbers lines.[2] |> Seq.tryHead |> Option.map int64
          IfTest = matchesNumbers1 lines.[3]
          IfTrue = matchesNumbers1 lines.[4]
          IfFalse = matchesNumbers1 lines.[5]
          WorryLevels = matchesNumbers lines.[1] |> Array.map int64
          InspectsCount = 0 })
    |> Seq.indexed
    |> Map

let playMonkey levelReducer (monkeys: Map<int, Monkey>) index =
    let monkey = Map.find index monkeys
    let monkeys'', i =
        monkey.WorryLevels
        |> Seq.fold
            (fun (monkeys', i) level ->
                let level' = (monkey.OpOperation) level (Option.defaultValue level monkey.OpValue) |> levelReducer
                let idx = if level' % monkey.IfTest = 0L then monkey.IfTrue else monkey.IfFalse
                let state' =
                    Map.change
                        idx
                        (Option.map (fun m -> { m with WorryLevels = Seq.append m.WorryLevels [ level' ] }))
                        monkeys'
                state', (i + 1L))
            (monkeys, 0L)
    monkeys''
    |> Map.change index (Option.map (fun m -> { m with WorryLevels = Seq.empty; InspectsCount = m.InspectsCount + i }))


let playRound levelReducer (monkeys: Map<int, Monkey>) =
    Seq.fold (playMonkey levelReducer) monkeys { 0 .. Map.count monkeys - 1 }

let puzzle levelReducer monkeys numberOfRounds =
    { 0 .. numberOfRounds - 1 }
    |> Seq.fold (fun monkeys' _ -> playRound levelReducer monkeys') monkeys
    |> Seq.map (fun kv -> kv.Value.InspectsCount)
    |> Seq.sortByDescending id
    |> Seq.take 2
    |> Seq.reduce (*)


let puzzle1 input = puzzle (fun x -> x / 3L) (loadData input) 20 |> string

let puzzle2 input =
    let monkeys = loadData input
    let magicNumber = monkeys |> Seq.map (fun kv -> kv.Value.IfTest) |> Seq.reduce (*)
    puzzle (fun x -> x % magicNumber) monkeys 10000 |> string
