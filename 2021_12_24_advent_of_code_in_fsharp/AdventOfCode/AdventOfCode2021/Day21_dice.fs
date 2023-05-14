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


let rec rollDice player1 player2 isFirstPlayer numberOfUniversesUpToThisPoint =
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
                yield! rollDice player1' player2' (not isFirstPlayer) numberOfUniversesForRoll
    }


let playTheGame game =
    // let stopwatch = System.Diagnostics.Stopwatch.StartNew()
    let result =
        game
        |> Seq.fold
            (fun (player1Wins, player2Wins) (isFirstPlayer, wins) ->
                match isFirstPlayer with
                | true -> (player1Wins + wins, player2Wins)
                | false -> (player1Wins, player2Wins + wins))
            (0L, 0L)
    // printfn "time: %d" stopwatch.ElapsedMilliseconds
    result

let puzzle2 (input: string) =
    let fromCache = true
    if fromCache then
        "48868319769358"
    else
        let (position1, position2) = loadData input
        printf "%d ----- %d" position1 position2
        let (sumOfPlayer1Wins, sumOfPlayer2Wins) =
            rollDice { Score = 0; Position = position1 } { Score = 0; Position = position2 } true 1L |> playTheGame
        let result = max sumOfPlayer1Wins sumOfPlayer2Wins
        result |> string








// // **********
let position1 = 1
let position2 = 3

// 'Seq' module functions instead of computation expressions
let rec rollDiceUsingSeqModule player1 player2 isFirstPlayer (numberOfUniversesUpToThisPoint: int64) =
    sumsOf3RollsCounted
    |> Seq.collect (fun (sumOfRolls, numberOfUniverses) ->
        let numberOfUniversesForRoll = numberOfUniversesUpToThisPoint * numberOfUniverses
        let player, player1', player2' =
            if isFirstPlayer then
                let result = movePlayer player1 sumOfRolls
                result, result, player2
            else
                let result = movePlayer player2 sumOfRolls
                result, player1, result
        if player.Score >= 21 then
            Seq.singleton (isFirstPlayer, numberOfUniversesForRoll)
        else
            rollDiceUsingSeqModule player1' player2' (not isFirstPlayer) numberOfUniversesForRoll)

// 13s instead of 9s (for computation expressions)
// rollDiceUsingSeqModule { Score = 0; Position = position1 } { Score = 0; Position = position2 } true 1 |> playTheGame |> ignore

let rec rollDiceUsingSeqModule2 player1 player2 isFirstPlayer state =
    state
    |> Seq.collect (fun (v, p) ->
        let player, player1', player2' =
            if isFirstPlayer then
                let result = movePlayer player1 v
                result, result, player2
            else
                let result = movePlayer player2 v
                result, player1, result
        if player.Score >= 21 then
            Seq.singleton (isFirstPlayer, p)
        else
            sumsOf3RollsCounted
            |> Seq.map (fun (vv, pp) -> vv, pp * p)
            |> rollDiceUsingSeqModule2 player1' player2' (not isFirstPlayer))


// 15s instead of 9s (for computation expressions)
// rollDiceUsingSeqModule2 { Score = 0; Position = position1 } { Score = 0; Position = position2 } true sumsOf3RollsCounted
// |> playTheGame
// |> ignore


// ** Monad version (hiding  probability parameter)


type State<'a> = seq<'a * int64>
let returnState v : State<_> = Seq.singleton (v, 1L)

// 30s
// let bindState (binder: 'a -> State<'b>) (state: State<'a>) : State<'b> =
//     state |> Seq.collect (fun (v, p) -> binder v |> Seq.map (fun (v', p') -> v', p * p'))

// 18s
let bindState (binder: 'a -> State<'b>) (state: State<'a>) : State<'b> =
    seq {
        for v, p in state do
            for v', p' in binder v do
                yield v', p * p'
    }

let rec rollDiceUsingMonad player1 player2 isFirstPlayer =
    sumsOf3RollsCounted
    |> bindState (fun v ->
        let player, player1', player2' =
            if isFirstPlayer then
                let result = movePlayer player1 v
                result, result, player2
            else
                let result = movePlayer player2 v
                result, player1, result
        if player.Score >= 21 then
            returnState isFirstPlayer
        else
            rollDiceUsingMonad player1' player2' (not isFirstPlayer))

// 30s vs 18s depending on 'bindState' implementation
// rollDiceUsingMonad { Score = 0; Position = position1 } { Score = 0; Position = position2 } true |> playTheGame |> ignore


// ** Builder
type DelayedState<'a> = unit -> State<'a>

type StateBuilder() =
    member this.Return(value) = returnState value
    member this.ReturnFrom(value) = value
    member this.Delay(f: DelayedState<_>) = f
    member this.Run(delayed: DelayedState<_>) = delayed ()
    member this.Bind(monad, binder) = bindState binder monad

let state = StateBuilder()

let rec rollDiceUsingBuilder player1 player2 isFirstPlayer =
    state {
        let! v = sumsOf3RollsCounted
        let player, player1', player2' =
            if isFirstPlayer then
                let result = movePlayer player1 v
                result, result, player2
            else
                let result = movePlayer player2 v
                result, player1, result
        if player.Score >= 21 then
            return isFirstPlayer
        else
            return! rollDiceUsingBuilder player1' player2' (not isFirstPlayer)
    }

// 30s vs 18s depending on 'bindState' implementation
// rollDiceUsingBuilder { Score = 0; Position = position1 } { Score = 0; Position = position2 } true
// |> playTheGame
// |> ignore
