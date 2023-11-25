package adventOfCode2020.day07_bags

import java.util.*

typealias Bag = Pair<String, List<Pair<String, Int>>>

fun parseLine(text: String): Bag {
    val (left, right) = text.split("contain")
    val outerBag = left.substringBefore(" bags")
    val innerBags =
        if ("no other bags" in right)
            emptyList()
        else right.split(",")
            .map { it.split(" ").let { (_, amount, name1, name2) -> Pair("$name1 $name2", amount.toInt()) } }
    return Pair(outerBag, innerBags)
}

fun loadData(input: String) = input.lineSequence().map(::parseLine)


typealias BagsMap = Map<String, List<Pair<String, Int>>>

fun createInnerToOuterBagsMap(bags: Sequence<Bag>): BagsMap =
    bags.flatMap { (outer, inners) -> inners.map { (inner, n) -> Triple(inner, outer, n) } }
        .groupBy({ (inner) -> inner }, { (_, outer, n) -> Pair(outer, n) })

fun createOuterToInnerBagsMap(bags: Sequence<Bag>): BagsMap = bags.toMap()

// BFS (breadth first search)
fun findOuterBags(startingBag: String, bags: BagsMap): Set<String> {
    val result = mutableSetOf<String>()
    val queue: Queue<String> = LinkedList()
    queue.add(startingBag)

    while (queue.isNotEmpty()) {
        bags[queue.poll()!!]?.let { outers ->
            for (outer in outers.asSequence().mapNotNull { (outer, _) -> if (outer in result) null else outer }) {
                result.add(outer)
                queue.add(outer)
            }
        }
    }

    return result as Set<String>  // repl error: type mismatch: inferred type is MutableSet<String> but Set<String> was expected
}

fun puzzle1(input: String): String {
    val data = loadData(input)
    val bagsMap = createInnerToOuterBagsMap(data)
    return findOuterBags("shiny gold", bagsMap).count().toString()
}


fun countInnerBags(bag: String, bags: BagsMap, cache: MutableMap<String, Int>): Int =
    cache.getOrPut(bag) {
        1 + bags.getValue(bag).sumOf { (inner, amount) -> amount * countInnerBags(inner, bags, cache) }
    }

fun puzzle2(input: String): String {
    val data = loadData(input)
    val bagsMap = createOuterToInnerBagsMap(data)
    return (countInnerBags("shiny gold", bagsMap, mutableMapOf()) - 1).toString()
}