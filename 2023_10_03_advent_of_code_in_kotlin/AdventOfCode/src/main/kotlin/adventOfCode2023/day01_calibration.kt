package adventOfCode2023.day01_calibration

typealias Mapping = List<Pair<String, String>>

val onlyDigits: Mapping = (1..9).asSequence().map { it.toString() }.map { it to it }.toList()

val alsoSpelledOut: Mapping =
    onlyDigits + sequenceOf("one", "two", "three", "four", "five", "six", "seven", "eight", "nine")
        .mapIndexed { index, value -> value to "${index + 1}" }
        .toList()

fun parseLine(text: String, mapping: Mapping): Int {
    val (firstDigit, _) = mapping
        .mapNotNull { (t, d) -> text.indexOf(t).let { if (it == -1) null else Pair(d, it) } }
        .minBy { (d, i) -> i }

    val (lastDigit, _) = mapping
        .mapNotNull { (t, d) -> text.lastIndexOf(t).let { if (it == -1) null else Pair(d, it) } }
        .maxBy { (d, i) -> i }

    return "${firstDigit}${lastDigit}".toInt()
}

fun loadData(input: String, mapping: Mapping) = input.lineSequence().map { parseLine(it, mapping) }

fun puzzle(input: String, mapping: Mapping) = loadData(input, mapping).sum().toString()

fun puzzle1(input: String) = puzzle(input, onlyDigits)

fun puzzle2(input: String) = puzzle(input, alsoSpelledOut)


