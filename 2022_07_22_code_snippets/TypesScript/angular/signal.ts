
type Disponse = () => void;

type Listener<T> = (value: T) => void;

interface Signal<T> {
  (): T;
  set: (value: T) => void;
  _subscribe(listener: Listener<T>): Disponse;
}

class SignalObj<T> {
  static deps?: SignalObj<any>[];

  private value: T;
  private subs: Listener<T>[] = [];

  constructor(value: T) {
    this.value = value;
  }

  public get(): T {
    if (SignalObj.deps) {
      SignalObj.deps.push(this);
    }
    return this.value;
  }

  public set(value: T) {
    this.value = value;
    this.subs.forEach(s => s(value));
  }

  subscribe(listener: Listener<T>): Disponse {
    this.subs.push(listener);
    const dispose = () => {
      const index = this.subs.indexOf(listener);
      if (index !== -1) {
        this.subs.splice(index, 1);
      }
    };
    return dispose;
  }
}

function signal<T>(value: T): Signal<T> {
  const s = new SignalObj(value);
  return Object.assign(() => s.get(), { set: s.set.bind(s), _subscribe: s.subscribe.bind(s) });
}

function computed<T>(action: () => T): Signal<T> {
  try {
    SignalObj.deps = [];
    const s = signal(action());
    SignalObj.deps.forEach(d => d.subscribe(_ => s.set(action())));
    return s;
  } finally {
    SignalObj.deps = undefined;
  }
}


var a = signal(1);
console.log("a:", a());



var dis = a._subscribe(v => console.log("value changed:", v));
a.set(10);
console.log("a:", a());

dis();
a.set(100);
console.log("a:", a());

var b = signal(3);
var c = computed(() => a() + b());
console.log(c());

c._subscribe(v => console.log("c changed: ", v));
a.set(0);
console.log(c());

