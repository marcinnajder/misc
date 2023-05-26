
#### Obiektowość 

w obiektowości łączymy dane (pola w klasach) i zachowanie (metody) w klasy

```typescript
abstract class Shape {
    abstract getArea(): number;
}

class Square extends Shape {
    constructor(public size: number) {
        super();
    }
    getArea() {
        return Math.pow(this.size, 2);
    }
}

class Rectangle extends Shape {
    constructor(public width: number, public height: number) {
        super();
    }
    getArea() {
        return this.width * this.height;
    }
}

let shapes = [new Square(3), new Rectangle(2, 3)];
for (const shape of shapes) {
    console.log("area: ", shape.getArea());
}
```

#### Funkcyjność

Funkcyjnie separujemy dane i zachowanie

```typescript 
abstract class Shape { }

class Square extends Shape {
    constructor(public size: number) {
        super();
    }
}

class Rectangle extends Shape {
    constructor(public width: number, public height: number) {
        super();
    }
}

function getArea(shape: Shape) {
    if (shape instanceof Square) {
        return Math.pow(shape.size, 2);
    }
    if (shape instanceof Rectangle) {
        return shape.width * shape.height;
    }
    throw new Error("unknown shape");
}

let shapes = [new Square(3), new Rectangle(2, 3)];
for (const shape of shapes) {
    console.log("area: ", getArea(shape));
}
```



#### Obiektowość vs funkcyjność

- reguła sOlid "Open Closed Principle" - "rozwiązanie jest otwarte na rozszerzenia, ale zamknięte na modyfikacje"
    - jakby bez zmian istniejącego kodu mogę wygodnie dodać nową funkcjonalność
- w podejściu obiektowym
    - łatwo można dodać nowe dane - wystarczy dodać klasę która dziedziczy po bazowej np. nowa figura `Circle extends Shape { ... }`
    - trudno dodać nowe zachowanie - brak OCP, dodanie nowej metody abstrakcyjnej wymaga zmiany wszystkich klas dziedziczących
- w podejściu funkcyjnym
    - łatwo można dodać nowe zachowanie - wystarczy dodać nową funkcję np `function drawShape(shape: Shape) { ... }`
    - trudno dodać nowe dane - brak OCP, trzeba zmienić wszystkie funkcje dodając obsługę nowego kształtu
- ogólnie problem to https://en.wikipedia.org/wiki/Expression_problem


#### Visitor Pattern

-   w książce https://craftinginterpreters.com/representing-code.html pada zdanie:
    -   The Visitor pattern lets you emulate the functional style in an object-oriented language
- chodzi o to abyśmy wygodnie mogli dodawać nowe zachowanie (nowe funkcje/metody) do istniejącej "struktury danych" 
    - możemy wygodnie dodawać "nowe zachowanie", ale komplikuje się nam dodawanie "nowych danych" (musimy zmienić definicję kodu visitora)
- ciekawym elementem wzorca visitora jest implementacji metod `accept(visit)` gdzie każda z implementacji zna specyficzny typ przekazując go jako `this` w sposób statycznie typowany

```typescript
abstract class ShapeVistor<T> {
    abstract visitSquare(square: Square): T;
    abstract visitRectangle(rectangle: Rectangle): T;
}

abstract class Shape {
    abstract accept<T>(visitor: ShapeVistor<T>): T;
}

class Square extends Shape {
    constructor(public size: number) {
        super();
    }
    accept<T>(visitor: ShapeVistor<T>): T {
        return visitor.visitSquare(this);
    }
}

class Rectangle extends Shape {
    constructor(public width: number, public height: number) {
        super();
    }
    accept<T>(visitor: ShapeVistor<T>): T {
        return visitor.visitRectangle(this);
    }
}

class GetAreaVisitor extends ShapeVistor<number>{
    visitSquare(square: Square): number {
        return Math.pow(square.size, 2);
    }
    visitRectangle(rectangle: Rectangle): number {
        return rectangle.width * rectangle.height;
    }
}

class ToStringVisitor extends ShapeVistor<string>{
    visitSquare(square: Square): string {
        return `Square: ${square.size}`;
    }
    visitRectangle(rectangle: Rectangle): string {
        return `Rectangle: ${rectangle.width}, ${rectangle.height}`;
    }
}


let shapes = [new Square(3), new Rectangle(2, 3)];

let getAreaVisitor = new GetAreaVisitor();
for (const shape of shapes) {
    console.log("area: ", shape.accept(getAreaVisitor));
}

let toStringVisitor = new ToStringVisitor();
for (const shape of shapes) {
    console.log(shape.accept(toStringVisitor));
}

```


