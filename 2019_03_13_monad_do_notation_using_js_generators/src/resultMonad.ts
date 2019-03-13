// https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/results

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

