using MongoDB.Bson;

namespace ExpressionTreesAndMongoDB.Tests;

[TestClass]
public class MappingTests
{
    [TestMethod]
    public void LimitTest()
    {
        BsonDocument bsonDocument = null;

        bsonDocument = new AggQuery<Item>().Limit(1).PipelineOperations[0];
        Assert.AreEqual(bsonDocument["$limit"].ToString(), "1");

        Console.WriteLine(bsonDocument);
    }

    [TestMethod]
    public void SkipTest()
    {
        BsonDocument bsonDocument = null;

        bsonDocument = new AggQuery<Item>().Skip(1).PipelineOperations[0];
        Assert.AreEqual(bsonDocument["$skip"].ToString(), "1");

        Console.WriteLine(bsonDocument);
    }

    [TestMethod]
    public void SortByTest()
    {
        BsonDocument bsonDocument = null;

        bsonDocument = new AggQuery<Item>().Sort(x => x.Name).PipelineOperations[0];
        Assert.AreEqual(bsonDocument["$sort"]["Name"].ToString(), "1");

        bsonDocument = new AggQuery<Item>().Sort(x => x.Name, false).PipelineOperations[0];
        Assert.AreEqual(bsonDocument["$sort"]["Name"].ToString(), "-1");

        bsonDocument = new AggQuery<Item>().Sort(x => x.SubItem.Name).PipelineOperations[0];
        Assert.AreEqual(bsonDocument["$sort"]["SubItem.Name"].ToString(), "1");

        bsonDocument = new AggQuery<Item>().Sort(x => x.Id).ThenBy(x => x.Name, false).PipelineOperations[0];
        Assert.AreEqual(bsonDocument["$sort"]["_id"].ToString(), "1");
        Assert.AreEqual(bsonDocument["$sort"]["Name"].ToString(), "-1");

        Console.WriteLine(bsonDocument);
    }

    [TestMethod]
    public void ProjectTest()
    {
        BsonDocument bsonDocument = null;

        // stale
        bsonDocument = new AggQuery<Item>().Project(x => new { I = 1, S = "s", B = false }).PipelineOperations[0];
        Assert.AreEqual((int)bsonDocument["$project"]["I"], 1);
        Assert.AreEqual((string)bsonDocument["$project"]["S"], "s");
        Assert.AreEqual((bool)bsonDocument["$project"]["B"], false);

        // podelementy
        bsonDocument = new AggQuery<Item>().Project(x => new { I = 1, Sub = new { S = "s", B = false } }).PipelineOperations[0];
        Assert.AreEqual((int)bsonDocument["$project"]["I"], 1);
        Assert.AreEqual((string)bsonDocument["$project"]["Sub"]["S"], "s");
        Assert.AreEqual((bool)bsonDocument["$project"]["Sub"]["B"], false);

        // sciezki
        bsonDocument = new AggQuery<Item>().Project(x => new { I = x.Id, x.Name, Id2 = x.SubItem.Id }).PipelineOperations[0];
        Assert.AreEqual((string)bsonDocument["$project"]["I"], "$_id");
        Assert.AreEqual((string)bsonDocument["$project"]["Name"], "$Name");
        Assert.AreEqual((string)bsonDocument["$project"]["Id2"], "$SubItem._id");

        // operatory
        bsonDocument = new AggQuery<Item>().Project((x, o) => new
        {
            Div = o.Divide(10, x.Id),
            Add = o.Add(0, x.Id, 1, x.SubItem.Id),
            Upper = o.ToUpper(x.Name),
            Concat = o.Concat(o.IfNull(x.Name, "null"), " ", o.IfNull(x.SubItem.Name, "NULL"))
        }).PipelineOperations[0];

        Assert.AreEqual(bsonDocument["$project"]["Div"]["$divide"].ToJson(), "[10, \"$_id\"]");
        Assert.AreEqual(bsonDocument["$project"]["Add"]["$add"].ToJson(), "[0, \"$_id\", 1, \"$SubItem._id\"]");
        Assert.AreEqual(bsonDocument["$project"]["Upper"]["$toUpper"].ToJson(), "\"$Name\"");
        var concatArray = (BsonArray)bsonDocument["$project"]["Concat"]["$concat"];
        Assert.AreEqual(concatArray[0]["$ifNull"].ToJson(), "[\"$Name\", \"null\"]");
        Assert.AreEqual(concatArray[1].ToJson(), "\" \"");
        Assert.AreEqual(concatArray[2]["$ifNull"].ToJson(), "[\"$SubItem.Name\", \"NULL\"]");

        Console.WriteLine(bsonDocument);
    }

    [TestMethod]
    public void GroupTest()
    {
        BsonDocument bsonDocument = null;
        bsonDocument = new AggQuery<Item>().Group((x, o) => new
        {
            _id_0 = (object)null,
            _id_1 = x.Id,
            _id_2 = new { x.Id, x.Name, Name2 = o.IfNull(x.SubItem.Name, "brak") },
            Sum = o.Sum(x.Id),
            Sum1 = o.Sum(1),
            Push = o.Push(x.Name),
        }).PipelineOperations[0];

        // nie  ma testow bo to spodem to samo co Project
        Console.WriteLine(bsonDocument);
    }

    [TestMethod]
    public void Unwind()
    {
        BsonDocument bsonDocument = null;
        bsonDocument = new AggQuery<Item>().Unwind(x => x.SubItems, (x, i) => new { x.Id, x.Name, SubItems = i }).PipelineOperations[0];
        // nie  ma testow bo to spodem to samo co Project
        Console.WriteLine(bsonDocument);
    }



    public class ItemGeoNear : Item
    {
        public GeoNear A { get; set; }

        public class GeoNear
        {
            public string Distance { get; set; }
            public int Locations { get; set; }
        }
    }

    [TestMethod]
    public void GeoNear()
    {
        BsonDocument bsonDocument = null;

        bsonDocument = new AggQuery<Item>().GeoNear(new double[] { 1, 2 }, "A.Distance",
            includeLocs: "A.Locations",
            limit: 1
            ).PipelineOperations[0];

        Assert.IsNotNull(bsonDocument["$geoNear"]);
        Assert.AreEqual((bsonDocument["$geoNear"] as BsonDocument).ElementCount, 4);
        Assert.AreEqual(bsonDocument["$geoNear"]["near"].ToJson(), "[1.0, 2.0]");
        Assert.AreEqual((int)bsonDocument["$geoNear"]["limit"], 1);
        Assert.AreEqual((string)bsonDocument["$geoNear"]["distanceField"], "A.Distance");
        Assert.AreEqual((string)bsonDocument["$geoNear"]["includeLocs"], "A.Locations");

        bsonDocument = new AggQuery<Item>().GeoNear<ItemGeoNear>(new double[] { 1, 2 }, x => x.A.Distance,
            includeLocsSelector: x => x.A.Locations
            ).PipelineOperations[0];

        Assert.IsNotNull(bsonDocument["$geoNear"]);
        Assert.AreEqual((bsonDocument["$geoNear"] as BsonDocument).ElementCount, 3);
        Assert.AreEqual(bsonDocument["$geoNear"]["near"].ToJson(), "[1.0, 2.0]");
        Assert.AreEqual((string)bsonDocument["$geoNear"]["distanceField"], "A.Distance");
        Assert.AreEqual((string)bsonDocument["$geoNear"]["includeLocs"], "A.Locations");
    }
}
