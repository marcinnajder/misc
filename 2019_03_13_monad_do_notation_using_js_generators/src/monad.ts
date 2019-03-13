export interface Monad<T = {}> { }

export interface MonadOperations {
    return_<T>(value: T): Monad<T>;
    bind<T1, T2>(m: Monad<T1>, f: (value: T1) => Monad<T2>): Monad<T2>;
}


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

