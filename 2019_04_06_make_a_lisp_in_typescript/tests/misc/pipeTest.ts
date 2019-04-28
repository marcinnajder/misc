
import { map, filter, toarray } from "powerseq";

type F<T1, T2> = (a: T1) => T2;



const r = pipe(
  1,
  _ => _ + 2,
  _ => "a".repeat(_),
  _ => _.toUpperCase(),
);

console.log(r);

const r2 = pipe(
  [1, 2, 3, 4],
  filter(x => x % 2 === 0),
  map(x => "a".repeat(x)),
  toarray()
);

console.log(r2);


function pipe<T1, T2, T3, T4>(a: T1, f1: F<T1, T2>, f2: F<T2, T3>, f4: F<T3, T4>): T4;
function pipe<T1, T2, T3>(a: T1, f1: F<T1, T2>, f2: F<T2, T3>): T3;
function pipe<T1, T2>(a: T1, f1: F<T1, T2>): T2;
function pipe<T1>(a: T1): T1;
function pipe(a: any, ...fs: Function[]) {
  return fs.reduce((prev, el) => el(prev), a);
}