package common

import java.lang.AssertionError

infix fun Any?.eq(obj2: Any?) {
    if (this != obj2) {
        throw AssertionError("'$this' <> '$obj2'")
    }
}
