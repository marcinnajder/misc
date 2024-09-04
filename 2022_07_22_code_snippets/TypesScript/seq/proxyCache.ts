type User = {}

class SuperProxy {
    getUserById(userId: string): Promise<User> { return null as any; }
    getAllUsers(): Promise<User[]> { return null as any; }
    add(a: number, b: number): Promise<number> { return null as any; }
}

// built-in type definitions required T to be a function
// type ReturnType<T extends (...args: any) => any> = T extends (...args: any) => infer R ? R : any
// type Parameters<T extends (...args: any) => any> = T extends (...args: infer P) => any ? P : never
type ReturnType2<T> = T extends (...args: any) => Promise<infer R> ? R : never;
type Parameters2<T> = T extends (...args: infer P) => any ? P : never;

class SuperCache {
    get<P, M extends keyof P>(proxy: P, method: M, ...args: Parameters2<P[M]>): ReturnType2<P[M]> {
        return null as any;
    }
}

const superProxy = new SuperProxy();
const cache = new SuperCache();

// infers correctly arguments and results
var user = cache.get(superProxy, "getUserById", "1");
var users = cache.get(superProxy, "getAllUsers");
var n = cache.get(superProxy, "add", 1, 2);

