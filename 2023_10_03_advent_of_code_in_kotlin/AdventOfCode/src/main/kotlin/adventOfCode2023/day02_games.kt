package adventOfCode2023.day02_games

typealias Game = List<List<Pair<String, Int>>>

fun parseLine(line: String): Game =
    line.substringAfter(":").split(";")
        .map { it.split(",").map { p -> p.trim().split(" ").let { (count, color) -> Pair(color, count.toInt()) } } }

fun loadData(input: String) = input.lines().map(::parseLine)

val requiredBags = mapOf("red" to 12, "green" to 13, "blue" to 14)

fun puzzle1(input: String) =
    loadData(input).mapIndexedNotNull { index, game ->
        if (game.asSequence().flatMap { it }.any { it.second > requiredBags.getValue(it.first) })
            null
        else
            index + 1
    }.sum().toString()


fun puzzle2(input: String) =
    loadData(input).sumOf { game ->
        game.asSequence().flatMap { it }.groupBy({ p -> p.first }, { p -> p.second })
            .asSequence().map { (_, counts) -> counts.max() }
            .reduce { a, b -> a * b }
    }.toString()


