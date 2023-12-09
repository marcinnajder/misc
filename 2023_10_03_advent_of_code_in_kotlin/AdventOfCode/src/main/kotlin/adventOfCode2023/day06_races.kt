package adventOfCode2023.day06_races

import common.parseNumbers

data class Race(val time: Long, val distance: Long)

fun loadData(input: String) =
    input.lines().map { parseNumbers(it.substringAfter(":")) }
        .let { (times, distances) -> times.zip(distances) { t, d -> Race(t.toLong(), d.toLong()) } }

fun countWinningConfigs(race: Race) =
    (1..<race.time).asSequence().map { (race.time - it) * it }
        .dropWhile { it <= race.distance }.takeWhile { it > race.distance }.count()

fun puzzle(input: String, transformRaces: (races: Sequence<Race>) -> Sequence<Race>) =
    loadData(input).let(transformRaces).map(::countWinningConfigs).filter { it > 0 }.reduce { a, b -> a * b }.toString()

fun puzzle1(input: String) = puzzle(input) { it }

fun fixRaces(races: Sequence<Race>) = races
    .fold(Pair(emptySequence<Long>(), emptySequence<Long>())) { (ts, ds), (t, d) -> Pair(ts + t, ds + d) }
    .let { (t, d) -> sequenceOf(Race(t.joinToString("").toLong(), d.joinToString("").toLong())) }

fun puzzle2(input: String) = puzzle(input, ::fixRaces)
