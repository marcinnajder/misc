import { distinct, filter, pipe } from "powerseq";
import { Iterator_ } from "./iteratorHelpers";


// ---- samples for blog post articles

//var array = [5, 10, 15];
//var array = [5, 10, 15][Symbol.iterator]();

// for (var item of array) {
//     console.log(item);
// }
// for (var item of array) {
//     console.log(item);
// }

for (const digit of filter("ab5c10d15ef", c => c >= "0" && c <= "9")) {
    console.log(digit); // 5, 1, 0, 1, 5
}

for (const digit of Iterator_.from("ab5c10d15ef").filter(c => c >= "0" && c <= "9").toArray()) {
    console.log(digit); // 5, 1, 0, 1, 5
}

console.log([1, 2, 3, 4, 5].values().filter(n => n % 2 === 0).toArray()); // 2, 4



const items1 = Iterator_.from({
    next() {
        return { done: false, value: 666 }
    }
}).take(3).toArray(); // [666, 666, 666]
console.log(items1);


function* range(start: number, count: number) {
    const end = start + count;
    for (let i = start; i < end; i++) {
        yield i;
    }
}

const items2 = range(0, Number.MAX_VALUE).drop(100).filter(x => x % 2 === 0).take(5).toArray();
console.log(items2); 
