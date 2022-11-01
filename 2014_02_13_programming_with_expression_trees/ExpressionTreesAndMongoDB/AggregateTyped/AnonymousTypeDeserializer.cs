using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace ExpressionTreesAndMongoDB;

public static class AnonymousTypeDeserializer
{
    private static readonly ConcurrentDictionary<Type, object> _defaultValuesCache = new ConcurrentDictionary<Type, object>();
    private static readonly MethodInfo GetDefaultValueGenericMethodInfo =
        typeof(AnonymousTypeDeserializer).GetMethod("GetDefaultValueGeneric", BindingFlags.Static | BindingFlags.NonPublic);

    private static T GetDefaultValueGeneric<T>() // nie zmieniac nazwy tej metody bo jestr wolana przez reflection
    {
        return default(T);
    }

    //UT
    public static object GetDefaultValue(Type type)
    {
        return _defaultValuesCache.GetOrAdd(type, t => GetDefaultValueGenericMethodInfo.MakeGenericMethod(t).Invoke(null, new object[0]));
    }

    // copied from C# mongo driver
    public static bool IsAnonymousType(Type type)
    {
        // don't test for too many things in case implementation details change in the future
        return
            Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false) &&
            type.IsGenericType &&
            type.Name.Contains("Anon"); // don't check for more than "Anon" so it works in mono also
    }

    public static object Deserialize(BsonValue bsonValue, Type descType)
    {
        // domyslne czyli Dictionaty<string,object> dla obiektow i List<object> dla tablic
        var options = BsonTypeMapperOptions.Defaults;
        var @object = BsonTypeMapper.MapToDotNetValue(bsonValue, options);
        return ConvertValue(@object, descType);
    }


    private static BsonDocument CleanUpDictionary(BsonDocument bsonDocument)
    {
        // DictionarySerializer serializje slownik tak ze obiera go w 2 pola _t i _v
        if (bsonDocument.ElementCount == 2 & bsonDocument.Contains("_t") && bsonDocument.Contains("_v"))
        {
            var bsonDocumentInside = bsonDocument["_v"].AsBsonDocument;

            return new BsonDocument(
                bsonDocumentInside.Select((BsonElement bsonElement) =>
                    bsonElement.Value.IsBsonDocument ?
                    new BsonElement(bsonElement.Name, CleanUpDictionary((BsonDocument)bsonElement.Value)) :
                    bsonElement)
            );

            // foreach (var bsonElement in bsonDocumentInside)
            // {
            //     if (bsonElement.Value.IsBsonDocument)
            //     {
            //         bsonElement.Value = CleanUpDictionary((BsonDocument)bsonElement.Value);
            //     }
            // }

            // return bsonDocumentInside;
        }

        return bsonDocument;
    }

    private static object ConvertValue(object value, Type descType)
    {
        if (value == null)
            return GetDefaultValue(descType);

        var valueType = value.GetType();
        if (valueType == descType)
            return value;

        if (valueType == typeof(Dictionary<string, object>))
        {
            if (IsAnonymousType(descType))
                return CreateAnonymousType((Dictionary<string, object>)value, descType);

            // serializacja aby ponownie zdeserializowac ;) (to nie powinno byc czeste bo glownie chodzi nam o anonimowe typy)
            var bsonDocument = value.ToBsonDocument();
            bsonDocument = CleanUpDictionary(bsonDocument);
            return BsonSerializer.Deserialize(bsonDocument, descType);
        }

        if (valueType == typeof(List<object>))
        {
            var list = (IList<object>)value;

            if (descType.IsArray)
            {
                var elementType = descType.GetElementType();
                var array = Array.CreateInstance(elementType, list.Count);
                for (int i = 0; i < list.Count; i++)
                    array.SetValue(ConvertValue(list[i], elementType), i);
                return array;
            }

            if (descType.IsGenericType && descType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = descType.GetGenericArguments()[0];
                var descList = (IList)Activator.CreateInstance(descType);
                foreach (var item in list)
                {
                    descList.Add(ConvertValue(item, elementType));
                }
                return descList;
            }

            throw new NotSupportedException("Only arrays and List<T> generic lists are supported for JSON arrays");
        }

        return Convert.ChangeType(value, descType);
    }

    private static object CreateAnonymousType(Dictionary<string, object> bsonDocument, Type type)
    {
        var constructorInfo = type.GetConstructors()[0];
        var arguments = constructorInfo.GetParameters();
        var argumentValues = new List<object>();

        foreach (var argument in arguments)
        {
            var argumentType = argument.ParameterType;
            object value = null;
            if (bsonDocument.TryGetValue(argument.Name, out value))
            {
                value = ConvertValue(value, argumentType);
            }
            else
            {
                value = GetDefaultValue(argumentType);
            }
            argumentValues.Add(value);
        }

        return Activator.CreateInstance(type, argumentValues.ToArray());
    }
}
