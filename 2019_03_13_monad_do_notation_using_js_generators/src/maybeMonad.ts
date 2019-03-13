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