
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ExpressionTreesAndMongoDB.Tests;

[TestClass]
public class IntegrationTests
{
    private const string DefaultDatabaseName = Samples.DefaultDbName;
    private const string DefaultCollectionName = "items";

    [TestMethod]
    public void LimitTest()
    {
        AddData(new Item[] { new() { Id = 1, Name = "a" }, new() { Id = 2, Name = "b" }, new() { Id = 3, Name = "c" }, });

        var resultDocuments = CreateCollection<Item>().AggregateTypedAsBson(q => q
            .Limit(2)
            .Log()
            );

        Print(resultDocuments);
        Assert.AreEqual(resultDocuments.Length, 2);
    }

    [TestMethod]
    public void SkipTest()
    {
        AddData(new Item[] { new() { Id = 1, Name = "a" }, new() { Id = 2, Name = "b" }, new() { Id = 3, Name = "c" }, });

        var resultDocuments = CreateCollection<Item>().AggregateTypedAsBson(q => q
            .Skip(2)
            .Log()
            );

        Print(resultDocuments);
        Assert.AreEqual(resultDocuments.Length, 1);
    }

    [TestMethod]
    public void OrderTest()
    {
        AddData(new Item[]
        {
            new () {Id = 1, Name = "a", Name2="a"},
            new () {Id = 2, Name = "b", Name2="b"},
            new () {Id = 3, Name = "c", Name2="b"},
            new () {Id = 4, Name = "c", Name2="c"},
            new () {Id = 5, Name = "c", Name2="a"},
        });

        var resultDocuments = CreateCollection<Item>().AggregateTypedAsBson(q => q
            .Sort(x => x.Name, false)
            .ThenBy(x => x.Name2)
            .Log()
            );

        Print(resultDocuments);
        Assert.AreEqual(resultDocuments.Length, 5);
        CollectionAssert.AreEqual(resultDocuments.Select(d => d["_id"].AsInt32).ToArray(), new[] { 5, 3, 4, 2, 1 });
    }

    [TestMethod]
    public void ProjectTest()
    {
        AddData(new[]
        {
            new Item() {Id = 1, Name = "a", Name2="A", Name3="_A", SubItem = new Item(){ Name = "aa"}},
            new Item() {Id = 2, Name = "b", Name2="B", Name3="_B", SubItem = new Item(){ Name = "bb"}},
        });

        var resultDocuments = CreateCollection<Item>().AggregateTypedAsBson(q => q
            .Project((x, o) => new
            {
                _id = x.Id,
                SubItemName = x.SubItem.Name,
                Concat = o.Concat(x.Name, " ", x.Name2),
                Sub = new { x.Name3 },
            })
            .Log()
            );

        Print(resultDocuments);
        var bsonDocument = resultDocuments[0];
        Assert.AreEqual(bsonDocument["SubItemName"].AsString, "aa");
        Assert.AreEqual(bsonDocument["Concat"].AsString, "a A");
        Assert.AreEqual(bsonDocument["Sub"]["Name3"].AsString, "_A");
    }

    [TestMethod]
    public void GroupTest()
    {
        AddData(new[]
        {
            new Item() {Id = 1, Name = "a"},
            new Item() {Id = 2, Name = "b"},
            new Item() {Id = 3, Name = "c"},
            new Item() {Id = 4, Name = "c"},
            new Item() {Id = 5, Name = "c"},
            new Item() {Id = 6, Name = "b"},
        });

        var resultDocuments = CreateCollection<Item>().AggregateTypedAsBson(q => q
            .Group((x, o) => new
            {
                _id = x.Name,
                Min = o.Min(x.Id),
                Max = o.Max(x.Id),
                Sum = o.Sum(x.Id),
                Push = o.Push(x.Id),
            })
            .Log()
            );

        Print(resultDocuments);
        var bsonDocument = resultDocuments.FirstOrDefault(d => d["_id"] == "c");
        Assert.AreEqual(bsonDocument["_id"].AsString, "c");
        Assert.AreEqual(bsonDocument["Min"].AsInt32, 3);
        Assert.AreEqual(bsonDocument["Max"].AsInt32, 5);
        Assert.AreEqual(bsonDocument["Sum"].AsInt32, 3 + 4 + 5);
        Assert.AreEqual(bsonDocument["Push"].ToJson(), "[3, 4, 5]");
    }

    [TestMethod]
    public void UnwindTest()
    {
        AddData(new[]
        {
            new Item() { Id = 1, Name = "a",SubItems = new[]
                {
                    new Item(){Name = "a1"}, new Item(){Name = "a2"}
                }
            },
            new Item() { Id = 2, Name = "b",SubItems = new[]
                {
                    new Item(){Name = "b1"}, new Item(){Name = "b2"},new Item(){Name = "b3"}
                }
            },
        });

        var resultDocuments = CreateCollection<Item>().AggregateTypedAsBson(q => q
            .Unwind(x => x.SubItems, (x, i) => new { x.Id, x.Name, SubItems = i })
            .Log()
            );


        Print(resultDocuments);
        Assert.AreEqual(resultDocuments.Length, 5);
    }

