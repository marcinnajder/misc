package common

import kotlin.math.max
import kotlin.math.min

fun parseNumbers(text: String, separator: String = " ") =
    text.splitToSequence(separator).mapNotNull { if (it.isEmpty()) null else it.toInt() }


fun parseNumbersL(text: String, separator: String = " ") =
    text.splitToSequence(separator).mapNotNull { if (it.isEmpty()) null else it.toLong() }


operator fun IntRange.component1() = this.first
operator fun IntRange.component2() = this.last
fun IntRange.length() = this.last - this.first + 1
fun IntRange.isOverlapping(range: IntRange) =
    this.first in range || this.last in range || range.first in this || range.last in this


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

operator fun LongRange.component1() = this.first
operator fun LongRange.component2() = this.last
fun LongRange.length() = this.last - this.first + 1
fun LongRange.isOverlapping(range: LongRange) =
    this.first in range || this.last in range || range.first in this || range.last in this


fun mergeLinesL(lines: LList<LongRange>?, line: LongRange): LList<LongRange> {
    return when {
        lines == null -> LList(line, null)
        else -> {
            val (fromL, toL) = line
            val (head, tail) = lines
            val (fromH, toH) = head
            when {
                toH + 1 < fromL -> LList(head, mergeLinesL(tail, line))
                toL + 1 < fromH -> LList(line, lines)
                else -> mergeLinesL(tail, min(fromH, fromL)..max(toH, toL))
            }
        }
    }
}

fun tests() {
    mergeLinesL(null, 6L..8L) eq LList(6L..8L, null)
    mergeLinesL(llistOf(10L..12L), 14L..16L) eq llistOf(10L..12L, 14L..16L)
    mergeLinesL(llistOf(10L..12L, 18L..20L), 14L..16L) eq llistOf(10L..12L, 14L..16L, 18L..20L)

    mergeLinesL(llistOf(10L..12L, 16L..18L), 6L..9L) eq llistOf(6L..12L, 16L..18L)
    mergeLinesL(llistOf(10L..12L, 16L..18L), 13L..14L) eq llistOf(10L..14L, 16L..18L)

    mergeLinesL(llistOf(10L..12L, 16L..18L), 8L..11L) eq llistOf(8L..12L, 16L..18L)
    mergeLinesL(llistOf(10L..12L, 16L..18L), 11L..13L) eq llistOf(10L..13L, 16L..18L)

    mergeLinesL(llistOf(10L..12L, 16L..18L), 13L..15L) eq llistOf(10L..18L)
    mergeLinesL(llistOf(10L..12L, 16L..18L), 11L..17L) eq llistOf(10L..18L)

    mergeLinesL(llistOf(10L..12L, 16L..18L, 25L..27L), 7L..20L) eq llistOf(7L..20L, 25L..27L)
    mergeLinesL(llistOf(10L..12L, 16L..18L, 25L..27L), 7L..30L) eq llistOf(7L..30L)

    mergeLinesL(llistOf(10L..12L), 6L..8L) eq llistOf(6L..8L, 10L..12L)
}
