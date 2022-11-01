using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace ExpressionTreesAndMongoDB;

public class Samples
{
    public const string DefaultDbName = "AggregateTyped";

    public static void BasicOperators()
    {
        var collection = MongoUtils.MongoClient.GetDatabase(DefaultDbName).GetCollection<Patient>("patients");

        var count = collection.CountDocuments(new BsonDocument());
        if (count == 0)
        {
            var patients = new[]
            {
                new Patient (1, new PatientBasic("Michael","Zyz")),
                new Patient (2,new PatientBasic("Michael","Klu")),
                new Patient (3, new PatientBasic("Marcin", "Najder" )),
            };
            collection.InsertMany(patients);
        }

        var result = collection.AggregateTyped(q => q
                .Project((x, o) => new
                {
                    x.Id,
                    x.Basic,
                    Terms = o.Concat(x.Basic.FirstName, " ", x.Basic.LastName)
                })
                // .Match(x => x.Terms.Contains("Mich") && x.Terms.Contains("Klu"))
                // .Match(x => x.Terms.Contains("Mich"))
                // .Match(x => x.Terms.Contains("Klu"))
                .Project(x => new { x.Id, x.Basic })
                // .Sort(x => x.Basic.LastName).ThenBy(x => x.Basic.FirstName)
                .Project(x => new { x.Id, x.Basic.LastName, x.Basic.FirstName, })
                .Log()
            );


        foreach (var patient in result)
        {
            Console.WriteLine(patient);
        }
    }



    public static void AllZipCodeExamples()
    {
        Console.WriteLine("https://www.mongodb.com/docs/manual/tutorial/aggregation-zip-code-data-set");

        var collection = MongoUtils.MongoClient.GetDatabase("zipcodes").GetCollection<ZipCode>("zipcodes");

        {
            Console.WriteLine("Return States with Populations above 10 Million");

            var rows = collection.AggregateTyped(q => q
              .Group((x, o) => new { _id = x.State, totalPop = o.Sum(x.Pop) })
              .Match(x => x.totalPop > 10 * 1000 * 1000)
              .Log()
              );

            PrintRows(rows, rowsCount: 1);
        }


        {
            Console.WriteLine("Return Average City Population by State");

            var rows = collection.AggregateTyped(q => q
             .Group((x, o) => new { _id = new { x.State, x.City }, pop = o.Sum(x.Pop) })
             .Group((x, o) => new { _id = x._id.State, avgCityPop = o.Avg(x.pop) })
             .Log()
             );

            PrintRows(rows, rowsCount: 1);
        }

        {
            Console.WriteLine("Return Largest and Smallest Cities by State");

            var rows = collection.AggregateTyped(q => q
                .Group((x, o) => new
                {
                    _id = new { x.State, x.City },
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
                .Log()
                );

            PrintRows(rows, rowsCount: 1);
        }
    }

    public static void PreferenceExamples()
    {
        Console.WriteLine("https://www.mongodb.com/docs/manual/tutorial/aggregation-with-user-preference-data/");

        var collection = MongoUtils.MongoClient.GetDatabase("likes").GetCollection<Like>("likes");

        var count = collection.CountDocuments(new BsonDocument());
        if (count == 0)
        {
            var data = new[]
            {
                new Like("jane", new DateTime(2011,03,02),new[]{"golf","racquetball"}),
                new Like("joe", new DateTime(2012,07,02),new[]{"tennis", "golf", "swimming"}),
                new Like("joe1", new DateTime(2012,07,02),new[]{"tennis", "golf", "swimming"}),
            };
            collection.InsertMany(data);
        }

        {
            Console.WriteLine("Normalize and Sort Documents");

            var rows = collection.AggregateTyped(q => q
                .Project((x, o) => new { name = o.ToUpper(x.Id) })
                .Sort(x => x.name)
                .Log()
                );

            PrintRows(rows);
        }

        {
            Console.WriteLine("Return Usernames Ordered by Join Month");

            var rows = collection.AggregateTyped(q => q
                .Project((x, o) => new { month_joined = o.Month(x.Joined), name = x.Id })
                .Sort(x => x.month_joined)
                .Log()
                );

            PrintRows(rows);
        }

        {
            Console.WriteLine("Return Total Number of Joins per Month");

            var rows = collection.AggregateTyped(q => q
                .Project((x, o) => new { month_joined = o.Month(x.Joined), })
                .Group((x, o) => new { _id = new { x.month_joined }, number = o.Sum(1) })
                .Sort(x => x._id.month_joined)
                .Log()
                );

            PrintRows(rows);
        }


        {
            Console.WriteLine("Return the Five Most Common Likes");

            var rows = collection.AggregateTyped(q => q
                .Unwind(x => x.Likes, (x, i) => new { x.Id, Likes = i })
                .Group((x, o) => new { _id = x.Likes, number = o.Sum(1) })
                .Sort(x => x.number, false)
                .Limit(5)
                .Log()
                );

            PrintRows(rows);
        }
    }

    private static void PrintRows<T>(T[] rows, int? rowsCount = null)
    {
        Console.WriteLine("docs: " + rows.Length);
        if (rowsCount == 1)
        {
            Console.WriteLine("first row: " + rows[0]);
        }
        else
        {
            Console.WriteLine("rows: ");
            foreach (var row in rows)
            {
                Console.WriteLine(row);
            }
        }
    }
}

public record Patient(int Id, PatientBasic Basic);

public record PatientBasic(string FirstName, string LastName);

public record Like(string Id, DateTime Joined, string[] Likes);

