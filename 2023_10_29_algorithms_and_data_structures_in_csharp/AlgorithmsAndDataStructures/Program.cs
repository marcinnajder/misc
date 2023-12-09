using System.Reflection;
using System.Runtime;
using AlgorithmsAndDataStructures;

Console.WriteLine();

LList<int> list1 = [12, 3, 4];
LList<int> list2 = [.. list1];


static void PrintAllMethods<T>(bool inherited = false) => PrintAllMethods2(typeof(T), inherited);

static void PrintAllMethods2(Type type, bool inherited = false)
{
    IEnumerable<Type> types = inherited ? GetParents(type) : [type];

    var names = types
        .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        .Select(m => m.Name)
        .Distinct();


    Console.WriteLine($"Methods of : {type.Name}");
    Console.WriteLine(String.Join(Environment.NewLine, names));
    Console.WriteLine();

    static IEnumerable<Type> GetParents(Type type)
    {
        foreach (var t in type.GetInterfaces().Concat([type.BaseType]).Where(t => t != null).SelectMany(GetParents))
        {
            yield return t;
        }
        yield return type;
    }
}

PrintAllMethods2(typeof(IList<>), inherited: true);

// PrintAllMethods<IList<int>>(inherited: true);
// PrintAllMethods<List<int>>();


// Console.WriteLine("IList");
// Console.WriteLine(String.Join(Environment.NewLine, typeof(IList<>).GetMethods(System.Reflection.BindingFlags.Instance).Select(m => m.Name)));

// Console.WriteLine();
// Console.WriteLine("List");
// Console.WriteLine(String.Join(Environment.NewLine, typeof(List<>).GetMethods().Select(m => m.Name)));