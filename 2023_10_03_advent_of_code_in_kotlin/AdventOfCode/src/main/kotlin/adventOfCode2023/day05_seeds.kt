package adventOfCode2023.day05_seeds

import common.partitionBy
import common.parseNumbersL

data class Line(val srcRange: LongProgression, val dest: Long)

fun LongProgression.contains(value: Long) = value >= this.first && value <= this.last

fun compareLines(line1: Line, line2: Line): Int {
    return when {
        line2.srcRange.contains(line1.srcRange.first) || line2.srcRange.contains(line1.srcRange.last) -> 0
        line1.srcRange.contains(line2.srcRange.first) || line1.srcRange.contains(line2.srcRange.last) -> 0
        else -> line1.srcRange.first.compareTo(line2.srcRange.first)
    }
}


data class Data(val seed: List<Long>, val mappings: List<List<Line>>)

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

fun puzzle1(input: String) =
    loadData(input).let { it.seed.minOf { n -> process(it, n) }.toString() }

fun puzzle2(input: String): String {
    return loadData(input).let {
        it.seed.asSequence().chunked(2)
            .flatMap { (start, len) -> start..<start + len }
            .distinct()
            //.take(100_000)
            .minOf { n -> process(it, n) }
            .toString()
    }
}

// loadData(input).let { it.seed.minOf { n -> process(it, n) }.toString() }

// val data = loadData(input)


//data.mappings.fold(data.seed) { ns, mappings ->
//    ns.map { n ->
//        mappings.binarySearch(Line(n..n, 0), ::compareLines)
//            .let { i -> if (i < 0) n else mappings[i].let { m -> m.dest + n - m.srcRange.first } }
//    }
//}.toList()


// val seed = data.seed.asSequence().chunked(2).flatMap { (start, len) -> start..<start + len }.count()

fun process(data: Data, seed: Long) =
    data.mappings.fold(seed) { n, mappings ->
        mappings.binarySearch(Line(n..n, 0), ::compareLines)
            .let { i ->
                if (i < 0) n else mappings[i].let { m ->
                    m.dest + n - m.srcRange.first
                }
            }
    }

//data.seed.asSequence().chunked(2).flatMap { (start, len) -> start..<start + len }.count()
//
//val seeds = data.seed.asSequence().chunked(2).flatMap { (start, len) -> start..<start + len }
//
//seeds.minOf(::process)
//
//data.seed.minOf(::process)


//compareLines(Line(0..<5, 0), Line(10..15, 0)) eq -1
//compareLines(Line(10..15, 0), Line(0..<5, 0)) eq 1
//
//compareLines(Line(0..<5, 0), Line(0..5, 0)) eq 0
//compareLines(Line(0..<5, 0), Line(0..1, 0)) eq 0
//compareLines(Line(0..<5, 0), Line(1..3, 0)) eq 0
//compareLines(Line(0..<5, 0), Line(-1..8, 0)) eq 0
//compareLines(Line(0..<5, 0), Line(-1..1, 0)) eq 0
//compareLines(Line(0..<5, 0), Line(4..8, 0)) eq 0
//
//compareLines(Line(0..<5, 0), Line(5..10, 0)) eq -1

//val sortedList = listOf(Line(15..20, 0), Line(2..3, 0), Line(50..60, 0), Line(10..12, 0)).sortedWith(::compareLines)
//
//sortedList.binarySearch(sortedList[0], ::compareLines) eq 0
//sortedList.binarySearch(sortedList[3], ::compareLines) eq 3
//sortedList.binarySearch(sortedList[1], ::compareLines) eq 1
//sortedList.binarySearch(Line(1..5, 0), ::compareLines) eq 0
//sortedList.binarySearch(Line(2..2, 0), ::compareLines) eq 0
//sortedList.binarySearch(Line(51..51, 0), ::compareLines) eq 3
//(sortedList.binarySearch(Line(1..1, 0), ::compareLines) < 0) eq true
//(sortedList.binarySearch(Line(4..4, 0), ::compareLines) < 0) eq true
