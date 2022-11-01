using MongoDB.Bson;

namespace ExpressionTreesAndMongoDB;

internal class OperatorAttribute : Attribute
{
    public string OperatorName { get; private set; }

    public OperatorAttribute(string operatorName)
    {
        OperatorName = operatorName;
    }
}

public class CommonOperators
{
    /// <summary>
    /// Takes an array with two expressions. $ifNull returns the first expression if it evaluates to a non-null value. Otherwise, $ifNull returns the second expressionâs value.
    /// </summary>
    [Operator("$ifNull")] public TResult IfNull<T, TResult>(T first, TResult second) => throw new NotImplementedException();

    /// <summary>
    /// $cond is a ternary operator that takes an array of three expressions, where the first expression evaluates to a Boolean value. If the first expression evaluates to true, then $cond evaluates and returns the value of the second expression. If the first expression evaluates to false, then $cond evaluates and returns the third expression.
    /// </summary>
    [Operator("$cond")] public TResult Cond<TResult>(BsonDocument bsonDocument, TResult trueCase, TResult falseCase) => throw new NotImplementedException();

    public TResult Bson<TResult>(BsonValue bsonValue) => throw new NotImplementedException();

    public TResult Bson<TResult>(BsonValue bsonValue, TResult resultShape) => throw new NotImplementedException();
}

public class ProjectOperators : CommonOperators
{
    // ** Arithmetic Aggregation Operators

    /// <summary>
    /// Computes the sum of an array of numbers.
    /// </summary>
    [Operator("$add")] public T Add<T>(params T[] numbers) => throw new NotImplementedException();

    /// <summary>
    /// Computes the sum of an array of numbers.
    /// </summary>
    [Operator("$add")] public TResult Add<T, TResult>(params T[] numbers) => throw new NotImplementedException();

    /// <summary>
    /// Computes the product of an array of numbers.
    /// </summary>
    [Operator("$multiply")] public T Multiply<T>(params T[] numbers) => throw new NotImplementedException();

    /// <summary>
    /// Computes the product of an array of numbers.
    /// </summary>
    [Operator("$multiply")] public TResult Multiply<T, TResult>(params T[] numbers) => throw new NotImplementedException();

    /// <summary>
    /// Takes two numbers and divides the first number by the second.
    /// </summary>
    [Operator("$divide")] public T Divide<T>(T first, T second) => throw new NotImplementedException();

    /// <summary>
    /// Takes two numbers and divides the first number by the second.
    /// </summary>
    [Operator("$divide")] public TResult Divide<T, TResult>(T first, T second) => throw new NotImplementedException();

    /// <summary>
    /// Takes two numbers and calculates the modulo of the first number divided by the second.
    /// </summary>
    [Operator("$mod")] public T Mod<T>(T first, T second) => throw new NotImplementedException();

    /// <summary>
    /// Takes two numbers and calculates the modulo of the first number divided by the second.
    /// </summary>
    [Operator("$mod")] public TResult Mod<T, TResult>(T first, T second) => throw new NotImplementedException();

    /// <summary>
    /// Takes two numbers and subtracts the second number from the first.
    /// </summary>
    [Operator("$subtract")] public T Subtract<T>(T first, T second) => throw new NotImplementedException();




    // ** String Aggregation Operators

    /// <summary>
    /// Takes an array of strings, concatenates the strings, and returns the concatenated string
    /// </summary>
    [Operator("$concat")] public string Concat(params string[] strings) => throw new NotImplementedException();

    /// <summary>
    /// Takes in two strings. Returns a number. $strcasecmp is positive if the first string is âgreater thanâ the second and negative if the first string is âless thanâ the second. $strcasecmp returns 0 if the strings are identical.
    /// </summary>
    [Operator("$strcasecmp")] public int Strcasecmp(string first, string second) => throw new NotImplementedException();

    /// <summary>
    /// $substr takes a string and two numbers. The first number represents the number of bytes in the string to skip, and the second number specifies the number of bytes to return from the string.
    /// </summary>
    [Operator("$substr")] public string Substr(string @string, int skip, int @return) => throw new NotImplementedException();

