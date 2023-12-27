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


val input =
    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2023/day13.txt")
        .readText()


fun loadData(input: String) =
    input.lineSequence().partitionBy { it.isEmpty() }.filter { it.size > 1 }

typealias RowsComparer = (Sequence<String>, Sequence<String>) -> Boolean

fun findReflectionRowId(
    topRows: LList<String>?, bottomRows: LList<String>?, rowId: Int, comparer: RowsComparer
): Int? =
    when {
        topRows == null || bottomRows == null -> null
        comparer(topRows.toSequence(), bottomRows.toSequence()) -> rowId
        else -> findReflectionRowId(LList(bottomRows.head, topRows), bottomRows.tail, rowId + 1, comparer)
    }


fun calcPointsForRows(rows: List<String>, comparer: RowsComparer): Int? =
    findReflectionRowId(LList(rows[0], null), rows.toLList()!!.tail, 1, comparer)

fun transpose(rows: List<String>) = sequence {
    val iterators = rows.map { it.iterator() }
    while (iterators[0].hasNext()) {
        yield(iterators.asSequence().map { it.next() }.joinToString(""))
    }
}

fun calcPointsForRowsAndColumns(rows: List<String>, comparer: RowsComparer): Int =
    when (val rowIndex = calcPointsForRows(rows, comparer)) {
        null -> calcPointsForRows(transpose(rows).toList(), comparer)!!
        else -> rowIndex * 100
    }

fun puzzle1(input: String) =
    loadData(input).sumOf {
        calcPointsForRowsAndColumns(it) { rows1, rows2 ->
            rows1.zip(rows2).all { (row1, row2) -> row1 == row2 }
        }
    }.toString()


fun puzzle2(input: String) =
    loadData(input).sumOf {
        calcPointsForRowsAndColumns(it) x@{ rows1, rows2 ->
//            rows1.zip(rows2).filter { (row1, row2) -> row1 != row2 }
//                .singleOrNull { (row1, row2) ->
//                    row1.asSequence().zip(row2.asSequence()).singleOrNull { (c1, c2) -> c1 != c2 } != null
//                } != null
            var rowWithSmudge = false;
            for ((row1, row2) in rows1.zip(rows2).filter { (row1, row2) -> row1 != row2 }) {
                if (!rowWithSmudge && row1.asSequence().zip(row2.asSequence())
                        .singleOrNull { (c1, c2) -> c1 != c2 } !== null
                ) {
                    rowWithSmudge = true
                } else {
                    return@x false
                }
            }
            return@x rowWithSmudge
        }
    }


//            rows1.toSequence().zip(rows2.toSequence())
//                .scan(Pair(false, true)) { (rowWithSmudge, result), (row1, row2) ->
//                    when {
//                        row1 == row2 -> Pair(rowWithSmudge, true)
//                        // exactly one char is different
//                        row1.asSequence().zip(row2.asSequence()).singleOrNull { (c1, c2) -> c1 != c2 } !== null ->
//                            Pair(true, !rowWithSmudge)
//
//                        else -> Pair(rowWithSmudge, false)
//                    }
//                }.all { (_, result) -> result }



