//import sun.security.util.Length
import java.lang.AssertionError
import java.util.*
import kotlin.enums.EnumEntries
import kotlin.math.abs
import kotlin.math.min
import kotlin.math.max
import kotlin.math.pow
import kotlin.time.measureTime
import kotlin.time.Duration

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
    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2023/day17_____.txt")
        .readText()


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

val onlyForwardsMap: Map<Direction, List<Direction>> =
    allDirections.associateWith { d -> d.toOpposite().let { opp -> allDirections.filter { dd -> dd != opp } } }

val onlyTurnsMap: Map<Direction, List<Direction>> =
    allDirections.associateWith { d ->
        d.toOpposite().let { opp -> allDirections.filter { dd -> dd != opp && dd != d } }
    }

val onlyStraightsOnMap: Map<Direction, List<Direction>> =
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

fun puzzle1(input: String) = findMinCost(loadData(input), 1, 4).toString()
fun puzzle2(input: String) = findMinCost(loadData(input), 4, 10).toString()

measureTime {
    val data = loadData(input)
    val result = findMinCost(data, 1, 4)
    println("result: $result")
}


//data class Board(
//    val lines: List<List<Int>>,
//    val width: Int,
//    val height: Int
//)
//
//fun loadData(input: String): Board {
//    val lines = input.lines().map { line -> line.map { it.digitToInt() } }
//    return Board(lines, lines[0].size, lines.size)
//}
//
//
//enum class Direction { Left, Right, Top, Bottom }
//
//data class Position(val row: Int, val column: Int)
//
//data class Move(val buffer: List<Direction>, val position: Position)
//
//fun Direction.toOpposite() = when (this) {
//    Direction.Left -> Direction.Right
//    Direction.Right -> Direction.Left
//    Direction.Bottom -> Direction.Top
//    Direction.Top -> Direction.Bottom
//}
//
////val maxStraightOn = 3
////var minStraightOn = 1 // val
//val maxStraightOn = 10
//var minStraightOn = 4 // val
//
//val allDirections = Direction.entries.toList()
//
//val nextDirectionsWithoutComingBackMap: Map<Direction, List<Direction>> =
//    allDirections.associateWith { d -> d.toOpposite().let { opp -> allDirections.filter { dd -> dd != opp } } }
//
//val nextDirectionsOnlyTurnsMap: Map<Direction, List<Direction>> =
//    allDirections.associateWith { d ->
//        d.toOpposite().let { opp -> allDirections.filter { dd -> dd != opp && dd != d } }
//    }
//
//val nextDirectionsOnlyStraightOnMap: Map<Direction, List<Direction>> =
//    allDirections.associateWith { d -> listOf(d) }
//
//
//fun getPositionsForDirections(pos: Position, directions: Iterable<Direction>, board: Board) =
//    directions.mapNotNull {
//        when (it) {
//            Direction.Left -> if (pos.column == 0) null else Pair(it, pos.copy(column = pos.column - 1))
//            Direction.Right -> if (pos.column == board.width - 1) null else Pair(it, pos.copy(column = pos.column + 1))
//            Direction.Top -> if (pos.row == 0) null else Pair(it, pos.copy(row = pos.row - 1))
//            Direction.Bottom -> if (pos.row == board.height - 1) null else Pair(it, pos.copy(row = pos.row + 1))
//        }
//    }
//
//
////fun getNextMoves(move: Move, board: Board): Iterable<Move> {
////    if (move.buffer.isEmpty()) { // first move
////        return getPositionsForDirections(move.position, allDirections, board).map { (d, p) -> Move(listOf(d), p) }
////    }
////
////    val lastDirection = move.buffer.last()
////    val isBufferFull = move.buffer.size == maxStraightOn
////    val nextDirectionsMap =
////        if (isBufferFull && move.buffer.all { it == lastDirection }) nextDirectionsOnlyTurnsMap else nextDirectionsWithoutComingBackMap
////    val prevDirections = if (isBufferFull) move.buffer.drop(1) else move.buffer
////
////    return getPositionsForDirections(move.position, nextDirectionsMap.getValue(lastDirection), board)
////        .map { (d, p) -> Move(prevDirections + d, p) }
////}
//
//
//fun isMinStraightOn(move: Move, bufferSize: Int, lastDirection: Direction) =
//    bufferSize >= minStraightOn && (bufferSize - minStraightOn..<bufferSize).all { move.buffer[it] == lastDirection }
//
//fun isMinStraightOn(move: Move) = isMinStraightOn(move, move.buffer.size, move.buffer.last())
//
//
//fun getNextMoves2(move: Move, board: Board): Iterable<Move> {
//    if (move.buffer.isEmpty()) { // first move
//        return getPositionsForDirections(move.position, allDirections, board).map { (d, p) -> Move(listOf(d), p) }
//    }
//
//    val lastDirection = move.buffer.last()
//    val bufferSize = move.buffer.size
//    val isBufferFull = bufferSize == maxStraightOn
//
//    val nextDirectionsMap = when {
//        isBufferFull && move.buffer.all { it == lastDirection } -> nextDirectionsOnlyTurnsMap
////        minStraightOn > 1 && (bufferSize < minStraightOn || (bufferSize - minStraightOn..<bufferSize).any { move.buffer[it] != lastDirection }) ->
////            nextDirectionsOnlyStraightOnMap
//        minStraightOn > 1 && !isMinStraightOn(move, bufferSize, lastDirection) -> nextDirectionsOnlyStraightOnMap
//        else -> nextDirectionsWithoutComingBackMap
//    }
//
//    val prevDirections = if (isBufferFull) move.buffer.drop(1) else move.buffer
//
//    return getPositionsForDirections(move.position, nextDirectionsMap.getValue(lastDirection), board)
//        .map { (d, p) -> Move(prevDirections + d, p) }
//}
//
//
///** BFS (breadth first search) */
//fun findMinCost(firstMove: Move, board: Board): Int {
//    val visitedMinCosts = mutableMapOf(firstMove to 0) // Move is stored instead of only Position
//    val queue: Queue<Move> = LinkedList()
//    queue.add(firstMove)
//
//    while (queue.isNotEmpty()) {
//        val currentMove = queue.poll()!!
//        // println("$currentMove -> ")
//        val currentMoveMinCost = visitedMinCosts[currentMove]!!
//
//        for (nextMove in getNextMoves2(currentMove, board)) {
//            val nextMovePositionCost = board.lines[nextMove.position.row][nextMove.position.column]
//            val nextMoveMinCost = currentMoveMinCost + nextMovePositionCost
//            val nextMoveMinCostSoFar = visitedMinCosts[nextMove]
//
//            if (nextMoveMinCostSoFar == null || nextMoveMinCost < nextMoveMinCostSoFar) {
//                visitedMinCosts[nextMove] = nextMoveMinCost
//                queue.add(nextMove)
//                // caution: "visited" means also those waiting in the queue, we don't want to duplicate items in queue
//            }
//        }
//    }
//
//    val finishPosition = Position(board.height - 1, board.width - 1)
//
//    val pred: (Move) -> Boolean =
//        if (minStraightOn > 1) {
//            { move -> move.position == finishPosition && isMinStraightOn(move) }
//        } else {
//            { move -> move.position == finishPosition }
//        }
//
//    return visitedMinCosts.mapNotNull { (move, minCost) -> if (pred(move)) minCost else null }.min()
//}
//
//measureTime {
//    val data = loadData(input)
//    val result =
//        findMinCost(Move(emptyList(), Position(0, 0)), data)
//
//    println("result: $result")
//}
//
//val testBoard = Board(emptyList(), 10, 10)
//
//getPositionsForDirections(Position(0, 0), allDirections, testBoard).toList() eq listOf(
//    Direction.Right to Position(0, 1),
//    Direction.Bottom to Position(1, 0)
//)
//getPositionsForDirections(Position(0, 1), allDirections, testBoard).toList() eq listOf(
//    Direction.Left to Position(0, 0),
//    Direction.Right to Position(0, 2),
//    Direction.Bottom to Position(1, 1)
//)
//getPositionsForDirections(Position(1, 1), allDirections, testBoard).toList() eq listOf(
//    Direction.Left to Position(1, 0),
//    Direction.Right to Position(1, 2),
//    Direction.Top to Position(0, 1),
//    Direction.Bottom to Position(2, 1),
//)
//getPositionsForDirections(Position(9, 9), allDirections, testBoard).toList() eq listOf(
//    Direction.Left to Position(9, 8),
//    Direction.Top to Position(8, 9),
//)
//getPositionsForDirections(Position(8, 9), allDirections, testBoard).toList() eq listOf(
//    Direction.Left to Position(8, 8),
//    Direction.Top to Position(7, 9),
//    Direction.Bottom to Position(9, 9)
//)
//
//
//val moves = getNextMoves(Move(emptyList(), Position(0, 0)), testBoard).toList()
//moves eq listOf(
//    Move(listOf(Direction.Right), Position(0, 1)),
//    Move(listOf(Direction.Bottom), Position(1, 0)),
//)
//
//val afterR = moves.first() // -> Right
//val nextAfterR = getNextMoves(afterR, testBoard).toList()
//nextAfterR eq listOf(
//    Move(listOf(Direction.Right, Direction.Right), Position(0, 2)),
//    Move(listOf(Direction.Right, Direction.Bottom), Position(1, 1)),
//)
//
//val afterRR = nextAfterR.first() // -> Right
//val nextAfterRR = getNextMoves(afterRR, testBoard).toList()
//nextAfterRR eq listOf(
//    Move(listOf(Direction.Right, Direction.Right, Direction.Right), Position(0, 3)),
//    Move(listOf(Direction.Right, Direction.Right, Direction.Bottom), Position(1, 2)),
//)
//
//val afterRRR = nextAfterRR.first()  // -> Right
//val nextAfterRRR = getNextMoves(afterRRR, testBoard).toList()
//nextAfterRRR eq listOf(
//    Move(listOf(Direction.Right, Direction.Right, Direction.Bottom), Position(1, 3)),
//)
//
//val afterRRRB = nextAfterRRR.first()  // -> Bottom
//val nextAfterRRRB = getNextMoves(afterRRRB, testBoard).toList()
//nextAfterRRRB eq listOf(
//    Move(listOf(Direction.Right, Direction.Bottom, Direction.Right), Position(1, 4)),
//    Move(listOf(Direction.Right, Direction.Bottom, Direction.Bottom), Position(2, 3)),
//    Move(listOf(Direction.Right, Direction.Bottom, Direction.Left), Position(1, 2)),
//)
//
//val afterRRRBR = nextAfterRRRB.first()  // -> Right
//val nextAfterRRRBR = getNextMoves(afterRRRBR, testBoard).toList()
//nextAfterRRRBR eq listOf(
//    Move(listOf(Direction.Bottom, Direction.Right, Direction.Top), Position(0, 4)),
//    Move(listOf(Direction.Bottom, Direction.Right, Direction.Right), Position(1, 5)),
//    Move(listOf(Direction.Bottom, Direction.Right, Direction.Bottom), Position(2, 4)),
//)
//
//
//// [Move(buffer=[Right], position=Position(row=0, column=1)), Move(buffer=[Bottom], position=Position(row=1, column=0))]
//
//
/////** BFS (breadth first search) */
////fun findOuterPositions(board: Board): Set<Pair<Int, Int>> {
////    val wall = enumerateAllPositions(board).mapNotNull { (c, row, column) ->
////        if (c == ' ') null else Pair(row, column)
////    }.toSet()
////
////    val visited = mutableSetOf(Pair(0, 0))
////    val queue: Queue<Pair<Int, Int>> = LinkedList()
////    queue.add(Pair(0, 0))
////
////    while (queue.isNotEmpty()) {
////        val currentPosition = queue.poll()!!
////        val (row, column) = currentPosition
////
////        val next = getAdjacentPositions(board, row, column)
////            .filter { it !in wall && it !in visited }.toList()
////
////        queue.addAll(next)
////        visited.addAll(next)
////        // caution: "visited" means also those waiting in the queue, we don't want to duplicate items in queue
////    }
////
////    return visited
////}
//
//
//
//
