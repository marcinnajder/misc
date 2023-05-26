import type { } from "fs";

abstract class ShapeVistor<T> {
    abstract visitSquare(square: Square): T;
    abstract visitRectangle(rectangle: Rectangle): T;
}

abstract class Shape {
    abstract accept<T>(visitor: ShapeVistor<T>): T;
}


class Square extends Shape {
    constructor(public size: number) {
        super();
    }
    accept<T>(visitor: ShapeVistor<T>): T {
        return visitor.visitSquare(this);
    }
}

class Rectangle extends Shape {
    constructor(public width: number, public height: number) {
        super();
    }
    accept<T>(visitor: ShapeVistor<T>): T {
        return visitor.visitRectangle(this);
    }
}

class GetAreaVisitor extends ShapeVistor<number>{
    visitSquare(square: Square): number {
        return Math.pow(square.size, 2);
    }
    visitRectangle(rectangle: Rectangle): number {
        return rectangle.width * rectangle.height;
    }
}

class ToStringVisitor extends ShapeVistor<string>{
    visitSquare(square: Square): string {
        return `Square: ${square.size}`;
    }
    visitRectangle(rectangle: Rectangle): string {
        return `Rectangle: ${rectangle.width}, ${rectangle.height}`;
    }
}


let shapes = [new Square(3), new Rectangle(2, 3)];

let getAreaVisitor = new GetAreaVisitor();
for (const shape of shapes) {
    console.log("area: ", shape.accept(getAreaVisitor));
}

let toStringVisitor = new ToStringVisitor();
for (const shape of shapes) {
    console.log(shape.accept(toStringVisitor));
}

