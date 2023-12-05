package adventOfCode2023.day03_board

import common.eq
import common.partitionBy

typealias Line = Pair<Map<Int, Char>, Map<Int, String>>

fun parseLine(line: String): Line =
    line.asSequence().withIndex()
        .partitionBy { (i, c) -> if (c.isDigit()) ' ' else c }
        .fold(Pair(emptySequence<Pair<Int, Char>>(), emptySequence<Pair<Int, String>>()))
        { state, pack ->
            val (symbols, numbers) = state
            val (index, value) = pack[0]
            when {
                value == '.' -> state
                value.isDigit() -> Pair(symbols, numbers + Pair(index, pack.map { it.value }.joinToString("")))
                else -> Pair(symbols + (index to value), numbers)
            }
        }.let { (symbols, numbers) -> Pair(symbols.toMap(), numbers.toMap()) }


fun loadData(input: String) = input.lineSequence().map(::parseLine)

val emptyLine: Line = Pair(emptyMap(), emptyMap())

fun puzzle(input: String, findNumbers: (lines: List<Line>) -> Sequence<Int>) =
    (sequenceOf(emptyLine) + loadData(input) + sequenceOf(emptyLine)).windowed(3).flatMap(findNumbers).sum().toString()

fun puzzle1(input: String) =
    puzzle(input) { (prev, current, next) ->
        current.second.asSequence().mapNotNull { (indexN, number) ->
            val indexes = (indexN - 1)..(indexN + number.length)
            when {
                sequenceOf(indexes.first, indexes.last).any { it in current.first } -> number.toInt()
                indexes.any { it in prev.first } || indexes.any { it in next.first } -> number.toInt()
                else -> null
            }
        }
    }

fun IntProgression.isOverlapping(range: IntProgression) = this.any { it in range }

fun puzzle2(input: String) =
    puzzle(input) { lines ->
        lines[1].first.asSequence().mapNotNull { (indexS, symbol) ->
            if (symbol == '*') {
                val indexes = (indexS - 1)..(indexS + 1)
                val adjacentNumbers = lines.asSequence().flatMap { it.second.asSequence() }
                    .mapNotNull { (indexN, number) -> if (indexes.isOverlapping(indexN..<indexN + number.length)) number.toInt() else null }
                    .take(3).toList()
                if (adjacentNumbers.count() == 2) adjacentNumbers.reduce { a, b -> a * b } else null
            } else {
                null
            }
        }
    }


fun tests() {
    (1..3).isOverlapping(1..3) eq true
    (1..3).isOverlapping(2..4) eq true
    (1..3).isOverlapping(3..5) eq true
    (1..3).isOverlapping(4..5) eq false
}
