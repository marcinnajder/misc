#### Porównanie iteratora do obserwatora

Erik Meijer (twórca Rx i LINQ) wyjaśniał, że `Iterable<T>` (wzorzec iteratora) to właściwie jest to samo co `Observable<T>` (wzorzec obserwatora), ale nikt mu tego nie powiedział. Zawsze pokazywał jak jeden interfejs powstał z drugiego wykonując zupełnie manualne operacja zamiany argumentów z parametrami metod. Tutaj zobaczymy na czym to polega i jak zaimplementować prosto sekwencje i Rx wraz z operatorami.

Uproszona definicja interfejsów wygląda następująco;

```typescript
// Iterable vs Observable
interface Iterable<T> {
  iterator(): Iterator<T>;
}
interface Iterator<T> {
  next(): { done: boolean; value: T };
}

interface Observable<T> {
  subscribe(observer: Observator<T>): () => void;
}
interface Observator<T> {
  onNext(value: T): void;
  onError(error: Error): void;
  onCompleted(): void;
}
```

`Iterable<T>` i `Iterator<T>` posiadają jedynie jedną metodę. Interfejs z jedną metodą można zastąpić funkcją i powstanie coś takiego

```ts
type Iter<T> = () => () => null | T;
```

Aby pobrać kolejny element należy wykonać funkcję `() => null | T`, wrócenie `null` oznacza brak kolejnego elementu. (to jest tylko tymczasowe uproszczenie).

Okazuje się że `Observable<T>` to nic innego jak "odwrócony" `Iterable<T>`:

```ts
type Reverse<F> = F extends (...args: infer Args) => infer Result
  ? (args: Reverse<Result>) => Reverse<Args>
  : F;
type Obs<T> = Reverse<Iter<T>>;
```

Typ pomocniczy `Reverse<F>` odwraca zamienia parametry z argumentami, rekurencyjnie. Przykładowo, dla `Reverse<(arg1: number) => string>` zwróci typ `(args: string) => [arg1: number]`. Dla naszego `Obs<T>` zwróci `(args: (args: Reverse<T> | null) => []) => []`. Możemy na spokojnie zerknąć na ten typ `Obs<T>` oraz typ `Observable<T>`, czy widać podobieństwa ? :)

Teraz możemy zrobić bardziej kompletny przykład. Zaczniemy od przypomnienia sobie czym są "discriminated unions". Wiele języków programowania je wspiera, zerknijmy sobie na taki F#

```fsharp
type Res<'a> =
    | Value of 'a
    | Completed
    | Error of Exception

let results = [ 5; 10; 15 ] |> Seq.map Value // seq<Res<int>>
let firstResult = results |> Seq.head // Res<int>
let text =
    match firstResult with
    | Error err -> "blad: " + err.Message
    | Completed -> "koniec"
    | Value value -> "wartosc: " + value.ToString()
```

Główne cechy "discriminated unions" to

- to taki "enum na sterydach", definiujemy zamknięty zbiór możliwości, ale dodatkowo każda z możliwości może zawierać dodatkowe dane
- każda z możliwości definiuje jednocześnie konstruktor, czyli zwykła funkcję która można wykorzystać do tworzenia typu `Res<'a>`, tutaj widzimy że konstruktor `Value` przekazywany jest do operatora `Seq.map Value`
- wsparcie dla pattern matching, w F# jest to dedykowane wyrażenie `match ... with ...`, musimy obsłużyć wszystkie przypadki aby kod F# skompilował się

To samo możemy zapisać w TS np tak:

```ts
type Res<T> =
  | { type: "error"; err: Error }
  | { type: "completed" }
  | { type: "value"; value: T };
```

Tylko pojawia się pytanie, co z funkcjami konstruktorów i pattern matchingiem? Z pomocą przychodzi biblioteka https://github.com/marcinnajder/powerfp ;) Dostarcza ona pomocnicza funkcję `matchUnion` oraz generator kodu, który na podstawie powyższej definicji typu potrafi wygenerować kod TS dla funkcji konstruktorów. Jeśli nie chcemy korzystać z generatora kodu, możemy wyjść od drugiej strony. Możemy sami napisać konstruktory, następnie biblioteka dostarcza typ `SumType` który wywnioskuje typ `Res<T>` na podstawie konstruktorów.

```ts
const value = <T>(value: T) => ({ type: "value", value } as const);
const completed = { type: "completed" } as const;
const error = (error: Error) => ({ type: "error", err: error } as const);

type Res<T> = SumType<typeof value<T> | typeof completed | typeof error>;

const results: Iterable<Res<number>> = pipe([5, 10, 15], map(value));
const firstResult = pipe(results, find())!; // Res<number>
const text = matchUnion(firstResult, {
  error: ({ err }) => "blad: " + err.message,
  completed: () => "koniec",
  value: ({ value }) => "wartosc: " + value,
});
```

Typ `Res<T>` został wywnioskowany w następujący sposób:

```ts
type Res<T> =
  | {
      readonly type: "completed";
    }
  | {
      readonly type: "value";
      readonly value: T;
    }
  | {
      readonly type: "error";
      readonly err: Error;
    };
```

Teraz zdefiniujmy ponownie typy `Iter<T>` i `Obs<T>` uwzględniając nasz typ `Res<T>`:

```ts
type Iter<T> = () => () => Res<T>;

type Dis = () => void;
type Obs<T> = (sub: (res: Res<T>) => void) => Dis;
```

