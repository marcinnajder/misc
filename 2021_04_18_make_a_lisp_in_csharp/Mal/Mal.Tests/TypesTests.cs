
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerFP;
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

            Func<LList<MalType>?, MalType> f = _ => NilV;
            Assert.IsTrue(MalEqual(new Fn(f, NilV), new Fn(f, NilV)));
            Assert.IsFalse(MalEqual(new Fn(_ => NilV, NilV), new Fn(f, NilV)));


            Assert.IsTrue(MalEqual(
                new Map(MapM.MapFrom<MalType, MalType>((new Str("name"), new Str("marcin"))), NilV),
                new Map(MapM.MapFrom<MalType, MalType>((new Str("name"), new Str("marcin"))), NilV)));

            Assert.IsFalse(MalEqual(
                new Map(MapM.MapFrom<MalType, MalType>((new Keyword("name"), new Str("marcin"))), NilV),
                new Map(MapM.MapFrom<MalType, MalType>((new Str("name"), new Str("marcin"))), NilV)));


            Assert.IsFalse(MalEqual(
                new Map(MapM.MapFrom<MalType, MalType>((new Str("name"), new Keyword("marcin"))), NilV),
                new Map(MapM.MapFrom<MalType, MalType>((new Str("name"), new Str("marcin"))), NilV)));

            Assert.IsFalse(MalEqual(
                new Map(MapM.MapFrom<MalType, MalType>((new Str("name"), new Keyword("marcin")), (new Str("age"), new Number(30))), NilV),
                new Map(MapM.MapFrom<MalType, MalType>((new Str("name"), new Str("marcin"))), NilV)));
        }
    }
}

