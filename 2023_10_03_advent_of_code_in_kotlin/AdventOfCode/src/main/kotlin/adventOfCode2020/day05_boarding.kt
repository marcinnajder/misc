package adventOfCode2020.day05_boarding

import common.*

fun loadData(input: String) = input.lineSequence()

fun decodeChars(chars: CharSequence, range: IntRange, leftHalf: Char) =
    chars.fold(range) { r, c ->
        val middle = (r.first + r.last) / 2
        when (c) {
            leftHalf -> r.first..middle
            else -> middle + 1..r.last
        }
    }.first

fun decodeSeatId(chars: String) =
    decodeChars(chars.take(7), 0..127, 'F') * 8 + decodeChars(chars.drop(7), 0..7, 'L')


fun puzzle1(input: String) = loadData(input).maxOf(::decodeSeatId)

fun puzzle2(input: String): String {
    val occupied = loadData(input).map(::decodeSeatId).toSet()
    val free = (0..<(128 * 8)).find { it !in occupied && (it - 1) in occupied && (it + 1) in occupied }
    return free.toString()
}


fun tests() {
    decodeChars("FBFBBFF", 0..127, 'F') eq 44
    decodeChars("RLR", 0..7, 'L') eq 5

    decodeSeatId("FBFBBFFRLR") eq 357
    decodeSeatId("BFFFBBFRRR") eq 567
    decodeSeatId("FFFBBBFRRR") eq 119
    decodeSeatId("BBFFBBFRLL") eq 820
}