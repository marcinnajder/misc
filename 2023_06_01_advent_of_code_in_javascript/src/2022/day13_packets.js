var { readFileSync } = require("fs");
var { EOL } = require("os");
var { pipe, map, toarray, buffer, count, sum, filter, zip, find, groupby, skip, take, toobject, reduce } = require("powerseq");


function compareNumber(a, b) {
    return a === b ? 0 : a < b ? -1 : 1; // a - b;
}

function loadData(input, parseLine_ = JSON.parse) {
    return pipe(
        input.split(EOL),
        filter(line => line !== ""),
        map(parseLine_),
        toarray
    );
}


function comaprePackets(left, right) {
    if (typeof left === "number" && typeof right === "number") {
        return compareNumber(left, right);
    }
    if (typeof left === "number") {
        return comaprePackets([left], right);
    }
    if (typeof right === "number") {
        return comaprePackets(left, [right]);
    }

    var result = find(zip(left, right, comaprePackets), r => r !== 0);
    return result || compareNumber(left.length, right.length);
}


function puzzle1(input) {
    return pipe(
        input,
        loadData,
        buffer(2, 2),
        map((pair, i) => [pair, i]),
        filter(([pair, _]) => comaprePackets(...pair) === -1),
        sum(([_, i]) => i + 1)
    );
}


function puzzle2(input) {
    var devider2 = [[2]];
    var devider6 = [[6]];
    var { "-1": lowerThan2, "1": greaterThen2 } =
        pipe(
            input,
            loadData,
            groupby(p => comaprePackets(p, devider2)),
            toobject(gr => gr.key, gr => [...gr]));
    var lowerThan6Count = count(greaterThen2, p => comaprePackets(p, devider6) == -1);
    return (lowerThan2.length + 1) * (lowerThan2.length + 1 + lowerThan6Count + 1);
}


var input = readFileSync("./src/2022/day13.txt", "utf-8");

console.log(puzzle1(input));
console.log(puzzle2(input));


function readTokens(tokens, items) {
    var { value: token, done } = tokens.next();

    if (done || token === "]") {
        return items;
    }

    items.push(token === "[" ? readTokens(tokens, []) : parseInt(token));
    return readTokens(tokens, items);
}


function parseLine(line) {
    var tokens = line.match(/\d+|\[|\]/g);
    return readTokens(tokens[Symbol.iterator](), [])[0];
}



// https://www.typescriptlang.org/play?#code/GYVwdgxgLglg9mABBOBbADgQwE4FMByIqARrtgBSYBcYRp2ANIsVYrSWQJQDeAUIgMR4oIbEkyIAtMwDciAPTyhuEWMQSAvFuaIA-IgAMiVhIA8O-ZICMxxFZm8Avr16LEAKneIA7jCgALRBgwAGcoTEhcOGAXTGIw7ExoZAAbTBCQxAAFJIBrFT5BdXioROSUDBxccnRWHIh8qE5WdnoHZ14INIzEADVMFJBcesbEXAAPKFwwABNMkYL+QRRQ0pBoOAp0EGIUmAhEADcBoZo6LkRuRCWigRCQdDJyTgci5yKKrDwauryVZrY52wlxut2EoiQ6CCqwiECiwD6J2GfygoNugn0nyqhA4FACMBCADpjoNcEx0MSkZw0ejbGBcN5EAAZAlQBZQcgAbXxIQAupxCVjvugXqDHO0XF10pkWWF2WNJtM5tkUSCPggEusoJsajs9gc-LhUCEqOzOfzLtdaYh7o8KKK3qChdVaiqGv8WkC1bSVmEglNjYgNIgocEwrD4czWfL9BTDYHWDTrcnEDVoeHItFEaSY4hOeheXSQCkUqKk4hgDqUioghoDHIYOYALKYAKE1DBcg8wnxonVsAAcwCTF7AEJCf2h-4XkEANSznjlorVqDKe4pVfB7u9zkwXmCtBfapj3f814poLAch4der0faAyLi9glQQtfFqDnlPvWk-9HgtRnRxegu38AkewDPtpinEdIPHScAgdQRxScFw3E8Hw-H8OAQFXMNwkzGJeDiBIklXKUenZAAmQpBBI0oyOQQ8qh+N1GiogFWjIBxQXospyOYvAABU4H6UlWLEoZqM4oEv2KUjykE3ARNlDlXVU6TPVxCVOm6TJJORd0oCohUplmeYURop0NTWDYtj1fYjiRM5cU4S1y1tJ4kIEP8mMqYVfiMjitPob1-1fNQKWdESDNAgk3K8aw5JQ6z-OU0SkQkpFNMBXFaPC1QkCAoE4qJEkhnJSlSW8xBfOiuBVNYjTLJkvLywApB6UZZqgq5Hl+QPNKahq5wOgomVo0s0ylQsoKwqY1ZsC1HVtl2Rze1NSzzTcq4PIeLzktSo9WJyrjgXyl9CpDQajxU1lSoSqR7DFI6qhizLXQM06gQuooOsQbtnXILqozlSyuQLTgRte4SGvu9TJqC1r6F+wRKwoFda3rIJm1bfx207bdIInaDh2u3sScHRCG3nJ8L0xm8PyDAGwKJHc9xuli40g08autGAr0ZjdEHvYNH1RlN-qFz8l2Q8tfL+iKiqU4CniJo0oKp-xKophDp0OjpXCUDCQkwDBqxCFxQEgWAEAVXAIFw6pgCoAAxcBoHgMA6YEX04GrCc4AHchgEJbUAGVSmCYPOCYK8oZ03hMd6LJma+yyHExplU+DHr2J4zHDisIxgxBlPyGL6lC6sABWZmy6yCua9FQu69Lhk+kb5ueImB2nfIVM3I0AA+I5i8574i8fUVe8dqYB+eINR6nifqiL5uZ-GPv58Hpex4MVfyEODee63ufql3ke2A77OuWPpgp4GoGQdvzl7-3-kE94Wf+8v0eX8bm-GuD9i5PyUsDG+gD37H0-pvbeF9F5XwAXfYBH8BTP0gSgkBNdYGn3gQvIe-9MFAJAQYMBQ1kEkP3g-HBUM4HnwIXvSh0Di40PIcdZhqDH50LwQwv+19upQNoYfY+X8f470QcvGuh9OG4JcOIhBhCBHMiEewliU8xFn1-pIo40iMGCLvqAnhLggA

