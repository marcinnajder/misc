using System.Collections.Immutable;
namespace AlgorithmsAndDataStructures;

class Cre2
{
    record Package(int Id, ImmutableHashSet<int> Deps);

    static IEnumerable<List<int>> GetLevels(List<Package> currentPackages)
    {
        var lastLevel = new List<int>();
        var nextPackages = new List<Package>();

        foreach (var package in currentPackages)
        {
            if (package.Deps.Count == 0)
            {
                lastLevel.Add(package.Id);
            }
            else
            {
                nextPackages.Add(package);
            }
        }

        // remove last level packages from dependencies
        nextPackages = nextPackages.ConvertAll(p => p with { Deps = p.Deps.Except(lastLevel) });

        return lastLevel.Count == 0 ? [] : [lastLevel, .. GetLevels(nextPackages)];
    }

    public static void Run()
    {
        var allPackages = new List<Package>()
        {
            new (1, []),
            new (2, [1]),
            new (3, [2]),
            new (4, [2,3,5,6]),
            new (5, [1]),
            new (6, []),
        };

        allPackages.Sort((p1, p2) => p1.Id.CompareTo(p2.Id)); // sort packages within the same level

        var q = GetLevels(allPackages);

        foreach (var level in q)
        {
            Console.WriteLine(string.Join(',', level));
        }
    }
}


#region yield
// if (lastLevel.Count > 0)
// {
//     yield return lastLevel;

//     nextPackages = nextPackages.ConvertAll(p => p with { Deps = p.Deps.Except(lastLevel) });
//     foreach (var level in GetLevels(nextPackages))
//     {
//         yield return level;
//     }
// }
#endregion