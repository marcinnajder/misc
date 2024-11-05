
interface ABCD { a: number; b: string; c: boolean; d?: Date; }

const abcd: ABCD = { a: 1, b: "b", c: true, d: undefined };



type EntryPair<T> = [string, keyof T];

type Entry<T> = keyof T | EntryPair<T>;

type ToEntryPair<T, E extends Entry<T>> =
    E extends [infer NewKey, infer Key] ? E : [E, E]

type TestToEntryPair = ToEntryPair<ABCD, ["aa", "a"] | "b">;

type KeysOfEntry<T, E extends Entry<T>> =
    // E extends [infer NewKey, infer Key] ? Key : E
    E extends [infer NewKey, infer Key] ? Key : (E extends keyof T ? E : never)


type TestKeysOfEntry = KeysOfEntry<ABCD, ["", "a"] | "b">; // "b" | "a"

// type Pick1M<T, E extends EntryPair<T>> = {
//     [P in E[1]]: T[E[0]]
// }

type Pick1M<T, E extends EntryPair<T>> = {
    [K in E[0]]: T[E[1]]
}


type TestPick1M = Pick1M<ABCD, ["aa", "a"] | ["bb", "b"]>; // { aa: string | number; bb: string | number; }

// <- 
// - generanie wydaje sie ze rozwiazanie powinno byc mniej wiecej tak jak na gorze, ale widac nie dziala
// - powinno wyjsc "{ aa: number; bb: string}" w typy wlasciwosci maja wszystkie kombinacje :/
// - dzieje sie tak dlatego, ze wejsciem jest type dla ["a", "aa"] | ["b", "bb"] i pod spodem buduja sie 
// wszystkie 4 kombinacje par  "a-aa, a-bb, b-bb, b-aa" czyli 
// { aa: number ; bb: string; } | { aa: string ; bb: string; } | { aa: number ; bb: number; } | { aa: string ; bb: number; }
// - pojedyncza para miala opisywac mapowanie nazwy wlasciwosci [From,To], a tak sie nie dzieje dlatego szukalem
// szukalem innej reprezentacji mapowania ktora moglaby byc przekazana do "PickM" np obiektu { [key: From]: To}


type Pick2M<T, P extends keyof T, M extends { [key in P]: string }> = {
    [K in P as M[K]]: T[K];
}

type Pick2MTest = Pick2M<ABCD, "a" | "b", { "a": "aa", "b": "bb" }> // { aa: number; bb: string; }

// <-
// - wow, to zadzialalo poprawnie, czyli wybrane zostaly odowiednie typy wlasciwosci
// - kluczowe bylo to ze aby poprawnie wybraly sie typy wlasciwosci to piszac " ... : T[K]",
// to K musial musialo by typu "keyof T", niestety nie udawalo sie do poprawnie wyliczyc z 
// parametru generucznego M (obiektu mapujacego), wiec niby nadmiarowo przekazywany jest P ktory
// wprost przekazuje typ "keyof T", to tez nie jest duzy problem ze to jest nadmiarowe bo mozna to przekazac
// - zotala jeszcze tylko kwestia tego jak z "["aa", "a"] | ["bb", "b"]" bo takie cos zostanie przekazane
// do metody pick zrobic typ {"a": "aa", {"b": "bb"} i zaczalem od czegos takiego


type EntryToObj<T, ME extends Entry<T>> =
    ME extends [infer NewKey, infer Key] ?
    (NewKey extends string ? { [key in NewKey]: Key } : never) :
    (ME extends string ? { [key in ME]: ME } : never)


type TestEntryToObj = EntryToObj<ABCD, "a" | ["bb", "b"]>; // { a: "a"; } | { b: "bb"; }

// <-
// - niestety to nie jest to co potrzebujemy bo pomiedzy obiekrami jest | a powinno byc &
// - byc moze mozna jakos napisac typ ktory zamieni | na & (chwile probowalem i sie nie udawalo)
// - inny podejscie mogloby by inne reprezentowanie "entries", zamiast "a" | ["bb", "b"] 
// mozna reprezentowac jako tablice ["a" , ["bb", "b"]] bo bardzo prosto mozna napisac typ 
// pomocniczy ktory zmapuje w obie strony
// - wtedy juz mozna napisal typ ktory bedzie przetwarzal kolejne elementy tablicy budujac obiekty
// polaczone & czyli {"a":"a"} & {"b":"bb"}
// - a mowiac o tablicy ...to miedzy czasie okazalo sie ze sam opis typow metody pick staje sie problematyczny 

function pick1<T, E extends Entry<T>[]>(obj: T, ...props: E): E { return null as any; }
var testpick1 = pick1(abcd, "a", ["bb", "b"]); // ["a", [string, "b"]]

