package adventOfCode2020.day01_sum

import common.*

import common.allUniquePairs
import common.allUniqueTriples
import common.allUniqueTuples

fun loadData(input: String) = input.lineSequence().map { it.toInt() }

fun puzzle1(input: String): String {
    val (a, b) = loadData(input).allUniquePairs().first { (a, b) -> a + b == 2020 }
    return (a * b).toString()
}

fun puzzle2(input: String): String {
    val (a, b, c) = loadData(input).toList().allUniqueTriples().first { (a, b, c) -> a + b + c == 2020 }
    return (a * b * c).toString()
}


// for n=3 and triple no 787322 is found (217+196+1607=2020) and the execution takes 2m 1.872156193s :) in F# 1s
fun puzzle(input: String, n: Int): String {
    val result = loadData(input).allUniqueTuples(n).first { lst -> lst.toSequence().sum() == 2020 }
        .toSequence().reduce { a, b -> a * b }
    return result.toString()
}

fun puzzle1b(input: String) = puzzle(input, 2)
fun puzzle2b(input: String) = puzzle(input, 3)

