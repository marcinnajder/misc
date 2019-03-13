
import { just, Maybe } from "./maybeMonad";
import { Result, ok } from "./resultMonad";
import { MonadOperations } from "./monad";

export const maybeMonadOps: MonadOperations = {
    return_<T>(value: T): Maybe<T> {
        return just(value);
    },
    bind<T1, T2>(m: Maybe<T1>, f: (value: T1) => Maybe<T2>): Maybe<T2> {
        return m.bind(f);
    }
};

export const resultMonadOps: MonadOperations = {
    return_<T>(value: T): Result<T> {
        return ok(value);
    },
    bind<T1, T2>(m: Result<T1>, f: (value: T1) => Result<T2>): Result<T2> {
        return m.bind(f);
    }
};


export const promiseMonadOps: MonadOperations = {
    return_<T>(value: T): Promise<T> {
        return Promise.resolve(value);
    },
    bind<T1, T2>(m: Promise<T1>, f: (value: T1) => Promise<T2>): Promise<T2> {
        return m.then(f);
    }
};

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



export const iterableMonadOps: MonadOperations = {
    return_<T>(value: T): Iterable<T> {
        return (function* () {
            yield value;
        })();
    },
    bind<T1, T2>(m: Iterable<T1>, f: (value: T1) => Iterable<T2>): Iterable<T2> {
        return flatmap(m, f);
    }
};


export function* flatmap<T1, T2>(m: Iterable<T1>, f: (value: T1) => Iterable<T2>): Iterable<T2> {
    for (const item of m) {
        yield* f(item);
    }
}
