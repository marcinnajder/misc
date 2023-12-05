import adventOfCode2020.*
import java.io.File

fun main(args: Array<String>) {
    val folderPath =
        "/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/"
    val input = File("$folderPath/adventOfCode2023/day03.txt").readText();
    println(adventOfCode2023.day03_board.puzzle1(input))
    println(adventOfCode2023.day03_board.puzzle2(input))
    // println("Program arguments: ${args.joinToString()}")
}