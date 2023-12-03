package adventOfCode2020.day08_assembler

import common.*

enum class OpType { Nop, Acc, Jmp }

data class Op(val type: OpType, val value: Int)

val mapping = listOf("nop" to OpType.Nop, "acc" to OpType.Acc, "jmp" to OpType.Jmp)

fun parseLine(line: String) =
    mapping.firstNotNullOf { (prefix, type) ->
        when {
            line.startsWith(prefix) -> Op(type, line.substringAfter("$prefix ").toInt())
            else -> null
        }
    }

fun loadData(input: String) = input.lines().map(::parseLine)

enum class ResultType { AlreadyVisited, IndexOutOfBound }

fun run(index: Int, acc: Int, visited: Set<Int>, getOp: (Int) -> Op?): Pair<ResultType, Int> =
    when {
        index in visited -> Pair(ResultType.AlreadyVisited, acc)
        else -> getOp(index).let {
            when {
                it == null -> Pair(ResultType.IndexOutOfBound, acc)
                else -> when (it.type) {
                    OpType.Nop -> run(index + 1, acc, visited + index, getOp)
                    OpType.Acc -> run(index + 1, acc + it.value, visited + index, getOp)
                    OpType.Jmp -> run(index + it.value, acc, visited + index, getOp)
                }
            }
        }
    }


fun puzzle1(input: String): String {
    val data = loadData(input)
    return run(0, 0, emptySet()) { data[it] }.second.toString()
}


fun puzzle2(input: String): String {
    val data = loadData(input)
    return data.asSequence()
        .mapIndexedNotNull { i, (type, value) ->
            if (type == OpType.Acc) null else Pair(i, Op(if (type == OpType.Nop) OpType.Jmp else OpType.Nop, value))
        }
        .firstNotNullOf { (i, op) ->
            run(0, 0, emptySet()) { index ->
                when {
                    index == i -> op
                    else -> data.getOrNull(index)
                }
                //}.let { (resultType, acc) -> if (resultType == ResultType.IndexOutOfBound) acc else null }
            }.let { (resultType, acc) -> acc.takeIf { resultType == ResultType.IndexOutOfBound } }
        }
        .toString()
}


fun tests() {
    parseLine("nop +0") eq Op(OpType.Nop, 0)
    parseLine("acc +1") eq Op(OpType.Acc, 1)
    parseLine("jmp +4") eq Op(OpType.Jmp, 4)
}


