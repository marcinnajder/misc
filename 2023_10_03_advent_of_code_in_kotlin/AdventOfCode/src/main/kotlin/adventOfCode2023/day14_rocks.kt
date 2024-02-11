package adventOfCode2023.day14_rocks

import common.partitionBy
import common.* // LList
import java.util.*

enum class Rock { Rounded, Hash }

typealias Board = Map<Int, Map<Int, Rock>>

data class Data(val board: Board, val rowCount: Int, val columnCount: Int)

fun loadData(input: String): Data {
    val lines = input.lines()
    val board = lines.asSequence().mapIndexed { rowIndex, row ->
        row.asSequence().mapIndexedNotNull { columnIndex, char ->
            if (char == '.') null else columnIndex to (if (char == 'O') Rock.Rounded else Rock.Hash)
        }.let { rowIndex to it.toMap() }
    }.toMap()
    return Data(board, lines.size, lines[0].length)
}


fun pushRocksNorth(data: Data) = sequence {
    val scraper = MutableList(data.columnCount) { Rock.Hash to -1 }
    for ((rowIndex, rowMap) in (0..<data.rowCount).asSequence()
        .mapNotNull { i -> data.board[i]?.let { m -> i to m } }) {
        for ((columnIndex, rock) in rowMap) {
            when (rock) {
                Rock.Hash -> {
                    scraper[columnIndex] = Rock.Hash to rowIndex
                    yield(Triple(rowIndex, columnIndex, Rock.Hash))
                }

                Rock.Rounded -> {
                    val newRowIndex = scraper[columnIndex].second + 1
                    scraper[columnIndex] = Rock.Rounded to newRowIndex
                    yield(Triple(newRowIndex, columnIndex, Rock.Rounded))
                }
            }
        }
    }
}

fun rocksToBoard(rocks: Sequence<Triple<Int, Int, Rock>>): Board =
    rocks
        .groupBy({ (rowIndex, _, _) -> rowIndex }, { (_, columnIndex, rock) -> columnIndex to rock })
        .mapValues { (_, values) -> values.toMap() }

fun calcLoad(data: Data) =
    data.board.asSequence().sumOf { (rowIndex, row) ->
        row.count { (_, rock) -> rock == Rock.Rounded } * (data.rowCount - rowIndex)
    }

fun puzzle1(input: String) =
    loadData(input)
        .let { data -> pushRocksNorth(data).let(::rocksToBoard).let { data.copy(board = it) } }
        .let(::calcLoad).toString()


fun pushNorthAndRotate90Degrees4Times(data: Data) =
    (1..4).fold(data) { state, _ ->
        pushRocksNorth(state).map { (rowIndex, columnIndex, rock) ->
            Triple(columnIndex, data.rowCount - rowIndex - 1, rock) // rotate 90 degrees to the left
        }.let { Data(rocksToBoard(it), data.columnCount, data.rowCount) } // swap counts of columns and rows
    }

fun <T> findPattern(items: List<T>, nextItem: T, reps: Int): Int? {
    if (reps < 1) {
        return null
    }

    val lastIndex = items.lastIndexOf(nextItem)
    if (lastIndex == -1) {
        return null
    }

    val lengthOfPattern = items.size - lastIndex
    if (items.size < lengthOfPattern * reps) {
        return null
    }

    return (lastIndex..<items.size).asSequence().map { i -> i to items[i] }
        .all { (i, item) -> (1..<reps).all { step -> items[i - step * lengthOfPattern] == item } }
        .let { if (it) lengthOfPattern else null }
}


fun puzzle2(input: String): String {
    val iterations = 1000000000
    val loads = mutableListOf<Int>()
    var data = loadData(input)

    for (index in 0..<iterations) {
        data = pushNorthAndRotate90Degrees4Times(data) // 4 moves = 1 cycle
        val load = calcLoad(data)
        val lengthOfPattern = findPattern(loads, load, 3)

        if (lengthOfPattern == null) {
            loads.add(load)
        } else {
            val offsetInsidePattern = (iterations - index - 1) % lengthOfPattern
            val value = loads[index - lengthOfPattern + offsetInsidePattern]
            return value.toString()
        }
    }

    return loads.last().toString()
}


fun tests() {
    findPattern(listOf(), 10, 1) eq null
    findPattern(listOf(10), 10, 0) eq null

    findPattern(listOf(10), 10, 1) eq 1
    findPattern(listOf(10), 10, 2) eq null
    findPattern(listOf(10, 10), 10, 3) eq null

    findPattern(listOf(10, 20, 30), 30, 1) eq 1
    findPattern(listOf(10, 20, 30), 20, 1) eq 2
    findPattern(listOf(10, 20, 30), 10, 1) eq 3
    findPattern(listOf(10, 20, 30), 40, 1) eq null

    findPattern(listOf(0, 10, 20, 30, 10, 20, 30), 10, 1) eq 3
    findPattern(listOf(0, 10, 20, 30, 10, 20, 30), 10, 2) eq 3
    findPattern(listOf(0, 10, 20, 30, 10, 20, 30), 10, 3) eq null
    findPattern(listOf( /*0, 10,*/ 20, 30, 10, 20, 30), 10, 2) eq null

    findPattern(listOf(0, 10, 200, 30, 10, 20, 30), 10, 1) eq 3
    findPattern(listOf(0, 10, 200, 30, 10, 20, 30), 10, 2) eq null
}

// different implementation of part 1 using immutable linked list

data class Column(val lines: LList<IntRange>?, val block: Int?)

fun sumUpColumn(column: Column) =
    column.lines.toSequence().flatMap { it }.sum()

fun puzzle11(input: String): String {
    val lines = input.lines()
    val columnCount = lines.size
    val rowCount = lines[0].length

    val initColumns = List(columnCount) { Column(null, rowCount + 1) }

    return lines.foldIndexed(initColumns) { rowIndex, columns, row ->
        val rowLevel = rowCount - rowIndex
        columns.mapIndexed { columnIndex, column ->
            when (row[columnIndex]) {
                'O' -> {
                    (if (column.block == null) (column.lines!!.head.first - 1) else (column.block - 1))
                        .let { Column(mergeLines(column.lines, it..it), null) }
                }

                '#' -> column.copy(block = rowLevel)
                else -> column
            }
        }
    }.sumOf(::sumUpColumn).toString()
}

typealias Board2 = SortedMap<Int, SortedMap<Int, Boolean>>

fun loadData2(input: String): Board2 =
    input.lineSequence().foldIndexed(sortedMapOf()) { rowIndex, rows, line ->
        line.foldIndexed(sortedMapOf<Int, Boolean>()) { columnIndex, row, char ->
            if (char == '.') row else row.apply { set(columnIndex, char == 'O') }
        }.let { row -> if (row.isEmpty()) rows else rows.apply { set(rowIndex, row) } }
    }
