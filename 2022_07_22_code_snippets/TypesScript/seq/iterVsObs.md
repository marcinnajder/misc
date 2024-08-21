#### Porównanie iteratora do obserwatora

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

- linki
  - https://www.developerfusion.com/media/92222/bart-de-smet-minlinq-the-essence-of-linq/ nie ma już dostępnego chyba tego bloga
  - https://github.com/markrendle/FuncLinq "An even more functional version of Bart de Smet's MinLinq"
- generalnie chodzi o pokazanie jak czym właściwie jest Iterable/Observable jak bardzo spokrewnione ze sobą są
