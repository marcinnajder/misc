package common

data class LList<T>(val head: T, val tail: LList<T>?)

fun <T> cons(head: T, tail: LList<T>?) = LList(head, tail) as LList<T>? // returns nullable type

fun <T> emptyLList(): LList<T>? = null

//fun <T> llistToSequence(lst: LList<T>?) = sequence {
//    var node = lst
//    while (node != null) {
//        yield(node.head)
//        node = node.tail;
//    }
//}

fun <T> llistToSequence(lst: LList<T>?): Sequence<T> = sequence {
    if (lst != null) {
        yield(lst.head)
        yieldAll(llistToSequence(lst.tail))
    }
}

fun <T> llistOf(vararg items: T): LList<T>? {
    return items.foldRight(null as LList<T>?) { item, lst -> LList(item, lst) }
}

fun <T> llistSize(lst: LList<T>?): Int = if (lst == null) 0 else 1 + llistSize(lst.tail)

// extensions
fun <T> LList<T>?.toSequence() = llistToSequence(this)
fun <T> LList<T>?.size() = llistSize(this)

fun llistTests() {
    llistToSequence(LList(1, LList(2, null))).toList() eq listOf(1, 2)
    llistToSequence<Int>(null).toList() eq emptyList<Int>()

    emptyLList<Int>() eq llistOf<Int>()
    llistOf(1, 2) eq LList(1, LList(2, null))

    llistSize<Int>(emptyLList<Int>()) eq 0
    llistSize(llistOf(1, 2, 3)) eq 3

    // sample usages
    llistOf(1, 2, 3)?.let { llistSize(it) } // 3
    llistOf(1, 2, 3)?.run { llistSize(this) } // 3
    with(llistOf(1, 2, 3)!!) { print("head: $head") } // LList(head=1, ...)
    llistOf(1, 2, 3)?.apply { print("headŚ: $head") } // LList(head=1, ...)
    llistOf(1, 2, 3)?.also { print("head: $it.head") } // LList(head=1, ...)
}


