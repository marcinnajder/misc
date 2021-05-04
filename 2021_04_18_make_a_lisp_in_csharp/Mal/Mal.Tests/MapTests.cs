
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerFP;
using static Mal.Types;
using static PowerFP.LListM;
using static PowerFP.MapM;


namespace Mal.Tests
{
    [TestClass]
    public class MapTests
    {
        [TestMethod]
        public void AddTests()
        {
            LList<(int, string)>? map = null;

            Assert.AreEqual(LListFrom((5, "5!")), map = map.Add(5, "5!"));
            Assert.AreEqual(LListFrom((5, "5!"), (10, "10!")), map = map.Add(10, "10!"));
            Assert.AreEqual(LListFrom((3, "3!"), (5, "5!"), (10, "10!")), map = map.Add(3, "3!"));
            Assert.AreEqual(LListFrom((3, "3!"), (5, "5!"), (7, "7!"), (10, "10!")), map = map.Add(7, "7!"));

            Assert.AreEqual(LListFrom((3, "3!"), (5, "5!"), (7, "7!!"), (10, "10!")), map = map.Add(7, "7!!"));

            Assert.ThrowsException<Exception>(() => new LList<(string?, string)>(("", ""), null).Add(null, ""));

            Assert.AreEqual(LListFrom((3, "3!"), (5, "5!"), (7, "7!"), (10, "10!")), map = map.Add(7, "7!"));
        }

        [TestMethod]
        public void MapFromTests()
        {
            var map1 = MapFrom(("name", "marcin"), ("surname", "najder"));
            var map2 = MapFrom(("surname", "najder"), ("name", "marcin"));
            Assert.AreNotSame(map1, map2);
            Assert.AreEqual(map1, map2);
            Assert.IsNull(MapM.MapFrom<string, string>());
        }

        [TestMethod]
        public void TryFindTests()
        {
            LList<(int, int)>? map = null;
            Assert.AreEqual((false, 0), map.TryFind(1));

            map = MapFrom((5, 50), (7, 70));
            Assert.AreEqual((false, 0), map.TryFind(1));
            Assert.AreEqual((true, 50), map.TryFind(5));
            Assert.AreEqual((true, 70), map.TryFind(7));
            Assert.AreEqual((false, 0), map.TryFind(10));

            var person = MapFrom(("name", "marcin"), ("surname", "najder"));
            Assert.AreEqual((true, "marcin"), person.TryFind("name"));
            Assert.AreEqual((false, null), person.TryFind("age"));
        }

        [TestMethod]
        public void FindTests()
        {
            var person = MapFrom(("name", "marcin"), ("surname", "najder"));
            Assert.AreEqual("marcin", person.Find("name"));
            Assert.ThrowsException<Exception>(() => person.Find("age"));
        }

        [TestMethod]
        public void ContainsKeyTests()
        {
            var person = MapFrom(("name", "marcin"), ("surname", "najder"));
            Assert.AreEqual(true, person.ContainsKey("name"));
            Assert.AreEqual(false, person.ContainsKey("age"));
        }

        [TestMethod]
        public void RemoveTests()
        {
            LList<(int, int)>? map = null;
            Assert.AreEqual(null, map.Remove(1));

            map = MapFrom((5, 50), (7, 70));
            Assert.AreSame(map, map.Remove(1));
            var map1 = map.Remove(10);
            Assert.AreNotSame(map, map1);
            Assert.AreEqual(map, map1);

            Assert.AreEqual(MapFrom((7, 70)), map = map.Remove(5));
            Assert.AreEqual(null, map = map.Remove(7));
        }

        [TestMethod]
        public void ChangeTests()
        {
            LList<(int, int)>? map = null;
            Assert.AreEqual(null, map.Change(1, _ => (false, 0)));
            Assert.AreEqual(MapFrom((1, 10)), map.Change(1, _ => (true, 10)));

            map = MapFrom((5, 50), (7, 70));

            // key not found -> adds item
            Assert.AreEqual(map.Add(1, 100), map.Change(1, _ => (true, 100)));
            Assert.AreEqual(map.Add(1000, 100), map.Change(1000, _ => (true, 100)));

            // key not found -> leaves items unchanged 
            Assert.AreEqual(map, map.Change(1, r => (r.Item1, 100)));
            Assert.AreEqual(map, map.Change(1000, r => (r.Item1, 100)));

            // key found -> changes item
            Assert.AreEqual(MapFrom((5, 500), (7, 70)), map.Change(5, r => (r.Item1, r.Item2 * 10)));
            Assert.AreEqual(MapFrom((5, 50), (7, 700)), map.Change(7, r => (r.Item1, r.Item2 * 10)));

            // key found -> removes item
            Assert.AreEqual(map.Remove(5), map.Change(5, r => (!r.Item1, r.Item2 * 10)));
            Assert.AreEqual(map.Remove(7), map.Change(7, r => (!r.Item1, r.Item2 * 10)));
        }
    }
}