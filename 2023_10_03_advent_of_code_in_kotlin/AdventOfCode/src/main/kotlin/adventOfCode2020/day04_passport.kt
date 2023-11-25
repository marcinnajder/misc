package adventOfCode2020.day04_passport

typealias Passport = Map<String, String>

fun loadData(input: String): Sequence<Passport> = sequence {
    var pairs = emptySequence<Pair<String, String>>()

    for (line in input.lineSequence()) {
        if (line.isBlank()) {
            yield(pairs.toMap())
            pairs = emptySequence()
        } else {
            pairs += line.splitToSequence(" ").map { it.split(":").let { (key, value) -> Pair(key, value) } }
        }
    }

    yield(pairs.toMap())
}

fun puzzle(input: String, validator: (Passport) -> Boolean) =
    loadData(input).count(validator).toString()

fun puzzle1(input: String) = puzzle(input) {
    it.size == 8 || (it.size == 7 && !it.containsKey("cid"))
}

val eyeColors = setOf("amb", "blu", "brn", "gry", "grn", "hzl", "oth")

val validators = listOf<Pair<String, (String) -> Boolean>>(
    "byr" to { it.toIntOrNull() in 1920..2002 },
    "iyr" to { it.toIntOrNull() in 2010..2020 },
    "eyr" to { it.toIntOrNull() in 2020..2030 },
    "hgt" to {
        when {
            it.endsWith("cm") -> it.substringBefore("cm").toIntOrNull() in 150..193
            it.endsWith("in") -> it.substringBefore("in").toIntOrNull() in 59..76
            else -> false
        }
    },
    "hcl" to { it[0] == '#' && it.drop(1).all { c -> c in '0'..'9' || c in 'a'..'f' } },
    "ecl" to { eyeColors.contains(it) },
    "pid" to { it.length == 9 && it.all { c -> c in '0'..'9' } }
)

fun puzzle2(input: String) = puzzle(input) {
    validators.all { (key, validator) ->
        it[key].let { value -> if (value == null) false else validator(value) }
    }
}