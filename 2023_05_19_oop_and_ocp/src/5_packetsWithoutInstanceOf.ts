import { compareNumber, executeSamples } from "./packetsCommon"

// https://adventofcode.com/2022/day/13

abstract class Packet {
    abstract compare(p: Packet): number;

    abstract compareToValue(p: ValuePacket): number;
    abstract compareToList(p: ListPacket): number;
}

class ValuePacket extends Packet {
    constructor(public value: number) {
        super();
    }
    compare(p: Packet): number {
        return p.compareToValue(this) * -1;
    };

    compareToValue(p: ValuePacket): number {
        return compareNumber(this.value, p.value);
    }
    compareToList(p: ListPacket): number {
        return new ListPacket([this]).compare(p);
    }
}

class ListPacket extends Packet {
    constructor(public items: Packet[]) {
        super();
    }
    compare(p: Packet): number {
        return p.compareToList(this) * -1;
    }
    compareToValue(p: ValuePacket): number {
        return this.compare(new ListPacket([p]));
    }
    compareToList(p: ListPacket): number {
        for (let i = 0; i < Math.min(this.items.length, p.items.length); i++) {
            let result = this.items[i].compare(p.items[i]);
            if (result !== 0) {
                return result;
            }
        }
        return compareNumber(this.items.length, p.items.length);
    }
}


executeSamples(ValuePacket, ListPacket);
