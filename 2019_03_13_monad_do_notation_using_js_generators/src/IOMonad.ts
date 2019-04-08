
import { Observable, of } from "rxjs";
import { flatMap } from "rxjs/operators";
import { MonadOperations } from "./monad";
import { timingSafeEqual } from "crypto";

export type IO<T> = { bind<T2>(f: (value: T) => IO<T2>): IO<T2> };
export function return_<T>(value: T): IO<T> {
  return new IOObservable<T>(of(value));
}
export function bind<T1, T2>(m: IO<T1>, f: (value: T1) => IO<T2>): IO<T2> {
  return (m as IOObservable<T1>).bind(f);
}
export function main<T>(io: IO<T>) {
  (io as IOObservable<T>).source$.subscribe();
}


export const ioMonadOps: MonadOperations = {
  return_<T>(value: T): IO<T> {
    return return_(value);
  },
  bind<T1, T2>(m: IO<T1>, f: (value: T1) => IO<T2>): IO<T2> {
    return bind(m, f);
  }
};




class IOObservable<T>{
  constructor(public source$: Observable<T>) {
  }
  bind<T2>(f: (value: T) => IO<T2>): IO<T2> {
    return new IOObservable(flatMap((x: T) => (f(x) as IOObservable<T2>).source$)(this.source$));
  }
}



export function readString(): IO<string> {
  return new IOObservable(new Observable<string>(observer => {
    function handler(data: Buffer) {
      observer.next(data.toString());
      observer.complete();
    }

    process.stdin.on("data", handler);
    return () => process.stdin.removeListener("data", handler)
  }));
}

export function writeString(text: string): IO<void> {
  return new IOObservable(new Observable<void>(observer => {
    process.stdout.write(text, (err) => {
      if (err) {
        observer.error(err);
      } else {
        observer.next();
        observer.complete();
      }
    });
  }));
}
