
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Mal.Types;
using static Mal.Reader;
using static PowerFP.LListM;
using PowerFP;

namespace Mal.Tests
{
    [TestClass]
    public class ReaderTests
    {
        [TestMethod]
        public void ReadAtomTest()
        {
            Assert.AreEqual(TrueV, ReadAtom("true"));
            Assert.AreEqual(FalseV, ReadAtom("false"));
            Assert.AreEqual(NilV, ReadAtom("nil"));
            Assert.AreEqual(new Keyword("hej"), ReadAtom(":hej"));
            Assert.AreEqual(new Symbol("hej", NilV), ReadAtom("hej"));
            Assert.AreEqual(new Str("hej"), ReadAtom("\"hej\""));
            Assert.AreEqual(new Str("hej \\ \n "), ReadAtom("\"hej \\\\ \\n \""));

            Assert.ThrowsException<Exception>(() => ReadAtom("\""));
            Assert.ThrowsException<Exception>(() => ReadAtom("\"hej"));
            Assert.ThrowsException<Exception>(() => ReadAtom("\"hej\" "));
        }


        [TestMethod]
        public void MalsToKeyValuePairsTest()
        {
            Assert.AreEqual(null, MalsToKeyValuePairs(null));
            Assert.AreEqual(
                new (MalType Key, MalType Value)[] { (new Str("a"), TrueV), (new Keyword("b"), FalseV) }.ToLList(),
                MalsToKeyValuePairs(LListFrom<MalType>(new Str("a"), TrueV, new Keyword("b"), FalseV)));

            Assert.ThrowsException<Exception>(() => MalsToKeyValuePairs(LListFrom<MalType>(new Str("a"))));
            Assert.ThrowsException<Exception>(() => MalsToKeyValuePairs(LListFrom<MalType>(TrueV, TrueV)));
        }


        [TestMethod]
        public void ReadListTest()
        {
            Assert.AreEqual(
                new ListReader(new("a", null), LListFrom<MalType>(new Str("a"), FalseV, new Number(12))),
                ReadList(LListFrom("\"a\"", "false", "12", ")", "a"), ")"));

            Assert.ThrowsException<Exception>(() => ReadList(null, ")"));
            Assert.ThrowsException<Exception>(() => ReadList(LListFrom("\"a\"", "false"), ")"));
        }

        [TestMethod]
        public void ReadMetaTest()
        {
            var result = ListFrom(new Symbol("with-meta", NilV), FalseV, TrueV);

            Assert.AreEqual(new FormReader(new("aa", null), result), ReadMeta(LListFrom("true", "false", "aa")));
            Assert.AreEqual(new FormReader(null, result), ReadMeta(LListFrom("true", "false")));

            Assert.AreEqual(new FormReader(null, null), ReadMeta(LListFrom("true")));
            Assert.AreEqual(new FormReader(null, null), ReadMeta(LListFrom<string>()));
        }

        [TestMethod]
        public void ReadMacroTest()
        {
            const string macroToken = "quote";
            var result = ListFrom(new Symbol(macroToken, NilV), TrueV);

            Assert.AreEqual(new FormReader(new("aa", null), result), ReadMacro(LListFrom("true", "aa"), macroToken));
            Assert.AreEqual(new FormReader(null, result), ReadMacro(LListFrom("true"), macroToken));

            Assert.AreEqual(new FormReader(null, null), ReadMacro(LListFrom<string>(), macroToken));
        }


        [TestMethod]
        public void ReadFormTest()
        {
            Assert.AreEqual(new FormReader(null, null), ReadForm(null));
            Assert.AreEqual(
                new FormReader(new("aa", null), ListFrom(new Symbol("with-meta", NilV), FalseV, TrueV)),
                ReadForm(LListFrom("^", "true", "false", "aa")));
            Assert.AreEqual(
                new FormReader(new("aa", null), ListFrom(new Symbol("quote", NilV), TrueV)),
                ReadForm(LListFrom("'", "true", "aa")));
            Assert.AreEqual(
                new FormReader(new("aa", null), ListFrom(TrueV, FalseV)),
                ReadForm(LListFrom("(", "true", "false", ")", "aa")));
            Assert.AreEqual(
                new FormReader(new("aa", null), ListFrom(TrueV, FalseV, ListFrom(NilV))),
                ReadForm(LListFrom("(", "true", "false", "(", "nil", ")", ")", "aa")));

            var (Tokens, Mal) = ReadForm(LListFrom("{", ":name", "\"marcin\"", ":age", "30", "}", "aa"));
            Assert.AreEqual(new("aa", null), Tokens);
            Assert.IsTrue(Mal is Map);
            Assert.IsTrue(Types.MalEqual(
                new Map(new() { { new Keyword("name"), new Str("marcin") }, { new Keyword("age"), new Number(30) } }, NilV),
                Mal!));
        }
    }
}

