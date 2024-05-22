import adventOfCode2020.*
import java.io.File
import kotlin.math.pow
import kotlin.time.measureTime


fun main(args: Array<String>) {
    val folderPath =
        "/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/"
    val input = File("$folderPath/adventOfCode2023/day17.txt").readText();


//    val time = measureTime {
////        adventOfCode2023.day05_seeds.loadData(input)
////        println(adventOfCode2023.day17_graph.puzzle2(input))
//        println(adventOfCode2023.day17_graph.puzzle1(input))
//        println(adventOfCode2023.day17_graph.puzzle2(input))
//    }
//    print("time: $time")

    println(adventOfCode2023.day17_graph.puzzle1(input))
    println(adventOfCode2023.day17_graph.puzzle2(input))
    println("Program arguments: ${args.joinToString()}")
}