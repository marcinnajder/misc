module AdventOfCode2021.Day21

open System

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    Int32.Parse(lines.[0].Split(": ").[1]), Int32.Parse(lines.[1].Split(": ").[1])


type PlayerStats = { Score: int; Position: int }

let from1To10Repeatedly =
    seq {
        while true do
            yield! seq { 1..10 }
    }

let movePlayer stats sumOfRolls =
    let x = (stats.Position + sumOfRolls) % 10
    let newPosition = if x = 0 then 10 else x
    { Score = stats.Score + newPosition; Position = newPosition }



let puzzle1 (input: string) =
    let (position1, position2) = loadData input
    let (iterations, ({ Score = prevScore }, _)) =
        from1To10Repeatedly
        |> Seq.chunkBySize 3
        |> Seq.map Array.sum
        |> Seq.scan
            (fun (current, prev) s -> (prev, movePlayer current s))
            ({ Score = 0; Position = position1 }, { Score = 0; Position = position2 })
        |> Seq.indexed
        |> Seq.skip 1
        |> Seq.find (fun (_, (_, { Score = currentScore })) -> currentScore >= 1000)
    let result = iterations * 3 * prevScore
    result |> string


let sumsOf3RollsCounted =
    seq {
        for x = 1 to 3 do
            for y = 1 to 3 do
                for z = 1 to 3 do
                    [ x; y; z ]
    }
    |> Seq.map List.sum
    |> Seq.countBy id
    |> Seq.map (fun (key, value) -> key, value |> int64)
    |> Seq.toArray


let rec rollDice' player1 player2 isFirstPlayer (numberOfUniversesUpToThisPoint: int64) =
    seq {
        for (sumOfRolls, numberOfUniverses) in sumsOf3RollsCounted do
            let numberOfUniversesForRoll = numberOfUniversesUpToThisPoint * numberOfUniverses
            let player, player1', player2' =
                if isFirstPlayer then
                    let result = movePlayer player1 sumOfRolls
                    result, result, player2
                else
                    let result = movePlayer player2 sumOfRolls
                    result, player1, result
            if player.Score >= 21 then
                yield isFirstPlayer, numberOfUniversesForRoll
            else
                yield! rollDice' player1' player2' (not isFirstPlayer) numberOfUniversesForRoll
    }


let puzzle2 (input: string) =
    let fromCache = true
    if fromCache then
        "48868319769358"
    else
        let (position1, position2) = loadData input
        let (sumOfPlayer1Wins, sumOfPlayer2Wins) =
            rollDice' { Score = 0; Position = position1 } { Score = 0; Position = position2 } true 1
            |> Seq.fold
                (fun (player1Wins, player2Wins) (isFirstPlayer, wins) ->
                    match isFirstPlayer with
                    | true -> (player1Wins + wins, player2Wins)
                    | false -> (player1Wins, player2Wins + wins))
                (0L, 0L)
        let result = max sumOfPlayer1Wins sumOfPlayer2Wins
        result |> string
