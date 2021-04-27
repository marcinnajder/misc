
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerFP;
using static Mal.Types;

namespace Mal.Tests
{
    [TestClass]
    public class PrinterTests
    {
        [TestMethod]
        public void PrintStrTest()
        {
            Assert.AreEqual("true", TrueV.PrintStr());
            Assert.AreEqual("false", FalseV.PrintStr());
            Assert.AreEqual("nil", NilV.PrintStr());
            Assert.AreEqual("123", new Number(123).PrintStr());
            Assert.AreEqual("abc", new Symbol("abc").PrintStr());
            Assert.AreEqual(":abc", new Keyword("abc").PrintStr());
            Assert.AreEqual("(atom abc)", new Atom(new Str("abc")).PrintStr());
            Assert.AreEqual("#<function>", new Fn((_) => NilV).PrintStr());
            Assert.AreEqual("(a true [nil])", new List(new MalType[] { new Str("a"), TrueV, new List(new(NilV, null), ListType.Vector) }.ToLList(), ListType.List).PrintStr());
            Assert.AreEqual("{name marcin :age 30}", new Map(new() { { new Str("name"), new Str("marcin") }, { new Keyword("age"), new Number(30) } }).PrintStr());
        }

    }
}

