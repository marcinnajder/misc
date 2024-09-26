import { pipe } from "powerseq";
import { promiseBind, promiseMap } from "powerfp";

function inc(n: number) {
    return n + 1;
}
async function getUserNameById(id: number) {
    return "admin";
}
function toUpperCase(text: string) {
    return text.toUpperCase();
}
async function saveUserName(name: string) {
    return true;
}

const result = pipe(10, inc, getUserNameById, promiseMap(toUpperCase), promiseBind(saveUserName));

result.then(x => console.log(x));


