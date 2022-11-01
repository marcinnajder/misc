using MongoDB.Bson;

namespace ExpressionTreesAndMongoDB.Tests;

[TestClass]
public class DeserializationTests
{
    [TestMethod]
    public void DefaultValuesTests()
    {
        Assert.AreEqual(AnonymousTypeDeserializer.GetDefaultValue(typeof(int)), 0);
        Assert.AreEqual(AnonymousTypeDeserializer.GetDefaultValue(typeof(int?)), null);
        Assert.AreEqual(AnonymousTypeDeserializer.GetDefaultValue(typeof(bool)), false);
        Assert.AreEqual(AnonymousTypeDeserializer.GetDefaultValue(typeof(string)), null);
        Assert.AreEqual(AnonymousTypeDeserializer.GetDefaultValue(typeof(object)), null);
    }



    [TestMethod]
    public void NonanonymousTypeTest()
    {
        object o = new Item() { Id = 1, Name2 = "name2", Name3 = "name3" };
        AssertSerialization(o);
    }

    [TestMethod]
    public void SimpleAnonymous()
    {
        object o = new { I = 1, S = "", B = false };
        AssertSerialization(o);
    }

    [TestMethod]
    public void AnonymousWithAnonymousSubitemTest()
    {
        object o = new { I = 1, Sub = new { B = false } };
        AssertSerialization(o);
    }

    [TestMethod]
    public void AnonymousWithNonanonymousSubitemTest()
    {
        object o = new { I = 1, Sub = new Item { Id = 1 } };
        AssertSerialization(o);
    }

    [TestMethod]
    public void AnonymousWithNonanonymousSubitemWithNonanonymousSubitemTest()
    {
        object o = new Item() { Id = 1, Name2 = "name2", Name3 = "name3", SubItem = new Item() { Id = 123 } };
        AssertSerialization(o);
    }



    [TestMethod]
    public void AnonymousWithArrayOfPrimivesTest()
    {
        object o = new { I = 1, Array = new[] { 1, 2, 3 } };
        AssertSerialization(o);
    }


    [TestMethod]
    public void AnonymousWithArrayOfAnonymousTest()
    {
        object o = new { I = 1, Array = new[] { new { B = false }, new { B = true } } };
        AssertSerialization(o);
    }

    [TestMethod]
    public void AnonymousWithArrayOfNonanonymousTest()
    {
        object o = new { I = 1, Array = new[] { new Item { Id = 1 }, new Item { Id = 2 }, } };
        AssertSerialization(o);
    }


    [TestMethod]
    public void AnonymousWithListOfPrimivesTest()
    {
        object o = new { I = 1, List = new List<int> { 1, 2, 3 } };
        AssertSerialization(o);
    }


    [TestMethod]
    public void AnonymousAndDifferentBsonFieldTypeTest()
    {
        object o = new { I = 1 };
        AssertSerialization(o, new { I = (byte)1 }.GetType());
    }

    [TestMethod]
    public void AnonymousAndMissingBsonFieldTest()
    {
        var o = new { I = 1, S = "" };
        var oBsonDocument = o.ToBsonDocument();
        oBsonDocument.Remove("S");

        object o2 = AnonymousTypeDeserializer.Deserialize(oBsonDocument, o.GetType());
        dynamic d = o2;
        Assert.AreEqual(d.I, o.I);
        Assert.AreEqual(d.S, null);
    }


    private static void AssertSerialization(object o, Type descType = null)
    {
        // poniewaz test sprawdzy czy wyjsciowe JSONy sa takie same to trzeba zawsze wolac metode 
        // 'string ToJson<TNominalType>(this TNominalType obj)' na tych samych typach, bo inny TNominalType 
        // sprawia ze troche inny json sie generuje, gdy object to dodaje sie info o type  '_t="Item"'
        // dlatego tutaj obiekt oryginaly jest object, zdeserializowny takze jest obiekt

        var oBsonDocument = o.ToBsonDocument();
        var oJson = o.ToJson();

        descType = descType ?? o.GetType();
        object o2 = AnonymousTypeDeserializer.Deserialize(oBsonDocument, descType);
        var o2Json = o2.ToJson();

        Assert.AreEqual(oJson, o2Json);
        Console.WriteLine(o2Json);
    }
}
