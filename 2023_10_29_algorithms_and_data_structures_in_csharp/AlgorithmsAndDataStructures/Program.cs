using System.Reflection;
using System.Runtime;
using AlgorithmsAndDataStructures;


_ = SuperTest().Excute().ContinueWith(_ => Console.WriteLine("done"));

Console.ReadLine();

static IEnumerable<Task> SuperTest()
{
    for (int i = 0; i < 5; i++)
    {
        yield return Task.Delay(1000);
        Console.WriteLine(i);
    }
}

static class Yo
{
    public static Task Excute(this IEnumerable<Task> enumerable)
    {
        TaskCompletionSource tcs = new TaskCompletionSource();
        var enumerator = enumerable.GetEnumerator();

        Action moveNext = null;
        moveNext = () =>
        {
            if (enumerator.MoveNext())
            {
                enumerator.Current.ContinueWith(_ => moveNext());
            }
            else
            {
                tcs.SetResult();
            }
        };

        moveNext();

        return tcs.Task;
    }
}



// IEnumerable<Task> SearchComarchEnumerable()
// {
//     var google = new GimageSearchClient();

//     // Task<IList<IImageResult>> imagesTask2 = google.SearchTask("comarch", 10);

//     // yield return imagesTask2;

//     // var images2 = imagesTask2.Result;

//     for (int i = 0; i < 100; i++)
//     {
//         Task<IList<IImageResult>> imagesTask = google.SearchTask("comarch", 10);

//         yield return imagesTask;

//         var images = imagesTask.Result;
//     }

//     // foreach (var image in images)
//     // {
//     //     // Console.WriteLine(image.Url);
//     // }
// }

internal class GimageSearchClient
{
    public GimageSearchClient()
    {
    }

    internal Task<IList<IImageResult>> SearchTask(string v1, int v2)
    {
        throw new NotImplementedException();
    }
}

internal interface IImageResult
{
}



// // w C# mamy 'string interpolation' zaczynajac od $"...
// var text = $"jest {name} i mam {age} lat"; // jest marcin i mam 20 lat

// // ale dodali takze """... i $"""... aby sprytnie eskejpować problematyczne znaki
// // nizej nie ma potrzeby pisac """ poniewaz nie uzywamy problematycznych znakow takich jak ", {, }, ... 
// var data1 = $"[{age}, {age}, 123]"; // -> [20, 20, 123]
// var data2 = $"""[{age}, {age}, 123]"""; // -> [20, 20, 123]

// // ale tutaj juz trzeba pisac """ bo jest znak "
// var data3 = $"""[{age}, {age}, 123, "mama" ]"""; // -> [20, 20, 123, "mama" ]

// // gdy chcemy napisac JSON ktory ma obiekt JS i jednoczesnie uzyc string interpolation, to nizej sie nie skompiluje
// // var data4 = $"""{ "name": {name}}""";

// // mozna dodac dwa $$ mowiac dwa {{ teraz wkleja wartosc 
// var data4 = $$"""{"name": "{{name}}" } """; // -> {"name": "marcin" }


// var people = $""" { {age}, {age}, 123}"""; // to si nie

// Console.WriteLine(data1);
// Console.WriteLine(data2);
// Console.WriteLine(data3);
// Console.WriteLine(data4);


// LList<int> list2 = [.. list1];



// SynchronizationContext.Current!.Post((a) => { }, null);

// Task.Delay(100).ContinueWith()


// ThreadPoolTaskScheduler
// Task.Factory.StartNew()
// static void PrintAllMethods<T>(bool inherited = false) => PrintAllMethods2(typeof(T), inherited);

// static void PrintAllMethods2(Type type, bool inherited = false)
// {
//     IEnumerable<Type> types = inherited ? GetParents(type) : [type];

//     var names = types
//         .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
//         .Select(m => m.Name)
//         .Distinct();


//     Console.WriteLine($"Methods of : {type.Name}");
//     Console.WriteLine(String.Join(Environment.NewLine, names));
//     Console.WriteLine();

//     static IEnumerable<Type> GetParents(Type type)
//     {
//         foreach (var t in type.GetInterfaces().Concat([type.BaseType]).Where(t => t != null).SelectMany(GetParents))
//         {
//             yield return t;
//         }
//         yield return type;
//     }
// }

// PrintAllMethods2(typeof(IList<>), inherited: true);

// PrintAllMethods<IList<int>>(inherited: true);
// PrintAllMethods<List<int>>();


// Console.WriteLine("IList");
// Console.WriteLine(String.Join(Environment.NewLine, typeof(IList<>).GetMethods(System.Reflection.BindingFlags.Instance).Select(m => m.Name)));

// Console.WriteLine();
// Console.WriteLine("List");
// Console.WriteLine(String.Join(Environment.NewLine, typeof(List<>).GetMethods().Select(m => m.Name)));