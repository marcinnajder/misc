package adventOfCode2020.day03_trees

fun loadData(input: String) = input.lines()

fun countTrees(lines: List<String>, xLength: Int, yLength: Int, xStep: Int, yStep: Int): Int {
    val ys = (0..<yLength step yStep).asSequence()
    val xs = generateSequence(0) { i -> (i + xStep).let { if (it < xLength) it else (it % xLength) } }
    return ys.zip(xs) { y, x -> lines[y][x] }.drop(1).count { it == '#' }
}

fun puzzle(input: String, slopes: Sequence<Pair<Int, Int>>): Long {
    val lines = loadData(input)
    val xLength = lines[0].length
    val yLength = lines.size
    return slopes.map { (xStep, yStep) ->
        countTrees(lines, xLength, yLength, xStep, yStep).toLong()
    }.fold(1L) { a, b -> a * b }
}

fun puzzle1(input: String) = puzzle(input, sequenceOf(3 to 1))
fun puzzle2(input: String) = puzzle(input, sequenceOf(1 to 1, 3 to 1, 5 to 1, 7 to 1, 1 to 2))
