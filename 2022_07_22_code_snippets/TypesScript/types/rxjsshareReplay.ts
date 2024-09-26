import { defer, delay, every, filter, from, interval, lastValueFrom, multicast, Observable, of, ReplaySubject, share, shareReplay, ShareReplayConfig, take, tap, TapObserver, timer } from "rxjs";

// uruchamiac pojedyncze metody bo kazda z nich wypisuje cos na ekran i mozna sie pgubic odpalajac wiele

// testShareInfiniteIntervalWithoutRefCount();
// testShareInfiniteIntervalWithRefCount();
// testShareInfiniteIntervalWithWindowTime();

testShareFiniteAjaxWithoutRefCount();
// testShareFiniteAjaxWithRefCount();

// testShareFiniteAjaxWithoutRefCountAndTake();
// testShareFiniteAjaxWithRefCountAndTake();
// testShareFiniteAjaxWithRefCountAndUnsubscribe();

// tutaj nizej opisalem roznice w dzialaniu gdy wykoniujemy operator shareReplay w 2 przypadkach: 
// - gdy observable wczesniej sie nie konczy np 'interval'
// - gdy observable wczesniej sie konczy np strzal ajxa na serwer
// (to wyszlo podczas implementowania interceptora Angular do keszowania)





const a = of("123", 123, true).pipe(filter(x => typeof x === "string"));
const b = of("123", 123, true).pipe(every(x => typeof x === "string"));

// ************************************************************************************************************************
// https://rxjs.dev/api/operators/shareReplay
// - ten operator pod spodem tworzy sobie ReplaySubject i zapina na niego wszystkie Observer zapiete na wynik tego operatora 
// - parametry 'bufferSize' i 'windowTime' przekazywane do ReplaySubject podczas tworzenia
// - 'bufferSize' - ile ostatnich trzymac tzn jesli za jakis czas sie ktos zapnie to na starcie dostanie ponada ilosc wiadomosci
// - 'windowTime' - z jakiegos czasu trzymac ostatnie skeszowane wartosci, jak ktos sie zapmnie to na wejsciu dostanie ostatnie zebrane 
// - 'refCount' domyslnie jest 'false' i oznacza ze jak wszystkie Observer sie odepna to i tak nadal ReplaySubject bedzie zapiety

function callShareReplay<T>(obs: Observable<T>, shareConfig: ShareReplayConfig, log = true) {
    return obs.pipe(
        log ? tap(consoleObserver("tap.before-shareReplay.")) : x => x,
        shareReplay(shareConfig),
        log ? tap(consoleObserver("tap.after-shareReplay.")) : x => x,
    );
}

// - nizej widzimy ze nawet po odpieciu sie wszystkich, to i tak wartosci plyna do operatora 'shareReplay' wypisujac "tap.before-shareReplay.next"
function testShareInfiniteIntervalWithoutRefCount() {

    const obs = callShareReplay(interval(1000), { refCount: false });

    var sub = obs.subscribe(consoleObserver("result-interval"));
    setTimeout(() => sub.unsubscribe(), 3000);

    // var unsub = obs.subscribe(consoleObserver("result-interval"));
    // setTimeout(() => unsub.unsubscribe(), 3000);
}

// - nizej widzimy ze po odpiciu sie wszystkich, wartosci juz nie plyna
function testShareInfiniteIntervalWithRefCount() {
    const obs = callShareReplay(interval(1000), { refCount: true });

    var sub = obs.subscribe(consoleObserver("result-interval"));
    setTimeout(() => sub.unsubscribe(), 3000);

    // var unsub = obs.subscribe(consoleObserver("result-interval"));
    // setTimeout(() => unsub.unsubscribe(), 3000);
}


// - nizej sluchamy przez 5s, wartosci keszujemy z ostatnich 3s, w 10s zapisujemy sie kolejny raz i od razu dostajemy 3 ostatnie wartosci
function testShareInfiniteIntervalWithWindowTime() {
    const obs = callShareReplay(interval(1000), { refCount: false, windowTime: 3000 }); // tutaj 'refCount' obojetnie jaki

    var sub = obs.subscribe(consoleObserver("result-interval"));
    setTimeout(() => sub.unsubscribe(), 3000);

    setTimeout(() => obs.subscribe(consoleObserver("result-interval")), 10000)
}