    [TestMethod]
    public void GroupThenProjectTest()
    {
        AddData(new[]
        {
                new Item() {Id = 1, Name = "a"},
                new Item() {Id = 2, Name = "b"},
                new Item() {Id = 3, Name = "c"},
                new Item() {Id = 4, Name = "c"},
                new Item() {Id = 5, Name = "c"},
                new Item() {Id = 6, Name = "b"},
            });

        var resultDocuments = CreateCollection<Item>().AggregateTypedAsBson(q => q
            .Group((x, o) => new
            {
                _id = x.Name,
                Count = o.Sum(1),
                Min = o.Min(x.Id),
            })
            .Project((x, o) => new
            {
                UpperId = o.ToUpper(x._id),
                Count_Plus_Min_Plus_100 = o.Add(x.Count, x.Min, 100)
            })
            .Log()
            );


        Print(resultDocuments);
        Assert.AreEqual(resultDocuments.Length, 3);
    }

    [TestMethod]
    public void GroupTypedAggregationTest()
    {
        AddData(new[]
        {
            new Item() {Id = 1, Name = "a"},
            new Item() {Id = 2, Name = "b"},
            new Item() {Id = 3, Name = "c"},
            new Item() {Id = 4, Name = "c"},
            new Item() {Id = 10, Name = "c"},
            new Item() {Id = 6, Name = "b"},
        });

        var result = CreateCollection<Item>().AggregateTyped(q => q
            .Group((x, o) => new
            {
                _id = x.Name,
                Count = o.Sum(1),
                Max = o.Max(x.Id),
                Avg = o.Avg<int, float>(x.Id),
            })
            .Log()
            );

        result.ToList().ForEach(Console.WriteLine);

        Assert.AreEqual(result.Length, 3);
        var c = result.Single(item => item._id == "c");
        Assert.AreEqual(c.Max, 10);
        Assert.AreEqual(c.Count, 3);
        Assert.AreEqual(c.Avg, ((float)(3 + 4 + 10) / 3));
    }

    [TestMethod]
    public void SimpleMatchUsingBsonDocumentTest()
    {
        AssertSimpleMatch(q => q.Match(new BsonDocument() { { "Name", "c" } }));
    }

    [TestMethod]
    public void SimpleMatchUsingLambdaTest()
    {
        AssertSimpleMatch(q => q.Match(x => x.Name == "c"));
    }

    private void AssertSimpleMatch(Func<AggQuery<Item>, AggQuery<Item>> matchBuilder)
    {
        AddData(new[]
        {
            new Item() {Id = 1, Name = "a"},
            new Item() {Id = 2, Name = "b"},
            new Item() {Id = 3, Name = "c"},
        });

        var result = CreateCollection<Item>().AggregateTyped(q =>
            matchBuilder(q)
            .Log()
            );

        result.Select(x => new { x.Id, x.Name }).ToList().ForEach(Console.WriteLine);

        Assert.AreEqual(result.Length, 1);
        var c = result.First();
        Assert.AreEqual(c.Name, "c");
        Assert.AreEqual(c.Id, 3);
    }

    [TestMethod]
    public void GroupThenMatchThenProjectTest()
    {
        AddData(new[]
        {
            new Item() {Id = 1, Name = "a"},
            new Item() {Id = 2, Name = "b"},
            new Item() {Id = 3, Name = "c"},
            new Item() {Id = 4, Name = "c"},
            new Item() {Id = 6, Name = "b"},
            new Item() {Id = 7, Name = "b"},
        });

        var result = CreateCollection<Item>().AggregateTyped(q => q
            .Group((x, o) => new
            {
                _id = x.Name,
                Count = o.Sum(1)
            })
            .Match(x => x.Count == 2 || x.Count == 3)
            .Project((x, o) => new { Ilosc = o.Multiply(x.Count, 10) })
            .Limit(2)
            .Log()
            );

        result.ToList().ForEach(Console.WriteLine);
    }


    [TestMethod]
    public void BsonMethodTest()
    {
        AddData(new[]
        {
            new Item() {Id = 1, Name = "a"},
            new Item() {Id = 2, Name = "b"},
            new Item() {Id = 3, Name = "c"},
        });

        var result = CreateCollection<Item>().AggregateTyped(q => q
            .Project((x, o) => new
            {
                Name = x.Name,
                Name_ = o.Bson<string>(new BsonString("$Name")),
                Sub = new { x.Name },
                Sub_ = o.Bson(new BsonDocument() { { "Name", "$Name" } }, new { Name = "" })
            })
            .Log()
            );

        result.ToList().ForEach(Console.WriteLine);
        var first = result.First();
        Assert.AreEqual(first.Name, first.Name_);
        Assert.AreEqual(first.Sub, first.Sub_);

    }