    /// <summary>
    /// Takes a single string and converts that string to lowercase, returning the result. All uppercase letters become lowercase
    /// </summary>
    [Operator("$toLower")] public string ToLower(string @string) => throw new NotImplementedException();

    /// <summary>
    /// Takes a single string and converts that string to uppercase, returning the result. All lowercase letters become uppercase.
    /// </summary>
    [Operator("$toUpper")] public string ToUpper(string @string) => throw new NotImplementedException();

    // ** Date Aggregation Operators

    /// <summary>
    /// Converts a date to a number between 1 and 366
    /// </summary>
    [Operator("$dayOfYear")] public int DayOfYear(DateTime? date) => throw new NotImplementedException();

    /// <summary>
    /// Converts a date to a number between 1 and 31
    /// </summary>
    [Operator("$dayOfMonth")] public int DayOfMonth(DateTime? date) => throw new NotImplementedException();

    /// <summary>
    /// Converts a date to a number between 1 and 7
    /// </summary>
    [Operator("$dayOfWeek")] public int DayOfWeek(DateTime? date) => throw new NotImplementedException();

    /// <summary>
    /// Converts a date to the full year.
    /// </summary>
    [Operator("$year")] public int Year(DateTime? date) => throw new NotImplementedException();

    /// <summary>
    /// Converts a date into a number between 1 and 12
    /// </summary>
    [Operator("$month")] public int Month(DateTime? date) => throw new NotImplementedException();

    /// <summary>
    /// Converts a date into a number between 1 and 12
    /// </summary>
    [Operator("$week")] public int Week(DateTime? date) => throw new NotImplementedException();

    /// <summary>
    /// Converts a date into a number between 0 and 23
    /// </summary>
    [Operator("$hour")] public int Hour(DateTime? date) => throw new NotImplementedException();

    /// <summary>
    /// Converts a date into a number between 0 and 59
    /// </summary>
    [Operator("$minute")] public int Minute(DateTime? date) => throw new NotImplementedException();

    /// <summary>
    /// Converts a date into a number between 0 and 59. May be 60 to account for leap seconds
    /// </summary>
    [Operator("$second")] public int Second(DateTime? date) => throw new NotImplementedException();

    /// <summary>
    /// Returns the millisecond portion of a date as an integer between 0 and 999
    /// </summary>
    [Operator("$millisecond")] public int Millisecond(DateTime? date) => throw new NotImplementedException();
}

public class GroupOperators : CommonOperators
{
    /// <summary>
    /// Returns an array of all the unique values for the selected field among for each document in that group
    /// </summary>
    [Operator("$addToSet")] public T[] AddToSet<T>(T value) => throw new NotImplementedException();

    /// <summary>
    /// Returns the first value in a group
    /// </summary>
    [Operator("$first")] public T First<T>(T value) => throw new NotImplementedException();

    /// <summary>
    /// Returns the last value in a group
    /// </summary>
    [Operator("$last")] public T Last<T>(T value) => throw new NotImplementedException();

    /// <summary>
    /// Returns the highest value in a group
    /// </summary>
    [Operator("$max")] public T Max<T>(T value) => throw new NotImplementedException();

    /// <summary>
    /// Returns the lowest value in a group
    /// </summary>
    [Operator("$min")] public T Min<T>(T value) => throw new NotImplementedException();

    /// <summary>
    /// Returns an average of all the values in a group
    /// </summary>
    [Operator("$avg")] public T Avg<T>(T value) => throw new NotImplementedException();

    /// <summary>
    /// Returns an average of all the values in a group
    /// </summary>
    [Operator("$avg")] public TResult Avg<T, TResult>(T value) => throw new NotImplementedException();

    /// <summary>
    /// Returns an array of all values for the selected field among for each document in that group
    /// </summary>
    [Operator("$push")] public T[] Push<T>(T value) => throw new NotImplementedException();

    /// <summary>
    /// Returns the sum of all the values in a group
    /// </summary>
    [Operator("$sum")] public T Sum<T>(T value) => throw new NotImplementedException();

    /// <summary>
    /// Returns the sum of all the values in a group
    /// </summary>
    [Operator("$sum")] public TResult Sum<T, TResult>(T value) => throw new NotImplementedException();
}
