import java.lang.AssertionError
import java.util.*
import kotlin.math.abs
import kotlin.math.min
import kotlin.math.max

infix fun Any?.eq(obj2: Any?) {
    if (this != obj2) {
        throw AssertionError("'$this' <> '$obj2'")
    }
}

fun parseNumbers(text: String, separator: String = " ") =
    text.splitToSequence(separator).mapNotNull { if (it.isEmpty()) null else it.toInt() }

fun parseNumbersL(text: String, separator: String = " ") =
    text.splitToSequence(separator).mapNotNull { if (it.isEmpty()) null else it.toLong() }


val input =
    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2023/day11.txt")
        .readText()

// https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-seqmodule.html#mapFold

data class LList<T>(val head: T, val tail: LList<T>?)

fun <T> cons(head: T, tail: LList<T>?) = LList(head, tail) as LList<T>? // returns nullable type

fun <T> emptyLList(): LList<T>? = null

fun <T> llistToSequence(lst: LList<T>?): Sequence<T> = sequence {
    if (lst != null) {
        yield(lst.head)
        yieldAll(llistToSequence(lst.tail))
    }
}

fun <T> Sequence<T>.allUniquePairs() = sequence {
    val iterator = iterator()
    var processedItems = emptyLList<T>()

    while (iterator.hasNext()) {
        val current = iterator.next()
        yieldAll(llistToSequence(processedItems).map { Pair(current, it) })
        processedItems = LList(current, processedItems)
    }
}


// Pair<Int, List<Int>>

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

//    val pointsWithRowOffsetSeq = input.lineSequence().mapIndexed { row, line ->
//        Pair(row, line.mapIndexedNotNull { column, c -> if (c == '.') null else column })
//    }.scan(Triple(0, 0, listOf(0))) { (emptyRows, _), (row, columns) ->
//        Triple(if (columns.isEmpty()) emptyRows + 1 else emptyRows, row, columns)
//    }.drop(1).flatMap { (emptyRows, row, columns) ->
//        columns.map { Pair(emptyRows + row, it) }
//    }
//
//    val (columns, pointsWithRowOff) = pointsWithRowOffsetSeq.fold(
//        Pair(mutableSetOf<Int>(), mutableListOf<Point>())
//    ) { (columns, points), p ->
//        Pair(columns.apply { add(p.second) }, points.apply { add(p) })
//    }

//val x by bla;

//
//lines.mapfold
//
//lines.

//fun abc(string: String) {
//    string
//}


