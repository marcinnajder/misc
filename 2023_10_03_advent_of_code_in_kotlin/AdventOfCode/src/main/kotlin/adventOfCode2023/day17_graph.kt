package adventOfCode2023.day17_graph

import java.util.*
import common.eq


data class Board(
    val lines: List<List<Int>>,
    val width: Int,
    val height: Int
)

fun loadData(input: String): Board {
    val lines = input.lines().map { line -> line.map { it.digitToInt() } }
    return Board(lines, lines[0].size, lines.size)
}


enum class Direction { Left, Right, Top, Bottom }

data class Position(val row: Int, val column: Int)

data class Move(val prev: Direction, val prevCount: Int, val position: Position)

fun Direction.toOpposite() = when (this) {
    Direction.Left -> Direction.Right
    Direction.Right -> Direction.Left
    Direction.Bottom -> Direction.Top
    Direction.Top -> Direction.Bottom
}

val allDirections = Direction.entries.toList()

val onlyForwardsMap: Map<Direction, List<Direction>> = // L -> [L,B,T]
    allDirections.associateWith { d -> d.toOpposite().let { opp -> allDirections.filter { dd -> dd != opp } } }

val onlyTurnsMap: Map<Direction, List<Direction>> = //  L -> [B,T]
    allDirections.associateWith { d ->
        d.toOpposite().let { opp -> allDirections.filter { dd -> dd != opp && dd != d } }
    }

val onlyStraightsOnMap: Map<Direction, List<Direction>> = //  L -> [L]
    allDirections.associateWith { d -> listOf(d) }


fun getPositionsInBounds(pos: Position, directions: Iterable<Direction>, board: Board) =
    directions.mapNotNull {
        when (it) {
            Direction.Left -> if (pos.column == 0) null else Pair(it, pos.copy(column = pos.column - 1))
            Direction.Right -> if (pos.column == board.width - 1) null else Pair(it, pos.copy(column = pos.column + 1))
            Direction.Top -> if (pos.row == 0) null else Pair(it, pos.copy(row = pos.row - 1))
            Direction.Bottom -> if (pos.row == board.height - 1) null else Pair(it, pos.copy(row = pos.row + 1))
        }
    }

fun getNextMoves(move: Move, board: Board, minStraightOn: Int, maxStraightOn: Int): Iterable<Move> {
    val nextDirectionsMap = when {
        move.prevCount == maxStraightOn -> onlyTurnsMap
        minStraightOn > 1 && move.prevCount < minStraightOn -> onlyStraightsOnMap
        else -> onlyForwardsMap
    }

    return getPositionsInBounds(move.position, nextDirectionsMap.getValue(move.prev), board)
        .map { (d, p) -> Move(d, if (d == move.prev) move.prevCount + 1 else 1, p) }
}


/** BFS (breadth first search) */
fun findMinCost(board: Board, minStraightOn: Int, maxStraightOn: Int): Int {
    val firstMoves = listOf(Move(Direction.Right, 0, Position(0, 0)), Move(Direction.Bottom, 0, Position(0, 0)))

    val visitedMinCosts = mutableMapOf(*firstMoves.map { it to 0 }.toTypedArray())

    val queue: Queue<Move> = LinkedList(firstMoves)

    while (queue.isNotEmpty()) {
        val currentMove = queue.poll()!!
        val currentMoveMinCost = visitedMinCosts[currentMove]!!

        for (nextMove in getNextMoves(currentMove, board, minStraightOn, maxStraightOn)) {
            val nextMovePositionCost = board.lines[nextMove.position.row][nextMove.position.column]
            val nextMoveMinCost = currentMoveMinCost + nextMovePositionCost
            val nextMoveMinCostSoFar = visitedMinCosts[nextMove]

            if (nextMoveMinCostSoFar == null || nextMoveMinCost < nextMoveMinCostSoFar) {
                visitedMinCosts[nextMove] = nextMoveMinCost
                queue.add(nextMove)
                // caution: "visited" means also those waiting in the queue, we don't want to duplicate items in queue
                // (what's why adding to "visit" collection is done just after adding to the queue, not after pulling element from queue)
            }
        }
    }

    val finishPosition = Position(board.height - 1, board.width - 1)

    val predicate: (Move) -> Boolean =
        if (minStraightOn > 1) {
            { move -> move.position == finishPosition && move.prevCount >= minStraightOn }
        } else {
            { move -> move.position == finishPosition }
        }

    return visitedMinCosts.mapNotNull { (move, minCost) -> if (predicate(move)) minCost else null }.min()
}

fun puzzle1(input: String) = findMinCost(loadData(input), 1, 3).toString()
fun puzzle2(input: String) = findMinCost(loadData(input), 4, 10).toString()

fun tests() {

    val testBoard = Board(emptyList(), 10, 10)

    getPositionsInBounds(Position(0, 0), allDirections, testBoard).toList() eq listOf(
        Direction.Right to Position(0, 1),
        Direction.Bottom to Position(1, 0)
    )
    getPositionsInBounds(Position(0, 1), allDirections, testBoard).toList() eq listOf(
        Direction.Left to Position(0, 0),
        Direction.Right to Position(0, 2),
        Direction.Bottom to Position(1, 1)
    )
    getPositionsInBounds(Position(1, 1), allDirections, testBoard).toList() eq listOf(
        Direction.Left to Position(1, 0),
        Direction.Right to Position(1, 2),
        Direction.Top to Position(0, 1),
        Direction.Bottom to Position(2, 1),
    )
    getPositionsInBounds(Position(9, 9), allDirections, testBoard).toList() eq listOf(
        Direction.Left to Position(9, 8),
        Direction.Top to Position(8, 9),
    )
    getPositionsInBounds(Position(8, 9), allDirections, testBoard).toList() eq listOf(
        Direction.Left to Position(8, 8),
        Direction.Top to Position(7, 9),
        Direction.Bottom to Position(9, 9)
    )


    val moves = getNextMoves(Move(Direction.Right, 0, Position(0, 0)), testBoard, 2, 3).toList()
    moves eq listOf(
        Move(Direction.Right, 1, Position(0, 1)),
    )

    val afterR = moves.first() // -> Right
    val nextAfterR = getNextMoves(afterR, testBoard, 2, 3).toList()
    nextAfterR eq listOf(
        Move(Direction.Right, 2, Position(0, 2)),
    )

    val afterRR = nextAfterR.first() // -> Right -> Right
    val nextAfterRR = getNextMoves(afterRR, testBoard, 2, 3).toList()
    nextAfterRR eq listOf(
        Move(Direction.Right, 3, Position(0, 3)),
        Move(Direction.Bottom, 1, Position(1, 2)),
    )

    val afterRRR = nextAfterRR.first()  // -> Right -> Right -> Right
    val nextAfterRRR = getNextMoves(afterRRR, testBoard, 2, 3).toList()
    nextAfterRRR eq listOf(
        Move(Direction.Bottom, 1, Position(1, 3)),
    )

    val afterRRRB = nextAfterRRR.first()  // -> Right -> Right -> Right -> Bottom
    val nextAfterRRRB = getNextMoves(afterRRRB, testBoard, 2, 3).toList()
    nextAfterRRRB eq listOf(
        Move(Direction.Bottom, 2, Position(2, 3)),
    )
}