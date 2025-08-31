using System.Linq.Expressions;
using static System.Linq.Expressions.ExpressionType;

namespace AlgorithmsAndDataStructures;

// expression trees + pattern matching
class ExpressionTreesPatternMatching
{
    public static void ExpressionTrees()
    {
        ParameterExpression aParamExpr = Expression.Parameter(typeof(int), "a");
        ParameterExpression bParamExpr = Expression.Parameter(typeof(int), "b");
        BinaryExpression addExpr = Expression.Add(aParamExpr, bParamExpr);
        Expression<Func<int, int, int>> lambdaExpr =
            Expression.Lambda<Func<int, int, int>>(addExpr, aParamExpr, bParamExpr);

        Func<int, int, int> add = lambdaExpr.Compile();
        Console.WriteLine("10+5=" + add(10, 5)); // 10+5=15

        Func<int, int, int> addDelegate = (a, b) => a + b;
        Expression<Func<int, int, int>> addExpression = (a, b) => a + b;
    }

    public static void Linq()
    {
        var elements = new[] { 1, 2, 3 };
        var q = from el in elements where el > 10 select el * 10;
        // code above is translated into code below
        // var q = elements.Where(el => el > 10).Select(el => el * 10);

    }

    public static void VisitorTest()
    {
        Expression<Func<int, bool>> expr1 = p => p > 3 && p <= 200;

        Expression<Func<int, bool>> expr2 = p => p.CompareTo(3) > 0 && p.CompareTo(200) <= 0;

        var vistor = new MyVisitor();
        Expression<Func<int, bool>> expr3 = (Expression<Func<int, bool>>)vistor.Visit(expr2);
        Console.WriteLine(expr3); // p => ((p > 3) AndAlso (p <= 200))

        var vistor2 = new MyVisitor();
        Expression<Func<int, bool>> expr4 = (Expression<Func<int, bool>>)vistor.Visit(expr2);


        var funcs = new[] { expr1, expr2, expr3, expr4 }.Select(expr => expr.Compile()).ToArray();

        Assert(false, funcs.Select(f => f(0)));
        Assert(false, funcs.Select(f => f(3)));
        Assert(true, funcs.Select(f => f(4)));
        Assert(true, funcs.Select(f => f(100)));
        Assert(true, funcs.Select(f => f(200)));
        Assert(false, funcs.Select(f => f(201)));
        Console.WriteLine("unit tests succeeded");

        static void Assert<T>(T expected, IEnumerable<T> actuals)
            where T : notnull
        {
            foreach (var actual in actuals.Where(a => !a.Equals(expected)))
            {
                throw new Exception($"expected: {expected}, actual: {actual}");
            }
        }
    }
}


class MyVisitor : ExpressionVisitor
{
    protected override Expression VisitBinary(BinaryExpression node)
    {
        if (node is
            {
                NodeType: GreaterThan or GreaterThanOrEqual or LessThan or LessThanOrEqual,
                Left: MethodCallExpression { Method: { Name: "CompareTo" }, Object: { } obj, Arguments: [var arg] }
            })
        {
            return Expression.MakeBinary(node.NodeType, obj, arg);
        }

        return base.VisitBinary(node);
    }
}


class MyVisitor2 : ExpressionVisitor
{
    protected override Expression VisitBinary(BinaryExpression node)
    {
        var type = node.NodeType;
        if (type == GreaterThan || type == GreaterThanOrEqual || type == LessThan || type == LessThanOrEqual)
        {
            var call = node.Left as MethodCallExpression;
            if (call != null)
            {
                if (call.Method.Name == "CompareTo")
                {
                    return Expression.MakeBinary(node.NodeType, call.Object!, call.Arguments[0]);
                }
            }
        }

        return base.VisitBinary(node);
    }
}

