package common

// https://kotlinlang.org/api/latest/jvm/stdlib/kotlin.sequences/
// https://kotlinlang.org/api/latest/jvm/stdlib/kotlin.sequences/-sequence/
// https://adventofcode.com/2020/day/1#part2


fun <T> Sequence<T>.allUniquePairs() = sequence {
    val iterator = iterator()
    var processedItems = emptyLList<T>()

    while (iterator.hasNext()) {
        val current = iterator.next()
        yieldAll(llistToSequence(processedItems).map { Pair(current, it) })
        processedItems = LList(current, processedItems)
    }
}


fun <T> List<T>.allUniqueTriples() = sequence {
    for (i in 0..<size) {
        for (j in 0..<i) {
            for (k in 0..<j) {
                yield(Triple(get(k), get(j), get(i)))
            }
        }
    }
}


typealias LListLenPair<T> = Pair<Int, LList<T>?>

private fun <T> insertThenPartition(n: Int, item: T, lists: LList<LListLenPair<T>>?) =
    if (n == 1)
        Pair(llistOf(llistOf(item)), lists)
    else
        lists.toSequence().fold(
            Pair(emptyLList<LList<T>?>(), LList(Pair(1, llistOf(item)), lists))
        ) { (completed, lists2), (len, lst) ->
            val len2 = len + 1
            val lst2 = LList(item, lst)
            if (len2 == n)
                Pair(LList(lst2, completed), lists2)
            else
                Pair(completed, LList(Pair(len2, lst2), lists2))
        }

fun <T> Sequence<T>.allUniqueTuples(n: Int) =
    scan(
        Pair(emptyLList<LList<T>?>(), emptyLList<LListLenPair<T>>())
    ) { (_, lists), item -> insertThenPartition(n, item, lists) }
        //.flatMap { it.first.toSequence() }
        .flatMap { it ->
            // println(it.first)
            it.first.toSequence()
        }


// https://clojuredocs.org/clojure.core/partition-by
fun <T, R> Sequence<T>.partitionBy(f: (T) -> R?) = sequence {
    var pack = mutableListOf<T>()

    val iterator = iterator()

    if (iterator.hasNext()) {
        var item = iterator.next()
        var prevPartitioner = f(item)
        pack.add(item)

        while (iterator.hasNext()) {
            item = iterator.next()
            val partitioner = f(item)
            val theSame = partitioner == prevPartitioner
            prevPartitioner = partitioner

            if (theSame) {
                pack.add(item)
            } else {
                yield(pack as List<T>)
                pack = mutableListOf(item)
            }
        }
    }

    if (pack.any()) {
        yield(pack as List<T>)
    }
}


fun operatorsTests() {
    sequenceOf(0, 1, 2).allUniquePairs().toList() eq listOf(Pair(1, 0), Pair(2, 1), Pair(2, 0))
    sequenceOf(0, 1).allUniquePairs().toList() eq listOf(Pair(1, 0))
    sequenceOf(0).allUniquePairs().toList() eq listOf<Pair<Int, Int>>()
    sequenceOf<Int>().allUniquePairs().toList() eq listOf<Pair<Int, Int>>()

    listOf(0, 1, 2, 4).allUniqueTriples().toList() eq
            listOf(Triple(0, 1, 2), Triple(0, 1, 4), Triple(0, 2, 4), Triple(1, 2, 4))

    insertThenPartition<Int>(1, 10, emptyLList()) eq Pair(llistOf(llistOf(10)), emptyLList<LListLenPair<Int>>())

    sequenceOf(0, 1, 2, 3).allUniqueTuples(2).map { it.toSequence().toList() }.toList() eq
            listOf(listOf(1, 0), listOf(2, 0), listOf(2, 1), listOf(3, 0), listOf(3, 1), listOf(3, 2))

    sequenceOf(0, 1, 2, 3).allUniqueTuples(3).map { it.toSequence().toList() }.toList() eq
            listOf(listOf(2, 1, 0), listOf(3, 1, 0), listOf(3, 2, 1), listOf(3, 2, 0))


    sequenceOf<Int>().partitionBy { it % 2 == 0 }.toList() eq emptyList<Int>()

    sequenceOf(1, 1, 1, 2, 2, 3, 3).partitionBy { it % 2 == 0 }.toList() eq listOf(
        listOf(1, 1, 1),
        listOf(2, 2),
        listOf(3, 3)
    )
}

