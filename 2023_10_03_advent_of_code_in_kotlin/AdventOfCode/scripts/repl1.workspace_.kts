import sun.security.util.Length
import java.lang.AssertionError
import java.util.*
import kotlin.math.abs
import kotlin.math.min
import kotlin.math.max
import kotlin.math.pow

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


fun process(chars: String, numbers: List<Int>): Long {
    val cache = mutableMapOf<String, Long>()

    fun processChar(cIndex: Int, nIndex: Int, number: Int): Long {
        return cache.getOrPut("$cIndex,$nIndex,$number") getValue@{
            val endOfChars = cIndex == chars.length
            val endOfNumbers = nIndex == numbers.size

            fun processDot() =
                when {
                    number == 0 -> processChar(cIndex + 1, nIndex, 0)
                    number != numbers[nIndex] -> 0
                    else -> processChar(cIndex + 1, nIndex + 1, 0)
                }

            fun processHash() =
                if (number + 1 > numbers[nIndex]) 0 else processChar(cIndex + 1, nIndex, number + 1)

            return when {
                endOfChars && endOfNumbers -> 1
                endOfChars -> if ((nIndex == numbers.size - 1) && (number == numbers[nIndex])) 1 else 0
                endOfNumbers -> if (chars[cIndex] == '#') 0 else processChar(cIndex + 1, nIndex, number)
                else -> when (chars[cIndex]) {
                    '.' -> processDot() // finish segment
                    '#' -> processHash() // increment segment length
                    else -> processHash() + processDot()
                }
            }
        }
    }

    return processChar(0, 0, 0)
}

process("???.###".trim('.'), listOf(1, 1, 3)) eq 1L
process(".??..??...?##.".trim('.'), listOf(1, 1, 3)) eq 4L
process("?#?#?#?#?#?#?#?".trim('.'), listOf(1, 3, 1, 6)) eq 1L
process("????.#...#...".trim('.'), listOf(4, 1, 1)) eq 1L
process("????.######..#####.".trim('.'), listOf(1, 6, 5)) eq 4L
process("?###????????".trim('.'), listOf(3, 2, 1)) eq 10L

val input =
    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2023/day12.txt")
        .readText()


typealias Entry = Pair<String, List<Int>>

fun loadData(input: String): Sequence<Entry> =
    input.lineSequence()
        .map { it.split(' ').let { (left, right) -> Pair(left, parseNumbers(right, ",").toList()) } }

fun puzzle(input: String, transformEntry: (Entry) -> Entry) =
    loadData(input).take(5).map(transformEntry).sumOf { (left, right) -> process(left.trim('.'), right) }.toString()

fun puzzle1(input: String) =
    puzzle(input) { it }

fun puzzle2(input: String) =
    puzzle(input) { (left, right) -> Pair((1..5).joinToString("?") { left }, (1..5).flatMap { right }) }

