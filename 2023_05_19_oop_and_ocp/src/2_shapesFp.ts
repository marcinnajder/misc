import type { } from "fs";

abstract class Shape { }

class Square extends Shape {
    constructor(public size: number) {
        super();
    }
}

class Rectangle extends Shape {
    constructor(public width: number, public height: number) {
        super();
    }
}

function getArea(shape: Shape) {
    if (shape instanceof Square) {
        return Math.pow(shape.size, 2);
    }
    if (shape instanceof Rectangle) {
        return shape.width * shape.height;
    }
    throw new Error("unknown shape");
}

let shapes = [new Square(3), new Rectangle(2, 3)];
for (const shape of shapes) {
    console.log("area: ", getArea(shape));
}
