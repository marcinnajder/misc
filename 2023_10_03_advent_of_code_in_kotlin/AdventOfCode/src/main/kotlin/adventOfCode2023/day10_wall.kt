package adventOfCode2023.day10_wall

import common.expand
import java.util.*

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
    board.lines.flatMapIndexed { row, line ->
        line.mapIndexedNotNull { column, c -> Triple(c, row, column) }
    }

fun findOuterPositions(board: Board): Set<Pair<Int, Int>> {
    val wall = enumerateAllPositions(board).mapNotNull { (c, row, column) ->
        if (c == ' ') null else Pair(row, column)
    }.toSet()

    val visited = mutableSetOf(Pair(0, 0))
    val queue: Queue<Pair<Int, Int>> = LinkedList()
    queue.add(Pair(0, 0))

    while (queue.isNotEmpty()) { // breadth first search
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

    return enumerateAllPositions(board)
        .count { (_, row, column) ->
            Pair(row, column) !in wall && Pair(expandPosition(row), expandPosition(column)) !in outsidePositions
        }.toString()
}


//val input =
//    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2023/day10.txt")
//        .readText()

// very slow (1s) comparing to the alternative implementation above, very strange ...
//fun expandBoard(data: Board, routeSet: Set<Pair<Int, Int>>): Board {
//    val width = (data.width * 2) + 2
//    val height = (data.height * 2) + 2
//    val startingColumn = expandPosition(data.startingColumn)
//    val startingRow = expandPosition(data.startingRow * 2)
//    val lines = data.lines.asSequence().flatMapIndexed { row, line ->
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
//    }
//    val emptyLine = " ".repeat(width)
//    return Board((sequenceOf(emptyLine) + lines + emptyLine).toList(), startingRow, startingColumn, width, height)
//}

