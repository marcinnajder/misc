package adventOfCode2023.day09_triangle

import common.parseNumbers

fun loadData(input: String) =
    input.lineSequence().map { parseNumbers(it).toList() }

typealias SumUpFn = (List<Int>, Int) -> Int

fun processLine(numbers: List<Int>, sumUp: SumUpFn): Int {
    return sumUp(numbers, when {
        numbers.all { it == 0 } -> 0
        else -> numbers.asSequence().windowed(2).map { (prev, next) -> next - prev }
            .let { processLine(it.toList(), sumUp) }
    })
}

fun puzzle(input: String, sumUp: SumUpFn) =
    loadData(input).sumOf { processLine(it, sumUp) }.toString()

fun puzzle1(input: String) =
    puzzle(input) { numbers, result -> numbers.last() + result }

fun puzzle2(input: String) =
    puzzle(input) { numbers, result -> numbers.first() - result }

