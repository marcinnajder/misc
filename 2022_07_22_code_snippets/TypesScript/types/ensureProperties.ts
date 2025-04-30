
export function ensureValue<T>(value: T | null | undefined): asserts value is NonNullable<T> {
    if (value === null || typeof value === "undefined") {
        throw new Error(`value '${value}' is null or undefined`);
    }
}

export type RequiredProperties<T extends object, P extends keyof T> = Omit<T, P> & Required<Pick<T, P>>;

export function ensureProperties<T extends object, P extends keyof T>(
    obj: T | RequiredProperties<T, P>, ...propertyNames: P[]): asserts obj is RequiredProperties<T, P> {
    for (const propertyName of propertyNames) {
        ensuredProperty(obj, propertyName);
    }
}


export function ensuredProperty<T extends object, P extends keyof T>(obj: T, propertyName: P): NonNullable<T[P]> {
    if (obj[propertyName] === null || typeof obj[propertyName] === "undefined") {
        throw new Error(`property '${String(propertyName)}' of object '{${Object.keys(obj)}}' is null or undefined`);
    }
    return obj[propertyName];
}

export function ensuredValue<T>(value: T | null | undefined): NonNullable<T> {
    ensureValue(value)
    return value;
}


interface User {
    email: string;
    patientProfile?: {
        name: string;
        address?: {
            street: string;
        }
    }
}


function test(user: User) {
    ensureProperties(user, "patientProfile");
    ensureProperties(user.patientProfile, "address");

    console.log("user.patientProfile.address.street:", user.patientProfile?.address?.street);
    //console.log("user.patientProfile.address.street:", user.patientProfile?.address?.street);
}



test({ email: "", patientProfile: { name: "", address: { street: "wiosenna" } } });

test({ email: "", patientProfile: { name: "" } });


// type Paths<O> = PathsAux<O, [], "">[number];
// type PathsAux<O, Acc extends string[], Current extends string> =
//   O extends object ?
//     keyof O extends infer Keys
//       ? Keys extends string
//         ? Keys extends keyof O
//           ? PathsAux<O[Keys], [...Acc, `${Current}${Keys}`], `${Current}${Keys}.`>
//           : Acc : [...Acc, Keys] : Acc : Acc;


// interface Foo {
//   bar: {
//     baz: string;
//     qux: {
//       quux: number;
//     };
//   };
//   quuux: boolean;
// }

// type FooPaths = Paths<Foo>; // = "bar" | "quuux" | "bar.baz" | "bar.qux" | "bar.qux.quux"