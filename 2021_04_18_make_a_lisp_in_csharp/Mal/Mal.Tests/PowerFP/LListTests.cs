using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerFP;

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


            // Assert.IsTrue(list.ToEnumerableRec().SequenceEqual(new[] { 11, 22, 33 }));
            // Assert.IsFalse(LListM.ToEnumerable<string>(null).Any());
            // Assert.IsFalse(LListM.ToEnumerableRec<string>(null).Any());
        }


    }
}

