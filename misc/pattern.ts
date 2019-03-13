const x: any = 1;

interface A { }
interface B { }

function hej(x: A | B): x is A {
  return true;
}

if (hej(x)) {
  console.log(x);
}

// function match<T>(x: A | T, ...y: T[]): x is T {

function match123<T, TResult>(x: any | T, y: T, a: (item: T) => TResult): TResult {
  return null as any as TResult;
}

match123(x, Date, x => x.name);
match123(x, { name: "marcin" }, x => x.name);
match123(x, [1, 2, 3, 4], x => x.length);




//function match<T>(x: any | T, y: Date[]): x is Date;
function match<T>(x: any | T, ...y: T[]): x is T {
  return true;
}

const temp = [
  ({ x, y }: { x: string, y: string }) => x + y,
  ({ x = "marcin" }) => 1,
  ([a, b, ...c]) => 1,
];



// function matchIf<T>(x: A | T, y: T): x is T {
//   return true;
// }

[

  (x = { imie: "maricin" }) => x.imie,
  (x = 1) => x,
  (x = [1, 2, 3]) => x,
  ([o, ...bla] = [1, 2, 3]) => x
]

if (match(x, null)) { console.log(x); }


// typy
if (match(x, Number)) { console.log(x); }
if (match(x, String)) { console.log(x); }
if (match<Date>(x, Date as any)) { console.log(x); }
if (match<Date>(x, Date as any)) { console.log(x); }

// stale
if (match(x, "marcin")) { console.log(x); }
if (match(x, false)) { console.log(x); }
if (match(x, null)) { console.log(x); }
if (match(x, undefined)) { console.log(x); }
if (match(x, Array)) { console.log(x); }


if (match(x, { name: "marcin" })) { console.log(x); }

if (match(x, ["a"])) { console.log(x); }

type A2 = typeof Date;


function bla2(a: typeof Date): Date {
  return new Date();
}


var aa: ReturnType<bla2>;

var a: InstanceType<typeof Date>;


declare class Date2 {
  new(...args: any[]);
}

class Person { }

function a<T extends new (...args: any[]) => any>(a: T) {
  return null as InstanceType<T>
}


type XY = { type: "x", x: 1 } | { type: "y", y: 1 };

if (x instanceof Person) {

}

if (match(x, null)) console.log(x);
if (x instanceof Date) console.log(x);
if (match(x, undefined)) console.log(x);
if (match(x, 1, 2, 4, 5)) console.log(x);
if (match(x, [1, 2])) console.log(x);
if (match(x, Date)) console.log(x);
if (match(x, { name: "marcin" })) console.log(x.name);
if (match(x, Date)) console.log(x.name);



const bla =
  match(x, 1) ? "jeden " + x :
    match(x, 1, 2) ? "jeden lub dwa " + x :
      //match<XY>(x, null) && x.type === "x" && x. ? "jeden lub dwa " + x : ?? 
      //match(x, Date) ? "jeden lub dwa " + x :
      match(x, Person) ? "jeden lub dwa " + x :
        match(x, []) && x.length === 6 ? "pusta tablica " + x :


          match(x, { name: "marcin" }) ? "obiekt marcin" + x.name :
            "cos innego";






// jak zrobic dekonstrukcje

// pattern matching
// - to jest wyrazenie 
// - sprawdzenie wzroca: 1, "", [ 1,2], {name: "marcin"}, String, Int
// - uwzycie danych po spawdzeniu patterna (destrukcja: [x:es], lub wywnioskwoanie typu w TS)
// - mozlwiosc definiwoania kliku warunkow
// - wildcards first (x, _, _) = x 
// - capital all@(x:xs) = "The first letter of " ++ all ++ " is " ++ [x]
// - | a > b     = a  



// C# 
// - sprawszenie ze danego typu i dostep do zmiennej tego typu,
// - Person {NAme:"marcin"}
// - warunek when
// - dekonstrukcja klasy do tupla








