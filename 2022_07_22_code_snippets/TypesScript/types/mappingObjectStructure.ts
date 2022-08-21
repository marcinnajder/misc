import { EOL } from "os";
import { pipe, map, join } from "powerseq";

interface TextQuestion {
    elementType: "text",
    properties: {
        questionId: string;
        question: string;
        required: boolean;
    };
}

type TextAnswer = string;

interface CheckboxesQuestion {
    elementType: "checkboxes";
    properties: {
        questionId: string;
        question: string;
        required: boolean;
        answers: {
            answerId: string;
            description: string;
            allowCustom?: boolean;
        }[];
    }
}

interface CheckboxesAnswer {
    [key: string]: null | string;
}

type Question_ = TextQuestion | CheckboxesQuestion;
type Answer_ = TextAnswer | CheckboxesAnswer;

type SurveyDefinition_ = Question_[];
type SurveyResult_ = { [questionId: string]: Answer_ };


let surveyDefinition: SurveyDefinition_ = [
    { elementType: "text", properties: { questionId: "1", required: true, question: "What is your name ?" } },
    { elementType: "text", properties: { questionId: "2", required: false, question: "What is your job ?" } },
    {
        elementType: "checkboxes", properties: {
            questionId: "3", required: true, question: "What are your favourite programming languages?",
            answers: [
                { answerId: "ts", description: "TypeScript" },
                { answerId: "csharp", description: "C#" },
                { answerId: "fsharp", description: "F#" },
                { answerId: "other", description: "Other", allowCustom: true }
            ]
        }
    },
];

let surveyResult: SurveyResult_ = {
    "1": "Marcin",
    "2": "Developer",
    "3": {
        "fsharp": null,
        "other": "Haskell"
    }
};

function formatTextQuestion_(definition: TextQuestion, result: TextAnswer) {
    return `${definition.properties.question}: ${result}`;
}

function formatCheckboxesQuestion_(definition: CheckboxesQuestion, result: CheckboxesAnswer) {
    return "...";
}

function formatEmail_(definition: SurveyDefinition_, result: SurveyResult_): string {
    return "...";
}

function formatPdf_(definition: SurveyDefinition_, result: SurveyResult_): Object {
    return {}; // http://pdfmake.org
}


// ************************************************************************************

type Mapping = [
    [CheckboxesQuestion, CheckboxesAnswer],
    [TextQuestion, TextAnswer],
]

type Question = Mapping[number][0];
type Answer = Mapping[number][1];

type SurveyDefinition = Question[];
type SurveyResult = { [questionId: string]: Answer };

type AnswerFor<Q extends Question> = Extract<Mapping[number], [Q, any]>[1];

function formatTextQuestion(definition: TextQuestion, result: AnswerFor<TextQuestion>) {
    return `${definition.properties.question}: ${result}`;
}

// ************************************************************************************


// https://github.com/marcinnajder/powerfp/blob/master/src/types.ts
type ElementType<T = string> = { elementType: T };
type UnionChoice<T extends ElementType<string>, TT extends T["elementType"]> = Extract<T, { elementType: TT }>;

type TransformObj<R> = {
    [P in Question["elementType"]]: (question: UnionChoice<Question, P>, answer: AnswerFor<UnionChoice<Question, P>>) => R;
};

function formatSurvey<T>(definition: SurveyDefinition, result: SurveyResult, transformObj: TransformObj<T>) {
    return pipe(definition,
        join(Object.entries(result), q => q.properties.questionId, ([id]) => id, (q, [_, a]) => ({ q, a })),
        map(({ q, a }) => transformObj[q.elementType](q as any, a as any)));
}

function formatEmail(definition: SurveyDefinition_, result: SurveyResult_) {
    const lines = formatSurvey(definition, result, {
        text(definition, result) {
            return `${definition.properties.question}: ${result}`;
        },
        checkboxes(definition, result) {
            const text = Object.entries(result)
                .map(([id, a]) => a ?? definition.properties.answers.find(a => a.answerId === id)?.description)
                .join();
            return `${definition.properties.question}: ${text}`;
        }
    })

    return [...lines].join(EOL);
}

let email = formatEmail(surveyDefinition, surveyResult);

console.log(email);
