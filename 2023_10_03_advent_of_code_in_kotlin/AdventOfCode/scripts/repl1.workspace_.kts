import java.lang.AssertionError


infix fun Any?.eq(obj2: Any?) {
    if (this != obj2) {
        throw AssertionError("'$this' <> '$obj2'")
    }
}
//  A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, or 2

val (pointsWithoutJ, pointsWithJ) = listOf(
    listOf('A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'),
    listOf('A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J')
).map { it.asReversed().mapIndexed { index, c -> c to index }.toMap() }


// val cardsPoint = listOf('A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2')


//val cardsPoint = listOf('A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J')
//    .asReversed().mapIndexed { index, c -> c to index }.toMap()


fun compareCardsWhenDraw(points: Map<Char, Int>, text1: String, text2: String) =
    text1.zip(text2)
        .firstNotNullOf { (c1, c2) -> if (c1 == c2) null else points.getValue(c1) - points.getValue(c2) }

//(compareCardsWhenDraw("AKJ", "AKT") > 0) eq true
//(compareCardsWhenDraw("AKT", "AKJ") < 0) eq true


data class Card(val text: String, val bid: Int = 0, val originalText: String? = null) {
    val grouped: Map<Char, List<Char>> = text.groupBy { it }// .toSortedMap() // ! toSortedMap
    val maxGroupSize: Int = grouped.maxOf { e -> e.value.size }
}


//(compareCards("AAAAA", "AAAAA") == 0) eq true
//(compareCards("AAAA8", "AA8AA") == 0) eq true
//
//val sortedCards = listOf("AAAAA", "AAAA8", "AAA88", "AA882", "23456")
//for (i in sortedCards.indices) {
//    for (j in (i + 1)..<sortedCards.size) {
//        println("${sortedCards[i]}-${sortedCards[j]} ")
//        (compareCards(sortedCards[i], sortedCards[j]) > 0) eq true
//    }
//}

//
//(compareCards(Card("AAAAA"), Card("AAAAA")) == 0) eq true
//(compareCards(Card("AAAA8"), Card("AA8AA")) == 0) eq true
//
//val sortedCards = listOf("AAAAA", "AAAA8", "AAA88", "AA882", "23456").map(::Card)
//for (i in sortedCards.indices) {
//    for (j in (i + 1)..<sortedCards.size) {
//        // println("${sortedCards[i]}-${sortedCards[j]} ")
//        (compareCards(sortedCards[i], sortedCards[j]) > 0) eq true
//    }
//}

// val aa = listOf("32T3K", "T55J5", "KK677", "KTJJT", "QQQJA").map(::Card).sortedWith(::compareCards)
//val aa = listOf("32T3K", "T55J5", "KK677", "KTJJT", "QQQJA").sortedWith(::compareCards)


fun compareCardsWithPoints(points: Map<Char, Int>) =
    { card1: Card, card2: Card ->
        when {
            card1.originalText == null && card2.originalText == null && card1.text == card2.text -> 0
            card1.originalText != null && card2.originalText != null && card1.originalText == card2.originalText -> 0

            // card1.originalText != null && card1.originalText == card2.originalText -> 0 // cards are identical

            // card1.text == card2.text -> 0 // cards are identical

//            card1.grouped == card2.grouped ->
//                compareCardsWhenDraw(points, card1.originalText ?: card1.text, card2.originalText ?: card2.text)

            //0 // the same cards but in different order

            card1.maxGroupSize == card2.maxGroupSize -> // possible the same type
                if (card1.grouped.size == card2.grouped.size)
                    compareCardsWhenDraw(points, card1.originalText ?: card1.text, card2.originalText ?: card2.text)
                else
                    card2.grouped.size - card1.grouped.size // different type

            else -> card1.maxGroupSize - card2.maxGroupSize // different type
        }
    }

compareCardsWithPoints(pointsWithJ)(Card("77888"), Card("77788"))


val input =
    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2023/day07.txt")
        .readText()

fun loadData(input: String) =
    input.lineSequence().map { it.split(" ").let { (card, bid) -> Card(card, bid.trim().toInt()) } }



loadData(input)
    .sortedWith(compareCardsWithPoints(pointsWithoutJ))
    .mapIndexed { index, card -> (index + 1) * card.bid }
    .sum()

