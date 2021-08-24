using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerFP;
using static PowerFP.LListM;

namespace Mal.Tests
{
    [TestClass]
    public class LListTests
    {
        [TestMethod]
        public void ToEnumerableTest()
        {
            LList<int> list = new(11, new(22, new(33, null)));

            Assert.IsTrue(list.ToEnumerable().SequenceEqual(new[] { 11, 22, 33 }));
            Assert.IsTrue(list.ToEnumerableRec().SequenceEqual(new[] { 11, 22, 33 }));
            Assert.IsFalse(LListM.ToEnumerable<string>(null).Any());
            Assert.IsFalse(LListM.ToEnumerableRec<string>(null).Any());
        }

        [TestMethod]
        public void ToLListTest()
        {
            LList<int> list = new(11, new(22, new(33, null)));

            Assert.AreEqual(new[] { 11, 22, 33 }.ToLList(), list);
            Assert.IsNull(new int[] { }.ToLList());

            Assert.AreEqual(LListM.LListFrom(11, 22, 33), list);
            Assert.IsNull(LListM.LListFrom<int>());
        }

        [TestMethod]
        public void CountTest()
        {
            Assert.AreEqual(3, new LList<int>(11, new(22, new(33, null))).Count());
            Assert.AreEqual(1, new LList<int>(11, null).Count());
            Assert.AreEqual(0, (null as LList<int>).Count());
        }

        [TestMethod]
        public void SelectTest()
        {
            Assert.AreEqual(new LList<int>(110, new(220, null)), new LList<int>(11, new(22, null)).Select(x => x * 10));
            Assert.AreEqual(new LList<string>("11", new("22", null)), new LList<int>(11, new(22, null)).Select(x => x.ToString()));
            Assert.AreEqual(null, (null as LList<int>).Select(x => x.ToString()));
        }

        [TestMethod]
        public void WhereTest()
        {
            Assert.AreEqual(new LList<int>(11, new(33, null)), new LList<int>(11, new(22, new(33, null))).Where(x => x % 2 != 0));
            Assert.AreEqual(null, new LList<int>(11, new(22, new(33, null))).Where(x => x > 100));
            Assert.AreEqual(null, (null as LList<int>).Where(x => true));
        }

        [TestMethod]
        public void AggregateTest()
        {
            Assert.AreEqual(6, new LList<int>(1, new(2, new(3, null))).Aggregate(0, (a, x) => a + x));
            Assert.AreEqual("123", new LList<int>(1, new(2, new(3, null))).Aggregate("", (a, x) => a + x));
            Assert.AreEqual(1, new LList<int>(1, null).Aggregate(0, (a, x) => a + x));
            Assert.AreEqual(0, (null as LList<int>).Aggregate(0, (a, x) => a + x));

            Assert.AreEqual(6, new LList<int>(1, new(2, new(3, null))).Aggregate((a, x) => a + x));
            Assert.AreEqual(3, new LList<int>(1, new(2, null)).Aggregate((a, x) => a + x));
            Assert.AreEqual(1, new LList<int>(1, null).Aggregate((a, x) => a + x));

            Assert.ThrowsException<Exception>(() => (null as LList<int>).Aggregate((a, x) => a + x));
        }

        [TestMethod]
        public void TakeTest()
        {
            var list123 = new LList<int>(1, new(2, new(3, null)));

            Assert.AreEqual(list123 with { }, list123.Take(5));

            Assert.AreEqual(list123 with { }, list123.Take(3));
            Assert.AreEqual(new LList<int>(1, new(2, null)), list123.Take(2));
            Assert.AreEqual(new LList<int>(1, null), list123.Take(1));
            Assert.AreEqual(null, list123.Take(0));

            Assert.AreEqual(null, (null as LList<int>).Take(1));
        }

        [TestMethod]
        public void SkipTest()
        {
            var list123 = new LList<int>(1, new(2, new(3, null)));

            Assert.AreEqual(null, list123.Skip(5));
            Assert.AreEqual(null, list123.Skip(3));
            Assert.AreEqual(new LList<int>(3, null), list123.Skip(2));
            Assert.AreEqual(new LList<int>(2, new(3, null)), list123.Skip(1));
            Assert.AreEqual(list123 with { }, list123.Skip(0));

            Assert.AreEqual(null, (null as LList<int>).Skip(1));
        }

