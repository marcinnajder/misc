export type Option<T = {}> = ({ type: "some", value: T } | { type: "none" })
  & { bind<T2>(f: (value: T) => Option<T2>): Option<T2> };

export function some<T>(value: T): Option<T> {
  return { type: "some", value, bind: bind_ };
}

export function none<T>(): Option<T> {
  return { type: "none", bind: bind_ };
}

function bind_<T1, T2>(this: Option<T1>, f: (value: T1) => Option<T2>): Option<T2> {
  return this.type === "none" ? none<T2>() : f(this.value);
}