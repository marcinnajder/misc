
interface ABCD { a: number; b: string; c: boolean; d?: Date; }

const abcd: ABCD = { a: 1, b: "b", c: true, d: undefined };



type EntryPair<T> = [keyof T, string]

type Entry<T> = (keyof T) | EntryPair<T>

type ToEntryPair<T, E extends Entry<T>> =
    E extends [infer From, infer To] ? E[0] : [E, E]

type KeysOfEntry<T, E extends Entry<T>> =
    //E extends [infer Key, infer Value] ? Key : E
    E extends [infer Key, infer Value] ? Key : (E extends keyof T ? E : never)


type TestKeysOfEntry = KeysOfEntry<ABCD, ["a", ""] | "b">; // "b" | "a"


type Pick1M<T, E extends EntryPair<T>> = {
    [K in E[1]]: T[E[0]]
}


type TestPick1M = Pick1M<ABCD, ["a", "aa"] | ["b", "bb"]>; // { aa: string | number; bb: string | number; }

// <- 
// - generanie wydaje sie ze rozwiazanie powinno byc mniej wiecej tak jak na gorze, ale widac nie dziala
// - powinno wyjsc "{ aa: number; bb: string}" w typy wlasciwosci maja wszystkie kombinacje :/
// - dzieje sie tak dlatego, ze wejsciem jest type dla ["a", "aa"] | ["b", "bb"] i pod spodem buduja sie 
// wszystkie 4 kombinacje par "a-aa, a-bb, b-bb, b-aa"
// - pojedyncza para miala opisywac mapowanie nazwy wlasciwosci [From,To], a tak sie nie dzieje dlatego szukalem
// szukalem innej reprezentacji mapowania ktora moglaby byc przekazana do "PickM" np obiektu { [key: From]: To}


type Pick2M<T, P extends keyof T, M extends { [key in P]: string }> = {
    //type Pick2M<T, P extends keyof T, M extends { [key in keyof T]: string }> = {
    [K in P as M[K]]: T[K];
}

type Pick2MTest = Pick2M<ABCD, "a" | "b", { "a": "aa", "b": "bb" }> // { aa: number; bb: string; }

// <-
// - wow, to zadzialalo poprawnie, czyli wybrane zostaly odowiednie typy wlasciwosci
// - kluczowe bylo to ze aby poprawnie wybraly sie typy wlasciwosci to piszac " ... : T[K]",
// to K musial musialo by typu "keyof T", niestety nie udawalo sie do poprawnie wyliczyc z 
// parametru generucznego M (obiektu mapujacego), wiec niby nadmiarowo przekazywany jest P ktory
// wprost przekazuje typ "keyof T", to tez nie jest duzy problem ze to jest nadmiarowe bo mozna to przekazac
// - zotala jeszcze tylko kwestia tego jak z "["a", "aa"] | ["b", "bb"]" bo takie cos zostanie przekazane
// do metody pick zrobic typ {"a": "aa", {"b": "bb"} i zaczalem od czegos takiego


type EntryToObj<T, ME extends Entry<T>> =
    ME extends [infer From, infer To] ?
    (From extends string ? { [key in From]: To } : never) :
    (ME extends string ? { [key in ME]: ME } : never)


type TestEntryToObj = EntryToObj<ABCD, "a" | ["b", "bb"]>; // { a: "a"; } | { b: "bb"; }

// <-
// - niestety to nie jest to co potrzebujemy bo pomiedzy obiekrami jest | a powinno byc &
// - byc moze mozna jakos napisac typ ktory zamieni | na & (chwile probowalem i sie nie udawalo)
// - inny podejscie mogloby by inne reprezentowanie "entries", zamiast "a" | ["b", "bb"] 
// mozna reprezentowac jako tablice ["a" , ["b", "bb"]] bo bardzo prosto mozna napisac typ 
// pomocniczy ktory zmapuje w obie strony
// - wtedy juz mozna napisal typ ktory bedzie przetwarzal kolejne elementy tablicy budujac obiekty
// polaczone & czyli {"a":"a"} & {"b":"bb"}
// - a mowiac o tablicy ...to miedzy czasie okazalo sie ze sam opis typow metody pick staje sie problematyczny 

