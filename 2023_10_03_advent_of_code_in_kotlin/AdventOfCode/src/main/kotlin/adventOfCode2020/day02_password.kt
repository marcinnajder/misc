package adventOfCode2020.day02_password

import common.*

data class PasswordEntry(val min: Int, val max: Int, val char: Char, val password: String)

fun parseLine(line: String) =
    line.split(" ").let { (rangePart, charPart, passwordPart) ->
        val (min, max) = rangePart.split("-").map { it.toInt() }
        PasswordEntry(min, max, charPart.substringBefore(':')[0], passwordPart)
    }


fun loadData(input: String) = input.lineSequence().map(::parseLine)

fun validPassword1(entry: PasswordEntry): Boolean {
    var counter = 0;
    for (c in entry.password.filter { it == entry.char }) {
        counter++
        if (counter > entry.max) {
            return false
        }
    }
    return counter >= entry.min
}

fun validPassword2(entry: PasswordEntry) =
    (entry.password[entry.min - 1] == entry.char) xor (entry.password[entry.max - 1] == entry.char)


fun puzzle(input: String, validator: (PasswordEntry) -> Boolean) = loadData(input).count(validator).toString()

fun puzzle1(input: String) = puzzle(input, ::validPassword1)
fun puzzle2(input: String) = puzzle(input, ::validPassword2)


fun tests() {
    parseLine("1-3 a: abcde") eq PasswordEntry(1, 3, 'a', "abcde")

    validPassword1(parseLine("1-3 a: xx")) eq false
    validPassword1(parseLine("1-3 a: xax")) eq true
    validPassword1(parseLine("1-3 a: xaaax")) eq true
    validPassword1(parseLine("1-3 a: xaaaax")) eq false

    validPassword2(parseLine("1-3 a: abcde")) eq true
    validPassword2(parseLine("1-3 b: cdefg")) eq false
    validPassword2(parseLine("2-9 c: ccccccccc")) eq false
}