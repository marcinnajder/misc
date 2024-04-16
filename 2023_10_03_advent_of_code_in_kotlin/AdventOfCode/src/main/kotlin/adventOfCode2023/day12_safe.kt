package adventOfCode2023.day12_safe

import common.parseNumbers
import common.eq

typealias Entry = Pair<String, List<Int>>

fun loadData(input: String): Sequence<Entry> =
    input.lineSequence()
        .map { it.split(' ').let { (left, right) -> Pair(left, parseNumbers(right, ",").toList()) } }

fun process(chars: String, numbers: List<Int>): Long {
    val cache = mutableMapOf<String, Long>()

    fun processChar(cIndex: Int, nIndex: Int, number: Int): Long {
        return cache.getOrPut("$cIndex,$nIndex,$number") currentLambda@{
            val endOfChars = cIndex == chars.length
            val endOfNumbers = nIndex == numbers.size

            fun processDot() =
                when {
                    number == 0 -> processChar(cIndex + 1, nIndex, 0)
                    number != numbers[nIndex] -> 0
                    else -> processChar(cIndex + 1, nIndex + 1, 0)
                }

            fun processHash() =
                if (number + 1 > numbers[nIndex]) 0 else processChar(cIndex + 1, nIndex, number + 1)

            // caution: avoid using "return" keyword inside lambda function because of the way how kotlin works !!
            // - if we remove '@currentLambda' below, everything works the same but the cache is not used :////
            // - such a "return" inside lambda return from the function containing lambda
            // ("process" function in this case and the code after "return" is not executed at all)
            return@currentLambda when {
                endOfChars && endOfNumbers -> 1
                endOfChars -> if ((nIndex == numbers.size - 1) && (number == numbers[nIndex])) 1 else 0
                endOfNumbers -> if (chars[cIndex] == '#') 0 else processChar(cIndex + 1, nIndex, number)
                else -> when (chars[cIndex]) {
                    '.' -> processDot() // finish segment
                    '#' -> processHash() // increment segment length
                    else -> processHash() + processDot()
                }
            }
        }
    }

    return processChar(0, 0, 0)
}

fun puzzle(input: String, transformEntry: (Entry) -> Entry) =
    loadData(input).map(transformEntry).sumOf { (left, right) -> process(left.trim('.'), right) }.toString()

fun puzzle1(input: String) =
    puzzle(input) { it }

fun puzzle2(input: String) =
    puzzle(input) { (left, right) -> Pair((1..5).joinToString("?") { left }, (1..5).flatMap { right }) }

fun tests() {
    process("???.###".trim('.'), listOf(1, 1, 3)) eq 1L
    process(".??..??...?##.".trim('.'), listOf(1, 1, 3)) eq 4L
    process("?#?#?#?#?#?#?#?".trim('.'), listOf(1, 3, 1, 6)) eq 1L
    process("????.#...#...".trim('.'), listOf(4, 1, 1)) eq 1L
    process("????.######..#####.".trim('.'), listOf(1, 6, 5)) eq 4L
    process("?###????????".trim('.'), listOf(3, 2, 1)) eq 10L
}