// <-
// - chcielibysmy wygodnie moc przekazywac mapowanie jako spread ... czyli pick(abcd, "a", ["bb","b"]),
// ale typ ktory sie wnioskowal ["a", [string, "b"]] czyli tutaj nagle nagle nie zostal wyciagniety "bb" tylko string
// - byc moze mozna jakos wyluskac potem jeszcze ten typ "bb", jednym ze sposobow (byc moze jedynym) jest przekazanie
// zwyklej tablicy zamiast spread ... bo wtedy mozna uzyc "... as const"
// - (nizej sie okazalo ze faktycznie "const" jest potrzebny, ale nie koniecznie "as const" bo od TS mozna pisac "<const T>" przy
// parametrze generycznym )

function pick2<T, E extends Entry<T>[]>(obj: T, props: E): E { return null as any; }
var testpick2 = pick1(abcd, "a", ["bb", "b"] as const); // ["a", ["bb", "b"]]

// <-
// - to dziala prawidlowo, byc moze da sie jakos bez tego "... as const" otypowac metode pick ale juz sobie odpuscilem 

// ... uff wiedzac to co wiem do tej pory tzn ze chyba bede musial i tak stosowac tablice zamist spread do opisu mapowania
// ["a", ["bb", "b"]] i ze z tablicy chyba bede mogl stworzyc juz obiekt mapujacy {"a": "a", "b": "bb"} lub nawet od razu
// zbudowac finalny obiekt z typami wlasciwosci {"a": number, "bb": string} to zbudowalem rozwiazanie


type Duplicate<T> =
    T extends [infer Item, ...infer Rest]
    ? (Item extends [infer Item1, infer Item2] ? [[Item1, Item2], ...Duplicate<Rest>] : [[Item, Item], ...Duplicate<Rest>])
    : []

type TestDuplicate = Duplicate<["a", "b", ["x", "y"]]>;// [["a", "a"], ["b", "b"], ["x", "y"]]

type PickM<T, Es extends EntryPair<T>[] | unknown> =
    Es extends [[infer NewKey, infer Key], ...infer Rest]
    ? (Key extends keyof T ? { [key in (string & NewKey)]: T[Key] } & PickM<T, Rest> : unknown) : unknown;

type TestPickM = PickM<ABCD, [["aa", "a"], ["bb", "b"]]>; // { aa: number; } & { bb: string; }


// <-
// - to dziala poprawnie, wydaje sie ze cos takiego powinno dzialac ale TS nie daje rady :/
// type PickM_<T, Es extends EntryPair<T>[]> =
//     Es extends [[infer V, infer K], ...infer Rest]
//     ? { [key in (string & V)]: T[K] } & PickM_<T, Rest> : unknown;


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

//function pickm<T, const Es extends Entry<T>[]>(obj: T, ...entries: Es): PickM<T, Duplicate<Es>> {
function pickm<T, const Es extends Entry<T>[]>(obj: T, ...entries: Es): PickMC<T, Es[number]> {
    const result = {} as any;
    for (const entry of entries) {
        if (Array.isArray(entry)) {
            const [newKey, key] = entry;
            const value = obj[key];
            if (typeof value !== "undefined") {
                result[newKey] = value;
            }
        } else {
            const value = obj[entry];
            if (typeof value !== "undefined") {
                result[entry] = value;
            }
        }
    }
    return result;
}

const p1 = pick({ id: 1, name: "marcin", age: 12 }, "id");
console.log(p1);
const p2 = pick(abcd, "a", "d");
console.log(p2);

const pm1 = pickm({ id: 1, name: "marcin", age: 12 }, "id", ["firstName", "name"]);
console.log(pm1);
const pm2 = pickm(abcd, "a", "d");
console.log(pm2);














// <-
// - po zapytaniu chatGPT o rozwiazanie zwrocil kompletne rozwiazanie na dole :)
// - 1 podowiedz to ze mozna zamiast "as const" napisac przy typie generyczny <const T>
// - 2 podpowiedz ze faktycznie zamiast procesowac kolejnych elementow tablicy tak ja ja to robile, mozna napisac jeden "mapped type" 
// bo tablica "[E,E,E]"" to w pewnym sensie to samo co "E | E | E" co wlasciwie jest tym samym co "E"
// - nizej finalna postac od chata ale nawet prosciej wylicza klucz

type PickMC<T, P extends Entry<T>> = {
    [K in P as K extends [infer NewKey, infer Key] ? NewKey : K]:
    K extends [infer NewKey, infer Key] ? (Key extends keyof T ? T[Key] : never) : (K extends keyof T ? T[K] : never)
};

type TestPickMC = PickMC<ABCD, ["aa", "a"] | ["bb", "b"]> // { aa: number; } | { bb: string; }


type PickMC_<T, P extends Entry<T>> =
    P extends [infer NewKey, infer Key] ? Key extends keyof T ?
    {
        [K in P as (string & NewKey)]: T[Key]
    } : never : never;


type TestPickMC_ = PickMC_<ABCD, ["aa", "a"] | ["bb", "b"]> // { aa: number; } | { bb: string; }

// <- 
// - tutaj proba jescze wyciagniecia wspolnej czesci kodu, ale powstaje wnik "{} | {}", wiec chyba definicja typu musi byc jednej duzym mapped type



