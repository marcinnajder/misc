import { pipe, map, pairwise, scan, every, } from "powerseq";

// https://adventofcode.com/2024/day/2
// - kolejne liczby albo rosna albo majeja ale tylko o 1..3
// - tutaj chodzilo o implementacje ktora zatrzymuje iteracje w momencie napotkania blednej liczby

var codes = [
    [1, 3, 4, 5, 8],
    [-1, -3, -4, -5, -8],
    [1, 3, 4, 4, 5, 8],
    [1, 3, 40, 4, 5, 8],
    [5, 4, 6, 7],
    [-5, -4, -6, -7],
];


for (var code of codes) {
    var result = pipe(code,
        pairwise(),
        scan(([i, r], [a, b]) => [i + 1, r + (a == b || Math.abs(a - b) > 3 ? 0 : (a > b ? 1 : -1))], [0, 0]),
        every(([i, r]) => i === r || i === -r)
    );
    console.log(code, " -> ", result);
}


