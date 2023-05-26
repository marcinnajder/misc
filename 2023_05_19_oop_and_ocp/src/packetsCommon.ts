import type { } from "fs";

export function compareNumber(a: number, b: number) {
    return a - b; // return a === b ? 0 : a < b ? -1 : 1;
}

function execute(f: Function) {
    console.log(f.toString(), f());
}



class Packet {
    compare(p: Packet): number {
        return 0;
    }
}
class ValuePacket extends Packet {
    constructor(value: number) {
        super();
    }
}
class ListPacket extends Packet {
    constructor(items: Packet[]) {
        super();
    }
}


export function executeSamples(valuePacketType: typeof ValuePacket, listPacketType: typeof ListPacket) {
    let VP = valuePacketType;
    let LP = listPacketType;

    let v10 = new VP(10)
    let v15 = new VP(15);
    let v5 = new VP(5);

    execute(() => v10.compare(v10));
    execute(() => v10.compare(v15));
    execute(() => v10.compare(v5));

    execute(() => new LP([v5, v10]).compare(new LP([v5, v10])));
    execute(() => new LP([v5, v10]).compare(new LP([v5, v5])));
    execute(() => new LP([v5, v10]).compare(new LP([v5, v15])));

    execute(() => new LP([v5, v10]).compare(new LP([v5, v10, v5])));
    execute(() => new LP([v5, v10, v5]).compare(new LP([v5, v10])));

    execute(() => new LP([v5]).compare(v5));
    execute(() => v5.compare(new LP([v5])));

    execute(() => new LP([v5]).compare(v10));
    execute(() => v5.compare(new LP([v10])));
}