    [TestMethod]
    public void CondOperatorTest()
    {
        AddData(new[]
        {
            new Item() {Id = 1, Name = "a"},
            new Item() {Id = 2, Name = "b"},
            new Item() {Id = 3, Name = "c"},
        });

        var result = CreateCollection<Item>().AggregateTyped(q => q
            .Group((x, o) => new
            {
                _id = o.Cond(new BsonDocument()
                {
                        {"$eq", new BsonArray(new []{"$Name", "c"})}
                }, 5, 10),
                Count = o.Sum(1)
            })
            .Log()
            );

        result.ToList().ForEach(Console.WriteLine);
        Assert.AreEqual(result.Length, 2);
        Assert.AreEqual(result.Single(i => i._id == 5).Count, 1);
        Assert.AreEqual(result.Single(i => i._id == 10).Count, 2);
    }


    // [TestMethod]
    // public void GeoNearTest()
    // {
    //     var data = Enumerable.Range(1, 5).Select(i => new Location() { Id = i.ToString(), Loc = new double[] { i, i } });
    //     AddData(data, collectionName: "locs");

    //     var coll = CreateCollection<Location>(collectionName: "locs");

    //     IndexKeysDefinition<Location> keys = "{ Loc: \"2d\" }";
    //     var indexModel = new CreateIndexModel<Location>(keys);
    //     coll.Indexes.CreateOne(indexModel);

    //     // coll.CreateIndex(new IndexKeysDocument("Loc", "2d"));

    //     {
    //         // nietypowane rezulaty
    //         var result = coll.AggregateTypedAsBson(q => q
    //            .GeoNear(new double[] { 3, 3 },
    //                 "Result.Distance",
    //                 includeLocs: "Result.Loc",
    //                 query: x => x.Id != "2" && x.Id != "5"
    //             )
    //            .Log()
    //            );

    //         Print(result);
    //         Assert.IsTrue(result.Select(doc => doc["_id"].AsString).SequenceEqual(new[] { "3", "4", "1" }));
    //     }


    //     {
    //         // typowane z project
    //         var result = coll.AggregateTyped(q => q
    //            .GeoNear(new double[] { 3, 3 },
    //                 "Result.Distance",
    //                 includeLocs: "Result.Loc",
    //                 query: x => x.Id != "2" && x.Id != "5"
    //             )
    //             .Project((x, o) => new
    //             {
    //                 DocId = x.Id,
    //                 DocLoc = x.Loc,
    //                 Result = new
    //                 {
    //                     Distance = o.Bson<double>("$Result.Distance"),
    //                     Loc = o.Bson<double[]>("$Result.Loc"),
    //                 }
    //             })
    //            .Log()
    //            );

    //         Print(result);
    //         Assert.IsTrue(result.Select(doc => doc.DocId).SequenceEqual(new[] { "3", "4", "1" }));
    //     }


    //     {
    //         //typowane z wlasnym typem
    //         var result = coll.AggregateTyped(q => q
    //               .GeoNear<LocationGeoNear>(new double[] { 3, 3 },
    //                    x => x.Result.Distance,
    //                    includeLocsSelector: x => x.Result.Loc,
    //                    query: x => x.Id != "2" && x.Id != "5"
    //                )
    //               .Log()
    //               );

    //         Print(result);
    //         Assert.IsTrue(result.Select(doc => doc.Id).SequenceEqual(new[] { "3", "4", "1" }));
    //     }
    // }


    private void Print<T>(IEnumerable<T> docs)
    {
        foreach (var doc in docs)
        {
            Print(doc);
        }
    }
    private void Print<T>(T bsonDocument)
    {
        Console.WriteLine(bsonDocument.ToJson());
    }


    private static IMongoCollection<T> CreateCollection<T>(
        string databaseName = Samples.DefaultDbName, string collectionName = DefaultCollectionName)
    {
        ExpressionTreesAndMongoDB.MongoUtils.InitMongo(camelCaseTypes: new[] { typeof(Location) }, enableLogging: false);

        var settings = new MongoClientSettings();
        var client = new MongoClient(settings);
        var database = client.GetDatabase(databaseName);
        var collection = database.GetCollection<T>(collectionName);
        return collection;
    }


    private void AddData<T>(IEnumerable<T> items, string databaseName = DefaultDatabaseName, string collectionName = DefaultCollectionName, bool dropBeforeAdding = true)
    {
        var collection = CreateCollection<T>(databaseName, collectionName);

        if (dropBeforeAdding)
        {
            collection.DeleteMany(new BsonDocument());
        }

        foreach (var item in items)
        {
            collection.InsertOne(item);
        }
    }
}


public class SomeClass
{
    public int Id { get; set; }
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Name2 { get; set; }

    [BsonElement("name3")]
    public string Name3 { get; set; }

    public Item SubItem { get; set; }
    public Item[] SubItems { get; set; }
}

public class Location
{
    public string Id { get; set; }
    public double[] Loc { get; set; }
}

public class LocationGeoNear : Location
{
    public class GeoNearResult
    {
        public double Distance { get; set; }
        public double[] Loc { get; set; }
    }

    public GeoNearResult Result { get; set; }
}