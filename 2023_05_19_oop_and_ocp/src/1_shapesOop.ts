import type { } from "fs";

abstract class Shape {
    abstract getArea(): number;
}

class Square extends Shape {
    constructor(public size: number) {
        super();
    }
    getArea() {
        return Math.pow(this.size, 2);
    }
}

class Rectangle extends Shape {
    constructor(public width: number, public height: number) {
        super();
    }
    getArea() {
        return this.width * this.height;
    }
}

let shapes = [new Square(3), new Rectangle(2, 3)];
for (const shape of shapes) {
    console.log("area: ", shape.getArea());
}

