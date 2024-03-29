import adventOfCode2020.*
import java.io.File
import kotlin.time.measureTime

fun main(args: Array<String>) {
    val folderPath =
        "/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/"
    val input = File("$folderPath/adventOfCode2023/day16.txt").readText();

//    val time = measureTime {
//        //adventOfCode2023.day05_seeds.loadData(input)
//        println(adventOfCode2023.day05_seeds.puzzle2(input))
//    }
//    print("time: $time")

    println(adventOfCode2023.day16_splitters.puzzle1(input))
    println(adventOfCode2023.day16_splitters.puzzle2(input))
    // println("Program arguments: ${args.joinToString()}")
}