//
//
//enum class Direction { Left, Right, Top, Bottom }
//
//fun Direction.toOpposite() = when (this) {
//    Direction.Left -> Direction.Right
//    Direction.Right -> Direction.Left
//    Direction.Top -> Direction.Bottom
//    Direction.Bottom -> Direction.Top
//}
//
//val pipes: Map<Char, Map<Direction, Direction>> = sequenceOf(
//    Triple('-', Direction.Left, Direction.Right),
//    Triple('|', Direction.Top, Direction.Bottom),
//    Triple('7', Direction.Bottom, Direction.Left),
//    Triple('F', Direction.Bottom, Direction.Right),
//    Triple('J', Direction.Top, Direction.Left),
//    Triple('L', Direction.Top, Direction.Right),
//).map { (pipe, a, b) -> pipe to mapOf(a to b, b to a) }.toMap()
//
//
//data class Board(
//    val lines: List<String>,
//    val startingRow: Int,
//    val startingColumn: Int,
//    val width: Int,
//    val height: Int
//)
//
//fun loadData(input: String): Board {
//    val lines = input.lines()
//    val (row, column) = lines.asSequence().withIndex()
//        .firstNotNullOf { (i, l) -> l.indexOf('S').let { if (it == -1) null else Pair(i, it) } }
//    return Board(lines, row, column, lines[0].length, lines.size)
//}
//
//
//fun <T> Sequence<T>.expand(f: (T) -> Sequence<T>) = sequence {
//    val empty = emptySequence<T>()
//    var nextItems = this@expand
//
//    while (nextItems !== empty) { // referential comparison.
//        val items = nextItems
//        nextItems = empty
//        for (item in items) {
//            nextItems += f(item)
//            yield(item)
//        }
//    }
//}
//
//sequenceOf(1, 2, 3).expand { emptySequence<Int>() }.toList() eq listOf(1, 2, 3)
//emptySequence<Int>().expand { emptySequence<Int>() }.toList() eq listOf<Int>()
//sequenceOf(2).expand { if (it == 2) sequenceOf(1, 1) else emptySequence() }.toList() eq listOf(2, 1, 1)
//
//
//fun movePosition(direction: Direction, row: Int, column: Int) =
//    when (direction) {
//        Direction.Right -> Pair(row, column + 1)
//        Direction.Left -> Pair(row, column - 1)
//        Direction.Top -> Pair(row - 1, column)
//        Direction.Bottom -> Pair(row + 1, column)
//    }
//
//fun getAdjacentDirections(board: Board, row: Int, column: Int) =
//    sequence {
//        if (row > 0) yield(Direction.Top)
//        if (row < board.height - 1) yield(Direction.Bottom)
//        if (column > 0) yield(Direction.Left)
//        if (column < board.width - 1) yield(Direction.Right)
//    }
//
//fun getAdjacentPositions(board: Board, row: Int, column: Int) =
//    getAdjacentDirections(board, row, column).map { movePosition(it, row, column) }
//
//
//fun findFirstMove(data: Board) =
//    getAdjacentDirections(data, data.startingRow, data.startingColumn)
//        .firstNotNullOf { outputDirection ->
//            movePosition(outputDirection, data.startingRow, data.startingColumn)
//                .let { (row, column) ->
//                    val shape = data.lines[row][column]
//                    val inputDirection = outputDirection.toOpposite()
//                    if (shape in pipes && inputDirection in pipes[shape]!!)
//                        Triple(inputDirection, row, column)
//                    else
//                        null
//                }
//        }
//
//
//fun getPositionsOfWall(board: Board) =
//    sequenceOf(findFirstMove(board)).expand { (inputDirection, row, column) ->
//        val pipe = board.lines[row][column]
//        if (pipe != 'S') {
//            val outputDirection = pipes.getValue(pipe).getValue(inputDirection)
//            val (nextRow, nextColumn) = movePosition(outputDirection, row, column)
//            sequenceOf(Triple(outputDirection.toOpposite(), nextRow, nextColumn))
//        } else {
//            emptySequence()
//        }
//    }.map { (_, row, column) -> Pair(row, column) }
//
//
//fun expandPosition(value: Int) = 1 + (value * 2)
//
////fun expandBoard(data: Board, routeSet: Set<Pair<Int, Int>>): Board {
////    val width = (data.width * 2) + 2
////    val height = (data.height * 2) + 2
////    val startingColumn = expandPosition(data.startingColumn)
////    val startingRow = expandPosition(data.startingRow * 2)
////    val emptyLine = " ".repeat(width)
////    val lines = sequenceOf(emptyLine) +
////            data.lines.asSequence().flatMapIndexed { row, line ->
////                val firstLine = line.asSequence().flatMapIndexed { column, c ->
////                    if (Pair(row, column) in routeSet)
////                        sequenceOf('X', if (c == 'S' || Direction.Right in pipes.getValue(c)) 'X' else ' ')
////                    else
////                        sequenceOf(' ', ' ')
////                }.joinToString("")
////                val secondLine = line.asSequence().flatMapIndexed { column, c ->
////                    if (Pair(row, column) in routeSet)
////                        sequenceOf(if (c == 'S' || Direction.Bottom in pipes.getValue(c)) 'X' else ' ', ' ')
////                    else
////                        sequenceOf(' ', ' ')
////                }.joinToString("")
////                sequenceOf(" $firstLine ", " $secondLine ")
////            } + sequenceOf(emptyLine)
////    return Board(lines.toList(), startingRow, startingColumn, width, height)
////}
//
////sequenceOf('a', 'b', 'c').foldIndexed("") { i, s, c -> "$s - $i $c"}
//
//fun expandBoard(data: Board, routeSet: Set<Pair<Int, Int>>): Board {
//    val width = (data.width * 2) + 2
//    val height = (data.height * 2) + 2
//    val startingColumn = expandPosition(data.startingColumn)
//    val startingRow = expandPosition(data.startingRow * 2)
//    val emptyLine = " ".repeat(width)
//    val lines = sequenceOf(emptyLine) + data.lines.asSequence().flatMapIndexed { row, line ->
//        val (line1, line2) = line.asSequence()
//            .foldIndexed(Pair(sequenceOf(' '), sequenceOf(' '))) { column, (line1, line2), c ->
//                if (Pair(row, column) in routeSet)
//                    Pair(
//                        line1 + sequenceOf('X', if (c == 'S' || Direction.Right in pipes.getValue(c)) 'X' else ' '),
//                        line2 + sequenceOf(if (c == 'S' || Direction.Bottom in pipes.getValue(c)) 'X' else ' ', ' ')
//                    )
//                else
//                    Pair(line1 + sequenceOf(' ', ' '), line2 + sequenceOf(' ', ' '))
//            }
//        sequenceOf((line1 + ' ').joinToString(""), (line2 + ' ').joinToString(""))
//
//    } + sequenceOf(emptyLine)
//    return Board(lines.toList(), startingRow, startingColumn, width, height)
//}
//
//val input =
//    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2023/day10.txt")
//        .readText()
//
//
//fun puzzle1(input: String) = (getPositionsOfWall(loadData(input)).count() / 2).toString()
//
//
//fun enumerateAllPositions(board: Board) =
//    board.lines.flatMapIndexed { row, line ->
//        line.mapIndexedNotNull { column, c -> Triple(c, row, column) }
//    }
//
//fun findOuterPositions(board: Board): Set<Pair<Int, Int>> {
//    val wall = enumerateAllPositions(board).mapNotNull { (c, row, column) ->
//        if (c == ' ') null else Pair(row, column)
//    }.toSet()
//
//    val visited = mutableSetOf(Pair(0, 0))
//    val queue: Queue<Pair<Int, Int>> = LinkedList()
//    queue.add(Pair(0, 0))
//
//    while (queue.isNotEmpty()) { // breadth first search
//        val currentPosition = queue.poll()!!
//        val (row, column) = currentPosition
//
//        val next = getAdjacentPositions(board, row, column)
//            .filter { it !in wall && it !in visited }.toList()
//
//        queue.addAll(next)
//        visited.addAll(next)
//        // caution: "visited" means also those waiting in the queue, we don't want to duplicate items in queue
//    }
//
//    return visited
//}
//
//fun puzzle2(input: String): String {
//    val board = loadData(input)
//    val wall = getPositionsOfWall(board).toSet()
//    val expandedBoard = expandBoard(board, wall)
//    val outsidePositions = findOuterPositions(expandedBoard)
//
//    return enumerateAllPositions(board)
//        .count { (_, row, column) ->
//            Pair(row, column) !in wall && Pair(expandPosition(row), expandPosition(column)) !in outsidePositions
//        }.toString()
//}


