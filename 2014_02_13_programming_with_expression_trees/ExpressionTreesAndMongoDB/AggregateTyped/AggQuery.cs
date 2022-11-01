using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ExpressionTreesAndMongoDB;

public class AggQuery<T>
{
    private static MethodInfo[] _bsonMethodInfos = typeof(CommonOperators)
        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(m => m.Name == "Bson")
        .ToArray();

    protected List<BsonDocument> _parts = new List<BsonDocument>();
    protected IMongoCollection<T> _mongoCollection;

    public BsonDocument[] PipelineOperations
    {
        get { return _parts.ToArray(); }
    }

    public AggQuery(IMongoCollection<T> mongoCollection = null)
    {
        _mongoCollection = mongoCollection;
    }


    #region Operators

    // http://docs.mongodb.org/manual/reference/operator/aggregation/geoNear/#pipe._S_geoNear
    // http://stackoverflow.com/questions/21650385/mongodb-query-near-with-aggregation


    /// <summary>
    /// Returns an ordered stream of documents based on proximity to a geospatial point.
    /// </summary>
    public AggQuery<T> GeoNear(double[] near,
        string distanceField,
        int? limit = null,
        int? num = null,
        double? maxDistance = null,
        Expression<Func<T, bool>> query = null,
        bool? spherical = null,
        double? distanceMultiplier = null,
        string includeLocs = null,
        bool? uniqueDocs = null
        )
    {
        var bson = new BsonDocument()
            {
                {"near",new BsonArray(near)},
                {"distanceField",distanceField},
            };

        if (limit != null)
            bson.Add("limit", limit.Value);

        if (num != null)
            bson.Add("num", num.Value);

        if (maxDistance != null)
            bson.Add("maxDistance", maxDistance.Value);

        // if (query != null)
        // {
        //     if (_mongoCollection == null)
        //         throw new InvalidOperationException("GeoNear operator with query requires MongoCollection<> instance (set it through AggQuery<> constructor)");
        //     bson.Add("query", CreateQueryBson(query));
        // }

        if (spherical != null)
            bson.Add("spherical", spherical.Value);

        if (includeLocs != null)
            bson.Add("includeLocs", includeLocs);

        if (uniqueDocs != null)
            bson.Add("uniqueDocs", uniqueDocs.Value);

        _parts.Add(new BsonDocument()
            {
                {"$geoNear", bson}
            });

        return this;
    }


    /// <summary>
    /// Returns an ordered stream of documents based on proximity to a geospatial point.
    /// </summary>
    public AggQuery<TResult> GeoNear<TResult>(double[] near,
        Expression<Func<TResult, object>> distanceFieldSelector,
        int? limit = null,
        int? num = null,
        double? maxDistance = null,
        Expression<Func<T, bool>> query = null,
        bool? spherical = null,
        double? distanceMultiplier = null,
        Expression<Func<TResult, object>> includeLocsSelector = null,
        bool? uniqueDocs = null
        )
        where TResult : T
    {
        var distanceField = GetPathString(distanceFieldSelector.Body);
        var includeLocs = includeLocsSelector != null ? GetPathString(includeLocsSelector.Body) : null;

        var agg = GeoNear(near, distanceField, limit, num, maxDistance, query,
            spherical, distanceMultiplier, includeLocs, uniqueDocs);

        return new AggQuery<TResult>() { _parts = agg._parts, _mongoCollection = agg.CreateNewMongoCollection<TResult>() };
    }


    /// <summary>
    /// Takes an array of documents and returns them as a stream of documents.
    /// </summary>
    public AggQuery<TResult> Unwind<TItem, TResult>(
        Expression<Func<T, IEnumerable<TItem>>> exp,
        Expression<Func<T, TItem, TResult>> resultShapeCreator)
    {
        return ProjectOrGroup<TResult>("$unwind", exp);
    }

    /// <summary>
    /// Filters the document stream, and only allows matching documents to pass into the next pipeline stage. $match uses standard MongoDB queries.
    /// </summary>
    public AggQuery<T> Match(BsonDocument bsonQuery)
    {
        _parts.Add(new BsonDocument()
            {
                {"$match", bsonQuery }
            });

        return this;
    }

