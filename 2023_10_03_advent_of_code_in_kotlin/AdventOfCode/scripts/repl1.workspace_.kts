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


val input =
    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2023/day16.txt")
        .readText()

enum class Direction { Left, Right, Top, Bottom }

data class Position(val row: Int, val column: Int)

data class Move(val fromDirection: Direction, val toPosition: Position)

data class Board(val lines: List<String>, val rowCount: Int, val columnCount: Int)

fun loadData(input: String) =
    input.lines().let { Board(it, it.size, it[0].length) }

// loadData(input)


fun Direction.isLeftOrRight() = this == Direction.Left || this == Direction.Right

fun Direction.toOpposite() = when (this) {
    Direction.Left -> Direction.Right
    Direction.Right -> Direction.Left
    Direction.Bottom -> Direction.Top
    Direction.Top -> Direction.Bottom
}


fun Direction.toSlashReflected() = when (this) {
    Direction.Left -> Direction.Top
    Direction.Top -> Direction.Left
    Direction.Right -> Direction.Bottom
    Direction.Bottom -> Direction.Right
}

fun Move.moveToDirection(direction: Direction) =
    this.toPosition.let { (row, column) ->
        Move(
            direction.toOpposite(), when (direction) {
                Direction.Right -> this.toPosition.copy(column = column + 1)
                Direction.Left -> this.toPosition.copy(column = column - 1)
                Direction.Bottom -> this.toPosition.copy(row = row + 1)
                Direction.Top -> this.toPosition.copy(row = row - 1)
            }
        )

    }

fun Move.moveForward() = this.moveToDirection(this.fromDirection.toOpposite())


fun isMoveInsideBoard(move: Move, board: Board) =
    when (move.fromDirection) {
        Direction.Left -> move.toPosition.column < board.columnCount
        Direction.Right -> move.toPosition.column >= 0
        Direction.Top -> move.toPosition.row < board.rowCount
        Direction.Bottom -> move.toPosition.row >= 0
    }

fun executeMove(move: Move, board: Board) = sequence {
    if (isMoveInsideBoard(move, board)) {
        // println("executeMove[ $move ]")
        when (board.lines[move.toPosition.row][move.toPosition.column]) {
            '|' ->
                if (move.fromDirection.isLeftOrRight())
                    yieldAll(sequenceOf(Direction.Top, Direction.Bottom).map { move.moveToDirection(it) })
                else
                    yield(move.moveForward())

            '-' ->
                if (move.fromDirection.isLeftOrRight())
                    yield(move.moveForward())
                else
                    yieldAll(sequenceOf(Direction.Left, Direction.Right).map { move.moveToDirection(it) })

            '/' -> yield(move.moveToDirection(move.fromDirection.toSlashReflected()))
            '\\' -> yield(move.moveToDirection(move.fromDirection.toSlashReflected().toOpposite()))

            else -> yield(move.moveForward())
        }
    }
}.filter { m -> isMoveInsideBoard(m, board) }


/** BFS (breadth first search) */
fun findOuterPositions(firstMove: Move, board: Board): Int {
    val visitedMoves = mutableSetOf(firstMove)
    val queue: Queue<Move> = LinkedList()
    queue.add(firstMove)

    while (queue.isNotEmpty()) {
        val currentMove = queue.poll()!!

        val nextMoves = executeMove(currentMove, board).filter { it !in visitedMoves }.toList()

        queue.addAll(nextMoves)
        visitedMoves.addAll(nextMoves)
        // caution: "visited" means also those waiting in the queue, we don't want to duplicate items in queue
    }

    return mutableSetOf<Position>().let { visitedMoves.mapTo(it) { m -> m.toPosition } }.size
}

fun puzzle1(input: String) =
    loadData(input).let { board -> findOuterPositions(Move(Direction.Left, Position(0, 0)), board) }.toString()


fun puzzle2(input: String) =
    loadData(input).let { board ->
        sequenceOf(
            Triple(Direction.Left, 0..<board.rowCount, 0..0),
            Triple(Direction.Right, 0..<board.rowCount, board.columnCount - 1..<board.columnCount),
            Triple(Direction.Top, 0..0, 0..<board.columnCount),
            Triple(Direction.Bottom, board.rowCount - 1..<board.rowCount, 0..<board.columnCount)
        ).flatMap { (direction, rows, columns) ->
            sequence {
                for (row in rows) {
                    for (column in columns) {
                        yield(Move(direction, Position(row, column)))
                    }
                }
            }
        }.maxOf { findOuterPositions(it, board) }.toString()
    }


//val allll =
//    (0..<myboard.rowCount).asSequence().map { Move(Direction.Left, Position(it, 0)) } +
//            (myboard.rowCount - 1 downTo 0).asSequence()
//                .map { Move(Direction.Right, Position(it, myboard.columnCount - 1)) } +
//
//            // (10 downTo 0).toList()
//
//            (0..<myboard.rowCount)
//                .asSequence()
//                .map { Move(Direction.Left, Position(it, 0)) }
//                .maxOf { findOuterPositions(it, myboard) }


val board = Board(emptyList(), rowCount = 5, columnCount = 10)
isMoveInsideBoard(Move(Direction.Left, Position(row = 0, column = board.columnCount)), board) eq false
isMoveInsideBoard(Move(Direction.Left, Position(row = 0, column = board.columnCount - 1)), board) eq true

isMoveInsideBoard(Move(Direction.Right, Position(row = 0, column = -1)), board) eq false
isMoveInsideBoard(Move(Direction.Right, Position(row = 0, column = 0)), board) eq true

isMoveInsideBoard(Move(Direction.Top, Position(row = board.rowCount, column = 0)), board) eq false
isMoveInsideBoard(Move(Direction.Top, Position(row = board.rowCount - 1, column = 0)), board) eq true

isMoveInsideBoard(Move(Direction.Bottom, Position(row = -1, column = 0)), board) eq false
isMoveInsideBoard(Move(Direction.Bottom, Position(row = 0, column = 0)), board) eq true


Move(Direction.Left, Position(0, 0)).moveForward() eq Move(Direction.Left, Position(0, 1))
Move(Direction.Right, Position(0, 0)).moveForward() eq Move(Direction.Right, Position(0, -1))
Move(Direction.Top, Position(0, 0)).moveForward() eq Move(Direction.Top, Position(1, 0))
Move(Direction.Bottom, Position(0, 0)).moveForward() eq Move(Direction.Bottom, Position(-1, 0))




