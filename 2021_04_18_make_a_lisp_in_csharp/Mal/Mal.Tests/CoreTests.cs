
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
            Assert.AreEqual(TrueV, fn!.Value(MalLListFrom(new Number(1), new Number(1))));
            Assert.AreEqual(FalseV, fn!.Value(MalLListFrom(new Number(1), new Number(2))));
            Assert.AreEqual(FalseV, fn!.Value(MalLListFrom(new Number(1), new Str("a"))));

            Assert.ThrowsException<Exception>(() => fn!.Value(MalLListFrom(new Number(1), new Number(2), new Number(3))));
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
            Assert.AreEqual(new List(null, ListType.List, NilV), Core.ListFn(MalLListFrom()));
            Assert.AreEqual(new List(new(new Number(1), null), ListType.List, NilV), Core.ListFn(MalLListFrom(new Number(1))));
            Assert.AreEqual(new List(new(new Number(1), new(new Number(2), null)), ListType.List, NilV),
                Core.ListFn(MalLListFrom(new Number(1), new Number(2))));
        }


        [TestMethod]
        public void IsListFnTest()
        {
            Assert.ThrowsException<Exception>(() => Core.IsListFn(null));
            Assert.ThrowsException<Exception>(() => Core.IsListFn(MalLListFrom(new Number(1), new Number(2))));

            Assert.AreEqual(TrueV, Core.IsListFn(MalLListFrom(new List(new(new Number(1), null), ListType.List, NilV))));
            Assert.AreEqual(FalseV, Core.IsListFn(MalLListFrom(new List(new(new Number(1), null), ListType.Vector, NilV))));
            Assert.AreEqual(FalseV, Core.IsListFn(MalLListFrom(new Number(1))));
        }


        [TestMethod]
        public void IsEmptyFnTest()
        {
            Assert.ThrowsException<Exception>(() => Core.IsEmptyFn(null));
            Assert.ThrowsException<Exception>(() => Core.IsEmptyFn(MalLListFrom(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.IsEmptyFn(MalLListFrom(new Number(1), new Number(2))));

            Assert.AreEqual(TrueV, Core.IsEmptyFn(MalLListFrom(new List(null, ListType.List, NilV))));
            Assert.AreEqual(TrueV, Core.IsEmptyFn(MalLListFrom(new List(null, ListType.Vector, NilV))));
            Assert.AreEqual(FalseV, Core.IsEmptyFn(MalLListFrom(new List(new(new Number(1), null), ListType.List, NilV))));
        }



        [TestMethod]
        public void CountFnTest()
        {
            Assert.ThrowsException<Exception>(() => Core.CountFn(null));
            Assert.ThrowsException<Exception>(() => Core.CountFn(MalLListFrom(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.CountFn(MalLListFrom(new Number(1), new Number(2))));

            Assert.AreEqual(new Number(0), Core.CountFn(MalLListFrom(new List(null, ListType.List, NilV))));
            Assert.AreEqual(new Number(0), Core.CountFn(MalLListFrom(new List(null, ListType.Vector, NilV))));
            Assert.AreEqual(new Number(1), Core.CountFn(MalLListFrom(new List(new(new Number(100), null), ListType.List, NilV))));
            Assert.AreEqual(new Number(0), Core.CountFn(MalLListFrom(NilV)));
        }

        [TestMethod]
        public void EqualsFnTest()
        {
            Assert.ThrowsException<Exception>(() => Core.EqualsFn(null));
            Assert.ThrowsException<Exception>(() => Core.EqualsFn(MalLListFrom(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.EqualsFn(MalLListFrom(new Number(1), new Number(2), new Number(3))));

            Assert.AreEqual(TrueV, Core.EqualsFn(MalLListFrom(new Number(123), new Number(123))));
            Assert.AreEqual(FalseV, Core.EqualsFn(MalLListFrom(new Number(123), new Number(1230))));
            Assert.AreEqual(FalseV, Core.EqualsFn(MalLListFrom(new Number(123), new Str("123"))));

            Assert.AreEqual(TrueV, Core.EqualsFn(MalLListFrom(
                new List(new(new Number(4), null), ListType.Vector, NilV),
                new List(new(new Number(4), null), ListType.Vector, NilV)
            )));
        }


        [TestMethod]
        public void ExecuteComparisonFnTest()
        {
            Func<double, double, bool> lessThen = (a, b) => a < b;

            Assert.ThrowsException<Exception>(() => Core.ExecuteComparisonFn(null, lessThen));
            Assert.ThrowsException<Exception>(() => Core.ExecuteComparisonFn(MalLListFrom(new Number(1)), lessThen));
            Assert.ThrowsException<Exception>(() => Core.ExecuteComparisonFn(MalLListFrom(new Number(1), new Number(2), new Number(3)), lessThen));
            Assert.ThrowsException<Exception>(() => Core.ExecuteComparisonFn(MalLListFrom(new Str("1"), new Str("2")), lessThen));

            Assert.AreEqual(TrueV, Core.ExecuteComparisonFn(MalLListFrom(new Number(123), new Number(124)), lessThen));
            Assert.AreEqual(FalseV, Core.ExecuteComparisonFn(MalLListFrom(new Number(123), new Number(123)), lessThen));
        }

        [TestMethod]
        public void ReadStringFnTest()
        {
            Assert.ThrowsException<Exception>(() => Core.ReadStringFn(null));
            Assert.ThrowsException<Exception>(() => Core.ReadStringFn(MalLListFrom(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.ReadStringFn(MalLListFrom(new Str(""), new Str(""))));

            Assert.AreEqual(new Number(123), Core.ReadStringFn(MalLListFrom(new Str("123"))));
            Assert.AreEqual(NilV, Core.ReadStringFn(MalLListFrom(new Str(""))));
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
            Assert.AreEqual(new Number(102), Core.SwapFn(MalLListFrom(atom, new Fn(fn, NilV), new Number(100))));
        }


        [TestMethod]
        public void ConstTest()
        {
            Assert.ThrowsException<Exception>(() => Core.ConsFn(null));
            Assert.ThrowsException<Exception>(() => Core.ConsFn(MalLListFrom(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.ConsFn(MalLListFrom(new Number(1), new Number(2))));

            Assert.AreEqual(
                new List(MalLListFrom(new Number(1), new Number(2)), ListType.List, NilV),
                Core.ConsFn(MalLListFrom(new Number(1), new List(new(new Number(2), null), ListType.List, NilV))));
        }

        [TestMethod]
        public void ConcatTest()
        {
            Assert.ThrowsException<Exception>(() => Core.ConcatFn(MalLListFrom(new Number(1))));

            Assert.AreEqual(new List(null, ListType.List, NilV), Core.ConcatFn(null));
            Assert.AreEqual(
                new List(MalLListFrom(new Number(1), new Number(2), new Number(3)), ListType.List, NilV),
                Core.ConcatFn(MalLListFrom(
                    new List(null, ListType.List, NilV),
                    new List(MalLListFrom(new Number(1), new Number(2)), ListType.List, NilV),
                    new List(null, ListType.List, NilV),
                    new List(MalLListFrom(new Number(3)), ListType.List, NilV),
                    new List(null, ListType.List, NilV)
                )));
        }

        [TestMethod]
        public void VecTest()
        {
            Assert.ThrowsException<Exception>(() => Core.VecFn(MalLListFrom(new Number(1))));
            Assert.ThrowsException<Exception>(() => Core.VecFn(MalLListFrom()));

            Assert.AreEqual(
                new List(MalLListFrom(new Number(1)), ListType.Vector, NilV),
                Core.VecFn(new(new List(MalLListFrom(new Number(1)), ListType.Vector, NilV), null)));
            Assert.AreEqual(
                new List(MalLListFrom(new Number(1)), ListType.Vector, NilV),
                Core.VecFn(new(new List(MalLListFrom(new Number(1)), ListType.List, NilV), null)));

        }

        [TestMethod]
        public void NthTest()
        {
            Assert.ThrowsException<Exception>(() => Core.NthFn(MalLListFrom(MalListFrom(), new Number(0))));

            Assert.AreEqual(new Number(123),
                Core.NthFn(MalLListFrom(MalListFrom(new Number(123), new Number(456)), new Number(0))));
        }

        [TestMethod]
        public void FirstTest()
        {
            Assert.ThrowsException<Exception>(() => Core.FirstFn(MalLListFrom()));
            Assert.ThrowsException<Exception>(() => Core.FirstFn(MalLListFrom(MalListFrom(), new Number(0))));

            Assert.AreEqual(new Number(123), Core.FirstFn(MalLListFrom(MalListFrom(new Number(123), new Number(456)))));
            Assert.AreEqual(NilV, Core.FirstFn(MalLListFrom(MalListFrom())));
            Assert.AreEqual(NilV, Core.FirstFn(MalLListFrom(NilV)));
        }


        [TestMethod]
        public void RestTest()
        {
            Assert.ThrowsException<Exception>(() => Core.RestFn(MalLListFrom()));
            Assert.ThrowsException<Exception>(() => Core.RestFn(MalLListFrom(MalListFrom(), new Number(0))));

            Assert.AreEqual(MalListFrom(new Number(456)), Core.RestFn(MalLListFrom(MalListFrom(new Number(123), new Number(456)))));
            Assert.AreEqual(MalListFrom(), Core.RestFn(MalLListFrom(MalListFrom())));
            Assert.AreEqual(MalListFrom(), Core.RestFn(MalLListFrom(NilV)));
        }

        [TestMethod]
        public void ThrowTest()
        {
            Assert.ThrowsException<Exception>(() => Core.ThrowFn(MalLListFrom()));
            Assert.ThrowsException<Exception>(() => Core.ThrowFn(MalLListFrom(new Number(0), new Number(0))));

            Assert.ThrowsException<Core.MalException>(() => Core.ThrowFn(MalLListFrom(new Number(123))));
        }

        [TestMethod]
        public void ApplyTest()
        {
            Assert.ThrowsException<Exception>(() => Core.ApplyFn(MalLListFrom()));
            Assert.ThrowsException<Exception>(() => Core.ApplyFn(MalLListFrom(new Number(0), new Number(0))));

            Assert.AreEqual(new Number(3), Core.ApplyFn(MalLListFrom(
                new Fn(args => new Number(args.ToEnumerable().Max(mal => ((Number)mal).Value)), NilV), new Number(2), MalListFrom(new Number(3), new Number(1)))));
        }

        [TestMethod]
        public void MapTest()
        {
            Assert.ThrowsException<Exception>(() => Core.MapFn(MalLListFrom()));
            Assert.ThrowsException<Exception>(() => Core.MapFn(MalLListFrom(new Number(0), new Number(0))));

            Assert.AreEqual(MalListFrom(new Number(4), new Number(2)), Core.MapFn(MalLListFrom(
                new Fn(args => new Number(((Number)args!.Head).Value + 1), NilV), MalListFrom(new Number(3), new Number(1)))));
        }

        [TestMethod]
        public void AssocTest()
        {
            Assert.ThrowsException<Exception>(() => Core.AssocFn(MalLListFrom()));
            Assert.ThrowsException<Exception>(() => Core.AssocFn(MalLListFrom(new Number(0))));

            var malMap = new Map(MalMapFrom(
                (new Keyword("a") as Keyword, new Number(1)),
                (new Keyword("b"), new Number(2))
                ), NilV);

            Assert.ThrowsException<Exception>(() => Core.AssocFn(MalLListFrom(malMap, new Keyword("c"))));

            Assert.AreEqual(
                malMap with { Value = malMap.Value.Add(new Keyword("c"), new Number(3)).Add(new Keyword("d"), new Number(4)) },
                Core.AssocFn(MalLListFrom(malMap, new Keyword("c"), new Number(3), new Keyword("d"), new Number(4)))
                );
        }

        [TestMethod]
        public void DissocTest()
        {
            Assert.ThrowsException<Exception>(() => Core.DissocFn(MalLListFrom()));
            Assert.ThrowsException<Exception>(() => Core.DissocFn(MalLListFrom(new Number(0))));

            var malMap = new Map(MalMapFrom(
                (new Keyword("a") as Keyword, new Number(1)),
                (new Keyword("b"), new Number(2))
                ), NilV);

            Assert.AreEqual(
                malMap with { Value = malMap.Value.Remove(new Keyword("c")) },
                Core.DissocFn(MalLListFrom(malMap, new Keyword("c")))
                );
        }


    }
}