interface Array<T> {
    flatmap<T2>(f: (item: T) => T2[]): T2[];
}