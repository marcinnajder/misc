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

// https://clojuredocs.org/clojure.core/partition-by
fun <T, R> Sequence<T>.partitionBy(f: (T) -> R?) = sequence {
    var pack = mutableListOf<T>()

    val iterator = iterator()

    if (iterator.hasNext()) {
        var item = iterator.next()
        var prevPartitioner = f(item)
        pack.add(item)

        while (iterator.hasNext()) {
            item = iterator.next()
            val partitioner = f(item)
            val theSame = partitioner == prevPartitioner
            prevPartitioner = partitioner

            if (theSame) {
                pack.add(item)
            } else {
                yield(pack as List<T>)
                pack = mutableListOf(item)
            }
        }
    }

    if (pack.any()) {
        yield(pack as List<T>)
    }
}


data class LList<T>(val head: T, val tail: LList<T>?)

fun <T> cons(head: T, tail: LList<T>?) = LList(head, tail) as LList<T>? // returns nullable type

fun <T> emptyLList(): LList<T>? = null

//fun <T> llistToSequence(lst: LList<T>?) = sequence {
//    var node = lst
//    while (node != null) {
//        yield(node.head)
//        node = node.tail;
//    }
//}

fun <T> llistToSequence(lst: LList<T>?): Sequence<T> = sequence {
    if (lst != null) {
        yield(lst.head)
        yieldAll(llistToSequence(lst.tail))
    }
}

fun <T> llistOf(vararg items: T): LList<T>? {
    return items.foldRight(null as LList<T>?) { item, lst -> LList(item, lst) }
}

fun <T> llistOfIter(items: Iterable<T>): LList<T>? {
    fun next(iterator: Iterator<T>): LList<T>? =
        if (iterator.hasNext()) LList(iterator.next(), next(iterator)) else emptyLList()
    return next(items.iterator())
}

fun <T> llistOfSeq(items: Sequence<T>) = llistOfIter(items.asIterable())

fun <T> llistSize(lst: LList<T>?): Int = if (lst == null) 0 else 1 + llistSize(lst.tail)

// extensions
fun <T> LList<T>?.toSequence() = llistToSequence(this)
fun <T> LList<T>?.toIterable() = llistToSequence(this).asIterable()
fun <T> LList<T>?.size() = llistSize(this)
fun <T> Iterable<T>.toLList() = llistOfIter(this)
fun <T> Sequence<T>.toLList() = llistOfSeq(this)


operator fun IntRange.component1() = this.first
operator fun IntRange.component2() = this.last
fun IntRange.length() = this.last - this.first + 1

fun mergeLines(lines: LList<IntRange>?, line: IntRange): LList<IntRange> {
    return when {
        lines == null -> LList(line, null)
        else -> {
            val (fromL, toL) = line
            val (head, tail) = lines
            val (fromH, toH) = head
            when {
                toH + 1 < fromL -> LList(head, mergeLines(tail, line))
                toL + 1 < fromH -> LList(line, lines)
                else -> mergeLines(tail, min(fromH, fromL)..max(toH, toL))
            }
        }
    }
}

fun <T> Sequence<T>.expand(f: (T) -> Sequence<T>) = sequence {
    val empty = emptySequence<T>()
    var nextItems = this@expand

    while (nextItems !== empty) { // referential comparison.
        val items = nextItems
        nextItems = empty
        for (item in items) {
            nextItems += f(item)
            yield(item)
        }
    }
}


val input =
    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2023/day10.txt")
        .readText()


enum class Direction { Left, Right, Top, Bottom }

fun Direction.toOpposite() = when (this) {
    Direction.Left -> Direction.Right
    Direction.Right -> Direction.Left
    Direction.Top -> Direction.Bottom
    Direction.Bottom -> Direction.Top
}

val pipes: Map<Char, Map<Direction, Direction>> = sequenceOf(
    Triple('-', Direction.Left, Direction.Right),
    Triple('|', Direction.Top, Direction.Bottom),
    Triple('7', Direction.Bottom, Direction.Left),
    Triple('F', Direction.Bottom, Direction.Right),
    Triple('J', Direction.Top, Direction.Left),
    Triple('L', Direction.Top, Direction.Right),
).map { (pipe, a, b) -> pipe to mapOf(a to b, b to a) }.toMap()


data class Board(
    val lines: List<String>,
    val startingRow: Int,
    val startingColumn: Int,
    val width: Int,
    val height: Int
)

fun loadData(input: String): Board {
    val lines = input.lines()
    val (row, column) = lines.asSequence().withIndex()
        .firstNotNullOf { (i, l) -> l.indexOf('S').let { if (it == -1) null else Pair(i, it) } }
    return Board(lines, row, column, lines[0].length, lines.size)
}


