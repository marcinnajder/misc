namespace MiniMal.Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Types
open Printer


[<TestClass>]
type PrinterTests() =

    [<TestMethod>]
    member this.MalEqualTest() =
        Assert.AreEqual("nil", printStr Nil)
        Assert.AreEqual("true", printStr True)
        Assert.AreEqual("false", printStr False)
        Assert.AreEqual("\"hej\"", printStr (Str("hej")))
        Assert.AreEqual("bla", printStr (Symbol("bla")))
        Assert.AreEqual("123", printStr (Number(123.)))
        let mals = [ Str("name"); Nil ]
        Assert.AreEqual("(\"name\" nil)", printStr (MalList(mals, List)))
        Assert.AreEqual("((\"name\" nil) nil)", printStr (MalList([ MalList(mals, List); Nil ], List)))
        let map = MalMap(Map [ "name", Nil ])
        Assert.AreEqual("{\"name\" nil}", printStr map)
        Assert.AreEqual("#<function>", printStr (Fn((fun args -> Nil), false)))
