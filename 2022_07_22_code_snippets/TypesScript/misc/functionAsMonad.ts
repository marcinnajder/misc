// - monad type definition
type TArg = number;
type Func<T> = (arg: TArg) => T;

// - monad functions
const return_ = <T>(value: T): Func<T> => _ => value;
const map = <T, R>(m: Func<T>, f: (arg: T) => R): Func<R> => arg => f(m(arg))
const bind = <T, R>(m: Func<T>, f: (arg: T) => Func<R>): Func<R> => arg => f(m(arg))(arg)


// - example using monad function
const m1 = return_("abc"); // Func<string>
const m2 = bind(m1, s => context => s.repeat(context)); // Func<string>
const m3 = bind(m2, s => context => Array(context).fill(s)); // Func<string[]>
console.log(m3(3)); // -> ["abcabcabc", "abcabcabc", "abcabcabc"]


//- example using JS generators like Haskell "do notation"
const repeatChars = (s: string): Func<string> => context => s.repeat(context);
const repeatItems = (s: string): Func<string[]> => context => Array(context).fill(s);

function* doNotation(text: string) {
    const s: string = yield repeatChars(text);
    const result: string[] = yield repeatItems(s);
    return result;
}

function executeDoNotation<T>(iterable: Iterable<Func<any>>): Func<T> {
    return context => {
        const iterator = iterable[Symbol.iterator]();
        let iteratorResult: IteratorResult<Func<any>>;
        let result: any;

        while (!(iteratorResult = iterator.next(result)).done) {
            result = iteratorResult.value(context);
        }

        return result as T;
    }
}

const m4 = executeDoNotation2<string[]>(doNotation("abc") as any); // Func<string[]>
console.log(m4(3)); // -> ["abcabcabc", "abcabcabc", "abcabcabc"] 



// even better implementation using only monad functions
function executeDoNotation2<T>(iterable: Iterable<Func<any>>): Func<T> {
    const iterator = iterable[Symbol.iterator]();
    return next(undefined);

    function next(value: any): Func<any> {
        const result = iterator.next(value)
        return result.done ? return_(value) : bind(result.value, next);
    }
}

const m5 = executeDoNotation2<string[]>(doNotation("abc") as any); // Func<string[]>
console.log(m5(3)); // -> ["abcabcabc", "abcabcabc", "abcabcabc"] 

