
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Mal.Types;


namespace Mal.Tests
{
    [TestClass]
    public class ReaderTests
    {
        [TestMethod]
        public void ReadAtomTest()
        {
            Assert.AreEqual(TrueV, Reader.ReadAtom("true"));
            Assert.AreEqual(FalseV, Reader.ReadAtom("false"));
            Assert.AreEqual(NilV, Reader.ReadAtom("nil"));
            Assert.AreEqual(new Keyword("hej"), Reader.ReadAtom(":hej"));
            Assert.AreEqual(new Symbol("hej"), Reader.ReadAtom("hej"));
            Assert.AreEqual(new Str("hej"), Reader.ReadAtom("\"hej\""));
            Assert.AreEqual(new Str("hej \\ \n "), Reader.ReadAtom("\"hej \\\\ \\n \""));

            Assert.ThrowsException<Exception>(() => Reader.ReadAtom("\""));
            Assert.ThrowsException<Exception>(() => Reader.ReadAtom("\"hej"));
            Assert.ThrowsException<Exception>(() => Reader.ReadAtom("\"hej\" "));
        }

        // [TestMethod]
        // public void KeyToStringTest()
        // {
        //     Assert.AreEqual("s123", new Str("123").KeyToString());
        //     Assert.AreEqual("k123", new Keyword("123").KeyToString());
        //     Assert.ThrowsException<Exception>(() => TrueV.KeyToString());
        // }

    }
}