    /// <summary>
    /// Filters the document stream, and only allows matching documents to pass into the next pipeline stage. $match uses standard MongoDB queries.
    /// </summary>
    public AggQuery<T> Match(Expression<Func<T, bool>> exp)
    {
        // if (_mongoCollection == null)
        //     throw new InvalidOperationException("Match operator requires MongoCollection<> instance (set it through AggQuery<> constructor)");

        // var aa = PipelineStageDefinitionBuilder.Match(exp).ToString();

        // var aa = BsonSerializer.Deserialize<BsonDocument>(PipelineStageDefinitionBuilder.Match(exp).ToString()).ToJson();

        // var fluent = _mongoCollection.Aggregate().AppendStage(PipelineStageDefinitionBuilder.Match(exp));
        // var aa2 = fluent.Stages[0].ToBsonDocument().ToString();

        // var def = new PipelineStagePipelineDefinition<T, T>(fluent.Stages);





        // var a = PipelineStageDefinitionBuilder.Match(exp);


        _parts.Add(BsonSerializer.Deserialize<BsonDocument>(PipelineStageDefinitionBuilder.Match(exp).ToString()));
        return this;

        // if (_mongoCollection == null)
        //     throw new InvalidOperationException("Match operator requires MongoCollection<> instance (set it through AggQuery<> constructor)");

        // var bsonDocument = CreateQueryBson(exp);
        // return Match(bsonDocument);
    }

    // private BsonDocument CreateQueryBson(Expression<Func<T, bool>> exp)
    // {
    //     var query = _mongoCollection.AsQueryable().Where(exp);
    //     var provider = (MongoQueryProvider)query.Provider;
    //     var translatedQuery = (SelectQuery)MongoQueryTranslator.Translate(provider, query.Expression);
    //     var bsonDocument = translatedQuery.BuildQuery() as QueryDocument;
    //     return bsonDocument;
    // }


    /// <summary>
    /// Groups documents together for the purpose of calculating aggregate values based on a collection of documents.
    /// </summary>
    public AggQuery<TResult> Group<TResult>(Expression<Func<T, GroupOperators, TResult>> exp)
    {
        return ProjectOrGroup<TResult>("$group", exp);
    }
    /// <summary>
    /// Reshapes a document stream. $project can rename, add, or remove fields as well as create computed values and sub-documents.
    /// </summary>
    public AggQuery<TResult> Project<TResult>(Expression<Func<T, ProjectOperators, TResult>> exp)
    {
        return ProjectOrGroup<TResult>("$project", exp);
    }
    /// <summary>
    /// Reshapes a document stream. $project can rename, add, or remove fields as well as create computed values and sub-documents.
    /// </summary>
    public AggQuery<TResult> Project<TResult>(Expression<Func<T, TResult>> exp)
    {
        return Project(
            Expression.Lambda<Func<T, ProjectOperators, TResult>>(exp.Body,
                exp.Parameters[0], Expression.Parameter(typeof(ProjectOperators), "o")
                ));
    }
    /// <summary>
    /// Takes all input documents and returns them in a stream of sorted documents.
    /// </summary>
    public AggSortedQuery<T> Sort<T2>(Expression<Func<T, T2>> exp, bool asc = true)
    {
        _parts.Add(new BsonDocument()
            {
                {
                    "$sort",new BsonDocument()
                    {
                        {GetPathString(exp.Body), asc ? 1 : -1}
                    }
                }

            });
        return new AggSortedQuery<T>() { _parts = _parts, _mongoCollection = _mongoCollection };
    }

    /// <summary>
    /// Restricts the number of documents in an aggregation pipeline.
    /// </summary>
    public AggQuery<T> Limit(int count)
    {
        _parts.Add(new BsonDocument()
            {
                {"$limit",count}
            });
        return this;
    }

    /// <summary>
    /// Skips over a specified number of documents from the pipeline and returns the rest.
    /// </summary>
    public AggQuery<T> Skip(int count)
    {
        _parts.Add(new BsonDocument()
            {
                {"$skip",count}
            });
        return this;
    }
    #endregion

    #region Map


    //private BsonValue MapCondition(Expression expression, ParameterExpression firstParameter)
    //{
    //    if (_mongoCollection == null)
    //        throw new InvalidOperationException("$cond method requires MongoCollection<> instance (set it through AggQuery<> constructor)");

    //    var lambdaCondition =  Expression.Lambda<Func<T, bool>>(expression, firstParameter);
    //    BsonValue bsonDocument = CreateQueryBson(lambdaCondition);
    //    bsonDocument = AddDollarPrefix(bsonDocument);
    //    return bsonDocument;
    //}

    //private BsonValue AddDollarPrefix(BsonValue bsonValue)
    //{
    //    if (bsonValue.IsBsonArray)
    //        return new BsonArray(((BsonArray) bsonValue).Select(AddDollarPrefix));

    //    if (bsonValue.IsBsonDocument)
    //    {
    //        return new BsonDocument()
    //            .AddRange( ((BsonDocument) bsonValue)
    //                .Select( e => new BsonElement( e.Name.StartsWith("$") ? e.Name : "$" + e.Name, AddDollarPrefix(e.Value))) );
    //    }

    //    return bsonValue;
    //}


