package adventOfCode2023.day08_nww

import common.cycle

data class Data(val directions: Sequence<Char>, val mapping: Map<String, Pair<String, String>>)

fun loadData(input: String): Data {
    val lines = input.lineSequence()
    val directions = lines.first().asSequence().cycle()
    val mapping = lines.drop(2).map {
        it.split(" = ").let { (from, to) -> to.trim('(', ')').split(", ").let { (l, r) -> from to (l to r) } }
    }.toMap()
    return Data(directions, mapping)
}

fun findFirst(data: Data, initValue: String, condition: (String) -> Boolean) =
    data.directions.scan(initValue) { value, direction ->
        data.mapping.getValue(value).let { (l, r) -> if (direction == 'L') l else r }
    }.withIndex().firstNotNullOf { (index, value) -> if (condition(value)) index else null }

fun puzzle1(input: String) = findFirst(loadData(input), "AAA") { it == "ZZZ" }.toString()


// works correctly but slowly, there are to many iterations because "the step" is small
fun findLeastCommonMultiple(numbers: List<Long>) =
    numbers.max().let { maxValue ->
        sequenceOf(maxValue).cycle().scan(maxValue) { n, m -> n + m }.find { n -> numbers.all { n % it == 0L } }
    }

fun puzzle2(input: String): String {
    val data = loadData(input)
    val numbers = data.mapping.keys.asSequence()
        .filter { it.endsWith("A") }
        .map { initValue -> findFirst(data, initValue) { it.endsWith("Z") }.toLong() }
        .toList()

    // return findLeastCommonMultiple(numbers).toString()
    return findLCMOfListOfNumbers(numbers).toString()
}

// ** https://www.baeldung.com/kotlin/lcm Find Least Common Multiple of Two Numbers in Kotlin
fun findLCM(a: Long, b: Long): Long {
    val larger = if (a > b) a else b
    val maxLcm = a * b
    var lcm = larger
    while (lcm <= maxLcm) {
        if (lcm % a == 0L && lcm % b == 0L) {
            return lcm
        }
        lcm += larger
    }
    return maxLcm
}

fun findLCMOfListOfNumbers(numbers: List<Long>): Long {
    var result = numbers[0]
    for (i in 1..<numbers.size) {
        result = findLCM(result, numbers[i])
    }
    return result
}
// **

// https://calculator-online.net/pl/lcm-calculator/