// function compareNumber(a:number, b: number){
//     return a - b; // return a === b ? 0 : a < b ? -1 : 1;
// }

// // ** with instanceof

// abstract class Packet{
//     abstract compare(p: Packet): number;
// }

// class ValuePacket extends Packet{
//     constructor(public value:number) { 
//         super();
//     }
//     compare(p: Packet): number {
//         return p instanceof ValuePacket
//             ? compareNumber(this.value, p.value)
//             : new ListPacket([this]).compare(p);
//     };
// }

// class ListPacket extends Packet {
//     constructor(public items:Packet[]) { 
//         super();
//     }
//     compare(p: Packet): number {
//         const items = p instanceof ListPacket ? p.items : 
//                         (p instanceof ValuePacket ? [p] : null);

//         for(let i=0; i< Math.min(this.items.length, items!.length); i++){
//             let result = this.items[i].compare(items![i]);
//             if(result !== 0){
//                 return result;
//             }
//         }
//         return compareNumber(this.items.length, items!.length);
//     };
// }

// // ** without instanceof

// abstract class Packet2{
//     abstract compare(p: Packet2): number;

//     abstract compareToValue(p: ValuePacket2): number;
//     abstract compareToList(p: ListPacket2): number;
// }

// class ValuePacket2 extends Packet2{
//     constructor(public value:number) { 
//         super();
//     }
//     compare(p: Packet2): number {
//         return p.compareToValue(this) * -1;
//     };

//     compareToValue(p: ValuePacket2): number{
//         return compareNumber(this.value, p.value);
//     }
//     compareToList(p: ListPacket2): number{
//         return new ListPacket2([this]).compare(p);
//     }
// }

// class ListPacket2 extends Packet2 {
//     constructor(public items:Packet2[]) { 
//         super();
//     }
//     compare(p: Packet2): number {
//         return p.compareToList(this) * -1;
//     }
//     compareToValue(p: ValuePacket2): number{
//         return this.compare(new ListPacket2([p]));
//     }
//     compareToList(p: ListPacket2): number{
//         for(let i=0; i< Math.min(this.items.length, p.items.length); i++){
//             let result = this.items[i].compare(p.items[i]);
//             if(result !== 0){
//                 return result;
//             }
//         }
//         return compareNumber(this.items.length, p.items.length);
//     }
// }


// // ** samples

// function execute(f:Function){
//     console.log(f.toString(), f());
// }


// let VP = ValuePacket2;
// let LP = ListPacket2;

// let v10 = new VP(10)
// let v15 = new VP(15);
// let v5 = new VP(5);

// execute( () => v10.compare(v10));
// execute( () => v10.compare(v15));
// execute( () => v10.compare(v5));

// execute( () => new LP([v5, v10]).compare(new LP([v5, v10])));
// execute( () => new LP([v5, v10]).compare(new LP([v5, v5])));
// execute( () => new LP([v5, v10]).compare(new LP([v5, v15])));

// execute( () => new LP([v5, v10]).compare(new LP([v5, v10, v5])));
// execute( () => new LP([v5, v10, v5]).compare(new LP([v5, v10])));

// execute( () => new LP([v5]).compare(v5));
// execute( () => v5.compare(new LP([v5])));

// execute( () => new LP([v5]).compare(v10));
// execute( () => v5.compare(new LP([v10])));