    private BsonValue Map(Expression expression)
    {
        if (typeof(BsonValue).IsAssignableFrom(expression.Type))
            return Expression.Lambda<Func<BsonValue>>(expression, new ParameterExpression[0]).Compile().Invoke();

        switch (expression.NodeType)
        {
            case ExpressionType.New:
                return MapNewExpression((NewExpression)expression);
            case ExpressionType.Convert:
                return MapConvertExpression((UnaryExpression)expression);
            case ExpressionType.Constant:
                return MapConstantExpression((ConstantExpression)expression);
            case ExpressionType.MemberAccess:
                return MapMemberExpression((MemberExpression)expression);
            case ExpressionType.Call:
                return MapMethodCallExpression((MethodCallExpression)expression);
            default:
                throw new NotSupportedException(string.Format("'{0}' expression is not supported!", expression.NodeType));
        }
    }

    private BsonValue MapConvertExpression(UnaryExpression unaryExpression)
    {
        return Map(unaryExpression.Operand);
    }

    private BsonValue MapNewExpression(NewExpression expression)
    {
        if (expression.Members.Count == 0) // jesli nie anonimowy typ lub pusty anonimowy
            return new BsonDocument();

        var items = expression.Members
            .Zip(expression.Arguments, (m, a) => new { m, a })
            .Select(p => new BsonElement(p.m.Name, Map(p.a)));

        return new BsonDocument().AddRange(items);
    }

    private BsonValue MapMethodCallExpression(MethodCallExpression expression)
    {
        if (expression.Method.IsGenericMethod &&
            _bsonMethodInfos.Any(m => m == expression.Method.GetGenericMethodDefinition()))
        {
            return Map(expression.Arguments[0]);
        }

        var operatorAttribute = (OperatorAttribute)Attribute.GetCustomAttribute(expression.Method, typeof(OperatorAttribute));
        if (operatorAttribute == null)
            throw new NotSupportedException(string.Format("'{0}' method is not supported", expression.Method.Name));

        BsonValue bsonValue = null;
        if (expression.Arguments.Count > 1 ||
            (expression.Arguments.Count == 1 && expression.Arguments[0] is NewArrayExpression))
        {
            IEnumerable<Expression> argsExpressions = expression.Arguments.Count > 1
                ? expression.Arguments
                : ((NewArrayExpression)expression.Arguments[0]).Expressions;

            bsonValue = new BsonArray(argsExpressions.Select(Map));
        }
        else
        {
            bsonValue = Map(expression.Arguments[0]);
        }

        return new BsonDocument { { operatorAttribute.OperatorName, bsonValue } };
    }

    private BsonValue MapMemberExpression(MemberExpression expression)
    {
        return new BsonString("$" + GetPathString(expression));
    }

    private BsonValue MapConstantExpression(ConstantExpression expression)
    {
        return expression.Value == null ? BsonNull.Value : BsonValue.Create(expression.Value);
    }
    #endregion

    private AggQuery<TResult> ProjectOrGroup<TResult>(string operatorName, LambdaExpression exp)
    {
        _parts.Add(new BsonDocument()
            {
                {operatorName, Map(exp.Body)}
            });
        return new AggQuery<TResult>() { _parts = _parts, _mongoCollection = CreateNewMongoCollection<TResult>() };
    }

    private IMongoCollection<TResult> CreateNewMongoCollection<TResult>()
    {
        if (_mongoCollection == null)
            return null;

        return _mongoCollection.Database.GetCollection<TResult>("__"); // cos musi byc podane
    }

    private static IEnumerable<MemberInfo> GetMemberInfoPath(Expression expression)
    {
        if (expression.NodeType == ExpressionType.Convert)
            expression = ((UnaryExpression)expression).Operand;

        MemberExpression memberExpression = null;
        while ((memberExpression = expression as MemberExpression) != null)
        {
            yield return memberExpression.Member;
            expression = memberExpression.Expression;
        }
    }

    protected static string GetPathString(Expression expression)
    {
        Func<MemberInfo, string> mapToName = m =>
        {
            var map = BsonClassMap.LookupClassMap(m.DeclaringType);
            if (map != null)
            {
                var memberMap = map.GetMemberMap(m.Name);
                if (memberMap != null)
                    return memberMap.ElementName;
            }
            return m.Name;
        };

        return string.Join(".", GetMemberInfoPath(expression).Reverse().Select(mapToName));
    }
}

public sealed class AggSortedQuery<T> : AggQuery<T>
{
    public AggSortedQuery<T> ThenBy<T2>(Expression<Func<T, T2>> exp, bool asc = true)
    {
        var last = _parts.LastOrDefault();
        if (last != null)
        {
            var bsonDocument = last["$sort"].AsBsonDocument;
            bsonDocument.Add(GetPathString(exp.Body), asc ? 1 : -1);
        }
        return this;
    }
}
