
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Mal.EnvM;
using static Mal.Types;
using PowerFP;
using static PowerFP.LListM;
using static Mal.Core;
using System;
using System.Linq;

namespace Mal.Tests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void MalFunctionAttributeTest()
        {
            var fns = Core.GetMalFunctions().ToEnumerable().ToList();
            var fn = (fns.FirstOrDefault(f => f.Name.Name == "=")).Fn as Fn;

            Assert.IsNotNull(fn);
            Assert.AreEqual(TrueV, fn!.Value(LListM.LListFrom<MalType>(new Number(1), new Number(1))));
            Assert.AreEqual(FalseV, fn!.Value(LListM.LListFrom<MalType>(new Number(1), new Number(2))));
            Assert.AreEqual(FalseV, fn!.Value(LListM.LListFrom<MalType>(new Number(1), new Str("a"))));

            Assert.ThrowsException<Exception>(() => fn!.Value(LListM.LListFrom<MalType>(new Number(1), new Number(2), new Number(3))));
        }



        [TestMethod]
        public void ExecuteArithmeticFnTest()
        {
            Func<double, double, double> add = (a, b) => a + b;

            Assert.ThrowsException<Exception>(() => Core.ExecuteArithmeticFn(null, add));
            Assert.ThrowsException<Exception>(() => Core.ExecuteArithmeticFn(new(new Number(1), null), add));
            Assert.ThrowsException<Exception>(() => Core.ExecuteArithmeticFn(new(new Number(1), new(new Str("2"), null)), add));

            Assert.AreEqual(new Number(1 + 2 + 3), Core.ExecuteArithmeticFn(new Number[] { new(1), new(2), new(3) }.Cast<MalType>().ToLList(), add));
        }

        [TestMethod]
        public void ListFnTest()
        {
            Assert.AreEqual(new List(null, ListType.List, NilV), Core.ListFn(LListFrom<MalType>()));
            Assert.AreEqual(new List(new(new Number(1), null), ListType.List, NilV), Core.ListFn(LListFrom<MalType>(new Number(1))));
            Assert.AreEqual(new List(new(new Number(1), new(new Number(2), null)), ListType.List, NilV),
                Core.ListFn(LListFrom<MalType>(new Number(1), new Number(2))));
        }


        [TestMethod]
        public void IsListFnTest()
        {
            Assert.ThrowsException<Exception>(() => Core.IsListFn(null));
            Assert.ThrowsException<Exception>(() => Core.IsListFn(LListFrom<MalType>(new Number(1), new Number(2))));

            Assert.AreEqual(TrueV, Core.IsListFn(LListFrom<MalType>(new List(new(new Number(1), null), ListType.List, NilV))));
            Assert.AreEqual(FalseV, Core.IsListFn(LListFrom<MalType>(new List(new(new Number(1), null), ListType.Vector, NilV))));
            Assert.AreEqual(FalseV, Core.IsListFn(LListFrom<MalType>(new Number(1))));
        }


        [TestMethod]
        public void IsEmptyFnTest()
        {
            Assert.ThrowsException<Exception>(() => Core.IsEmptyFn(null));
            Assert.ThrowsException<Exception>(() => Core.IsEmptyFn(LListFrom<MalType>(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.IsEmptyFn(LListFrom<MalType>(new Number(1), new Number(2))));

            Assert.AreEqual(TrueV, Core.IsEmptyFn(LListFrom<MalType>(new List(null, ListType.List, NilV))));
            Assert.AreEqual(TrueV, Core.IsEmptyFn(LListFrom<MalType>(new List(null, ListType.Vector, NilV))));
            Assert.AreEqual(FalseV, Core.IsEmptyFn(LListFrom<MalType>(new List(new(new Number(1), null), ListType.List, NilV))));
        }



        [TestMethod]
        public void CountFnTest()
        {
            Assert.ThrowsException<Exception>(() => Core.CountFn(null));
            Assert.ThrowsException<Exception>(() => Core.CountFn(LListFrom<MalType>(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.CountFn(LListFrom<MalType>(new Number(1), new Number(2))));

            Assert.AreEqual(new Number(0), Core.CountFn(LListFrom<MalType>(new List(null, ListType.List, NilV))));
            Assert.AreEqual(new Number(0), Core.CountFn(LListFrom<MalType>(new List(null, ListType.Vector, NilV))));
            Assert.AreEqual(new Number(1), Core.CountFn(LListFrom<MalType>(new List(new(new Number(100), null), ListType.List, NilV))));
            Assert.AreEqual(new Number(0), Core.CountFn(LListFrom<MalType>(NilV)));
        }

        [TestMethod]
        public void EqualsFnTest()
        {
            Assert.ThrowsException<Exception>(() => Core.EqualsFn(null));
            Assert.ThrowsException<Exception>(() => Core.EqualsFn(LListFrom<MalType>(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.EqualsFn(LListFrom<MalType>(new Number(1), new Number(2), new Number(3))));

            Assert.AreEqual(TrueV, Core.EqualsFn(LListFrom<MalType>(new Number(123), new Number(123))));
            Assert.AreEqual(FalseV, Core.EqualsFn(LListFrom<MalType>(new Number(123), new Number(1230))));
            Assert.AreEqual(FalseV, Core.EqualsFn(LListFrom<MalType>(new Number(123), new Str("123"))));

            Assert.AreEqual(TrueV, Core.EqualsFn(LListFrom<MalType>(
                new List(new(new Number(4), null), ListType.Vector, NilV),
                new List(new(new Number(4), null), ListType.Vector, NilV)
            )));
        }


        [TestMethod]
        public void ExecuteComparisonFnTest()
        {
            Func<double, double, bool> lessThen = (a, b) => a < b;

            Assert.ThrowsException<Exception>(() => Core.ExecuteComparisonFn(null, lessThen));
            Assert.ThrowsException<Exception>(() => Core.ExecuteComparisonFn(LListFrom<MalType>(new Number(1)), lessThen));
            Assert.ThrowsException<Exception>(() => Core.ExecuteComparisonFn(LListFrom<MalType>(new Number(1), new Number(2), new Number(3)), lessThen));
            Assert.ThrowsException<Exception>(() => Core.ExecuteComparisonFn(LListFrom<MalType>(new Str("1"), new Str("2")), lessThen));

            Assert.AreEqual(TrueV, Core.ExecuteComparisonFn(LListFrom<MalType>(new Number(123), new Number(124)), lessThen));
            Assert.AreEqual(FalseV, Core.ExecuteComparisonFn(LListFrom<MalType>(new Number(123), new Number(123)), lessThen));
        }

        [TestMethod]
        public void ReadStringFnTest()
        {
            Assert.ThrowsException<Exception>(() => Core.ReadStringFn(null));
            Assert.ThrowsException<Exception>(() => Core.ReadStringFn(LListFrom<MalType>(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.ReadStringFn(LListFrom<MalType>(new Str(""), new Str(""))));

            Assert.AreEqual(new Number(123), Core.ReadStringFn(LListFrom<MalType>(new Str("123"))));
            Assert.AreEqual(NilV, Core.ReadStringFn(LListFrom<MalType>(new Str(""))));
        }


        [TestMethod]
        public void AtomTest()
        {
            var atom = Core.AtomFn(new(new Number(1), null));
            Assert.AreEqual(new Atom(new Number(1)), atom);

            Assert.AreEqual(TrueV, Core.IsAtomFn(new(atom, null)));
            Assert.AreEqual(FalseV, Core.IsAtomFn(new(new Number(1), null)));

            Assert.AreEqual(new Number(1), Core.DerefFn(new(atom, null)));

            Assert.AreEqual(new Number(2), Core.ResetFn(new(atom, new(new Number(2), null))));
            Assert.AreEqual(new Atom(new Number(2)), atom);

            FnDelegate fn = args => args switch
            {
                (Number(var number1), (Number(var number2), null)) => new Number(number1 + number2),
                _ => throw new Exception("")
            };
            Assert.AreEqual(new Number(102), Core.SwapFn(LListM.LListFrom<MalType>(atom, new Fn(fn, NilV), new Number(100))));
        }


        [TestMethod]
        public void ConstTest()
        {
            Assert.ThrowsException<Exception>(() => Core.ConsFn(null));
            Assert.ThrowsException<Exception>(() => Core.ConsFn(LListFrom<MalType>(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.ConsFn(LListFrom<MalType>(new Number(1), new Number(2))));

            Assert.AreEqual(
                new List(LListFrom<MalType>(new Number(1), new Number(2)), ListType.List, NilV),
                Core.ConsFn(LListFrom<MalType>(new Number(1), new List(new(new Number(2), null), ListType.List, NilV))));
        }

        [TestMethod]
        public void ConcatTest()
        {
            Assert.ThrowsException<Exception>(() => Core.ConcatFn(LListFrom<MalType>(new Number(1))));

            Assert.AreEqual(new List(null, ListType.List, NilV), Core.ConcatFn(null));
            Assert.AreEqual(
                new List(LListFrom<MalType>(new Number(1), new Number(2), new Number(3)), ListType.List, NilV),
                Core.ConcatFn(LListFrom<MalType>(
                    new List(null, ListType.List, NilV),
                    new List(LListFrom<MalType>(new Number(1), new Number(2)), ListType.List, NilV),
                    new List(null, ListType.List, NilV),
                    new List(LListFrom<MalType>(new Number(3)), ListType.List, NilV),
                    new List(null, ListType.List, NilV)
                )));
        }

        [TestMethod]
        public void VecTest()
        {
            Assert.ThrowsException<Exception>(() => Core.VecFn(LListFrom<MalType>(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.VecFn(LListFrom<MalType>()));

            Assert.AreEqual(
                new List(LListFrom<MalType>(new Number(1)), ListType.Vector, NilV),
                Core.VecFn(new(new List(LListFrom<MalType>(new Number(1)), ListType.Vector, NilV), null)));
            Assert.AreEqual(
                new List(LListFrom<MalType>(new Number(1)), ListType.Vector, NilV),
                Core.VecFn(new(new List(LListFrom<MalType>(new Number(1)), ListType.List, NilV), null)));

        }
    }
}