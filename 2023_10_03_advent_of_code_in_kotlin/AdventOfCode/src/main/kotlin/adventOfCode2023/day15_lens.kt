package adventOfCode2023.day15_lens

import common.eq

fun loadData(input: String) =
    input.splitToSequence(",")

fun decode(code: String) =
    code.fold(0) { sum, c -> ((sum + c.code) * 17) % 256 }

fun puzzle1(input: String) =
    loadData(input).sumOf(::decode).toString()

fun parseCode(code: String) =
    when {
        code.endsWith("-") -> Pair(code.substringBefore("-"), null)
        else -> code.split("=").let { (label, len) -> Pair(label, len.toInt()) }
    }

fun puzzle2(input: String) =
    loadData(input).map(::parseCode) // LinkedHashMap<K, V> preserves the order of insertion
        .fold(MutableList(256) { linkedMapOf<String, Int>() }) { boxes, (label, length) ->
            boxes.apply {
                // remove entry when 'length==null', add or update entry otherwise
                get(decode(label)).compute(label) { _, _ -> length }
            }
        }.asSequence().flatMapIndexed { boxId, lens ->
            lens.asSequence().mapIndexed { slotId, (_, length) -> (boxId + 1) * (slotId + 1) * length }
        }.sum().toString()


fun tests() {
    parseCode("rn=1") eq Pair("rn", 1)
    parseCode("cm-") eq Pair("cm", null)
    parseCode("pc=4") eq Pair("pc", 4)
}