// ->
// - zapytanie mackaJ do chata gpt :/

// https://chatgpt.com/share/6728c7c3-861c-800f-9cd0-96e7f9c7058b
// Modify this function so that you can pass to it something like:
// pick({...}, 'a', ['b', 'bRenamed'], 'c')
// Existing code:
// function pick<T extends object, const K extends Array<keyof T>>(obj: T, ...props: K): Pick<T, K[number]> {
//   const acc = {} as T;
//   for (const key of props) {
//     acc[key] = obj[key];
//   }  
//   return acc;
// }
// const test = pick({ a: '55', b: 55 }, 'a');
// console.log(test);

// https://www.typescriptlang.org/play/?ssl=20&ssc=3&pln=2&pc=1#code/C4TwDgpgBACgTgezAHgCoD4oF4oGsIgIBmUqUAPlANr6EmoA0UAzsHAJYB2A5gLoDcAWABQI0JFjsAxrgDq7YAAsAShE4BDALYQ0TGFAgAPYGoAmzKAEE4cdSGTwkadC+xQA3iKjUA0lC6wVJwArpoARhBwvFDqFn5GJpzm1FxEkVA+BEyp6QByEADumSC8Xt5QAPwZBAbGZha0xKRl5ZVQ+UU1CfUsbFzcLa1tHcWDrQBcUJwQAG6RY1CT03Nwg5PxdUkNBE2og1U+a1Oz88LevOu1icmN9PukVD6lZ+WX3VspnGlw1SDZX3lCsVnq0Dl1NjcdncXqCHsCFksTqsYYtjisRABfISiYREYKcKTAdgIThQMDSXDIMpkd7JBBhABWEEJDDKUhJrFgVx61ls9kcKAw6BE6AAFPSGZNGFAAHRysCIMDMSYwACUKop8iUqg02l0sEwnhe7M4nLgEGYwQANsBJupOCA3O4sSIykQED9RSbOQqkFAmr6laqPIN2CRReIIAHFdgsDgAOSsDg8ePBo1DfzhwP+UkStMLbzmy02qiB6I4CWlxUCBYYwYYgxW5jQMNQUW8uwy9jMDsgUWB4MAMkHZMVMqtam4SljOAATPmUd5vcBqLQmNNOiU3IHsRnW6LaDn-YyFxnCxbrcAgkCCOXjwyaLfd0M6yjX95X2VzcBgnBSUXL2xT9hAAehAqAAFFDC0MAJygABVZh1G4CBxhEZd-Q4bguHUK0nRiSZ4wAViI+MmDCSYSKYKRJjYYJoBdYQMPJGQIFMbcKXFLCcKtJh43UMjqHjMJBOEnUtDY+NeD4qRU2xdCOQQCdxwQbh+wpNjVWxMCoAAeWCYAwAMyZ3AIqBiNI8jxO0UxKKI6jaLgeioFfIA
// type Prop<T> = keyof T | [keyof T, string];

// type PickWithRename<T, P extends Array<Prop<T>>> = {
//   [K in P[number] as K extends [infer Key, infer NewKey]
//     ? Key extends keyof T
//       ? NewKey extends string
//         ? NewKey
//         : never
//       : never
//     : K extends keyof T
//     ? K
//     : never
//   ]: K extends keyof T
//     ? T[K]
//     : K extends [infer Key, infer NewKey]
//     ? Key extends keyof T
//       ? T[Key]
//       : never
//     : never
// };

// function pick<
//   T extends object,
//   const P extends Array<Prop<T>>
// >(obj: T, ...props: P): PickWithRename<T, P> {
//   const result: any = {};

//   for (const prop of props) {
//     if (typeof prop === 'string') {
//       if (prop in obj) {
//         result[prop] = obj[prop];
//       }
//     } else if (Array.isArray(prop) && prop.length === 2) {
//       const [key, newKey] = prop;
//       if (key in obj) {
//         result[newKey] = obj[key];
//       }
//     }
//   }

//   return result;
// }

// // Example Usage:
// const original = { a: '55', b: 55, c: true };
// const picked = pick(original, 'a', ['b', 'bRenamed'], 'c');

// console.log(picked);
// // Output: { a: '55', bRenamed: 55, c: true }





// - tutaj jest wyjasnienie tej nowosci TS 5  ze mozna przy parametrze generycznym napisac ...<const T> zamiast
//  pisac w miejscu wywolywania ... as const
// - https://devblogs.microsoft.com/typescript/announcing-typescript-5-0/#const-type-parameters
// 
// function identity<T>(value: T) {
//     return value;
// }
// const x1 = identity(["a", "b", "c"]); // -> const x1: string[]
// const x2 = identity(["a", "b", "c"] as const); // const x2: readonly ["a", "b", "c"]
// function identity2<const T>(value: T) {
//     return value;
// }
// const x3 = identity2(["a", "b", "c"]); // const x2: readonly ["a", "b", "c"]

