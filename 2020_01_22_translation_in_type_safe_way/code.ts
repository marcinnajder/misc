type DefaultLanguage = 'en';

type TranslationValue<D extends string = DefaultLanguage> =
    | { [l: string]: string }
    | ((...args: any) => { [l in D]: string } & { [l: string]: string });

type TranslationDef<D extends string = DefaultLanguage> = {
    [word: string]: TranslationValue<D>;
};

type Translation<T extends TranslationDef<D>, D extends string = DefaultLanguage> = {
    [P in keyof T]: T[P] extends (...args: infer A) => any ? (...args: A) => string : string;
};



function codeWithoutTranslation() {
    const n = 10;
    if (confirm(`Do you want to remove ${n} items ?`)) {
        alert("Items have been removed")
    } else {
        alert("Items have not been removed")
    }
}

const langs = {
    "Items have been removed": {
        pl: "Elementy zostały usuniete"
    },
    "Items have not been removed": {
        // we don't need to translate all words from the start
    },
    AskBeforeRemovingNItems: (n: number) => ({
        en: `Do you want to remove ${n} items ?`,
        pl: `Czy chcesz usunąć ${n} elementów ?`
    })
}

function codeWithTranslation() {
    const currentLanguage = prompt("Choose language ('en' or 'pl'): ", "en");
    const t = createTranslation(langs, /* defaultLanguage */ "en", currentLanguage!);

    const n = 10;
    if (confirm(t.AskBeforeRemovingNItems(n))) {
        alert(t["Items have been removed"])
    } else {
        alert(t["Items have not been removed"])
    }
}

function createTranslation<T extends TranslationDef<D>, D extends string = DefaultLanguage>(
    definition: T, defaultLanguage: D, currentLanguage: string, ): Translation<T, D> {

    return Object.keys(definition).reduce((obj, word) => {
        const t = definition[word];
        obj[word] = typeof t !== 'function' ? t[currentLanguage] ?? word
            : (...args: any[]) => {
                const tt = t(...args);
                return tt[currentLanguage] ?? tt[defaultLanguage];
            };
        return obj;
    }, {} as any);
}

// codeWithoutTranslation();
codeWithTranslation();