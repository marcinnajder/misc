package adventOfCode2023.day11_universe

import common.allUniquePairs
import kotlin.math.abs

typealias Point = Pair<Long, Long> // row, column

fun loadData(input: String, expansion: Long): List<Point> {
    val lines = input.lines()

    val (columns, pointsWithRowOffset) = lines.asSequence().mapIndexed { row, line ->
        Pair(row.toLong(), line.mapIndexedNotNull { column, c -> if (c == '.') null else column.toLong() }) // points
    }.scan(Triple(0L, 0L, listOf(0L))) { (emptyRows, _), (row, columns) ->
        Triple(if (columns.isEmpty()) emptyRows + expansion else emptyRows, row, columns)
    }.drop(1).flatMap { (emptyRows, row, columns) ->
        columns.map { Pair(emptyRows + row, it) } // points with row offset
    }.fold(Pair(mutableSetOf<Long>(), mutableListOf<Point>())) { (columns, points), p ->
        Pair(columns.apply { add(p.second) }, points.apply { add(p) }) // iterate only once
    }

    val columnOffsets = (0L..<lines[0].length).asSequence().scan(0L) { emptyColumns, column ->
        if (column in columns) emptyColumns else emptyColumns + expansion
    }.drop(1).toList()

    return pointsWithRowOffset.map { (row, column) -> Pair(row, column + columnOffsets[column.toInt()]) }
}

fun puzzle(input: String, expansion: Long) =
    loadData(input, expansion).asSequence().allUniquePairs().sumOf { (p1, p2) ->
        abs(p1.first - p2.first) + abs(p1.second - p2.second)
    }.toString()

fun puzzle1(input: String) = puzzle(input, 2 - 1)

fun puzzle2(input: String) = puzzle(input, 1000000 - 1)