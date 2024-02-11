package adventOfCode2023.day04_cards

import common.eq
import common.parseNumbers

data class Card(val winning: Set<Int>, val numbers: Set<Int>)

fun parseLine(line: String) =
    line.substringAfter(":").split("|")
        .let { (winning, numbers) -> Card(parseNumbers(winning).toSet(), parseNumbers(numbers).toSet()) }

fun loadData(input: String) = input.lineSequence().map(::parseLine)

fun calcPoints(n: Int): Int = if (n > 1) 2 * calcPoints(n - 1) else n

fun countWinningNumbers(card: Card) = card.winning.intersect(card.numbers).count()

fun puzzle1(input: String) = loadData(input).sumOf { calcPoints(countWinningNumbers(it)) }.toString()

fun puzzle2(input: String): String {
    val cards = loadData(input).toList()
    val amountsOfCards = MutableList(cards.size) { 1 }

    for ((index, card) in cards.withIndex()) {
        val result = countWinningNumbers(card)
        val amountOfCards = amountsOfCards[index]

        for (i in (index + 1)..<(index + 1 + result).coerceAtMost(cards.size)) {
            amountsOfCards[i] += amountOfCards
        }
    }

    return amountsOfCards.sum().toString()
}


fun tests() {
    calcPoints(0) eq 0
    calcPoints(1) eq 1
    calcPoints(2) eq 2
    calcPoints(3) eq 4
    calcPoints(4) eq 8
}