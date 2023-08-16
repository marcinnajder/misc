

// the same as built in "Pick<T, K>" type
type Projection<T, PN extends keyof T> = {
    [P in PN]: T[P];
};

function select<T, P extends keyof T>(obj: T, ...props: P[]): Projection<T, P> { // Pick
    var result = {} as any;
    for (const p of props) {
        result[p] = obj[p];
    }
    return result;
}

var patient = { n: 1, s: "", b: true };

var ns = select(patient, "n", "s");
console.log(ns);