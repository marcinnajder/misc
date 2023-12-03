import adventOfCode2020.*
import java.io.File

fun main(args: Array<String>) {
    val folderPath =
        "/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2020"
    val input = File("$folderPath/day09.txt").readText();
    println(adventOfCode2020.day09_numbers.puzzle1(input))
    println(adventOfCode2020.day09_numbers.puzzle2(input))
    // println("Program arguments: ${args.joinToString()}")
}