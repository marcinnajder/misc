import java.awt.Paint
import java.lang.AssertionError


infix fun Any?.eq(obj2: Any?) {
    if (this != obj2) {
        throw AssertionError("'$this' <> '$obj2'")
    }
}

val input =
    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2020/day09.txt")
        .readText()
