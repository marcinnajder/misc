namespace AlgorithmsAndDataStructures;

// this is exactly the same problem as https://marcinnajder.github.io/2023/08/16/dotnet-project-overview.html
class Cre1
{
    record Package(int Id, List<int> Deps);

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
        }.ToDictionary(p => p.Id);

        var cache = new Dictionary<int, int>();

        var q =
            from packageId in allPackages.Keys
            let level = GetLevel(packageId, allPackages, cache)
            group packageId by level into gr
            orderby gr.Key
            select gr.OrderBy(id => id);

        foreach (var level in q)
        {
            Console.WriteLine(string.Join(',', level));
        }
        // 1,6
        // 2,5
        // 3
        // 4
    }

    static int GetLevel(int packageId, Dictionary<int, Package> all, Dictionary<int, int> cache)
    {
        if (cache.TryGetValue(packageId, out int level))
        {
            return level;
        }

        var package = all[packageId];

        level = package.Deps.Count == 0
            ? 0
            : package.Deps.Max(pid => GetLevel(pid, all, cache)) + 1;

        cache[packageId] = level;
        return level;
    }
}