function pick1<T, E extends Entry<T>[]>(obj: T, ...props: E): E { return null as any; }
var testpick1 = pick1(abcd, "a", ["b", "bb"]); // ["a", ["b", string]]

// <-
// - chcielibysmy wygodnie moc przekazywac mapowanie jako spread ... czyli pick(abcd, "a", ["b","bb"]),
// ale typ ktory sie wnioskowal ["a", ["b", string]] czyli tutaj nagle nagle nie zostal wyciagniety "bb" tylko string
// - byc moze mozna jakos wyluskac potem jeszcze ten typ "bb", jednym ze sposobow (byc moze jedynym) jest przekazanie
// zwyklej tablicy zamiast spread ... bo wtedy mozna uzyc "... as const"

function pick2<T, E extends Entry<T>[]>(obj: T, props: E): E { return null as any; }
var testpick2 = pick1(abcd, "a", ["b", "bb"] as const); // ["a", ["b", "bb"]]

// <-
// - to dziala prawidlowo, byc moze da sie jakos bez tego "... as const" otypowac metode pick ale juz sobie odpuscilem 

// ... uff wiedzac to co wiem do tej pory tzn ze chyba bede musial i tak stosowac tablice zamist spread do opisu mapowania
// ["a", ["b", "bb"]] i ze z tablicy chyba bede mogl stworzyc juz obiekt mapujacy {"a": "a", "b": "bb"} lub nawet od razu
// zbudowac finalny obiekt z typami wlasciwosci {"a": number, "bb": string} to zbudowalem rozwiazanie


type Duplicate<T> =
    T extends [infer Item, ...infer Rest]
    ? (Item extends [infer Item1, infer Item2] ? [[Item1, Item2], ...Duplicate<Rest>] : [[Item, Item], ...Duplicate<Rest>])
    : []

type TestDuplicate = Duplicate<["a", "b", ["x", "y"]]>;// [["a", "a"], ["b", "b"], ["x", "y"]]

type PickM<T, Es extends EntryPair<T>[] | unknown> =
    Es extends [[infer K, infer V], ...infer Rest]
    ? (K extends keyof T ? { [key in (string & V)]: T[K] } & PickM<T, Rest> : unknown) : unknown;

type TestPickM = PickM<ABCD, [["a", "aa"], ["b", "bb"]]>; // { aa: number; } & { bb: string; }

// <-
// - to dziala poprawnie, wydaje sie ze cos takiego powinno dzialac ale TS nie daje rady :/
// type PickM_<T, Es extends EntryPair<T>[]> =
//     Es extends [[infer K, infer V], ...infer Rest]
//     ? { [key in (string & V)]: T[K] } & PickM_<T, Rest> : unknown;

var aaaa = pickM(abcd, [["d", "dd"], ["c", "cc"], "a"] as const);

var p = pickM({ id: 1, age: 123, name: "marcin" }, [["id", "ID"], "name"] as const);



function pickM<T, Es extends Entry<T>[]>(obj: T, enties: Es): PickM<T, Duplicate<Es>> {
    var result = {} as any;
    for (const entry of enties) {
        const [pFrom, pTo] = Array.isArray(entry) ? entry : [entry, entry] as EntryPair<T>;
        const value = obj[pFrom];
        if (typeof value !== "undefined") {
            result[pTo] = value;
        }
    }
    return result;
}

const pm1 = pickM({ id: 1, name: "marcin", age: 12 }, ["id", ["name", "patientName"]] as const);
console.log(pm1);
const pm2 = pickM(abcd, ["a", "d"] as const);
console.log(pm2);


function pick<T, P extends keyof T>(obj: T, ...props: P[]): Pick<T, P> {
    var result = {} as any;
    for (const p of props) {
        const value = obj[p];
        if (typeof value !== "undefined") {
            result[p] = value;
        }
    }
    return result;
}


const p1 = pick({ id: 1, name: "marcin", age: 12 }, "id", "name");
console.log(p1);
const p2 = pick(abcd, "a", "d");
console.log(p2);
