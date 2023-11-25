import java.lang.AssertionError

// import kotlin.collections.*

// import kotlinx.collections.immutable.*

infix fun Any?.eq(obj2: Any?): Unit {
    if (this != obj2) {
        throw AssertionError("'$this' <> '$obj2'")
    }
}

//
//val items = mutableListOf(1, 2, 3, 4)
//
//for ((index, item) in items.withIndex()) {
//    items[index] = when {
//        index % 2 == 0 -> item * -1
//        else -> continue
//    }
//    println(items)
//}

//  val folderPath = "/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2020"
val input =
    java.io.File("/Volumes/data/github/misc/2023_10_03_advent_of_code_in_kotlin/AdventOfCode/src/main/kotlin/adventOfCode2020/day08.txt").readText()


//enum class OpType { Nop, Acc, Jmp }
//
//data class Op(val type: OpType, val value: Int)
//
//val mapping = listOf("nop" to OpType.Nop, "acc" to OpType.Acc, "jmp" to OpType.Jmp)
//
//fun parseLine(line: String) =
//    mapping.firstNotNullOf { (prefix, type) ->
//        when {
//            line.startsWith(prefix) -> Op(type, line.substringAfter("$prefix ").toInt())
//            else -> null
//        }
//    }
//
//fun loadData(input: String) = input.lines().map(::parseLine)
//
//enum class ResultType { AlreadyVisited, IndexOutOfBound }
//
//fun process(index: Int, acc: Int, visited: Set<Int>, getOp: (Int) -> Op?): Pair<ResultType, Int> =
//    if (index in visited)
//        Pair(ResultType.AlreadyVisited, acc)
//    else
//        getOp(index).let {
//            if (it == null)
//                Pair(ResultType.IndexOutOfBound, acc)
//            else
//                when (it.type) {
//                    OpType.Nop -> process(index + 1, acc, visited + index, getOp)
//                    OpType.Acc -> process(index + 1, acc + it.value, visited + index, getOp)
//                    OpType.Jmp -> process(index + it.value, acc, visited + index, getOp)
//                }
//        }
//
//
//fun puzzle1(input: String): String {
//    val data = loadData(input)
//    return process(0, 0, emptySet()) { data[it] }.second.toString()
//}
//
//// assembler
//fun puzzle2(input: String): String {
//    val data = loadData(input)
//    return data.asSequence()
//        .mapIndexedNotNull { index, (type, value) ->
//            if (type == OpType.Acc) null else Pair(index, Op(if (type == OpType.Nop) OpType.Jmp else OpType.Nop, value))
//        }
//        .firstNotNullOf { (index, op) ->
//            process(0, 0, emptySet()) { i ->
//                when {
//                    i == index -> op
//                    i >= data.count() -> null
//                    else -> data[i]
//                }
//            }.let { (resultType, acc) -> if (resultType == ResultType.IndexOutOfBound) acc else null }
//        }
//        .toString()
//}
//
//
//fun tests() {
//    parseLine("nop +0") eq Op(OpType.Nop, 0)
//    parseLine("acc +1") eq Op(OpType.Acc, 1)
//    parseLine("jmp +4") eq Op(OpType.Jmp, 4)
//}
//

