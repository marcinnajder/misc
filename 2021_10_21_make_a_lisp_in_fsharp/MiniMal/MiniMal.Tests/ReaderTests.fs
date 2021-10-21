namespace MiniMal.Tests

open Microsoft.VisualStudio.TestTools.UnitTesting
open Types
open Reader
open TestUtils

[<TestClass>]
type ReaderTests() =

    [<TestMethod>]
    member this.ReadAtomTest() =
        assertMalEquals (readAtom "false") False
        assertMalEquals (readAtom "true") True
        assertMalEquals (readAtom "nil") Nil
        assertMalEquals (readAtom "abc") (Symbol("abc"))
        assertMalEquals (readAtom "\"abc\"") (Str("abc"))
        assertMalEquals (readAtom "123") (Number(123.))
        assertThrowsException (fun () -> readAtom "\"abc")


    [<TestMethod>]
    member this.ReadListTest() =
        let tokens = [ "a"; "123"; ")" ]
        let { ReadListResult.Result = items } = readList tokens ")"
        match items with
        | [ first; second ] ->
            assertMalEquals first (Symbol("a"))
            assertMalEquals second (Number(123.))
        | _ -> Assert.Fail("incorrect list created")


    [<TestMethod>]
    member this.ListToMapTest() =
        let nameProp = Str("name")
        let name = Str("adam")
        let ageProp = Str("age")
        let age = Number(20.)
        assertThrowsException (fun () -> listToMap ([ nameProp ]))
        assertThrowsException (fun () -> listToMap ([ nameProp; name; ageProp ]))
        assertThrowsException (fun () -> listToMap ([ nameProp; name; age; age ]))
        let malMap =
            listToMap ([ nameProp; name; ageProp; age ])
        match malMap with
        | MalMap (map) ->
            Assert.AreEqual(2, Map.count map)
            assertMalEquals name (Map.find "name" map)
            assertMalEquals age (Map.find "age" map)
        | _ -> Assert.Fail("incorrect map created")

    [<TestMethod>]
    member this.ReadFormTest() =
        assertMalEquals (readForm [ "nil"; "true" ]).Result.Value Nil

        assertMalEquals
            (readForm [ "("; "+"; "1"; "2"; ")" ])
                .Result
                .Value
            (MalList([ Symbol("+"); Number(1.); Number(2.) ], List))

        assertMalEquals
            (readForm [ "("
                        "1"
                        "["
                        "2"
                        "]"
                        ")" ])
                .Result
                .Value
            (MalList([ Number(1.); MalList([ Number(2.) ], Vector) ], List))

        assertMalEquals
            (readForm [ "{"
                        "\"name\""
                        "\"adam\""
                        "}" ])
                .Result
                .Value
            (MalMap(Map [ "name", Str("adam") ]))
