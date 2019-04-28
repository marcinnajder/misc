import { Option, some, none } from "./utils/option";
import { MalType, MalType_number, MalType_fn } from "./types";
import { error, ResultS, ok } from "./utils/result";
import { ns, Ns } from "./core";



export function defaultEnv(ns: Ns) {
  const env = new Env(none());

  for (const key of Object.keys(ns)) {
    env.set(key, { type: "fn", fn: ns[key] } as MalType_fn); // wrap into MalType_fn
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
  find(key: string): Option<Env> {
    if (key in this.data) {
      return some(this);
    };
    switch (this.outer.type) {
      case "none": return none();
      case "some": return this.outer.value.find(key);
    }
  }
  get(key: string): ResultS<MalType> {
    const envO = this.find(key);
    switch (envO.type) {
      case "none": return error(`key '${key}' not found in any env`);
      case "some": return ok(envO.value.data[key]);
    }
  }
}