        [TestMethod]
        public void ConcatTest()
        {
            var list123 = new LList<int>(1, new(2, new(3, null)));

            Assert.AreEqual(LListFrom(1, 2, 3, 2, 3), list123.Concat(new(2, new(3, null))));
            Assert.AreEqual(list123, list123.Concat(null));
            Assert.AreEqual(list123, (null as LList<int>).Concat(list123));
        }

        [TestMethod]
        public void ZipTest()
        {
            var list123 = new LList<int>(1, new(2, new(3, null)));
            var listAb = new LList<string>("a", new("b", null));

            Assert.AreEqual(LListFrom("a1", "b2"), list123.Zip(listAb, (l, r) => r + l));
            Assert.AreEqual(null, list123.Zip(null as LList<string>, (l, r) => r + l));
            Assert.AreEqual(null, (null as LList<int>).Zip(listAb, (l, r) => r + l));
        }

        [TestMethod]
        public void SelectManyTest()
        {
            var list = new LList<int>?[]
            {
                new(11, new(22, null)),
                null,
                new(111, new(222, null))
            }.ToLList();

            Assert.AreEqual(LListFrom(11, 22, 111, 222), list.SelectMany(x => x));
        }

        [TestMethod]
        public void SelectManyWithResultSelectorTest()
        {
            var list = new LList<int>?[]
            {
                new(11, new(22, null)),
                null,
                new(111, new(222, null))
            }.ToLList();

            Assert.AreEqual(LListFrom("11 11", "11 22", "111 111", "111 222"), list.SelectMany(x => x, (x, y) => x!.Head + " " + y));
        }

        [TestMethod]
        public void ReverseTests()
        {
            Assert.AreEqual(new LList<int>(22, new(11, null)), new LList<int>(11, new(22, null)).Reverse());
            Assert.AreEqual(new LList<int>(11, null), new LList<int>(11, null).Reverse());
            Assert.AreEqual(null, (null as LList<int>).Reverse());
        }

        [TestMethod]
        public void AllTest()
        {
            var list123 = new LList<int>(1, new(2, new(3, null)));

            Assert.AreEqual(true, list123.All(x => x > 0));
            Assert.AreEqual(false, list123.All(x => x < 3));
            Assert.AreEqual(true, (null as LList<int>).All(x => false));
        }

        [TestMethod]
        public void AnyTest()
        {
            var list123 = new LList<int>(1, new(2, new(3, null)));

            Assert.AreEqual(true, list123.Any(x => x > 2));
            Assert.AreEqual(false, list123.Any(x => x > 3));
            Assert.AreEqual(false, (null as LList<int>).Any(x => false));
        }


        [TestMethod]
        public void ElementAtTest()
        {
            Assert.ThrowsException<Exception>(() => LListFrom<int>().ElementAt(-1));
            Assert.ThrowsException<Exception>(() => LListFrom<int>().ElementAt(0));

            var list = LListFrom(11, 222, 444);
            Assert.ThrowsException<Exception>(() => list.ElementAt(3));

            Assert.AreEqual(11, list.ElementAt(0));
            Assert.AreEqual(222, list.ElementAt(1));
            Assert.AreEqual(444, list.ElementAt(2));
        }


        [TestMethod]
        public void SequenceEqualTest()
        {
            Func<int, int, bool> equals = (x, y) => x == y;
            var list123 = new LList<int>(1, new(2, new(3, null)));

            Assert.IsTrue(((LList<int>?)null).SequenceEqual(null, equals));
            Assert.IsTrue(list123.SequenceEqual(list123, equals));

            Assert.IsFalse(list123.SequenceEqual(null, equals));
            Assert.IsFalse(((LList<int>?)null).SequenceEqual(list123, equals));
            Assert.IsFalse(list123.SequenceEqual(new(0, list123), equals));
            Assert.IsFalse(list123.SequenceEqual(new LList<int>(1, new(2, new(3333, null))), equals));

            // Assert.AreEqual(false, list123.Any(x => x > 3));
            // Assert.AreEqual(false, (null as LList<int>).Any(x => false));
        }
    }
}

