namespace MiniMal.Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Types
open Core
open TestUtils

[<TestClass>]
type CoreTests() =

    [<TestMethod>]
    member this.ExecuteArithmeticFnTest() =
        let add (a: double) b = a + b
        assertThrowsException (fun () -> executeArithmeticFn [] add)
        assertThrowsException (fun () -> executeArithmeticFn [ Number(1.) ] add)
        assertThrowsException (fun () -> executeArithmeticFn [ Number(1.); Str("2") ] add)
        assertMalEquals (Number(1. + 2. + 3.)) (executeArithmeticFn [ Number(1.); Number(2.); Number(3.) ] add)

    [<TestMethod>]
    member this.ExecuteComparisonFnTest() =
        let lessThen (a: double) b = a < b
        assertThrowsException (fun () -> executeComparisonFn [] lessThen)
        assertThrowsException (fun () -> executeComparisonFn [ Number(1.); Number(2.); Number(3.) ] lessThen)
        assertThrowsException (fun () -> executeComparisonFn [ Str("1"); Str("2") ] lessThen)
        assertMalEquals True (executeComparisonFn [ Number(123.); Number(124.) ] lessThen)
        assertMalEquals False (executeComparisonFn [ Number(123.); Number(123.) ] lessThen)

    [<TestMethod>]
    member this.ListFnTest() =
        assertMalEquals (MalList([], List)) (listFn [])
        assertMalEquals (MalList([ Number(1.) ], List)) (listFn [ Number(1.) ])
        assertMalEquals (MalList([ Number(1.); Number(2.) ], List)) (listFn [ Number(1.); Number(2.) ])

    [<TestMethod>]
    member this.ConstTest() =
        assertThrowsException (fun () -> constFn [])
        assertThrowsException (fun () -> constFn [ MalList([ Number(1.) ], List) ])
        assertThrowsException (fun () -> constFn [ MalList([ Number(1.); Number(2.) ], List) ])

        assertMalEquals
            (MalList([ Number(1.); Number(2.) ], List))
            (constFn [ Number(1.)
                       MalList([ Number(2.) ], List) ])


    [<TestMethod>]
    member this.ConcatTest() =
        assertThrowsException (fun () -> concatFn [ Number(1.) ])
        assertMalEquals (MalList([], List)) (concatFn [])
        assertMalEquals
            (MalList([ Number(1.); Number(2.); Number(3.) ], List))
            (concatFn [ MalList([], List)
                        MalList([ Number(1.); Number(2.) ], List)
                        MalList([], List)
                        MalList([ Number(3.) ], List)
                        MalList([], List) ])


    [<TestMethod>]
    member this.ConjFnTest() =
        assertMalEquals
            (MalList([ Number(3.); Number(2.); Number(1.) ], List))
            (conjFn [ MalList([ Number(1.) ], List)
                      Number(2.)
                      Number(3.) ])
        assertMalEquals
            (MalList([ Number(1.); Number(2.); Number(3.) ], Vector))
            (conjFn [ MalList([ Number(1.) ], Vector)
                      Number(2.)
                      Number(3.) ])

    [<TestMethod>]
    member this.CountFnTest() =
        assertThrowsException (fun () -> countFn [])
        assertThrowsException (fun () -> countFn [ Number(1.) ])
        assertThrowsException (fun () -> countFn [ Number(1.); Number(2.) ])
        assertMalEquals (Number(0.)) (countFn [ MalList([], List) ])
        assertMalEquals (Number(0.)) (countFn [ MalList([], Vector) ])
        assertMalEquals (Number(1.)) (countFn [ MalList([ Number(100.) ], List) ])
        assertMalEquals (Number(0.)) (countFn [ Nil ])

    [<TestMethod>]
    member this.FirstTest() =
        assertThrowsException (fun () -> firstFn [])
        assertThrowsException
            (fun () ->
                firstFn [ MalList([], List)
                          Number(0.) ])

        assertMalEquals (Number(123.)) (firstFn [ MalList([ Number(123.); Number(456.) ], List) ])
        assertMalEquals Nil (firstFn [ MalList([], List) ])
        assertMalEquals Nil (firstFn [ Nil ])

    [<TestMethod>]
    member this.RestTest() =
        assertThrowsException (fun () -> restFn [])
        assertThrowsException (fun () -> restFn [ MalList([], List); Number(0.) ])

        assertMalEquals (MalList([ Number(456.) ], List)) (restFn [ MalList([ Number(123.); Number(456.) ], List) ])
        assertMalEquals (MalList([], List)) (restFn [ MalList([], List) ])
        assertMalEquals (MalList([], List)) (restFn [ Nil ])

    [<TestMethod>]
    member this.NthTest() =
        assertThrowsArgumentException (fun () -> nthFn [ MalList([], List); Number(0.) ])
        assertMalEquals
            (Number(123.))
            (nthFn [ MalList([ Number(123.); Number(456.) ], List)
                     Number(0.) ])
    [<TestMethod>]
    member this.IsEmptyFnTest() =
        assertThrowsException (fun () -> isEmptyFn [])
        assertThrowsException (fun () -> isEmptyFn [ Number(1.) ])
        assertThrowsException (fun () -> isEmptyFn [ Number(1.); Number(2.) ])

        assertMalEquals True (isEmptyFn [ MalList([], List) ])
        assertMalEquals True (isEmptyFn [ MalList([], Vector) ])
        assertMalEquals False (isEmptyFn [ MalList([ Number(1.) ], List) ])

    [<TestMethod>]
    member this.IsListTest() =
        assertMalEquals True (isListFn [ MalList([], List) ])
        assertMalEquals False (isListFn [ MalList([], Vector) ])

    [<TestMethod>]
    member this.VecTest() =
        assertThrowsException (fun () -> vecFn [ Number(1.) ])
        assertThrowsException (fun () -> vecFn [])

        assertMalEquals (MalList([ Number(1.) ], Vector)) (vecFn [ MalList([ Number(1.) ], Vector) ])
        assertMalEquals (MalList([ Number(1.) ], Vector)) (vecFn [ MalList([ Number(1.) ], List) ])

    [<TestMethod>]
    member this.AssocTest() =
        assertThrowsException (fun () -> assocFn [])
        assertThrowsException (fun () -> assocFn [ Number(0.) ])
        let map =
            Map([ ("a", Number(1.)); ("b", Number(2.)) ])
        let malMap = MalMap(map)
        assertThrowsException (fun () -> assocFn [ malMap; Str("c") ])
        let newMalType =
            assocFn [ malMap
                      Str("c")
                      Number(3.)
                      Str("d")
                      Number(4.) ]
        assertMalEquals (MalMap(map.Add("c", Number(3.)).Add("d", Number(4.)))) newMalType

    [<TestMethod>]
    member this.DissocTest() =
        assertThrowsException (fun () -> dissocFn [])
        assertThrowsException (fun () -> dissocFn [ Number(0.) ])
        let map =
            Map([ ("a", Number(1.)); ("b", Number(2.)) ])
        let malMap = MalMap(map)
        let newMalType = dissocFn [ malMap; Str("b") ]
        assertMalEquals (MalMap(map.Remove("b"))) newMalType


