package adventOfCode2023.day05_seeds

import common.*

data class Line(val srcRange: LongRange, val dest: Long)

data class Data(val seeds: List<Long>, val mappings: List<List<Line>>)

fun compareLines(line1: Line, line2: Line) =
    if (line1.srcRange.isOverlapping(line2.srcRange)) 0 else line1.srcRange.first.compareTo(line2.srcRange.first)

fun loadData(input: String): Data {
    val lines = input.lineSequence()
    val seeds = parseNumbersL(lines.first().substringAfter(":")).toList()
    val mappings = lines.drop(2).partitionBy { it.isEmpty() }
        .filter { it.size > 1 }
        .map {
            it.drop(1).map { l ->
                parseNumbersL(l).toList().let { (dest, src, len) -> Line(src..<src + len, dest) }
            }.sortedWith(::compareLines)
        }.toList()
    return Data(seeds, mappings)
}


fun moveValue(mapping: Line, value: Long) =
    mapping.dest + value - mapping.srcRange.first

fun moveLine(mapping: Line, line: LongRange) =
    moveValue(mapping, line.first)..moveValue(mapping, line.last)

fun splitLine(line: LongRange, splitter: Long) =
    Pair(line.first..splitter, splitter + 1..line.last)


typealias ItemsCreator<T> = (seeds: List<Long>) -> T
typealias ItemsTransformer<T> = (items: T, mapping: List<Line>) -> T
typealias MinFinder<T> = (items: T) -> Long

fun <T> puzzle(input: String, create: ItemsCreator<T>, transform: ItemsTransformer<T>, findMin: MinFinder<T>) =
    loadData(input).let { it.mappings.fold(create(it.seeds), transform).let(findMin).toString() }


fun transformValues(values: Set<Long>, mapping: List<Line>): Set<Long> =
    values.asSequence().map { value ->
        mapping.binarySearch(Line(value..value, 0), ::compareLines)
            .let { i -> if (i < 0) value else moveValue(mapping[i], value) }
    }.toSet()

fun puzzle1(input: String) =
    puzzle(input, { it.toSet() }, ::transformValues, { it.min() })


fun transformLine(mappings: LList<Line>?, input: LongRange, output: LList<LongRange>?)
        : LList<LongRange>? {

    if (mappings == null) {
        return mergeLinesL(output, input);
    }

    val (mapping, restMappings) = mappings
    val isInputStartInsideMapping = input.first >= mapping.srcRange.first
    val isInputEndInsideMapping = input.last <= mapping.srcRange.last

    if (isInputStartInsideMapping && isInputEndInsideMapping) { // input entirely inside mapping
        return mergeLinesL(output, moveLine(mapping, input))
    }

    if (isInputStartInsideMapping) { // exactly two parts (first half is inside mapping, second it not)
        val (firstPart, secondPart) = splitLine(input, mapping.srcRange.last)
        return transformLine(restMappings, secondPart, mergeLinesL(output, moveLine(mapping, firstPart)))
    }

    val (firstPart, secondPart) = splitLine(input, mapping.srcRange.first - 1)
    val outputWithFirstPart = mergeLinesL(output, firstPart)

    if (isInputEndInsideMapping) { // exactly two parts (first half is not inside mapping, second is)
        return mergeLinesL(outputWithFirstPart, moveLine(mapping, secondPart))
    }

    // more than two parts
    val (firstOfSecondPart, secondOfSecondPart) = splitLine(secondPart, mapping.srcRange.last)
    return transformLine(
        restMappings, secondOfSecondPart,
        mergeLinesL(outputWithFirstPart, moveLine(mapping, firstOfSecondPart))
    )
}

fun transformLines(lines: List<Line>, mapping: List<Line>) =
    lines.fold(emptyLList<LongRange>()) { state, line ->
        val mappingsMatched = mapping.dropWhile { compareLines(line, it) != 0 }
            .takeWhile { compareLines(line, it) == 0 }.toLList()
        transformLine(mappingsMatched, line.srcRange, state)
    }.toSequence().map { Line(it, 0) }.toList();

fun puzzle2(input: String) =
    puzzle(input,
        { it.asSequence().chunked(2).map { (start, len) -> Line(start..<start + len, 0) }.toList() },
        ::transformLines,
        { it.first().srcRange.first })


fun tests() {
    compareLines(Line(0L..<5L, 0), Line(10L..15L, 0)) eq -1
    compareLines(Line(10L..15L, 0), Line(0L..<5L, 0)) eq 1

    compareLines(Line(0L..<5L, 0), Line(0L..5L, 0)) eq 0
    compareLines(Line(0L..<5L, 0), Line(0L..1L, 0)) eq 0
    compareLines(Line(0L..<5L, 0), Line(1L..3L, 0)) eq 0
    compareLines(Line(0L..<5L, 0), Line(-1L..8L, 0)) eq 0
    compareLines(Line(0L..<5L, 0), Line(-1L..1L, 0)) eq 0
    compareLines(Line(0L..<5L, 0), Line(4L..8L, 0)) eq 0

    compareLines(Line(0L..<5L, 0), Line(5L..10L, 0)) eq -1

    val sortedList =
        listOf(Line(15L..20L, 0), Line(2L..3L, 0), Line(50L..60L, 0), Line(10L..12L, 0)).sortedWith(::compareLines)
    sortedList.binarySearch(sortedList[0], ::compareLines) eq 0
    sortedList.binarySearch(sortedList[3], ::compareLines) eq 3
    sortedList.binarySearch(sortedList[1], ::compareLines) eq 1
    sortedList.binarySearch(Line(1L..5L, 0), ::compareLines) eq 0
    sortedList.binarySearch(Line(2L..2L, 0), ::compareLines) eq 0
    sortedList.binarySearch(Line(51L..51L, 0), ::compareLines) eq 3
    (sortedList.binarySearch(Line(1L..1L, 0), ::compareLines) < 0) eq true
    (sortedList.binarySearch(Line(4L..4L, 0), ::compareLines) < 0) eq true

    moveValue(Line(10L..20L, 100), 10) eq 100L
    moveValue(Line(10L..20L, 100), 15) eq 105L

    moveLine(Line(10L..20L, 100), 10L..12L) eq 100L..102L
    moveLine(Line(10L..20L, 100), 15L..19L) eq 105L..109L

    splitLine(10L..20L, 15L) eq Pair(10L..15L, 16L..20L)

    val mapping = sequenceOf(Line(5L..10L, 100), Line(15L..20L, 200))


    transformLine(mapping.take(1).toLList(), 5L..10L, emptyLList()) eq llistOf(100L..105L)
    transformLine(mapping.take(1).toLList(), 6L..9L, emptyLList()) eq llistOf(101L..104L)

    transformLine(mapping.take(1).toLList(), 7L..12L, emptyLList()) eq llistOf(11L..12L, 102L..105L)

    transformLine(mapping.take(1).toLList(), 3L..7L, emptyLList()) eq llistOf(3L..4L, 100L..102L)

    transformLine(mapping.take(1).toLList(), 3L..12L, emptyLList()) eq llistOf(3L..4L, 11L..12L, 100L..105L)
    transformLine(mapping.toLList(), 3L..17L, emptyLList()) eq llistOf(3L..4L, 11L..14L, 100L..105L, 200L..202L)
    transformLine(mapping.toLList(), 3L..25L, emptyLList()) eq
            llistOf(3L..4L, 11L..14L, 21L..25L, 100L..105L, 200L..205L)
}