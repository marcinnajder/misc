import { Option, some, none, error, ResultS, ok, matchUnion } from "powerfp";
import { MalType, MalType_fn, fn } from "./types";
import { ns, Ns } from "./core";


export function defaultEnv(ns: Ns) {
  const env = new Env(none);
  for (const key of Object.keys(ns)) {
    env.set(key, fn(ns[key])); // wrap in MalType_fn    
  }
  return env;
}


export class Env {
  private data: any = {};

  constructor(private outer: Option<Env>, binds: string[] = [], exprs: MalType[] = []) {
    for (let i = 0; i < binds.length; ++i) {
      this.set(binds[i], exprs[i]);
    }
  }
  set(key: string, mal: MalType) {
    this.data[key] = mal;
    return mal;
  }
  private find(key: string): Option<Env> {
    if (key in this.data) {
      return some(this);
    };

    return this.outer.bind(e => e.find(key));
  }
  get(key: string): ResultS<MalType> {
    const envO = this.find(key);
    return matchUnion(envO, {
      none: _ => error(`key '${key}' not found in any env`),
      some: ({ value }) => ok(value.data[key])
    })
  }
}