fun movePosition(direction: Direction, row: Int, column: Int) =
    when (direction) {
        Direction.Right -> Pair(row, column + 1)
        Direction.Left -> Pair(row, column - 1)
        Direction.Top -> Pair(row - 1, column)
        Direction.Bottom -> Pair(row + 1, column)
    }

fun getAdjacentDirections(board: Board, row: Int, column: Int) =
    sequence {
        if (row > 0) yield(Direction.Top)
        if (row < board.height - 1) yield(Direction.Bottom)
        if (column > 0) yield(Direction.Left)
        if (column < board.width - 1) yield(Direction.Right)
    }

fun getAdjacentPositions(board: Board, row: Int, column: Int) =
    getAdjacentDirections(board, row, column).map { movePosition(it, row, column) }


fun findFirstMove(data: Board) =
    getAdjacentDirections(data, data.startingRow, data.startingColumn)
        .firstNotNullOf { outputDirection ->
            movePosition(outputDirection, data.startingRow, data.startingColumn)
                .let { (row, column) ->
                    val shape = data.lines[row][column]
                    val inputDirection = outputDirection.toOpposite()
                    if (shape in pipes && inputDirection in pipes[shape]!!)
                        Triple(inputDirection, row, column)
                    else
                        null
                }
        }


fun getPositionsOfWall(board: Board) =
    sequenceOf(findFirstMove(board)).expand { (inputDirection, row, column) ->
        val pipe = board.lines[row][column]
        if (pipe != 'S') {
            val outputDirection = pipes.getValue(pipe).getValue(inputDirection)
            val (nextRow, nextColumn) = movePosition(outputDirection, row, column)
            sequenceOf(Triple(outputDirection.toOpposite(), nextRow, nextColumn))
        } else {
            emptySequence()
        }
    }.map { (_, row, column) -> Pair(row, column) }

fun expandPosition(value: Int) = 1 + (value * 2)

fun expandBoard(data: Board, routeSet: Set<Pair<Int, Int>>): Board {
    val width = (data.width * 2) + 2
    val height = (data.height * 2) + 2
    val startingColumn = expandPosition(data.startingColumn)
    val startingRow = expandPosition(data.startingRow * 2)
    val emptyLine = " ".repeat(width)
    val lines = sequenceOf(emptyLine) +
            data.lines.asSequence().flatMapIndexed { row, line ->
                val firstLine = line.asSequence().flatMapIndexed { column, c ->
                    if (Pair(row, column) in routeSet)
                        sequenceOf('X', if (c == 'S' || Direction.Right in pipes.getValue(c)) 'X' else ' ')
                    else
                        sequenceOf(' ', ' ')
                }.joinToString("")
                val secondLine = line.asSequence().flatMapIndexed { column, c ->
                    if (Pair(row, column) in routeSet)
                        sequenceOf(if (c == 'S' || Direction.Bottom in pipes.getValue(c)) 'X' else ' ', ' ')
                    else
                        sequenceOf(' ', ' ')
                }.joinToString("")
                sequenceOf(" $firstLine ", " $secondLine ")
            } + sequenceOf(emptyLine)
    return Board(lines.toList(), startingRow, startingColumn, width, height)
}

fun enumerateAllPositions(board: Board) =
    board.lines.asSequence().flatMapIndexed { row, line ->
        line.asSequence().mapIndexed { column, c -> Triple(c, row, column) }
    }

/** BFS (breadth first search) */
fun findOuterPositions(board: Board): Set<Pair<Int, Int>> {
    val wall = enumerateAllPositions(board).mapNotNull { (c, row, column) ->
        if (c == ' ') null else Pair(row, column)
    }.toSet()

    val visited = mutableSetOf(Pair(0, 0))
    val queue: Queue<Pair<Int, Int>> = LinkedList()
    queue.add(Pair(0, 0))

    while (queue.isNotEmpty()) {
        val currentPosition = queue.poll()!!
        val (row, column) = currentPosition

        val next = getAdjacentPositions(board, row, column)
            .filter { it !in wall && it !in visited }.toList()

        queue.addAll(next)
        visited.addAll(next)
        // caution: "visited" means also those waiting in the queue, we don't want to duplicate items in queue
    }

    return visited
}


fun puzzle1(input: String) = (getPositionsOfWall(loadData(input)).count() / 2).toString()

fun puzzle2(input: String): String {
    val board = loadData(input)
    val wall = getPositionsOfWall(board).toSet()
    val expandedBoard = expandBoard(board, wall)
    val outsidePositions = findOuterPositions(expandedBoard)

    "." + expandedBoard.lines.joinToString("|") + "."


    for (line in expandedBoard.lines) {
        println(line)
        print(",")
    }

    return enumerateAllPositions(board)
        .count { (_, row, column) ->
            Pair(row, column) !in wall && Pair(expandPosition(row), expandPosition(column)) !in outsidePositions
        }.toString()
}


