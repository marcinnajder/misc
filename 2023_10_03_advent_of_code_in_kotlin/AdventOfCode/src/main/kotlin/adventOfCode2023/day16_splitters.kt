package adventOfCode2023.day16_splitters

import java.util.*
import common.eq

enum class Direction { Left, Right, Top, Bottom }

data class Position(val row: Int, val column: Int)

data class Move(val fromDirection: Direction, val toPosition: Position)

data class Board(val lines: List<String>, val rowCount: Int, val columnCount: Int)


fun loadData(input: String) =
    input.lines().let { Board(it, it.size, it[0].length) }


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
}.filter { m -> isMoveInsideBoard(m, board) }


/** BFS (breadth first search) */
fun findVisitedPositions(firstMove: Move, board: Board): Int {
    val visitedMoves = mutableSetOf(firstMove)
    val queue: Queue<Move> = LinkedList()
    queue.add(firstMove)

    while (queue.isNotEmpty()) {
        val currentMove = queue.poll()!!

        val nextMoves = executeMove(currentMove, board).filter { it !in visitedMoves }.toList()

        queue.addAll(nextMoves)
        visitedMoves.addAll(nextMoves)
        // caution: "visited" means also those waiting in the queue, we don't want to duplicate items in queue
        // (what's why adding to "visit" collection is done just after adding to the queue, not after pulling element from queue)
    }

    return mutableSetOf<Position>().let { visitedMoves.mapTo(it) { m -> m.toPosition } }.size
}

fun puzzle1(input: String) =
    loadData(input).let { board -> findVisitedPositions(Move(Direction.Left, Position(0, 0)), board) }.toString()

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
        }.maxOf { findVisitedPositions(it, board) }.toString()
    }


fun tests() {
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
}