//         [TestMethod]
//         public void EqualsFnTest()
//         {
//             Assert.ThrowsException<Exception>(() => Core.EqualsFn(null));
//             Assert.ThrowsException<Exception>(() => Core.EqualsFn(MalLListFrom(new Number(1))));
//             Assert.ThrowsException<Exception>(() => Core.EqualsFn(MalLListFrom(new Number(1), new Number(2), new Number(3))));

//             Assert.AreEqual(TrueV, Core.EqualsFn(MalLListFrom(new Number(123), new Number(123))));
//             Assert.AreEqual(FalseV, Core.EqualsFn(MalLListFrom(new Number(123), new Number(1230))));
//             Assert.AreEqual(FalseV, Core.EqualsFn(MalLListFrom(new Number(123), new Str("123"))));

//             Assert.AreEqual(TrueV, Core.EqualsFn(MalLListFrom(
//                 new List(new(new Number(4), null), ListType.Vector),
//                 new List(new(new Number(4), null), ListType.Vector)
//             )));
//         }




//         [TestMethod]
//         public void ReadStringFnTest()
//         {
//             Assert.ThrowsException<Exception>(() => Core.ReadStringFn(null));
//             Assert.ThrowsException<Exception>(() => Core.ReadStringFn(MalLListFrom(new Number(1))));
//             Assert.ThrowsException<Exception>(() => Core.ReadStringFn(MalLListFrom(new Str(""), new Str(""))));

//             Assert.AreEqual(new Number(123), Core.ReadStringFn(MalLListFrom(new Str("123"))));
//             Assert.AreEqual(NilV, Core.ReadStringFn(MalLListFrom(new Str(""))));
//         }















// }