W przypadku `Iter<T>` za każdym razem gdy chcemy pobrać nową wartość mamy 3 możliwości: brak wartość , wartość lub błąd (exception w przypadku `Iterable<T>`). `Obs<T>` jest właściwie identyczny jak poprzednio, doszedł tylko jeden interfejs `Dis` co oznacza `disposable` (w tym przypadku od-subskrybowanie się). W wielu jeżykach interfejs `Iterator<T>` (nie `Iterable<T>`) implementuje dodatkowo operacje zakończenie iterowania (w .NET i JS to jest wspierane), a więc tutaj nagle pojawienie się `Dis` nie jest kompletnie nieuzasadnione.

Implementacja kilku operatorów dla `Iter<T>`:

```ts
function rangeIter(start: number, count: number): Iter<number> {
  return () => {
    const max = start + count;
    let current = start;
    return () => (current < max ? value(current++) : completed);
  };
}

function fromSeqToIter<T>(items: Iterable<T>): Iter<T> {
  return () => {
    const iterator = items[Symbol.iterator]();
    return next;

    function next(): Res<T> {
      try {
        const res = iterator.next();
        return res.done ? completed : value(res.value);
      } catch (err: any) {
        return error(err);
      }
    }
  };
}

function* fromIterToSeq<T>(items: Iter<T>) {
  const iterator = items();
  let result = iterator();

  while (result.type === "value") {
    yield result.value;
    result = iterator();
  }

  if (result.type === "error") {
    throw result.err;
  }
}

function filterIter<T>(items: Iter<T>, f: (item: T) => boolean): Iter<T> {
  return () => {
    const iterator = items();
    return next;

    function next(): Res<T> {
      try {
        return matchUnion(iterator(), {
          error: (res) => res,
          completed: (res) => res,
          value: (res) => (f(res.value) ? res : next()),
        });
      } catch (err: any) {
        return error(err);
      }
    }
  };
}

function mapIter<T, R>(items: Iter<T>, f: (item: T) => R): Iter<R> {
  return () => {
    const iterator = items();
    return next;

    function next(): Res<R> {
      try {
        const x: Res<R> = matchUnion(iterator(), {
          error: (res) => res,
          completed: (res) => res,
          value: (res) => value(f(res.value)),
        });
        return x;
      } catch (err: any) {
        return error(err);
      }
    }
  };
}

const ys = pipe(
  [1, 2, 3, 4, 5],
  fromSeqToIter,
  (xs) => filterIter(xs, (x) => x % 2 === 0),
  (xs) => mapIter(xs, (x) => x.toString()),
  fromIterToSeq,
  toarray()
);
assert.deepStrictEqual(ys, ["2", "4"]);
```

Implementacja kilku operatorów dla `Obs<T>`:

```ts
function fromSeqToObs<T>(items: Iterable<T>): Obs<T> {
  return (sub) => {
    try {
      for (const item of items) {
        sub(value(item));
      }
      sub(completed);
    } catch (err: any) {
      sub(err);
    } finally {
      return () => {};
    }
  };
}

function fromObsToIter<T>(obs: Obs<T>): Promise<T[]> {
  return new Promise(function (resolve, reject) {
    let buffer: T[] = [];
    const _ = obs((res) => {
      matchUnion(res, {
        error: (res) => reject(res.err),
        completed: () => resolve(buffer),
        value: (res) => buffer.push(res.value),
      });
    });
  });
}

function intervalObs(ms: number): Obs<number> {
  return (sub) => {
    let index = 0;
    const id = setInterval(() => {
      sub(value(index++));
    }, ms);
    return () => {
      sub(completed);
      clearInterval(id);
    };
  };
}

function takeObs<T>(obs: Obs<T>, count: number): Obs<T> {
  if (count === 0) {
    return (sub) => {
      sub(completed);
      return () => {};
    };
  }

  return (sub) => {
    let index = 0;
    const unsub = obs((res) => {
      matchUnion(res, {
        error: sub,
        completed: sub,
        value: (r) => {
          sub(r);
          if (++index === count) {
            unsubscribe();
          }
        },
      });
    });

    function unsubscribe() {
      sub(completed);
      unsub();
    }
    return unsubscribe;
  };
}

function filterObs<T>(obs: Obs<T>, f: (item: T) => boolean): Obs<T> {
  return (sub) => {
    const unsub = obs((res) => {
      matchUnion(res, {
        error: sub,
        completed: sub,
        value: (r) => {
          if (f(r.value)) {
            sub(r);
          }
        },
      });
    });

    function unsubscribe() {
      sub(completed);
      unsub();
    }
    return unsubscribe;
  };
}

function mapObs<T, R>(obs: Obs<T>, f: (item: T) => R): Obs<R> {
  return (sub) => {
    const unsub = obs((res) => {
      matchUnion(res, {
        error: sub,
        completed: sub,
        value: (r) => {
          sub(value(f(r.value)));
        },
      });
    });

    function unsubscribe() {
      sub(completed);
      unsub();
    }
    return unsubscribe;
  };
}

pipe(
  intervalObs(10),
  (xs) => takeObs(xs, 3),
  (xs) => filterObs(xs, (x) => x % 2 === 0),
  (xs) => mapObs(xs, (x) => x.toString()),
  fromObsToIter
).then((items) => assert.deepEqual(items, ["0", "2"]));
```

Na koniec jeszcze fajny trick, funkcja `match` nie zawsze musi przyjmować obsługę przypadków przypadków, można zapisać tak:

```ts
const text = matchUnion(firstResult, {
  error: ({ err }) => "blad: " + err.message,
  _: (res) => "nie blad " + res,
});
```

- linki
  - https://www.developerfusion.com/media/92222/bart-de-smet-minlinq-the-essence-of-linq/ nie ma już dostępnego chyba tego bloga
  - https://github.com/markrendle/FuncLinq "An even more functional version of Bart de Smet's MinLinq"
- generalnie chodzi o pokazanie jak czym właściwie jest Iterable/Observable jak bardzo spokrewnione ze sobą są
