package adventOfCode2020.day06_customs

import common.*


fun loadData(input: String) =
    listOf("").let { splitter -> input.lineSequence().partitionBy { it.isEmpty() }.filter { it != splitter } }

fun puzzle(input: String, counter: (List<String>) -> Int) =
    loadData(input).sumOf(counter).toString()

fun puzzle1(input: String) =
    puzzle(input) { it.flatMap { str -> str.asSequence() }.distinct().count() }

fun puzzle2(input: String) =
    puzzle(input) { it.map { str -> str.toSet() }.reduce { set1, set2 -> set1.intersect(set2) }.size }


