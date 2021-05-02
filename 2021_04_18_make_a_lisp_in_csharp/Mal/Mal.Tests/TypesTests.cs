
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Mal.Types;


namespace Mal.Tests
{
    [TestClass]
    public class TypesTests
    {
        [TestMethod]
        public void MalEqualTest()
        {
            Assert.IsTrue(MalEqual(new Str("a"), new Str("a")));
            Assert.IsFalse(MalEqual(new Str("a"), new Str("b")));
            Assert.IsFalse(MalEqual(new Str("a"), new Symbol("b", NilV)));

            Func<MalType[], MalType> f = _ => NilV;
            Assert.IsTrue(MalEqual(new Fn(f, NilV), new Fn(f, NilV)));
            Assert.IsFalse(MalEqual(new Fn(_ => NilV, NilV), new Fn(f, NilV)));


            Assert.IsTrue(MalEqual(
                new Map(new() { { new Str("name"), new Str("marcin") } }, NilV),
                new Map(new() { { new Str("name"), new Str("marcin") } }, NilV)));

            Assert.IsFalse(MalEqual(
                new Map(new() { { new Keyword("name"), new Str("marcin") } }, NilV),
                new Map(new() { { new Str("name"), new Str("marcin") } }, NilV)));

            Assert.IsFalse(MalEqual(
                new Map(new() { { new Str("name"), new Keyword("marcin") } }, NilV),
                new Map(new() { { new Str("name"), new Str("marcin") } }, NilV)));

            Assert.IsFalse(MalEqual(
                new Map(new() { { new Str("name"), new Str("marcin") }, { new Str("age"), new Number(30) } }, NilV),
                new Map(new() { { new Str("name"), new Str("marcin") } }, NilV)));
        }
    }
}

