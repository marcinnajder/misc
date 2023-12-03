package adventOfCode2020.day09_numbers

import common.*


fun loadData(input: String) = input.lineSequence().map { it.toLong() }


typealias SumsMap = MutableMap<Long, LList<Pair<Long, Long>>>

operator fun <T> Pair<T, T>.contains(value: T) = this.first == value || this.second == value

fun computeSumsOfNumbers(sums: SumsMap, numbers: Iterable<Long>, next: Long) {
    for (number in numbers) {
        sums.compute(number + next) { _, v -> LList(Pair(number, next), v) }
    }
}

fun initSumsMap(numbers: Iterable<Long>): SumsMap {
    val result: SumsMap = mutableMapOf()
    var ns = numbers.toLList()

    while (ns !== null) {
        computeSumsOfNumbers(result, ns.tail.toIterable(), ns.head)
        ns = ns.tail
    }

    return result
}

fun removeNumberFromSums(sums: SumsMap, number: Long) {
    for (key in sums.keys.toList()) // iterate over the copy of keys because map can be changed
        sums.computeIfPresent(key) { _, v ->
            if ((v.tail == null) && number in v.head) // in most cases there is only one item in the list
                null
            else
                v.toSequence().let { s ->
                    if (s.any { number in it }) s.filter { number !in it }.toLList() else v
                }
        }
}


fun findAllInvalidNumbers(numbers: Sequence<Long>, bufferSize: Int) = sequence {
    val buffer = numbers.take(bufferSize).toMutableList()
    val sums = initSumsMap(buffer)
    var index = 0

    for (nextNumber in numbers.drop(bufferSize)) {
        if (nextNumber !in sums) {
            yield(nextNumber)
        }

        removeNumberFromSums(sums, buffer[index])
        computeSumsOfNumbers(sums, buffer.filterIndexed { i, _ -> i != index }, nextNumber)
        buffer[index] = nextNumber
        index = (index + 1) % bufferSize
    }
}

fun puzzle1(input: String): String {
    val bufferSize = 25
    val data = loadData(input)
    val invalidNumber = findAllInvalidNumbers(data, bufferSize).first()
    return invalidNumber.toString()
}


fun findContiguousNumbersSummingToValue(numbers: List<Long>, value: Long): Pair<Int, Int> {
    val sums = MutableList(numbers.count()) { 0L }
    sums[0] = numbers[0]
    var startIndexCurrent = 0

    for (endIndex in 1..<numbers.count()) {
        val endSum = sums[endIndex - 1] + numbers[endIndex]
        sums[endIndex] = endSum

        for (startIndex in startIndexCurrent..<endIndex) {
            val diffSum = if (startIndex == 0) endSum else endSum - sums[startIndex - 1]

            if (diffSum == value) {
                return Pair(startIndex, endIndex)
            }

            if (diffSum < value) {
                startIndexCurrent = startIndex
                break;
            }
        }
    }

    throw Exception("Numbers not found")
}

fun puzzle2(input: String): String {
    val bufferSize = 25
    val data = loadData(input).toList()
    val invalidNumber = findAllInvalidNumbers(data.asSequence(), bufferSize).first()
    val (start, end) = findContiguousNumbersSummingToValue(data, invalidNumber)
    val (min, max) = data.asSequence().drop(start).take(end - start + 1)
        .fold(Pair(Long.MAX_VALUE, Long.MIN_VALUE))
        { (min, max), item -> Pair(item.coerceAtMost(min), item.coerceAtLeast(max)) }
    return (min + max).toString()
}


fun tests() {
    val map1 = initSumsMap(listOf(1, 2, 3, 4, 5)).apply {
        removeNumberFromSums(this, 3)
    }

    map1.contains(4) eq false // 1 + '3' = 4
    map1.contains(8) eq false // 5 + '3' = 8
    map1.values.flatMap { it.toSequence() }.any { 3 in it } eq false

    findContiguousNumbersSummingToValue(listOf(1, 2, 3, 4, 5), 3) eq Pair(0, 1)
    findContiguousNumbersSummingToValue(listOf(1, 2, 3, 4, 5), 6) eq Pair(0, 2)
}


