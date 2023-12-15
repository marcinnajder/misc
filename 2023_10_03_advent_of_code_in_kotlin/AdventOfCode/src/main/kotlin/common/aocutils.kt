package common

import kotlin.math.max
import kotlin.math.min

fun parseNumbers(text: String, separator: String = " ") =
    text.splitToSequence(separator).mapNotNull { if (it.isEmpty()) null else it.toInt() }


fun parseNumbersL(text: String, separator: String = " ") =
    text.splitToSequence(separator).mapNotNull { if (it.isEmpty()) null else it.toLong() }


operator fun LongProgression.component1() = this.first
operator fun LongProgression.component2() = this.last
fun LongProgression.length() = this.last - this.first + 1

fun mergeLines(lines: LList<LongProgression>?, line: LongProgression): LList<LongProgression> {
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

fun tests() {
    mergeLines(null, 6L..8L) eq LList(6L..8L, null)
    mergeLines(llistOf(10L..12L), 14L..16L) eq llistOf(10L..12L, 14L..16L)
    mergeLines(llistOf(10L..12L, 18L..20L), 14L..16L) eq llistOf(10L..12L, 14L..16L, 18L..20L)

    mergeLines(llistOf(10L..12L, 16L..18L), 6L..9L) eq llistOf(6L..12L, 16L..18L)
    mergeLines(llistOf(10L..12L, 16L..18L), 13L..14L) eq llistOf(10L..14L, 16L..18L)

    mergeLines(llistOf(10L..12L, 16L..18L), 8L..11L) eq llistOf(8L..12L, 16L..18L)
    mergeLines(llistOf(10L..12L, 16L..18L), 11L..13L) eq llistOf(10L..13L, 16L..18L)

    mergeLines(llistOf(10L..12L, 16L..18L), 13L..15L) eq llistOf(10L..18L)
    mergeLines(llistOf(10L..12L, 16L..18L), 11L..17L) eq llistOf(10L..18L)

    mergeLines(llistOf(10L..12L, 16L..18L, 25L..27L), 7L..20L) eq llistOf(7L..20L, 25L..27L)
    mergeLines(llistOf(10L..12L, 16L..18L, 25L..27L), 7L..30L) eq llistOf(7L..30L)

    mergeLines(llistOf(10L..12L), 6L..8L) eq llistOf(6L..8L, 10L..12L)
}