fun useJocker(card: Card) =
    when {
        card.text == "JJJJJ" -> card
        card.text.contains('J') ->
            card.grouped.filter { e -> e.key != 'J' }.maxBy { e -> e.value.size }.key
                .let { Card(card.text.replace('J', it), card.bid, card.text) }

        else -> card
    }


//useJocker(Card("JJJJJ")).text eq "JJJJJ"
//useJocker(Card("QTJJ6")).text eq "QTQQ6"


//useJocker(Card("QQQJA")).let { { it.text eq "QQQQA"; it.originalText eq "QQQJA" } }
//useJocker(Card("QQQJJ")).let { { it.text eq "QQQQQ"; it.originalText eq "QQQJJ" } }

// 248910538 ??


// 248823769 too low

// 248909434

// 248910903 too high
// 250281415 too high
// 245470108 too high (google)
loadData(input)
    .map(::useJocker)
    .sortedWith(compareCardsWithPoints(pointsWithJ))
//    .toList() // !
//    .map { "${it.text}(${it.originalText})" } // !!
    .mapIndexed { index, card -> (index + 1) * card.bid }
    .sum()


loadData(input)
    .map(::useJocker)
    .sortedWith(compareCardsWithPoints(pointsWithJ))
    //.filter { it.text.contains('J') }
    .mapNotNull { if (it.originalText?.contains('J') == true) "${it.originalText} -> ${it.text}" else null }
    .toList()//!


//compareCards("QQQJA", "KTJJT")


//fun parseNumbers(text: String, separator: String = " ") =
//    text.splitToSequence(separator).mapNotNull { if (it.isEmpty()) null else it.toInt() }
//
//fun parseNumbersL(text: String, separator: String = " ") =
//    text.splitToSequence(separator).mapNotNull { if (it.isEmpty()) null else it.toLong() }
//
//
//// https://clojuredocs.org/clojure.core/partition-by
//fun <T, R> Sequence<T>.partitionBy(f: (T) -> R?) = sequence {
//    var pack = mutableListOf<T>()
//
//    val iterator = iterator()
//
//    if (iterator.hasNext()) {
//        var item = iterator.next()
//        var prevPartitioner = f(item)
//        pack.add(item)
//
//        while (iterator.hasNext()) {
//            item = iterator.next()
//            val partitioner = f(item)
//            val theSame = partitioner == prevPartitioner
//            prevPartitioner = partitioner
//
//            if (theSame) {
//                pack.add(item)
//            } else {
//                yield(pack as List<T>)
//                pack = mutableListOf(item)
//            }
//        }
//    }
//
//    if (pack.any()) {
//        yield(pack as List<T>)
//    }
//}

//
//val input =
//    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2023/day05_.txt")
//        .readText()


//data class MappingEntry(val srcRange: LongProgression, val dest: Long)


// let isRangeOverlapping (a, b) (c, d) = c <= b && c >= a

//fun LongProgression.containsValue(value: Long) = value >= this.first && value <= this.last
//
//fun LongProgression.isOverlapping(progression: LongProgression) =
//    (this.containsValue(progression.first) || this.containsValue(progression.last)) ||
//            (progression.containsValue(this.first) || progression.containsValue(this.last))
//
//
//// fun LongProgression.contains(value: Long) = value >= this.first && value <= this.last
//
//fun compareLines(line1: MappingEntry, line2: MappingEntry) =
//    if (line1.srcRange.isOverlapping(line2.srcRange)) 0 else line1.srcRange.first.compareTo(line2.srcRange.first)
//
//
//data class Data(val seed: List<Long>, val mappings: List<List<MappingEntry>>)


//fun loadData(input: String): Data {
//    val lines = input.lineSequence()
//    val seeds = parseNumbersL(lines.first().substringAfter(":")).toList()
//    val mappings = lines.drop(2).partitionBy { it.isEmpty() }
//        .filter { it.size > 1 }
//        .map {
//            it.drop(1).map { l ->
//                parseNumbersL(l).toList().let { (dest, src, len) -> MappingEntry(src..<src + len, dest) }
//            }.sortedWith(::compareLines)
//        }.toList()
//    return Data(seeds, mappings)
//}


//data.mappings.fold(data.seed) { ns, mappings ->
//    ns.map { n ->
//        mappings.binarySearch(Line(n..n, 0), ::compareLines)
//            .let { i -> if (i < 0) n else mappings[i].let { m -> m.dest + n - m.srcRange.first } }
//    }
//}.toList()


