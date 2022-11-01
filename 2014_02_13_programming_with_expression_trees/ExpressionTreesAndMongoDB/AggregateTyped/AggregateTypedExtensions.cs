using MongoDB.Bson;
using MongoDB.Driver;

namespace ExpressionTreesAndMongoDB;

public static class AggregateTypedExtensions
{
    public static BsonDocument[] AggregateTypedAsBson<T, TResult>(this IMongoCollection<T> collection,
        Func<AggQuery<T>, AggQuery<TResult>> queryBuilder)
    {
        var query = new AggQuery<T>(collection);
        queryBuilder(query);
        return collection.Aggregate<BsonDocument>(query.PipelineOperations).ToList().ToArray();
    }

    public static TResult[] AggregateTyped<T, TResult>(this IMongoCollection<T> collection,
        Func<AggQuery<T>, AggQuery<TResult>> queryBuilder)
    {
        var aggregationResult = collection.AggregateTypedAsBson(queryBuilder);

        // Converter<BsonDocument, TResult> mapper = AnonymousTypeDeserializer.IsAnonymousType(typeof(TResult)) ?
        //     doc => (TResult)AnonymousTypeDeserializer.Deserialize(doc, typeof(TResult)) :
        //     doc => BsonSerializer.Deserialize<TResult>(doc);

        // in reality the anonymous types as return type is supported, because the exception is thrown analyzing expression trees
        Converter<BsonDocument, TResult> mapper = doc => (TResult)AnonymousTypeDeserializer.Deserialize(doc, typeof(TResult));

        return Array.ConvertAll(aggregationResult, mapper);
    }

    public static AggQuery<T> Log<T>(this AggQuery<T> query, Action<string> logger = null)
    {
        logger = logger ?? Console.WriteLine;
        foreach (var doc in query.PipelineOperations)
        {
            logger(doc.ToJson());
        }
        return query;
    }
}
