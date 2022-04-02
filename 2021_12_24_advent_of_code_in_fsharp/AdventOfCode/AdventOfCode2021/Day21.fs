module AdventOfCode2021.Day21

// // let input =
// //     System.IO.File.ReadAllText
// //         "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day20.txt"



// open System

// type Input = { ImageAlgorithm: string; Image: char [,] }

// let loadData (input: string) =
//     let lines = input.Split Environment.NewLine
//     { ImageAlgorithm = lines.[0]; Image = lines |> Seq.skip 2 |> array2D }


// let bitsToInt bits = Seq.fold (fun (p, r) bit -> (2 * p, (if bit = '.' then r else p + r))) (1, 0) bits |> snd


// let getAdjacentBits expandingChar (image: _ [,]) (x, y) =
//     let size = Array2D.length1 image
//     seq {
//         for yy = y + 1 downto y - 1 do
//             for xx = x + 1 downto x - 1 do
//                 yield if xx < 0 || yy < 0 || xx >= size || yy >= size then expandingChar else image.[yy, xx]
//     }

// let enhanceImage expandingChar (imageAlgorithm: string) (image: char [,]) =
//     let size = Array2D.length1 image
//     let result = Array2D.create size size ' '
//     Seq.allPairs { 0 .. size - 1 } { 0 .. size - 1 }
//     |> Seq.map (fun (y, x) -> (y, x), getAdjacentBits expandingChar image (x, y) |> bitsToInt)
//     |> Seq.iter (fun ((y, x), index) -> result.[y, x] <- imageAlgorithm.[index])
//     result


// let toSeq array2D =
//     seq {
//         let count1, count2 = Array2D.length1 array2D, Array2D.length2 array2D
//         let b1, b2 = Array2D.base1 array2D, Array2D.base2 array2D
//         for i = b1 to b1 + count1 - 1 do
//             for j = b2 to b2 + count2 - 1 do
//                 yield array2D.[i, j]
//     }

// let expandImage expandingChar (image: _ [,]) =
//     let size = Array2D.length1 image
//     let expandedBy = 1
//     let newSize = size + expandedBy * 2
//     Array2D.init
//         newSize
//         newSize
//         (fun x y ->
//             let xx, yy = x - expandedBy, y - expandedBy
//             if xx < 0 || yy < 0 || xx >= size || yy >= size then expandingChar else image.[xx, yy])



// // let a = Seq.replicate 9 '#' |> bitsToInt
// // data.ImageAlgorithm[a + 1]

// let input =
//     System.IO.File.ReadAllText
//         "/Volumes/data/github/misc/2021_12_24_advent_of_code_in_fsharp/AdventOfCode/AdventOfCode2021/Day20.txt"

// let data = loadData input
// let image = data.Image
// let imageAlgorithm = data.ImageAlgorithm

// // let firstChar = data.ImageAlgorithm.[Seq.replicate 9 '.' |> bitsToInt]
// // let lastChar = data.ImageAlgorithm.[Seq.replicate 9 firstChar |> bitsToInt]
// // let image2 = image |> expandImage firstChar |> enhanceImage firstChar data.ImageAlgorithm
// // let image3 = image2 |> expandImage lastChar |> enhanceImage lastChar data.ImageAlgorithm


// let (image3, _) =
//     seq { 1 .. 50 }
//     |> Seq.fold
//         (fun (image, expandingChar) _ ->
//             (image |> expandImage expandingChar |> enhanceImage expandingChar data.ImageAlgorithm,
//              data.ImageAlgorithm.[Seq.replicate 9 expandingChar |> bitsToInt]))
//         (data.Image, '.')

// let count = image3 |> toSeq |> Seq.filter ((=) '#') |> Seq.length

// // 5479

// // 5564
// // 5584 - :/
// // 6074 - :/
// // 5569 - :/
// // 5087
// // 5061 - is too low
// // 4905
// // 4905



// //open System



// // Array.CreateInstance
// //

// // let findLowPoints (data: int [,]) =
// //     seq {
// //         let xUpperBound = data.GetLength(0) - 1
// //         let yUpperBound = data.GetLength(1) - 1
// //         for x = 0 to xUpperBound do
// //             for y = 0 to yUpperBound do
// //                 let value = data.[x, y]
// //                 if (x = 0 || value < data.[x - 1, y])
// //                    && (x = xUpperBound || value < data.[x + 1, y])
// //                    && (y = 0 || value < data.[x, y - 1])
// //                    && (y = yUpperBound || value < data.[x, y + 1]) then
// //                     yield (value)
// //     }


// // let loadData (input: string) : Point [] [] =
// //     let lines = input.Split Environment.NewLine
// //     lines
// //     |> Seq.filter (fun line -> line.Length > 0)
// //     |> chunkBySeparator (fun line -> line.StartsWith("--- "))
// //     |> Seq.map (Array.map (fun line -> let parts = Common.parseNumbers ',' line in parts.[0], parts.[1], parts.[2]))
// //     |> Seq.toArray