//fun puzzle2(input: String): String {
//    val data = loadData(input)
//    val expandedData = expandBoard(data, findRouteSequence(data).toSet())
//
//    // expandedData.lines.joinToString(System.lineSeparator())
//
//    val (allDots, visited) = expandedData.lines.flatMapIndexed { row, line ->
//        line.mapIndexedNotNull { column, c ->
//            if (c == ' ') null else Pair(Pair(row, column), c)
//        }
//    }.partition { (_, c) -> c == '.' }
//        .let { (dots, pipes) -> Pair(dots.map { it.first }.toSet(), pipes.map { it.first }.toMutableSet()) }
//
//    var visitedDots = 0
//    val queue: Queue<Pair<Int, Int>> = LinkedList()
//    queue.add(Pair(0, 0))
//    visited.add(Pair(0, 0))
//
//    while (queue.isNotEmpty()) {
//        val currentPosition = queue.poll()!!
//        val (row, column) = currentPosition
//
//        if (currentPosition in allDots) {
//            visitedDots++
//        }
//
//        val next = getAdjacentPositions(expandedData, row, column).filter { it !in visited }.toList()
//        queue.addAll(next)
//        // caution: "visited" means also those waiting in the queue, we don't want to duplicate items in queue
//        visited.addAll(next)
//    }
//
//    return (allDots.size - visitedDots).toString()
//}


