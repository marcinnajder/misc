This is a toy implementation of "do" notation from Haskell language using generators from ES6.

Lets define a very simplified definition of a `Monad`  type like a type providing two following operations:

```javascript
export interface MonadOperations{
	return_<T>(value: T): Monad<T>;
	bind<T1, T2>(m: Monad<T1>, f: (value: T1) => Monad<T2>): Monad<T2>;
}
```

You probably already know many `Monad` types in JavaScript ecosystem like `Promise`, `Array`, `Iterable`, `Observable`, ... lets see why

```javascript
// Promise
export const promiseMonadOps: MonadOperations = {
    return_<T>(value: T): Promise<T> {
        return Promise.resolve(value);
    },
    bind<T1, T2>(m: Promise<T1>, f: (value: T1) => Promise<T2>): Promise<T2> {
        return m.then(f);
    }
};

//Array
export const arrayMonadOps: MonadOperations = {
    return_<T>(value: T): Array<T> {
        return [value];
    },
    bind<T1, T2>(m: Array<T1>, f: (value: T1) => Array<T2>): Array<T2> {
        return m.flatmap(f);
    }
};
Array.prototype.flatmap = function <T, T2>(this: Array<T>, f: (item: T) => T2[]): T2[] {
    return this.map(f).reduce((p, c) => p.concat(c), []);
};
```

We can even define our own implementation of a very useful `MaybeMonad'` (counterpart of `Optional<T>` or `Nullable<T>` from many other technologies)

```javascript
export type Maybe<T = {}> = ({ type: "just", value: T } | { type: "nothing" })
    & { bind<T2>(f: (value: T) => Maybe<T2>): Maybe<T2> };
export function just<T>(value: T): Maybe<T> {
    return { type: "just", value, bind: bind_ };
}
export function nothing<T>(): Maybe<T> {
    return { type: "nothing", bind: bind_ };
}
function bind_<T1, T2>(this: Maybe<T1>, f: (value: T1) => Maybe<T2>): Maybe<T2> {
    return this.type === "nothing" ? nothing<T2>() : f(this.value);
}

export const maybeMonadOps: MonadOperations = {
    return_<T>(value: T): Maybe<T> {
        return just(value);
    },
    bind<T1, T2>(m: Maybe<T1>, f: (value: T1) => Maybe<T2>): Maybe<T2> {
        return m.bind(f);
    }
};
```

or [Result](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/results) type allowing to store error information in addition to `Maybe` type.

```javascript 
export type Result<T = {}, TError = {}> = ({ type: "ok", value: T } | { type: "error", error: TError })
    & { bind<T2>(f: (value: T) => Result<T2, TError>): Result<T2, TError> };
export function ok<T, TError = {}>(value: T): Result<T, TError> {
    return { type: "ok", value, bind: bind_ };
}
export function error<TError, T = {}>(err: TError): Result<T, TError> {
    return { type: "error", error: err, bind: bind_ };
}
function bind_<T1, T2, TError>(this: Result<T1, TError>, f: (value: T1) => Result<T2, TError>): Result<T2, TError> {
    return this.type === "error" ? error<TError, T2>(this.error) : f(this.value);
}

export const resultMonadOps: MonadOperations = {
    return_<T>(value: T): Result<T> {
        return ok(value);
    },
    bind<T1, T2>(m: Result<T1>, f: (value: T1) => Result<T2>): Result<T2> {
        return m.bind(f);
    }
};
```

So we use `Monad` type every time we write something like this:

```javascript
p = http.get("/api/some-api-1").then( r1 => http.get("/api/some-api-2").then( r2 => Promise.resove(r1 + r2)));

a = people.flatmap(({name,addresses}) => addresses.flatmap(address => [{name, address}] ));
```

`Monad` types are so useful that Haskell language provides a special `do` notation that allows us to avoid writing all those nested callbacks

```haskell
p = do
	r1 <- http.get("/api/some-api-1")
	r2 <- http.get("/api/some-api-2")
	return Promise.resove(r1 + r2)

a = do
	{name,addresses} <- people
	address <- addresses
	return [{name, address}]	
```

`do` notation is using `return` and `bind` functions underneath. Those two notations are equivalent.

But the question is what useful we can do with `Monad` type and `do` notation in JavaScript ? 

```javascript
function tryParse(text: string): Maybe<number> {
    return /^-{0,1}\d+$/.test(text) ? just(parseInt(text)) : nothing<number>();
}
function maybeWithBind() {
    const result = tryParse("10")
        .bind(v1 => tryParse("50").bind(v2 => just(v1 + v2)))
        .bind(v3 => just(v3.toString()));

    console.log(result);
}
```
That's nice, we don't have to handle the lack of value on each step after parsing string value. We have to use many nested callbacks but fortunately ES6 generators can help us.

```javascript
function maybeWithGenerator() {
    function* generator(): Iterator<Maybe> {
        const v1: number = yield tryParse("10");
        const v2: number = yield tryParse("50");
        const v3 = (v1 + v2).toString();
        return just(v3);
    }

    console.log(do_<Maybe<number>>(generator, maybeMonadOps));
}
```

The implementation of `do_` function is fairly simple

```javascript
export function do_<T extends Monad>(generator: () => Iterator<Monad>, ops: MonadOperations): T {
    const iterator = generator();

    const next = (value: any): Monad => {
        const iteratorResult = iterator.next(value);
        if (iteratorResult.done) {
            return (iteratorResult.value || ops.return_(undefined));
        } else {
            return ops.bind(iteratorResult.value, next);
        }
    };

    return next(undefined) as T;
}
```

But if you execute the following code

```javascript
function arrayWithGenerator() {
    function* generator(): IterableIterator<any[]> {
        const i: number = yield [1, 2, 3];
        const j: string = yield ["a", "b"];
        return [i + j];
    }
    const a = do_<number[]>(generator, arrayMonadOps);
    console.log(a);
}
```

you will get `[ '1a', undefined, undefined, undefined ]`, not `[ '1a', '1b', '2a', '2b', '3a', '3b' ]` as you would expect :) The problem is the way how generators work internally, they are cursors forward only. Here code between each `yield`s will be executed many times in a potentially arbitrary order (it depends on the particular implementation of the `Monad` type). With `Promise` and `Maybe` monads, lambda functions passed into `then` or `bind` method were called only once. For `Array`, function passed to the `flatmap` is called many times. But you now what ... we can fix this problem providing a special `do__` method :)

```javascript
export function do__<T extends Monad>(generator: () => Iterator<Monad>, ops: MonadOperations): T {
    function interateForValues(values: any[]) {
        const iterator = generator();
        let iteratorResult: IteratorResult<Monad> | undefined;

        for (const value of values) {
            iteratorResult = iterator.next(value);
            if (iteratorResult.done) {
                return iteratorResult;
            }
        }

        return iteratorResult!;
    }

    function createNext(values: any[]) {
        return (value: any): Monad => {
            const nextValues = [...values, value];
            const iteratorResult = interateForValues(nextValues);

            if (iteratorResult.done) {
                return (iteratorResult.value || ops.return_(undefined));
            } else {
                return ops.bind(iteratorResult.value, createNext(nextValues));
            }
        };
    }

    return createNext([])(undefined) as T;
}
```

I know, I know, ... it's a little hack ;) The generator needs to be iterated many times from the beginning but at least it works. Ok, only in some cases, it's very easy to get a wrong behavior. For instance, by introducing some number variable at the beginning of the generator function and then incrementing it in the middle of the function, we would see that this variable is incremented many times more that it should be.

I hope you have learnt something new. Functional programming is awesome ! (and not as scary as many programmers think).
