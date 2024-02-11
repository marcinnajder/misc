package adventOfCode2023.day03_board

import common.eq
import common.partitionBy
import common.isOverlapping

data class Line(val symbols: Map<Int, Char>, val numbers: Map<Int, String>)

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
        }.let { (symbols, numbers) -> Line(symbols.toMap(), numbers.toMap()) }


fun loadData(input: String) = input.lineSequence().map(::parseLine)

val emptyLine = Line(emptyMap(), emptyMap())

fun puzzle(input: String, findNumbers: (lines: List<Line>) -> Sequence<Int>) =
    (sequenceOf(emptyLine) + loadData(input) + sequenceOf(emptyLine)).windowed(3).flatMap(findNumbers).sum().toString()

fun puzzle1(input: String) =
    puzzle(input) { (prev, current, next) ->
        current.numbers.asSequence().mapNotNull { (indexN, number) ->
            val indexes = (indexN - 1)..(indexN + number.length)
            when {
                sequenceOf(indexes.first, indexes.last).any { it in current.symbols } -> number.toInt()
                indexes.any { it in prev.symbols } || indexes.any { it in next.symbols } -> number.toInt()
                else -> null
            }
        }
    }

fun puzzle2(input: String) =
    puzzle(input) { (prev, current, next) ->
        current.symbols.asSequence().mapNotNull { (indexS, symbol) ->
            if (symbol == '*') {
                val indexes = (indexS - 1)..(indexS + 1)
                val adjacentNumbers = sequenceOf(prev, current, next).flatMap { it.numbers.asSequence() }
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
    (2..4).isOverlapping(1..3) eq true

    (1..3).isOverlapping(0..4) eq true
    (0..4).isOverlapping(1..3) eq true

    (1..3).isOverlapping(3..5) eq true
    (3..5).isOverlapping(5..7) eq true

    (1..3).isOverlapping(4..5) eq false
    (4..5).isOverlapping(1..3) eq false
}
