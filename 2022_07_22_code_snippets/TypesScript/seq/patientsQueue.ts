// node22, es2023
import { intersect, some, pipe, orderby, filter, find, flatmap, groupbytotransformedobject, map } from "powerseq";
import * as assert from "assert";

type Markers = number[];

interface Item {
    userId: string;
    markers: Markers;
    time: number;
}

type Queue = ReadonlyArray<Item>;

const data: { [userId: string]: Markers } = {
    "p1": [1, 2],
    "p2": [3],
    "p3": [2],
    "p4": [3, 4],
    "p5": [4],
};

const queue: Queue = Object.entries(data).map(([k, v], i) => ({ time: i, userId: k, markers: v })).toReversed();

tests();
// console.log(queue);
console.log(calcPossitions(queue, [1, 2, 3]));

function areOverlapping(markers1: Markers, markers2: Markers): boolean {
    return some(intersect(markers1, markers2));
}

function dequeue(q: Queue, markers: Markers): [q: Queue, item: Item | null] {
    const deletedItem = pipe(q,
        filter(i => areOverlapping(i.markers, markers)),
        orderby(i => i.time),
        find()
    );

    if (!deletedItem) {
        return [q, null];
    }

    return [q.filter(i => i !== deletedItem), deletedItem];
}

function enqueue(q: Queue, item: Item): Queue {
    return [...q, item];
}

function calcPossitions(q: Queue, markers: Markers) {
    const positions = pipe(q,
        filter(i => areOverlapping(i.markers, markers)),
        flatmap(i => filter(i.markers, m => areOverlapping([m], markers)), (i, m) => ({ ...i, m })),
        groupbytotransformedobject(i => i.m, is => [...pipe(is, orderby(i => i.time), map(i => i.userId))])
    );

    return positions;
}

function tests() {
    assert.equal(areOverlapping([1, 2], [2, 3]), true);
    assert.equal(areOverlapping([1, 2], [2]), true);
    assert.equal(areOverlapping([1, 2], [3, 4]), false);

    {
        const [q1, item1] = dequeue(queue, [2]);
        assert.equal(item1?.userId, "p1");
        assert.equal(q1.length, queue.length - 1);
    }
    {
        const [q1, item1] = dequeue(queue, [4]);
        assert.equal(item1?.userId, "p4");
        assert.equal(q1.length, queue.length - 1);
    }

    {
        const [q1, item1] = dequeue(queue, [3]);
        assert.equal(item1?.userId, "p2");
        assert.equal(q1.length, queue.length - 1);

        const [q2, item2] = dequeue(q1, [4]);
        assert.equal(item2?.userId, "p4");
        assert.equal(q2.length, queue.length - 2);
    }
}

// - problem kolejek pacjentow, tylko pacjent przypisany jest do wielu potencjalnych grup, z drugiej strony lekarze takze sa
// przypisani do wielu grup, jak pojawia sie pacjent to faktycznie pojawia sie w wielu grupach na raz, wiec to nie jest
// jedna kolejka ale wiele kolejek, data pojawienia sie determinuje kolejnosc sciagania z kolejki
// - w tym kodzie chodzilo glownie o to aby zaimplementowac dzialanie, ktore potem i tak jest finalnie realizowane za pomoca
// bazy mongo, chodzi o to aby idealnie jednak operacja mongo (bo bedzie atomomo) znalezc pacjenta i sciagnac go z kolejki,
// nawet taka operacje istnieje https://www.mongodb.com/docs/manual/reference/method/db.collection.findOneAndDelete/
// - potem juz w drugiej kolejnosc mozna zrobic kolejne zapytanie ktore wyliczy nowe polozenia pacjentow na ktore
// usuniecia pacjenta wplynelo, czyli pacjencji ktorzy byli za tym usunietym z kolejki, chodzi o wyslasnie do nich notyfikacji
