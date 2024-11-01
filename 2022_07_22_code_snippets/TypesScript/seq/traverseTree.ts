import { pipe, flatmap, concat, map, distinct, toarray, find } from "powerseq";

interface Element {
    name: string;
    type: string;
    items?: Element[];
}


let root = {
    name: "C",
    type: "disc",
    items: [
        {
            name: "A",
            type: "folder",
            items: [
                { name: "file1", type: "file" },
                { name: "file1", type: "file" }
            ]
        },
        {
            name: "file666",
            type: "file",
        }
    ]
};


function traverse(elements: Element[]): Iterable<Element> {
    return flatmap(elements, e => !e.items ? [e] : concat([e], traverse(e.items)));
}

function findElementOfType(elements: Element[], elementType: string) {
    return pipe(
        traverse(elements),
        find(ee => ee.type === elementType)
    );
}

console.log("element of type 'file': ", findElementOfType([root], "file"));



function getUniqueElementTypes(element: Element) {
    return pipe(
        traverse([element]),
        distinct(ee => ee.type),
        toarray()
    );
}

console.log("unique element types: ", getUniqueElementTypes(root));
