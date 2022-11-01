using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace ExpressionTreesAndMongoDB;

public static class MongoUtils
{
    private static MongoClient _mongoClient;

    public static MongoClient MongoClient =>
        _mongoClient ?? throw new Exception($"MongoClient is not initialized, method '{nameof(InitMongo)}' was not executed");

    public static void InitMongo(Type[] camelCaseTypes = null, bool enableLogging = false)
    {
        // remarks:
        // - this method must be called before any access to mongodb
        // - `AggregateTyped` is using naming conventions mechanism, not all types shoud be "camel cased",
        // for instance anonymous types property names should be left as there are        
        ConfigureConventions(camelCaseTypes);
        _mongoClient = CreateMongoClient(enableLogging);
    }

    private static void ConfigureConventions(Type[] camelCaseTypes)
    {
        if (camelCaseTypes != null)
        {
            var set = new HashSet<Type>(camelCaseTypes);
            var camelCaseConvention = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, set.Contains);
        }
    }

    private static MongoClient CreateMongoClient(bool enableLogging)
    {
        var mongoSettings = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");

        if (enableLogging)
        {
            mongoSettings.ClusterConfigurator = cb =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    Console.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };
        }

        var client = new MongoClient(mongoSettings);
        return client;
    }
}


// https://www.mongodb.com/docs/manual/tutorial/aggregation-zip-code-data-set/
// https://www.mongodb.com/developer/languages/csharp/joining-collections-mongodb-dotnet-core-aggregation-pipeline/
// https://stackoverflow.com/questions/61599898/mongodb-c-sharp-fluent-aggregate

// https://mongodb.github.io/mongo-csharp-driver/2.18/apidocs/html/M_MongoDB_Driver_IAggregateFluent_1_Group__1.htm

// https://github.com/mongodb/mongo-csharp-driver/releases/tag/v2.0.0
// https://www.nuget.org/packages/mongocsharpdriver#versions-body-tab
// 2.0 02 Apr 2015
// 2.5 12 Dec 2017
// 2.10 10 Dec 2019
// 2.15 09 Mar 2022
// 2.17 18 Jul 2022

// https://github.com/mongodb/mongo-csharp-driver/commit/bb5910c6bc5db1f170003d9f9b11974c1d85d51a
// - 13 Jan 2015

// https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Driver/IAggregateFluentExtensions.cs
// https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Driver/PipelineStageDefinitionBuilder.cs

// https://github.com/mongodb/mongo-csharp-driver/commits/master?after=75df889021601f4c171c3b84e9556b880ec0a5da+34&branch=master&path%5B%5D=src&path%5B%5D=MongoDB.Driver&path%5B%5D=IAggregateFluentExtensions.cs&qualified_name=refs%2Fheads%2Fmaster


// https://github.com/mongodb/mongo-csharp-driver
// https://mongodb.github.io/mongo-csharp-driver/2.18/
// https://www.mongodb.com/developer/languages/csharp/joining-collections-mongodb-dotnet-core-aggregation-pipeline/

// https://www.mongodb.com/docs/manual/tutorial/aggregation-zip-code-data-set/

// https://www.mongodb.com/blog/post/introducing-20-net-driver
