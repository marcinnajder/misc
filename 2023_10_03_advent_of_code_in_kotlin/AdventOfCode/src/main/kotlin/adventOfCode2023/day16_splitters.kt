package adventOfCode2023.day16_splitters

import java.util.*

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
    if (isMoveInsideBoard(move, board)) {
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



