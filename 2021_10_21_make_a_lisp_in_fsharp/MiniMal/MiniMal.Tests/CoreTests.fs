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