// ************************************************************************************************************************

// ajax
// 

// symulacja strzalu na serwer ktory po 1s odpowiada aktualna data
function ajax() {
    return defer(() => of(new Date())).pipe(delay(1000));
}

// - nizej niezaleznie czy refCount jest true czy false dzialanie jest dokladne takie samo (a wydaje sie nie powinno tak byc)
// - na starcie zapisuja sie pierwsi 2 i po 1s otrzymuja odpowiedz, po 2s zapisuje sie kolejny i zwraca dokladnie ten sam rezultat
// (tutaj widac bo jest to ta sama data)
// - tutaj tez nie ma znaczenia czy sie recznie odpinamy poniewaz chcemy i tak otrzymac rezultat a potem odpiecie samo sie robi (widac na konsoli)
// - dopiero uzycie 'take' zmienia zachowanie i ostatni zapisujac sie wykonuje nowy strzal i pojawia sie nowa wartosc date
// - powod takiego dzialania jest chyba nastepujacy:
// -- gdy przeanalizuje sie co wypisane bylo na konsoli dla 'tap', jak wartosc plyna przez i po shareReplay
// -- poniewaz pierwsze zrodlo konczy generowanie wartosci (leci next i zaraz completed) to ReplaySubject takze jest skonczony
// i zawsze juz bedzie keszowal wartosci :/
// -- dzieje sie tak nawet gdy wszystkie zapisane odebna sie (poniewaz odpinanie sie po otrzymaniu 'complete' i tak nie ma wiekszego znczenia)
// -- gdy dodamy 'take(1)' w momencie otrzymania pierwszej wartosci od oryginalnego zrodla (a jeszcze przed jego zakonczeniem),
// nastepuje odpiecie wszystkich przypisanych do shareReplay i poniewaz mamy opcje refCount=true to nastepuje odpiecie od oryginalnego
// - ostatni przyklad pokazuje ze 'refCount' faktycznie pelni swoja funkcje gdy zapinamy sie i za chwile odpinamy wieloktornie i
// dopiero na koniec zapisamy sie tak aby poczelac na rezultat, gdy refCount=false na koncu otrzymuje "pierwsza date" (keszowana),
// gdy refCount=true otrzymujemy "ostatnia date"


function subscribeSharedAjaxCall<T>(refCount: boolean, transform: (obs: Observable<T>) => Observable<T>) {
    const obs = callShareReplay(ajax(), { refCount }).pipe(transform);


    obs.subscribe(consoleObserver("result-interval1"));
    obs.subscribe(consoleObserver("result-interval2"));
    setTimeout(() => { obs.subscribe(consoleObserver("result-interval3")); }, 3000)
}

function testShareFiniteAjaxWithoutRefCount() {
    subscribeSharedAjaxCall(false, x => x);
}

function testShareFiniteAjaxWithRefCount() {
    subscribeSharedAjaxCall(true, x => x);
}

function testShareFiniteAjaxWithoutRefCountAndTake() {
    subscribeSharedAjaxCall(false, take(1));
}

function testShareFiniteAjaxWithRefCountAndTake() {
    subscribeSharedAjaxCall(true, take(1));
}

async function testShareFiniteAjaxWithRefCountAndUnsubscribe() {
    // const refCount = true;
    const refCount = false;
    const obs = callShareReplay(ajax(), { refCount });

    for (let i = 0; i < 6; i++) {
        console.log("->", new Date());
        var sub = obs.subscribe(consoleObserver("result-interval1"));
        await delayPromise(800);
        sub.unsubscribe();
    }

    console.log("---->", new Date());
    obs.subscribe(consoleObserver("result-interval2"));
}




// pomocznicze metody

function consoleObserver(text: string): Partial<TapObserver<any>> {
    return {
        next(v) {
            console.log(text + ".next", v);
        },
        complete() {
            console.log(text + ".complete");
        },
        error(err) {
            console.log(text + ".error", err);
        },
        finalize() {
            console.log(text + ".finalize");
        },
    };
}

function delayPromise(ms: number) {
    return lastValueFrom(timer(ms));
}



