
class MyPromise<T> implements FunctorType<T>{

  then<TResult>(callback: (value: T) => TResult): MyPromise<TResult> {
    return new MyPromise<TResult>();
  }
}

class MyPromiseFunction implements Functor {
  fmap<TArg, TResult>(f: (item: TArg) => TResult, a: MyPromise<TArg>): MyPromise<TResult> {
    return a.then(v => f(v));
  }
}

var myPromiseType = new MyPromise<string>();
var myPromiseFunction: Functor = new MyPromiseFunction();
myPromiseFunction.fmap((s: string) => s.length, myPromiseType);


interface FunctorType<T> {
}



// class Bla2<T> {
//   returnThis(): this<T>{
//     return this;
//   }
// }

// new Bla2().returnThis()

type Bla<A extends FunctorType<B>, B> = A<B>;

interface Functor<T, F extends FunctorType<T>> {
  fmap<TArg, TResult>(f: (item: TArg) => TResult, a: F<T>): FunctorType<TResult>;
}

class Hej implements FunctorType<Hej>{

}

interface PromiseFunctor {
  fmap<TArg, TResult>(f: (item: TArg) => TResult, a: Promise<TArg>): Promise<TResult>;
}
interface ArrayFunctor {
  fmap<TArg, TResult>(f: (item: TArg) => TResult, a: Array<TArg>): Array<TResult>;
}

interface ArrayFunctor123 {
  fmap<TArg, TResult, TInput extends FunctorType<TArg>, TOutput extends FunctorType<TResult>>(
    f: (item: TArg) => TResult, a: TInput): TOutput;
}

