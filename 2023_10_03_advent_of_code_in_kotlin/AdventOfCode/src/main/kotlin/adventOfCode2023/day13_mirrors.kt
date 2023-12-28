package adventOfCode2023.day13_mirrors

import common.partitionBy
import common.* // LList

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
    rows.toLList()!!.let { (head, tail) ->
        findReflectionRowId(llistOf(head), tail, 1, comparer)
    }

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


fun differsBySingleItem(row1: String, row2: String) =
    row1.asSequence().zip(row2.asSequence()).singleOrNull { (c1, c2) -> c1 != c2 } != null

fun puzzle2(input: String) =
    loadData(input).sumOf {
        calcPointsForRowsAndColumns(it) { rows1, rows2 ->
            rows1.zip(rows2).filter { (row1, row2) -> row1 != row2 }
                .flatMap { (row1, row2) -> 1..if (differsBySingleItem(row1, row2)) 1 else 2 }
                .singleOrNull() != null
        }
    }


fun puzzle22(input: String) =
    loadData(input).sumOf {
        calcPointsForRowsAndColumns(it) x@{ rows1, rows2 ->
            var rowWithSmudge = false;
            for ((row1, row2) in rows1.zip(rows2).filter { (row1, row2) -> row1 != row2 }) {
                if (!rowWithSmudge && differsBySingle(row1, row2)) {
                    rowWithSmudge = true
                } else {
                    return@x false
                }
            }
            return@x rowWithSmudge
        }
    }