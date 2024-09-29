import { every, filter, find, last, single, takewhile } from "powerseq"; // powerseq > 2.0.3

const stringsOrNumbers = ["1", 2, "3", 4]; // typ ->  const stringsOrNumbers: (string | number)[]

// to jest bardzo stare -> https://www.typescriptlang.org/docs/handbook/advanced-types.html#user-defined-type-guards
function isString(value: any): value is string {
    return typeof value === "string";
}

// okazuje sie ze takie cos dziala juz od wiekow, nawet w TS3.3 dziala (najstarszy ktorym mozna testowac tu https://www.typescriptlang.org/play/
// tzn chodzi o to ze TS wie ze zwracany typ bedzie "string[]" zamiast "(string | number)[]"
// (czywiscie funkcja 'filter' musi byc specjalnie otypowana i ja tutaj nie wchodze w szczegoly)
const strings1 = stringsOrNumbers.filter(isString); // typ -> const a: string[]

// takie cos dziala od TS4.0 kiedy tzn w srodu tego ifa zmienil sie typ zmiennej
// to bylo mozliwe bo pojalo sie cos takiego jak https://www.typescriptlang.org/docs/handbook/2/classes.html#this-based-type-guards
if (stringsOrNumbers.every(isString)) {
    console.log(stringsOrNumbers); // typ -> const stringsOrNumbers: string[]
}

// ostatnia metoda tablicy wspierajacej ten feature jest 'find', on tutaj wie ze zwracany bedzie 'string'
const string1 = stringsOrNumbers.find(isString); // typ -> const string1: string


// od TS5.5 czyli calkiem w sumie nowe pojawilo sie analogiczne wsparcie gdy przekazujemy metody anonimowe zamiast funkcji "type-guard" jak 'isString'
const strings2 = stringsOrNumbers.filter(x => typeof x === "string"); // typ -> const strings2: string[]
if (stringsOrNumbers.every(x => typeof x === "string")) {
    console.log(stringsOrNumbers); // typ -> const stringsOrNumbers: string[]
}
const string2 = stringsOrNumbers.find(x => typeof x === "string"); // typ -> const string1: string


// no i teraz pytanie ktore operatory powerseq moga takze wspierac takie cuda ? :)

const strings3 = filter(stringsOrNumbers, x => typeof x === "string"); // typ -> const strings3: Iterable<string>
const string3 = find(stringsOrNumbers, x => typeof x === "string"); // typ -> const string3: string
const string4 = single(stringsOrNumbers, x => typeof x === "string"); // typ -> const string4: string
const string5 = last(stringsOrNumbers, x => typeof x === "string"); // const string5: string
const strings4 = takewhile(stringsOrNumbers, x => typeof x === "string"); // typ -> Iterable<string>

// aby tak idealnie sie wnioskowalo sie dla uzycia samego operatora 'every' to trzeba zrzutowac do Iterable<T>,
// gdy bedzie jakis pipe(..., every( ...)) to nie bedzie potrzebne
const stringsOrNumbersSeq = stringsOrNumbers as Iterable<string | number>;

if (every(stringsOrNumbersSeq, x => typeof x === "string")) {
    console.log(stringsOrNumbersSeq); // typ -> const stringsOrNumbersSeq: Iterable<string>
}

// bez tego rzutowania jak tak
if (every(stringsOrNumbers, x => typeof x === "string")) {
    console.log(stringsOrNumbers); // typ -> const stringsOrNumbers: (string | number)[] & Iterable<string>
}



