package common

import java.lang.AssertionError

infix fun Any?.eq(obj2: Any?): Unit {
    if (this != obj2) {
        throw AssertionError("'$this' <> '$obj2'")
    }
}
