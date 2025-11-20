
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExpressionTreesAndMongoDB;

/// https://www.mongodb.com/docs/manual/tutorial/aggregation-zip-code-data-set/
/// https://www.mongodb.com/docs/v4.4/tutorial/aggregation-zip-code-data-set/ - legacy documentation 

public class ZipCode
{
    public string Id { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public int Pop { get; set; }
    public double[] Loc { get; set; }
}

class SamplesZipCode
{
    public static void UseBsonApi()
    {
        var database = MongoUtils.MongoClient.GetDatabase("zipcodes");
        var collection = database.GetCollection<BsonDocument>("zipcodes");

        BsonDocument[] pipeline =
        {
            new BsonDocument
            {
                { "$group", new BsonDocument
                    {
                        { "_id", new BsonDocument { { "state", "$state"}, { "city", "$city"} } },
                        { "pop", new BsonDocument { { "$sum", "$pop"} } }
                    }
                }
            },
            new BsonDocument
            {
                { "$sort", new BsonDocument { { "pop", 1} } }
            },
            new BsonDocument
            {
                { "$group", new BsonDocument
                    {
                        { "_id", "$_id.state" },
                        { "biggestCity", new BsonDocument { { "$last" , "$_id.city"} } },
                        { "biggestPop", new BsonDocument { { "$last" , "$pop"} } },
                        { "smallestCity", new BsonDocument { { "$first" , "$_id.city"} } },
                        { "smallestPop", new BsonDocument { { "$first" , "$pop"} } },
                    }
                }
            },
            new BsonDocument
            {
                { "$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "state", "$_id"},
                        { "biggestCity", new BsonDocument { { "name" , "$biggestCity"},{ "pop" , "$biggestPop"}, } },
                        { "smallestCity", new BsonDocument { { "name" , "$smallestCity"},{ "pop" , "$smallestPop"}, } },
                    }
                }
            },
        };

        collection.Aggregate<BsonDocument>(pipeline).ToList().ForEach(Console.WriteLine);
    }

    public static void UseFluentApi()
    {
        var database = MongoUtils.MongoClient.GetDatabase("zipcodes");
        var collection = database.GetCollection<ZipCode>("zipcodes"); // ZipCode instead of BsonDocument

        collection.Aggregate()
            .Group(
                x => new { x.City, x.State },
                gr => new
                {
                    gr.Key,
                    Pop = gr.Sum(item => item.Pop)
                })
            .SortBy(x => x.Pop)
            .Group(
                x => x.Key.State,
                gr => new
                {
                    gr.Key,
                    BiggestCity = gr.Select(item => item.Key.City).Last(),
                    BiggestPop = gr.Select(item => item.Pop).Last(),
                    SmallestCity = gr.Select(item => item.Key.City).First(),
                    SmallestPop = gr.Select(item => item.Pop).First()
                })
            .Project(x => new
            {
                State = x.Key,
                BiggestCity = new { Name = x.BiggestCity, Pop = x.BiggestPop },
                SmallestCity = new { Name = x.SmallestCity, Pop = x.SmallestPop },
            })
            .ToList().ForEach(Console.WriteLine);
    }

    public static void UseAggregateTyped()
    {
        var collection = MongoUtils.MongoClient.GetDatabase("zipcodes").GetCollection<ZipCode>("zipcodes");

        var result = collection.AggregateTyped(q => q
            .Group((x, o) => new
            {
                _id = new { State = x.State, City = x.City },
                pop = o.Sum(x.Pop)
            })
            .Sort(x => x.pop)
            .Group((x, o) => new
            {
                _id = x._id.State,
                biggestCity = o.Last(x._id.City),
                biggestPop = o.Last(x.pop),
                smallestCity = o.First(x._id.City),
                smallestPop = o.First(x.pop),
            })
            .Project(x => new
            {
                state = x._id,
                biggestCity = new { name = x.biggestCity, pop = x.biggestPop },
                smallestCity = new { name = x.smallestCity, pop = x.smallestPop },
            })
            // .Log()
            );

        result.ToList().ForEach(Console.WriteLine);
    }
}

