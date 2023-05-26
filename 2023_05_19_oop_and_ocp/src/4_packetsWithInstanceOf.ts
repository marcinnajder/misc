import { compareNumber, executeSamples } from "./packetsCommon"

// https://adventofcode.com/2022/day/13

abstract class Packet {
    abstract compare(p: Packet): number;
}

class ValuePacket extends Packet {
    constructor(public value: number) {
        super();
    }
    compare(p: Packet): number {
        return p instanceof ValuePacket
            ? compareNumber(this.value, p.value)
            : new ListPacket([this]).compare(p);
    };
}

class ListPacket extends Packet {
    constructor(public items: Packet[]) {
        super();
    }
    compare(p: Packet): number {
        const items = p instanceof ListPacket ? p.items :
            (p instanceof ValuePacket ? [p] : null);

        for (let i = 0; i < Math.min(this.items.length, items!.length); i++) {
            let result = this.items[i].compare(items![i]);
            if (result !== 0) {
                return result;
            }
        }
        return compareNumber(this.items.length, items!.length);
    };
}


executeSamples(ValuePacket, ListPacket);
