#### Porównanie sekwencji do niezmienniczej listy jednokierunkowej

```typescript
// OOP (C++, C#)
class Node<T> {
  constructor(public value: T, public next: Node<T> | null) {}
}
class List1<T> {
  constructor(public first: Node<T> | null) {}
  // add/remove/...()
}
const l1 = new List1(new Node(1, new Node(2, null)));

// FP (SML, Haskell)
type List2<T> = { type: "empty" } | { type: "cons"; head: T; tail: List2<T> };
let l2: List2<number> = {
  type: "cons",
  head: 1,
  tail: { type: "cons", head: 2, tail: { type: "empty" } },
};

// FP (LISP)
type List3<T> = null | readonly [head: T, tail: List3<T>];
let l3: List3<number> = [1, [2, null]];

// Listy leniwe (OCaml, w5.pdf Zdzisław Spławski)
// type a' llist = LNil | LConst of 'a * (unit -> 'a llist)

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

- implementacji list jednokierunkowych
  - w C++ także mamy coś ala `class Node<T> { T value; Node<T> next; }`
  - w LISP, gdzie mamy język dynamiczny (fajnie opisane w książce SICP), lista to para/krotka `(head, tail)` (czyli lista 2 elementowa), dodatkowo mamy wartość `nil` dla pustej listy
  - w ML, gdzie mamy "union types", lista to`type List<'a> = Empty | Cons of head: 'a * tail : List<'a>`, ale to właściwie jest tym samym
  - w Haskell, lista zdefiniowana jest jak w ML, ale tam język jest leniwy więc mamy sekwencję z dodatkowym keszowaniem przeczytanych elementów
- lista jednokierunkowa w JS
  - `type List<T> = null | readonly [head: T, tail: List<T>];` - prosta, rekurencyjna, analogiczna do LISP definicja
  - wydaje się nawet taka definicja może być użyteczna w kodzie JS, możemy dla niej napisać konwersje z/do sekwencji oraz dedykowane operatory `map, filter, ...`
- leniwa lista jednokierunkowa w JS
  - `type LazyList<T> = () => null | readonly [head: T, tail: LazyList<T>];` - taki prosty typ finalnie mi wyszedł
  - ciekawe jest to że z różnych stron myślałem o tym zagadnieniu i finalnie wyszła dokładnie ta sama definicja typu
    - 1. załóżmy że nasz język ma już listę (F#/Clojure), ale zależy nam na leniwości sekwencji, jak to sobie zaimplementować?
    - 2. sekwencja czyli `Iterable` i `Iterator` mutuje obiekt, jak wyglądałaby definicja takiego iterfejsu ale immutable
      - `type PureIterable<T> = { iterator(): PureIterator<T>; }`
      - `type PureIterator<T> = () => null | [value: T, next: PureIterator<T>];`
      - i to jest to samo co wyżej `LazyList<T>` :)
      - a faktycznie przy działaniu immutable, może nawet nie koniecznie trzeba wprowadzać interfejs `PureIterable<T>`, a wystarczą factory metody `fromArrayToLazyList`, `fromMapToLazyList`, `fromIterableToLazyList`
- sekwencja vs niezmiennicza lista jednokierunkowa
  - (chodzi o to że taka lista jest na tyle pozbawiona funkcjonalności że właściwie może nam służyć jedynie do przechodzenia po elementach, tak jak sekwencja)
  - czuje się pewne różnice, gdy pracuje się z listami jednokierunkowymi (np w F#) a sekwencjami (np C#, TS, F#)
    - np kiedy mam kolekcje elementów i muszę wykonać serię operatorów `map/filter/reduce/...` to często dla zrozumienia kodu nie ma znaczenie czy użyje modułu `List` czy `Seq` ale
    - np mam liczby `1,2,3,4` i chciałbym wyliczyć wszystkie unikalne pary np `[1,2],[1,3],[1,4],[2,3],[2,4],[3,4]` to bardzo naturalne wydaje się wielokrotne przechodzenie po tych samych elementach ale tylko lista nam to wygodnie daje
  - sekwencja jest leniwa, lista nie jest (no "tylko" w Haskell jest) i dzięki temu
    - możemy potencjalnie korzystać z nieskończonych sekwencji
    - możemy leniwie pisać kod tzn. rozbijać algorytm na mniejsze reużywalne operacje
  - mutowalność, sekwencja opisana jest za pomocą 2 bytów `Iterable` i `Iterator`, lista za pomocą jednego `List<T>`
    - tworzymy nowy mutowalny obiekt `Iterator` który jednorazowo możemy przejść od początku do końca
    - w przypadku listy możemy się ustawić na dowolnym elemencie, a następnie wielokrotnie przejść elementy do końca
    - możemy (re)używać obiektu listy w wielu operacjach, bo on nie jest zmieniany
    - obiekt `Iterator` używa jedna operacja, ponieważ użycie go w innym miejscu zmieni niespodziewanie jego stan
  - lista jednokierunkowa ma rekurencyjna definicję
    - bardzo często dla listy piszemy rekurencyjny kod
    - dla sekwencji teoretycznie także można pisać rekurencyjny przekazując obiekt `Iterator`, ale z powodu mutacji trzeba być bardzo ostrożnym
  - dostęp do aktualnego elementy
    - jak mam listę do mam zawsze dostęp do głowy, w przypadku sekwencji trzeba wywołać `next()` aby dostać element (ale faktycznie `IEnumerator` posiada property `Current` inaczej jak to jest w JS czy Java)
- na koniec takie przemyślenie
  - jak programuje się z sekwencjami to właściwie jest to programowanie niezmiennicze tzn. korzystamy z dostarczonych operatorów lub funkcji generatorów, wszystko jest leniwe aż do momentu ewaluacji, przekazane lambdy są pure, ...
  - ale działanie pod spodem `Iterable` jest zmiennicze, taki operator `reduce` dla tablic najprawdopodobniej także posiada zmienną lokalną która mutuje, ale to jest jakby szczegół implementacyjny i jest to zamknięte w metodzie
  - w sumie taki kod jako całość jest zmienniczy czy nie ? :)
  - w sumie też widać tutaj, że gdy piszemy kod korzystając z `powerseq` to właściwie programujemy z Monadami, gdzie pojedyncze metody są pure (przyjmują i zwracają `Iterable<T>`, ale na końcu następuje wykonanie
