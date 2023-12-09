package adventOfCode2023.day07_poker


import common.eq

typealias CardsToPoints = Map<Char, Int>

fun pointsFor(vararg symbols: Char): CardsToPoints =
    symbols.reversed().mapIndexed { index, c -> c to index }.toMap()

val pointsWithoutJoker = pointsFor('A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2')
val pointsWithJoker = pointsFor('A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J')

data class Hand(val text: String, val bid: Int = 0, val originalText: String? = null) {
    val grouped: Map<Char, List<Char>> = text.groupBy { it }
    val maxGroupSize: Int = grouped.maxOf { e -> e.value.size }
}

fun compareHandsWhenDraw(points: CardsToPoints, textHand1: String, textHand2: String) =
    textHand1.zip(textHand2)
        .firstNotNullOf { (c1, c2) -> if (c1 == c2) null else points.getValue(c1) - points.getValue(c2) }

fun createHandComparer(points: CardsToPoints) =
    { hand1: Hand, hand2: Hand ->
        when {
            // hand1.text == hand2.text -> 0
            // <- optimization above cost me a lot of time to find a bug in the scenario with Joker, where
            // we change the cards in "nondeterministic way", lets say hand 1 has "23452" and hand 2 has "2345J"
            // and that can be changed to "2345(2)" (=) or "2345(3)" or "2345(4)" ... to make a "one pair",
            // 23452==2345(2) but  23452!=2345(3)!, working optimization could look like ->
            // hand1.originalText == null && hand2.originalText == null && hand1.text == hand2.text -> 0

            hand1.maxGroupSize == hand2.maxGroupSize -> // possible the same "type" of hands
                if (hand1.grouped.size == hand2.grouped.size) // the same "type"
                    compareHandsWhenDraw(points, hand1.originalText ?: hand1.text, hand2.originalText ?: hand2.text)
                else
                    hand2.grouped.size - hand1.grouped.size // different "type"

            else -> hand1.maxGroupSize - hand2.maxGroupSize // different "type"
        }
    }

fun loadData(input: String) =
    input.lineSequence().map { it.split(" ").let { (textHand, bid) -> Hand(textHand, bid.trim().toInt()) } }

fun puzzle(input: String, points: CardsToPoints, transformHand: (Hand) -> Hand) =
    loadData(input)
        .map(transformHand)
        .sortedWith(createHandComparer(points))
        .mapIndexed { index, card -> (index + 1) * card.bid }
        .sum().toString()


fun puzzle1(input: String) = puzzle(input, pointsWithoutJoker) { it }

fun useJoker(hand: Hand) =
    when {
        hand.text == "JJJJJ" -> hand // .maxBy() below returns exception for empty sequence
        hand.text.contains('J') ->
            hand.grouped.filter { e -> e.key != 'J' }.maxBy { e -> e.value.size }.key
                .let { Hand(hand.text.replace('J', it), hand.bid, hand.text) }

        else -> hand
    }

fun puzzle2(input: String) = puzzle(input, pointsWithJoker, ::useJoker)


fun tests() {
    (compareHandsWhenDraw(pointsWithoutJoker, "AKJ", "AKT") > 0) eq true
    (compareHandsWhenDraw(pointsWithoutJoker, "AKT", "AKJ") < 0) eq true

    (createHandComparer(pointsWithoutJoker)(Hand("AAAA8"), Hand("AA8AA")) == 0) eq false

    val sortedHands = listOf("AAAAA", "AAAA8", "AAA88", "AA882", "23456").map(::Hand)
    for (i in sortedHands.indices) {
        for (j in (i + 1)..<sortedHands.size) {
            (createHandComparer(pointsWithoutJoker)(sortedHands[i], sortedHands[j]) > 0) eq true
        }
    }

    useJoker(Hand("JJJJJ")).text eq "JJJJJ"
    useJoker(Hand("QTJJ6")).text eq "QTQQ6"
    useJoker(Hand("QQQJA")).text eq "QQQQA"
    useJoker(Hand("QQQJJ")).text eq "QQQQQ"
}
