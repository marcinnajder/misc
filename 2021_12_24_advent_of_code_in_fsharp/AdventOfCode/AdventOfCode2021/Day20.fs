module AdventOfCode2021.Day20

open System

type Input = { ImageAlgorithm: string; Image: char [,] }

let loadData (input: string) =
    let lines = input.Split Environment.NewLine
    { ImageAlgorithm = lines.[0]; Image = lines |> Seq.skip 2 |> array2D }


let bitsToInt bits = Seq.fold (fun (p, r) bit -> (2 * p, (if bit = '.' then r else p + r))) (1, 0) bits |> snd

let getAdjacentBits expandingChar (image: _ [,]) (x, y) =
    let size = Array2D.length1 image
    seq {
        for yy = y + 1 downto y - 1 do
            for xx = x + 1 downto x - 1 do
                yield if xx < 0 || yy < 0 || xx >= size || yy >= size then expandingChar else image.[yy, xx]
    }

let enhanceImage expandingChar (imageAlgorithm: string) (image: char [,]) =
    let size = Array2D.length1 image
    let result = Array2D.create size size ' '
    Seq.allPairs { 0 .. size - 1 } { 0 .. size - 1 }
    |> Seq.map (fun (y, x) -> (y, x), getAdjacentBits expandingChar image (x, y) |> bitsToInt)
    |> Seq.iter (fun ((y, x), index) -> result.[y, x] <- imageAlgorithm.[index])
    result

let expandImage expandingChar (image: _ [,]) =
    let size = Array2D.length1 image
    let expandedBy = 1
    let newSize = size + expandedBy * 2
    Array2D.init
        newSize
        newSize
        (fun x y ->
            let xx, yy = x - expandedBy, y - expandedBy
            if xx < 0 || yy < 0 || xx >= size || yy >= size then expandingChar else image.[xx, yy])

let toSeq array2D =
    seq {
        let count1, count2 = Array2D.length1 array2D, Array2D.length2 array2D
        let b1, b2 = Array2D.base1 array2D, Array2D.base2 array2D
        for i = b1 to b1 + count1 - 1 do
            for j = b2 to b2 + count2 - 1 do
                yield array2D.[i, j]
    }

let countLitPixelsAfterNIteration n data =
    let (finalImage, _) =
        seq { 1 .. n }
        |> Seq.fold
            (fun (image, expandingChar) _ ->
                (image |> expandImage expandingChar |> enhanceImage expandingChar data.ImageAlgorithm,
                 data.ImageAlgorithm.[Seq.replicate 9 expandingChar |> bitsToInt]))
            (data.Image, '.')
    finalImage |> toSeq |> Seq.filter ((=) '#') |> Seq.length


let puzzle1 (input: string) = input |> loadData |> countLitPixelsAfterNIteration 2 |> string
let puzzle2 (input: string) = input |> loadData |> countLitPixelsAfterNIteration 50 |> string