//fun findOuterBags(startingBag: String, bags: BagsMap): Set<String> {
//    val result = mutableSetOf<String>()
//    val queue: Queue<String> = LinkedList()
//    queue.add(startingBag)
//
//    while (queue.isNotEmpty()) {
//        bags[queue.poll()!!]?.let { outers ->
//            for (outer in outers.asSequence().mapNotNull { (outer, _) -> if (outer in result) null else outer }) {
//                result.add(outer)
//                queue.add(outer)
//            }
//        }
//    }
//
//    return result as Set<String>  // repl error: type mismatch: inferred type is MutableSet<String> but Set<String> was expected
//}


// .joinToString(System.lineSeparator())
//data.lines.asSequence().flatMapIndexed { row, line ->
//    val firstLine = line.asSequence().flatMapIndexed { column, c ->
//        if (Pair(row, column) in routeSet)
//            when (c) {
//                'S', '-', 'L', 'F' -> sequenceOf(c, '-')
//                else -> sequenceOf(c, ' ')
//            }
//        else
//            sequenceOf(' ', ' ')
//    }.joinToString("")
//    val secondLine = line.asSequence().flatMapIndexed { column, c ->
//        if (Pair(row, column) in routeSet)
//            when (c) {
//                'S', '|', 'F', '7' -> sequenceOf('|', ' ')
//                else -> sequenceOf(' ', ' ')
//            }
//        else
//            sequenceOf(' ', ' ')
//    }.joinToString("")
//    sequenceOf(firstLine, secondLine)
//}.joinToString(System.lineSeparator())


//(0..<data.height * 2).asSequence().flatMap { row ->
//    (0..<data.width * 2).map { Pair(row, it) }
//
//}.toList()


// .count()
//take(10).toList()

// 14132 / 7066

// That's not the right answer; your answer is too high.


//
//fun expandBoard(data: Data, routeSet: Set<Pair<Int, Int>>): Data {
//    val width = (data.width * 2) + 2
//    val height = (data.height * 2) + 2
//    val startingColumn = (data.startingColumn * 2) + 1
//    val startingRow = (data.startingRow * 2) + 1
//    val emptyLine = " ".repeat(width)
//    val lines = sequenceOf(emptyLine) +
//            data.lines.asSequence().flatMapIndexed { row, line ->
//                val firstLine = line.asSequence().flatMapIndexed { column, c ->
//                    if (Pair(row, column) in routeSet)
//                        sequenceOf('X', if (c == 'S' || Direction.Right in pipes.getValue(c)) 'X' else ' ')
//                    else
//                        sequenceOf(if (c == '.') '.' else 'X', ' ')
//                }.joinToString("")
//                val secondLine = line.asSequence().flatMapIndexed { column, c ->
//                    if (Pair(row, column) in routeSet)
//                        sequenceOf(if (c == 'S' || Direction.Bottom in pipes.getValue(c)) 'X' else ' ', ' ')
//                    else
//                        sequenceOf(' ', ' ')
//                }.joinToString("")
//                sequenceOf(" $firstLine ", " $secondLine ")
//            } + sequenceOf(emptyLine)
//    return Data(lines.toList(), startingRow, startingColumn, width, height)
//}