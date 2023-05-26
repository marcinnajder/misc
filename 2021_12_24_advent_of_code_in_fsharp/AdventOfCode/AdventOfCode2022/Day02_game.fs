module AdventOfCode2022.Day02

open System

type Shape =
    | Rock
    | Paper
    | Scissors

let textToShapesMap =
    Map [ "A", Rock
          "B", Paper
          "C", Scissors
          "X", Rock
          "Y", Paper
          "Z", Scissors ]

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    lines
    |> Seq.map (fun line ->
        let parts = line.Split(' ')
        Map.find parts.[0] textToShapesMap, Map.find parts.[1] textToShapesMap)



let shapesToPointsMap = Map [ Rock, 1; Paper, 2; Scissors, 3 ]

let playersToPointMap =
    Map [ (Rock, Paper), 6
          (Rock, Scissors), 0
          (Paper, Scissors), 6
          (Paper, Rock), 0
          (Scissors, Rock), 6
          (Scissors, Paper), 0 ]

let Draw = Paper
let Loss = Rock
let Win = Scissors

let reverseWinnerMap =
    Map [ (Rock, Loss), (Rock, Scissors)
          (Rock, Win), (Rock, Paper)
          (Paper, Loss), (Paper, Rock)
          (Paper, Win), (Paper, Scissors)
          (Scissors, Loss), (Scissors, Paper)
          (Scissors, Win), (Scissors, Rock)

          (Rock, Draw), (Rock, Rock)
          (Paper, Draw), (Paper, Paper)
          (Scissors, Draw), (Scissors, Scissors) ]


let calculate players =
    let points = Map.tryFind players playersToPointMap |> Option.defaultValue 3
    points + (Map.find (snd players) shapesToPointsMap)

let puzzle input transformPlay = input |> loadData |> Seq.map transformPlay |> Seq.map calculate |> Seq.sum |> string

let puzzle1 input = puzzle input id

let puzzle2 input = puzzle input (fun players -> Map.find players reverseWinnerMap)
