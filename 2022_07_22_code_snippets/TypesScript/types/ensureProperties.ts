

// *** '...Value'


export function ensureValue<T>(value: T | null | undefined): asserts value is NonNullable<T> {
    if (value === null || typeof value === "undefined") {
        throw new Error(`value '${value}' is null or undefined`);
    }
}

export function requiredValue<T>(value: T | null | undefined): NonNullable<T> {
    ensureValue(value)
    return value;
}

// *** '...Properties'

export type RequiredProperties<T extends object, P extends keyof T> = Omit<T, P> & Required<Pick<T, P>>;

function formatMessage(obj: object, propertyName: string): string {
    return `property '${propertyName}' of '{${Object.keys(obj)}}' object is null or undefined`
}
export function ensureProperties<T extends object, P extends keyof T>(obj: T | null | undefined | RequiredProperties<T, P>, ...propertyNames: P[]): asserts obj is RequiredProperties<T, P> {
    ensureValue(obj);
    for (const propertyName of propertyNames) {
        if (obj[propertyName] === null || typeof obj[propertyName] === "undefined") {
            throw new Error(formatMessage(obj, String(propertyName)));
        }
    }
}

export function requiredProperty<T extends object, P extends keyof T>(obj: T | null | undefined, propertyName: P): NonNullable<T[P]> {
    ensureValue(obj);
    if (obj[propertyName] === null || typeof obj[propertyName] === "undefined") {
        throw new Error(formatMessage(obj, String(propertyName)));
    }
    // ensureProperties(obj, propertyName);
    // <- zamiast tego ifa powinnismy wywolac 'ensureProperties', niestety bez ifa nizej pojawia sie blad TS ->   
    return obj[propertyName];
}

// *** '...Paths'

// Paths<O> -> dzieło Sebastiana :) zwraca wszystkie mozliwe kombinacje sciezek

export type Paths<O> = PathsAux<O, [], "">[number];

type PathsAux<O, Acc extends string[], Current extends string> =
    O extends object ?
    keyof O extends infer Keys
    ? Keys extends string
    ? Keys extends keyof O
    ? PathsAux<O[Keys], [...Acc, `${Current}${Keys}`], `${Current}${Keys}.`>
    : Acc : [...Acc, Keys] : Acc : Acc;

// - tutaj pojawia sie problem jak graf obiektow jest rekurencyjny 
// type Employee = { name: string; boss: Employee; }
// type EmployeePaths = Paths<Employee>; // error: Type instantiation is excessively deep and possibly infinite.

export type PathsOrString<T> = string & Paths<T>

// "person.address.city" -> ["person", "address.city"]
export type SplitPath<T extends string> = T extends `${infer Head}.${infer Tail}` ? [Head, Tail] : (T extends `${infer Head}` ? [Head, unknown] : never);

export type RequiredSinglePath<T extends {}, P extends PathsOrString<T>> =
    SplitPath<P> extends [infer Head extends keyof T, infer Tail]
    ? Omit<T, Head> & { [key in Head]-?: Tail extends string ? RequiredSinglePath<NonNullable<T[Head]>, Tail> : T[Head] }
    : never;

export type RequiredPaths<T extends {}, P extends Array<Paths<T>>> =
    P extends [infer Head extends PathsOrString<T>, ...infer Rest extends Array<PathsOrString<T>>]
    ? (RequiredSinglePath<T, Head> & RequiredPaths<T, Rest>) : unknown;


export function ensurePaths<T extends object, P extends Array<PathsOrString<T>>>(obj: T | null | undefined | RequiredPaths<T, P>, ...propertyPaths: P): asserts obj is RequiredPaths<T, P> {
    ensureValue(obj);
    for (const propertyPath of propertyPaths) {
        let currentObj: any = obj;
        for (const propertyName of propertyPath.split(".")) {
            ensureProperties(currentObj, propertyName);
            currentObj = currentObj[propertyName];
        }
    }
}

export type RequiredSinglePathValue<T extends {}, P extends PathsOrString<T>> =
    SplitPath<P> extends [infer Head extends keyof T, infer Tail]
    ? Tail extends string ? RequiredSinglePathValue<NonNullable<T[Head]>, Tail> : NonNullable<T[Head]>
    : never;

export function requiredPath<T extends object, P extends PathsOrString<T>>(obj: T | null | undefined, propertyPath: P): RequiredSinglePathValue<T, P> {
    ensurePaths(obj, propertyPath);
    return propertyPath.split(".").reduce((o, properyName) => o[properyName], obj as any);
}

// testy typow
type Person = { name?: string; age?: number; address?: { city?: string; country?: boolean, no?: number } }
type Type1 = RequiredProperties<Person, "age" | "name">;
type Type2 = Paths<Person>
type Type3 = SplitPath<"person.address.city">
type Type4 = RequiredSinglePath<Person, "address.city">
type Type5 = RequiredPaths<Person, ["address.city", "address.country", "name"]>
type Type6 = RequiredSinglePathValue<Person, "address.no">
// (null as any as Type5).address


export type SubProperties<T extends UserContext, P extends keyof T> = keyof NonNullable<T[P]>;

export type RequiredSubProperties<T extends UserContext, P extends keyof T, SP extends SubProperties<T, P>> =
    Omit<T, P> & { [key in P]-?: RequiredProperties<NonNullable<T[key]>, SP>; };

export function ensurePatientProfile<T extends UserContext, PP extends SubProperties<T, "patientProfile">>(user: T | null | undefined | RequiredSubProperties<T, "patientProfile", PP>, ...propertyNames: PP[]): asserts user is RequiredSubProperties<T, "patientProfile", PP> {
    ensureValue(user);
    ensureProperties(user, "patientProfile");
    // ensureProperties(user.patientProfile!, ...propertyNames);
    ensureProperties(user.patientProfile!, ...(propertyNames as SubProperties<UserContext, "patientProfile">[]));
    // <- bez rzutowania leci blad TS, poniewaz tutaj chcemy aby "nawigacja Shift+F12" dziala poprawnie
    // (to szczegolowo jest opisane wyzej), to nie operujemy na 'UserContext' ale na dowolnym 'T extends UserContext',
    // a wywolanie dla niego 'SubProperties<T, "patientProfile">' zwraca 'string | number | symbol' :/
}


// ** samples

interface UserContext {
    email?: string;
    patientProfile?: {
        name?: string;
        address?: {
            street?: string;
            city?: string;
        }
    }
}

function ensurePropertiesTest(user: UserContext) {
    ensureProperties(user, "patientProfile");
    ensureProperties(user.patientProfile, "address", "name");
    ensureProperties(user.patientProfile.address, "street", "city");
    const city1 = user.patientProfile.address.city.toString(); // string

    // bez wywolania 'ensureProperties' 
    // const city2 = user.patientProfile?.address?.city?.toString(); // string | undefinde
}

function ensurePatientProfileTest(user: UserContext) {
    ensurePatientProfile(user, "address", "name");

    const city1 = user.patientProfile.address.city?.toString(); // string | unedfined
}

function ensurePathsTest(user: UserContext) {
    ensurePaths(user, "patientProfile.address.city", "patientProfile.address.street");

    const city1 = user.patientProfile.address.city.toString(); // string | unedfined
}
