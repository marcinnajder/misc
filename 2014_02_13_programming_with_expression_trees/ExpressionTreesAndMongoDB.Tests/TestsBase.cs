
// using MongoDB.Bson;
// using MongoDB.Bson.Serialization.Attributes;
// using MongoDB.Driver;

// namespace ExpressionTreesAndMongoDB.Tests;

// public class Item
// {
//     public int Id { get; set; }
//     public string Name { get; set; }
//     public string Name2 { get; set; }

//     [BsonElement("name3")]
//     public string Name3 { get; set; }

//     public Item SubItem { get; set; }
//     public Item[] SubItems { get; set; }
// }

// public class TestsBase
// {
//     protected void Print<T>(T bsonDocument)
//     {
//         Console.WriteLine(bsonDocument.ToJson());
//     }

//     protected void Print<T>(IEnumerable<T> docs)
//     {
//         foreach (var doc in docs)
//         {
//             Print(doc);
//         }
//     }

//     protected static IMongoCollection<T> CreateCollection<T>(string databaseName = DefaultDatabaseName, string collectionName = DefaultCollectionName)
//     {
//         var settings = new MongoClientSettings();
//         var client = new MongoClient(settings);
//         var database = client.GetDatabase(databaseName);
//         var collection = database.GetCollection<T>(collectionName);
//         return collection;
//     }


//     protected void AddData<T>(IEnumerable<T> items, bool dropBeforeAdding = true, string databaseName = DefaultDatabaseName, string collectionName = DefaultCollectionName)
//     {
//         if (dropBeforeAdding)
//         {
//             DropData(databaseName, collectionName);
//         }

//         var collection = CreateCollection<T>(databaseName, collectionName);
//         foreach (var item in items)
//         {
//             collection.InsertOne(item);
//         }
//     }


//     protected void DropData(string databaseName = DefaultDatabaseName, string collectionName = DefaultCollectionName)
//     {
//         var collection = CreateCollection<Item>(databaseName, collectionName);
//         collection.DeleteMany(new BsonDocument());
//     }


//     private const string DefaultDatabaseName = "taf";
//     private const string DefaultCollectionName = "items";
// }