#### Packets z wykorzystaniem `instanceOf`


- w zadaniu https://adventofcode.com/2022/day/13 mamy 2 rodzaje pakietów danych (kolekcja licz vs pojedyncza liczba)
- przykładowa implementacja obiektowa mogła by korzystać z operatora `instanceOf` 
    - tylko to "śmierdzi" bo jest to jakby "podejściem funkcyjnym" (gdzie sprawdzamy w jakiś sposób typ i wykonujemy odpowiednie zachowanie), ale obietnicą obiektowości jest polimorfizm (wirtualne metody) które dopasują odpowiednie zachowanie do odpowiedniego typu
    - ale tutaj nie chodzi jedynie o użycie wprost operatora `instanceOf` ale każde inne rozwiazanie gdzie sprawdzamy/rejestrujemy typ i dla niego zachowanie, jest "śmierdzące"
        - np rzutowanie obiektu na dany typ i sprawdzanie czy nie jest `null` 
        - dodanie wartości wyliczeniowej enum dla każdego z typu 
        - dodaje Mapy która dla danego typu wykonują daną akcję
        - ...

```typescript
function compareNumber(a: number, b: number) {
    return a - b; // return a === b ? 0 : a < b ? -1 : 1;
}

abstract class Packet {
    abstract compare(p: Packet): number;
}

class ValuePacket extends Packet {
    constructor(public value: number) {
        super();
    }
    compare(p: Packet): number {
        return p instanceof ValuePacket
            ? compareNumber(this.value, p.value)
            : new ListPacket([this]).compare(p);
    };
}

class ListPacket extends Packet {
    constructor(public items: Packet[]) {
        super();
    }
    compare(p: Packet): number {
        const items = p instanceof ListPacket ? p.items :
            (p instanceof ValuePacket ? [p] : null);

        for (let i = 0; i < Math.min(this.items.length, items!.length); i++) {
            let result = this.items[i].compare(items![i]);
            if (result !== 0) {
                return result;
            }
        }
        return compareNumber(this.items.length, items!.length);
    };
}

```

#### Packets bez wykorzystania `instanceOf`

- inspirując się wzorcem visitor można pozbyć "smrodu`instanceOf`" 
- rozwiązanie o tyle ciekawe że prowadzając nowy rodzaj pakietu, dodajemy nową i kompilator w odpowiednich miejscach będzie kazał nam dopisać metodę  -> no ale niestety nie mamy tutaj "Open Close Principle" :)
- ale jakie rozwiązanie byłoby idealne **(tylko jak to zrobić ????)**
    - w zadaniu AoC 13 mamy 2 rodzaje pakietów, lista i pojedynczy element, chciałbym móc dodać kolejny rodzaj pakietu np `SuperDuperPacket extends Packet {... }` z jakimiś zadanymi regułami porównania "ten pakiet jest zawsze większy od tego do którego jest porównywany, ale jest taki sam jak on sam"
    - w tym przykładzie tutaj pakiet zawierający pojedynczą wartość może być skonwertowany do pakietu zawierających listę pakietów ... ale to jest szczegół implementacyjny tych 2 pakietów, ja powinienem móc dodać dowolny nowy pakiet który nie jest konwertowalny do innych, chce jakby móc napisać funkcje `compare(packat1 Packet1, packet2 Packet2): number { ... }`
    - chce dodać nowy pakiet bez modyfikacji istniejącego kodu oraz bez wprowadzanie rozwiązań opisywanych wyżej jako "śmierdzące" :) rozwiązanie powinno być "statycznie typowane"
    

```typescript
abstract class Packet {
    abstract compare(p: Packet): number;

    abstract compareToValue(p: ValuePacket): number;
    abstract compareToList(p: ListPacket): number;
}

class ValuePacket extends Packet {
    constructor(public value: number) {
        super();
    }
    compare(p: Packet): number {
        return p.compareToValue(this) * -1;
    };

    compareToValue(p: ValuePacket): number {
        return compareNumber(this.value, p.value);
    }
    compareToList(p: ListPacket): number {
        return new ListPacket([this]).compare(p);
    }
}

class ListPacket extends Packet {
    constructor(public items: Packet[]) {
        super();
    }
    compare(p: Packet): number {
        return p.compareToList(this) * -1;
    }
    compareToValue(p: ValuePacket): number {
        return this.compare(new ListPacket([p]));
    }
    compareToList(p: ListPacket): number {
        for (let i = 0; i < Math.min(this.items.length, p.items.length); i++) {
            let result = this.items[i].compare(p.items[i]);
            if (result !== 0) {
                return result;
            }
        }
        return compareNumber(this.items.length, p.items.length);
    }
}
```
