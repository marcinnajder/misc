### Praca z Socket.IO

Biblioteka **socket.io** umożliwia komunikację obustronną pomiędzy klientem a serwerem. W uproszczeniu o komunikacji REST-owej możemy myśleć następująco: nawiązywane jest połączenie, wykonywane jest żądanie, zwracana jest odpowiedź, połączenie jest kończone. Podczas połączenie za pomocą biblioteki **socket.io**, klient łączy się do serwera na cały czas korzystania użytkownika ze strony, następnie w obu kierunkach mogą być wysyłane wiadomości. Najczęściej strona wysyłająca wiadomość nie oczekuje rezultatu, ale zwracanie rezultatu jest także możliwe. Serwer może być połączony jednocześnie do wielu klientów. Przykładowy kod chatu mógłby wyglądać następująco:

```typescript
// serwer
const sockets = [];
server.on("connect", socket => {
    sockets.push(socket);
    socket.on("joinChat", userName => {
        sockets.forEach(s => s.emit("message", `New user ${userName}` ));
    });
    socket.on("message", ({userName, message}) => {
        sockets.forEach(s => s.emit("message", `${userName}: ${message}`)));
    });
});

// klient
const socket = ...;
socket.on("message", text => {
    console.log("New message received: ", text);
});
socket.emit("joinChat", "marcin");
socket.emit("message", { userName: "marcin", message: "hello!" });
```

W dalszej części zobaczymy:

- jak zrozumieć jakie jest właściwie API kiedy kodu jest całkiem sporo i musimy szukać wywołań metod **emit** oraz **on**
- jak w typowany sposób pracować z socket-em o określonym API
- jaki jest model danych przesyłanych, chcielibyśmy mieć obiekty DTO które będą współdzielone pomiędzy serwerem i klientem, chcemy walidować schemat DTO
- czasem po stronie klienta opakowuje sobie socket-y w RxJS, jak wygodnie to robić bez pisania zbędnego kodu
- jak pisać testy jednostkowe kodu korzystającego z socket-ów po stronie serwera i klienta

W pierwszej kolejności w pliku **dtos.d.ts** opisujemy api za pomocą interfejsu, dla naszego przykładu będzie to:

```typescript
interface ChatSocketApi {
  server: {
    joinChat: string;
    message: { userName: string; message: string };
  };
  client: {
    message: string;
  };
}
```

Tutaj zakładamy, że wysyłająca strona nie czeka nad odpowiedź. Gdyby jednak sewer w wyniku otrzymania wiadomości **message** odpowiadał wartością typu **boolean**, to API wyglądałoby następująco:

```typescript
interface ChatSocketApi {
  server: {
    // ...
    message: {
      _req_: { userName: string; message: string };
      _res_: boolean;
    };
  };
  // ...
}
```

W takim przypadku klient i serwer wyglądają następująco:

```typescript
// serwer
server.on("connect", socket => {
    socket.on("message", ({userName, message}, callback) => {
        // ...
        callback(true);
    });
});

// klient
socket.emit("message", { userName: "marcin", message: "hello!"}, booleanResult => { ... } );
```

Cała idea rozwiązania polega na tym, aby jedynie opisać API za pomocą interfejsu TS i właściwie dalej korzystać API socket.io. Po stronie serwera piszemy funkcje _connect_ która przyjmuje **socket**, ale nie typu pochodzącego z biblioteki socket.io, lecz naszego interfejsu opisującego wyrywek API oryginalnej biblioteki w typowany sposób.

```typescript
const sockets = [];
function connect(socket: ServerSocketInterface<ChatSocketApi>) {
    sockets.push(socket);
    socket.on("joinChat", userName => {
        sockets.forEach(s => s.emit("message", `New user ${userName}` ));
    });
    socket.on("message", ({userName, message}) => {
        sockets.forEach(s => s.emit("message", `${userName}: ${message}`)));
    });
}
```

Ostatnią rzeczą jest podłączenie się pod prawdziwy socket za pomocą dostarczonej funkcji **connectOnServer**

```typescript
const namespace = server.of("/chat");
connectOnServer<ChatSocketApi>(
  namespace,
  connect,
  "ChatSocketApi" /* nazwa DTO */
);
```

Nasz funkcja **connect** nie korzysta bezpośrednio z API socket.io, więc podczas testów jednostkowych otrzymujemy mock/stub udający prawdziwy socket na którym możemy wykonywać asercje. W każdym miejscu styku z obiektem **socket** pracujemy w typowany sposób. Wiemy dokładnie jak nazywają się wiadomości oraz jaki schemat mają obiekty DTO. Ewentualna zmiana API powoduje błędy komplikacji. Przed przekazaniem obiektu DTO do obsługi wiadomości (handler przekazany do metody **on**), schemat obiektu DTO jest sprawdzany automatycznie.

Po stronie klienta piszemy zwyczajny kod używający socket.io, ale API jest statycznie typowany względem naszego interfejsu chatu

```typescript
function connect(socket: ClientSocketInterface<ChatSocketApi>) {
    socket.on("connect", () => {
        socket.on("message", text => {
            console.log("New message received: ", text);
        });
        socket.emit("joinChat", "marcin");
        socket.emit("message", { userName: "marcin", message: "hello!" });
    });
}

const socket = ...;
connect(socket);
```

Jeśli ktoś chciałby opakować API socket.io w API korzystające z RxJS, to wykonuje prostą funkcję **createSocketProxy**

```typescript
var proxy = createSocketProxy<ChatSocketApi>(socket)(
  ["joinChat", "message"] /* wybrane wiadomosci czesc serwerowej */,
  ["message"] /* wybrane wiadomosci czesc klienckiej */
);

proxy.on$.message.subscribe((text) => {
  console.log("New message received: ", text);
});
proxy.joinChat("marcin");
proxy.message({ userName: "marcin", message: "hello!" });
```

Bardzo często kod po stronie klienta nie wykorzystuje wszystkich wiadomości. Przykładowo, w video konsultacjach pacjent obsługuje inne wiadomości jak lekarz. Dlatego w tablicy jawnie przekazujemy listę wiadomości. Obiekt **proxy** jest dynamicznie tworzony, posiada metody oraz właściwości typu **Observable<T>**. Wszędzie mamy typowalność.