// val seed = data.seed.asSequence().chunked(2).flatMap { (start, len) -> start..<start + len }.count()


//data.seed.asSequence().chunked(2).flatMap { (start, len) -> start..<start + len }.count()
//
//val seeds = data.seed.asSequence().chunked(2).flatMap { (start, len) -> start..<start + len }
//
//seeds.minOf(::process)
//
//data.seed.minOf(::process)

//data.seed.asSequence().map(::process).min()

val data = loadData(input)


// data


//val seedRanges = data.seed.asSequence().chunked(2)
//    .map { (start, len) -> MappingEntry(start..<start + len, 0) }.toList().sortedWith(::compareLines)
//    .toList()
//
//
//for (i in seedRanges.indices) {
//    for (j in i + 1..<seedRanges.size) {
//        if (compareLines(seedRanges[i], seedRanges[j]) == 0) {
//            println("nachodzi $i $j")
//        }
//    }
//}
//
//val edgesOfFistMapping = data.mappings.first()
//    .flatMap { e -> sequenceOf(e.srcRange.first, e.srcRange.last + 1) }
//    .filter { n -> seedRanges.binarySearch(MappingEntry(n..n, 0), ::compareLines) >= 0 }
//
//val allSeeds = (seedRanges.asSequence()
//    .flatMap { e -> sequenceOf(e.srcRange.first, e.srcRange.last) } + edgesOfFistMapping).toList()
//
//
//val allallSeeds = data.seed.asSequence().chunked(2).flatMap { (start, len) -> start..<start + len }
//
//data.mappings.fold(allallSeeds) { ns, mappings ->
//    ns.map { n ->
//        mappings.binarySearch(MappingEntry(n..n, 0), ::compareLines)
//            .let { i ->
//                if (i < 0) n else mappings[i].let { m ->
//                    m.dest + n - m.srcRange.first
//                }
//            }
//    }
//}.toList()
//.min()


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


//fun process(seed: Long) =
//    data.mappings.fold(seed) { n, mappings ->
//        mappings.binarySearch(MappingEntry(n..n, 0), ::compareLines)
//            .let { i ->
//                if (i < 0) n else mappings[i].let { m ->
//                    m.dest + n - m.srcRange.first
//                }
//            }
//    }


//fun compareCards(card1: String, card2: String): Int {
//    if (card1 == card2) {
//        return 0
//    }
//
//    val card1G = card1.groupBy { it }
//    val card2G = card2.groupBy { it }
//    val card1M = card1G.maxOf { e -> e.value.size }
//    val card2M = card2G.maxOf { e -> e.value.size }
//    println("card1M=$card1M card2M=$card2M")
//
//    if (card1G == card2G) {
//        return 0
//    }
//
//    return when {
////        card1G.size == card2G.size && card1G.size == 2 -> {
//////            val card1M = card1G.maxOf { e -> e.value.size }
//////            val card2M = card2G.maxOf { e -> e.value.size }
////            when {
////                card1M == card2M -> compareCardsWhenDraw(card1, card2)
////                else -> card1M - card2M
////            }
////        }
//
//        card1M == card2M ->
//            if (card1G.size == card2G.size)
//                compareCardsWhenDraw(card1, card2)
//            else
//                card2G.size - card1G.size
//        //card1G.size == card2G.size -> compareCardsWhenDraw(card1, card2)
//
//        //else -> card2G.size - card1G.size
//        //else -> card1G.maxOf { e -> e.value.size } - card2G.maxOf { e -> e.value.size }
//        else -> card1M - card2M
//    }
//}


// fun compareCards(card1: String, card2: String): Int {
//    if (card1 == card2) {
//        return 0
//    }
//
//    val card1G = card1.groupBy { it }
//    val card2G = card2.groupBy { it }
//    val card1M = card1G.maxOf { e -> e.value.size }
//    val card2M = card2G.maxOf { e -> e.value.size }
//    println("card1M=$card1M card2M=$card2M")
//
//    if (card1G == card2G) {
//        return 0
//    }
//
//    return when {
//        card1M == card2M ->
//            if (card1G.size == card2G.size)
//                compareCardsWhenDraw(card1, card2)
//            else
//                card2G.size - card1G.size
//
//        else -> card1M - card2M
//    }
//}