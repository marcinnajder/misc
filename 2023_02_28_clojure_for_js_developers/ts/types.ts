
// type Path<T> = []

// var aaa : SebkaType<Person> = ["adreess", "city", "length"]

interface Person {
    name: string;
    age: number;
    address: {
        city: string;
        country: string;
        no: number;
    };
    phoneNumbers: string[];
    // boss: Person;
}

type Path2__<O extends object> =
    keyof O extends infer K
    ? O extends object ? K extends keyof O ? O[K] extends object ? [K, ...Path2__<O[K]>] | [K] : [K] : never : never
    : never;

type TPaths123123 = Path2<Person>;

var aa: TPaths123123 = ["address", "country", "length"];





// może da się uprościć

// type TPaths = Path2<Person>;

// i niby jest fajnie

// type TPaths = ["name"] | ["age"] | ["address", "city"] | ["address", "country"] | ["address", "no"] | ["address"]






var p = {} as Person;

var p2 = { ...p, name: "marcin", address: { ...p.address, city: "Wooclaw" } };

assoc(p, ["name"], "");
assoc(p, ["address", "no"], 1);
assoc(p, ["phoneNumbers", 0], "666777888");

var t: [string, number] = ["mama", 70];





function assoc<T, P1 extends keyof T>(o: T, paths: [P1], value: T[P1]): null;
function assoc<T, P1 extends keyof T, P2 extends keyof T[P1]>(o: T, paths: [P1, P2], value: T[P1][P2]): null;

function assoc(o: any, paths: any, value: any): null {
    return null;
}


// type Bla<F> = F extends (infer ...args: any[]) => void ? string : number;


//type RestType<T> = T extends (o: infer O, p: infer TT, v: infer V) => any ? TT : never; 

// type Arr<O, Rest extends any[]> = Rest extends [infer P extends keyof O,  ... infer PP ] ? 

// ( [P, ...PP] )
// : never;


// assoc(p,["name"])
//P__ extends [infer P1 extends keyof O, ...infer Bla]



// function assoc2<O>(o: O, paths: [infer P extends keyof O , ...infer Bla], value: string) : null {
//     return null;
// }


type PathValue<O, PP> = PP extends [infer P extends keyof O, ... infer Rest]
    ? (Rest extends [] ? O[P] : PathValue<O[P], Rest>)
    : never;

type PathValueTest1 = PathValue<Person, ["address", "city", 0]>;
type PathValueTest2 = PathValue<Person, ["phoneNumbers", 0]>;

// type Path<O, PP> = PP extends [infer P e] ? 
//     [infer P extends keyof O] : never;



//type PathTest1 = Path<Person>;

// var a : PathTest1 = ["name"];




// PP extends [infer P extends keyof O,... infer Rest] 
//  ? (Rest extends [] ? O[P] : Path<O[P], Rest>)
//  : never;






function assoc2<O, P extends keyof O, PRest>(o: O, paths: [P, ...PRest[]], value: O[P]): null {
    return null;
}

// function assoc2<O, P extends keyof O, PRest  >(o: O, paths: [P, ...PRest[]], value: O[P]) : null {
//     return null;
// }







// type Path<O, P extends in keyof P> =  [P];

// type Path<O> = O extends [infer Z, ...infer W] ? [Z,W] : never;
/// var p : Path<Person,"name"> = ["name"];




type BuildTuple<Current extends [...T[]], T, Count extends number> =
    Current["length"] extends Count
    ? Current
    : BuildTuple<[T, ...Current], T, Count>

type Tuple<T, Count extends number> = BuildTuple<[], T, Count>

type StringQuintuple = Tuple<string, 5>



type Path<O, PP> = PP extends [infer P extends keyof O, ... infer Rest]
    ? (Rest extends [] ? never : Path<O[P], Rest>)
    : unknown;


type Bla123 = Path<Person, ["name"]>;
type Bla1234 = Path<Person, []>;

assoc3(p, ["name"], 1);


function assoc3<O, TProps, V extends PathValue<O, TProps>>(o: O, path: TProps, value: V) {
}


// type BuildPath<O, PP> =  O extends object 
//     ? [infer P extends keyof O, ...BuildPath<O[P]>] : never

// type PathValue<O, PP> = PP extends [infer P extends keyof O,... infer Rest] 
//  ? (Rest extends [] ? O[P] : PathValue<O[P], Rest>)
//  : never;



// Seba

type Path2<O extends object> =
    keyof O extends infer K
    ? O extends object ? K extends keyof O ? O[K] extends object ? [K, ...Path2<O[K]>] | [K] : [K] : never : never
    : never;

// type TPaths = ["name"] | ["age"] | ["address", "city"] | ["address", "country"] | ["address", "no"] | ["address"]
type TPaths = Path2<Person>;

type Path3<O, PP> = PP extends [infer P extends keyof O, ... infer Rest]
    ? (Rest extends [] ? [P] : [P, ...Path3<O[P], Rest>])
    : never

type E = Path3<Person, [""]>;
