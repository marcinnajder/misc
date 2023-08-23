namespace AdventOfCode.Tests

// open System
// open Microsoft.VisualStudio.TestTools.UnitTesting
// open AdventOfCode2021.Day22

// // dotnet watch test  -l "console;verbosity=detailed"

// [<TestClass>]
// type Day22Tests() =

//     [<TestMethod>]
//     member this.areRangesOverlapping() =
//         let range = { Min = 5; Max = 10 }
//         Assert.IsFalse(areRangesOverlapping range { Min = 1; Max = 2 })
//         Assert.IsFalse(areRangesOverlapping range { Min = 11; Max = 20 })
//         Assert.IsTrue(areRangesOverlapping range { Min = 10; Max = 20 })
//         Assert.IsTrue(areRangesOverlapping range { Min = 6; Max = 7 })
//         Assert.IsTrue(areRangesOverlapping range { Min = 2; Max = 5 })

//     [<TestMethod>]
//     member this.areCubesOverlapping() =
//         let cube1 = newCube 5 10 10 20 50 100
//         let cube2 = newCube 11 16 21 31 101 201
//         Assert.IsFalse(isCubeOverlapping cube1 cube2)
//         Assert.IsFalse(isCubeOverlapping cube1 { cube2 with X = cube1.X })
//         Assert.IsFalse(isCubeOverlapping cube1 { cube2 with X = cube1.X; Y = cube1.Y })
//         Assert.IsTrue(isCubeOverlapping cube1 { cube2 with X = cube1.X; Y = cube1.Y; Z = cube1.Z })

//     [<TestMethod>]
//     member this.isInsideRange() =
//         let range = { Min = 5; Max = 10 }
//         Assert.IsFalse(isInsideRange 1 range)
//         Assert.IsFalse(isInsideRange 15 range)
//         Assert.IsTrue(isInsideRange 5 range)
//         Assert.IsTrue(isInsideRange 7 range)
//         Assert.IsTrue(isInsideRange 10 range)

//     [<TestMethod>]
//     member this.isInsideCube() =
//         let cube = newCube 5 10 10 20 50 100
//         Assert.IsFalse(isInsideCube (4, 9, 49) cube)
//         Assert.IsFalse(isInsideCube (5, 9, 49) cube)
//         Assert.IsFalse(isInsideCube (5, 10, 49) cube)
//         Assert.IsTrue(isInsideCube (5, 10, 50) cube)
//         Assert.IsTrue(isInsideCube (6, 11, 51) cube)
//         Assert.IsFalse(isInsideCube (11, 20, 100) cube)

//     [<TestMethod>]
//     member this.findOnPoints() =
//         let onCube = newCube 1 2 5 7 10 10
//         let offCube1 = newCube 1 2 5 7 11 11
//         let offCube2 = newCube 2 2 5 7 10 10
//         Assert.AreEqual(6, findOnPoints onCube [] |> Seq.length)
//         Assert.AreEqual(6, findOnPoints onCube [ offCube1 ] |> Seq.length)
//         Assert.AreEqual(0, findOnPoints onCube [ onCube ] |> Seq.length)
//         Assert.AreEqual(3, findOnPoints onCube [ offCube1; offCube2 ] |> Seq.length)
// // printfn "%A" (findOnPoints onCube [ offCube1; offCube2 ])
