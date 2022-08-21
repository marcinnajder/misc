import { inspect } from "util";
import { pipe, map, toarray, filter } from "powerseq";

interface Results {
    [id: string]: { label: string; value: string; };
}

let results: Results = {
    companyName: { label: "CompanyName", value: "Super Company" },
    address: { label: "Address", value: "London" },
    nip: { label: "Nip", value: "00000" },
    regon: { label: "Regon", value: "000000" },
    email: { label: "Email", value: "example@example.com" },
    phone: { label: "Phone", value: "000000000" },
};

let LAYOUT = [
    ['companyName'],
    ['address'],
    ['nip', 'regon'],
    ['email', 'phone'],
];


function formatColumns(layout: string[][], answersObj: Results) {
    return pipe(layout,
        map(row =>
            pipe(row,
                map(id => answersObj[id]),
                filter(a => !!a),
                map(({ label, value }) => ({ text: `${label}: ${value}` })),
                toarray())
        ),
        filter(columns => columns.length > 0),
        map(columns => ({ columns })),
        toarray()
    );
}

let pdf = formatColumns(LAYOUT, results);
let pdfText = inspect(pdf, false, 10);
console.log("pdf", pdfText);

// http://pdfmake.org/playground.html
// var dd = {
// 	content:  [
//         { columns: [{ text: 'CompanyName: Super Company' }] },
//         { columns: [{ text: 'Address: London' }] },
//         { columns: [{ text: 'Nip: 00000' }, { text: 'Regon: 000000' }] },
//         { columns: [{ text: 'Email: example@example.com' }, { text: 'Phone: 000000000' },] }
//     ]
// }