using System.Linq.Expressions;

namespace MonadsInCSharp;

internal static class Utils
{
    internal static void Call(Expression<Action> action)
    {
        if (action.Body is MethodCallExpression
            {
                Method: { Name: { } methodName, DeclaringType: { Name: { } typeName } }
            })
        {
            Console.WriteLine($" --> {typeName}.{methodName}");
            try
            {
                action.Compile()();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception: {exception.Message}");
            }
        }
        else
        {
            throw new ArgumentException($"'{nameof(action)}' argument is not a function call");
        }
    }

    internal static string JoinItems<T>(this IEnumerable<T> items, string separator = ",")
    {
        return string.Join(separator, items);
    }
}
