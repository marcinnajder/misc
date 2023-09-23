type List<T> = [head: T, tail: List<T>] | null;

var listEmpty: List<number> = null;
var list123: List<number> = [1, [2, [3, null]]];

function rev<T>(l: List<T>, acc: List<T> = null): List<T> {
  return l === null ? acc : rev(l[1], [l[0], acc]);
}

console.log(rev(listEmpty)); // null
console.log(rev(list123)); // [3, [2, [1, null]]]

type Queue<T> = {
  front: List<T>;
  back: List<T>;
}

var queueEmpty: Queue<number> = { front: listEmpty, back: null };
var queue123: Queue<number> = { front: list123, back: null };

function peek<T>(q: Queue<T>) {
  return q.front === null ? null : q.front[0];
}

console.log(peek(queueEmpty)); // null
console.log(peek(queue123)); // 1

function enqueue<T>(q: Queue<T>, x: T): Queue<T> {
  return q.front === null ? { front: [x, null], back: null } : { ...q, back: [x, q.back] };
}

console.log(enqueue(queueEmpty, 1)); // { front: [ 1, null ], back: null } 
console.log(enqueue(enqueue(queueEmpty, 1), 2)); // { front: [ 1, null ], back: [ 2, null ] }

function denqueue<T>(q: Queue<T>): Queue<T> | null {
  return q.front === null ? null
    : q.front[1] === null ? { front: rev(q.back), back: null }
      : { ...q, front: q.front[1] }
}

var queue1234 = [1, 2, 3, 4].reduce(enqueue, queueEmpty);

console.log(queue1234); // {front:[1,null],back:[4,[3,[2,null]]]}
console.log(denqueue(queue1234)); // "{front:[2,[3,[4,null]]],back:null}
console.log(denqueue(denqueue(queue1234)!)); // "{front":[3,[4,null]],back:null}

[denqueue, denqueue, denqueue, denqueue, denqueue].reduce((q, f) => f(q)!, queue1234); // { "front": null, "back": null }
[denqueue, denqueue, denqueue, denqueue, denqueue, denqueue].reduce((q, f) => f(q)!, queue1234); null



