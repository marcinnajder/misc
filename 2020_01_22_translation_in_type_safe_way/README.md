
## Translations in type safe way

Let's say we have a very simple piece of code:

```typescript
function codeWithoutTranslation() {
    const n = 10;
    if (confirm(`Do you want to remove ${n} items ?`)) {
        alert("Items have been removed")
    } else {
        alert("Items have not been removed")
    }
}
```

and we would like to add support for translation to other foreign languages. First we can define a dictionary containing all necessary words:

```typescript
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
```

Now we can change slightly the original code:

```typescript
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
```

And that's all. We just call `createTranslation` function passing JS object with translations and the current language. We also need to specify a default language in case of missing translations. What we get as an output is an JS object containing translated words or functions returning translated words.

We can imagine how this function is implemented, nothing really fancy. But because we use TypeScript we get some extra features:
- if you pass a wrong text as a property of object `t`, you will get an error
- we have intellisense for object `t` and we can even change the text using code refactoring command "Rename Symbol"
- the schema of JS translation object is validated so we get an error, for instance if the type of translated word is different from `string` type or if we forget to set a translation for default language (`en` in this case)

Now let's look at the implementation:

```typescript
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
```


