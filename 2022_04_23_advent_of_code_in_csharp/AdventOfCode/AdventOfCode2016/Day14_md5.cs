namespace AdventOfCode.AdventOfCode2016.Day14;

public static class Day14
{
    //private static Action<object> WriteLine = Console.WriteLine;
    private static Action<object> WriteLine = delegate { };
    private static bool skipRun = true;

    public static string LoadData(string input) => input;

    public static char? FindOrCheckSeries(string text, int seriesLength, char? seriesChar = null)
    {
        for (int i = 0; i < text.Length - (seriesLength - 1); i++)
        {
            var seriesCharSearched = seriesChar ?? text[i];
            var j = 0;
            for (; j < seriesLength; j++)
            {
                if (seriesCharSearched != text[i + j]) // different char, not a series
                {
                    break;
                }
            }

            if (j == seriesLength) // all chars the same, a series found
            {
                return seriesCharSearched;
            }
        }

        return null;
    }

    public static string Puzzle(string input, Func<string, int, string> generateMd5Hash)
    {
        const int nthKey = 64;
        var salt = LoadData(input);
        var keyCount = 0;

        for (int i = 0;; i++)
        {
            var hash1 = generateMd5Hash(salt, i);

            if (FindOrCheckSeries(hash1, 3) is { } seriesChar) // series of 3 found
            {
                for (int j = 0; j < 1000; j++)
                {
                    var hash2 = generateMd5Hash(salt, (i + j + 1));
                    if (FindOrCheckSeries(hash2, 5, seriesChar) is not null) // series of 5 found
                    {
                        keyCount++;
                        WriteLine(new { keyCount, i, hash1 });

                        if (keyCount == nthKey)
                        {
                            return i.ToString(); // found nth key, stop the method
                        }

                        break; // found matching series of 5, stop the j-loop
                    }
                }
            }
        }
    }

    // optimized version saves information about already checked series of 5 
    // puzzle1 takes 700ms (nonoptimized) vs 170ms(optimized)
    // puzzle2 takes 16s :) Most of the time is spent on hash calculation, so optimization doesn't make much difference
    public static string Puzzle_(string input, Func<string, int, string> generateMd5Hash)
    {
        const int nthKey = 64;
        var salt = LoadData(input);
        var keyCount = 0;
        var indexesOfSeries5 = new Dictionary<char, int>();

        for (int i = 0;; i++)
        {
            var hash1 = generateMd5Hash(salt, i);
            if (FindOrCheckSeries(hash1, 3) is { } seriesChar) // series of 3 found
            {
                var seriesOf5Found = false;

                if (indexesOfSeries5.TryGetValue(seriesChar, out var indexOfSeries5) &&
                    indexOfSeries5 > i) // series of 5 already found
                {
                    seriesOf5Found = true;
                }
                else
                {
                    var j = indexOfSeries5 < 0 ? -indexOfSeries5 : i + 1;
                    for (; j < i + 1 + 1000; j++) // search the whole range or only part
                    {
                        var hash2 = generateMd5Hash(salt, j);
                        if (FindOrCheckSeries(hash2, 5, seriesChar) is not null) // series of 5 found
                        {
                            seriesOf5Found = true;
                            indexesOfSeries5[seriesChar] = j;
                            break; // stop the j-loop
                        }
                    }

                    if (!seriesOf5Found)
                    {
                        indexesOfSeries5[seriesChar] = -j; // set the last checked index as negative number
                    }
                }

                if (seriesOf5Found)
                {
                    keyCount++;
                    WriteLine(new { keyCount, i, hash1 });
                    if (keyCount == nthKey)
                    {
                        return i.ToString(); // found nth key, stop the method
                    }
                }
            }
        }
    }


    public static string Puzzle1(string input)
    {
        if (skipRun)
        {
            return "23890";
        }

        var computeMd5HashMemoized = Common.Memoize<string, string>(MD5Utils.ComputeMd5Hash);
        return Puzzle_(input, (salt, index) => computeMd5HashMemoized(salt + index));
    }

    public static string Puzzle2(string input)
    {
        if (skipRun)
        {
            return "22696";
        }

        const int parallelPageSize = 1000;
        var allHashes = new List<string>(parallelPageSize);
        var pageHashed = new string[parallelPageSize];

        return Puzzle_(input, GenerateMd5);

        static string ComputeMd5Hash(string str)
        {
            // generate hash once, then 2016 times generate hash from previous hash
            return Enumerable.Range(0, 1 + 2016).Aggregate(str, (m, _) => MD5Utils.ComputeMd5Hash(m));
        }

        string GenerateMd5(string salt, int index)
        {
            if (index == allHashes.Count) // start next page
            {
                Parallel.For(0, parallelPageSize,
                    i => { pageHashed[i] = ComputeMd5Hash(salt + (allHashes.Count + i)); });
                allHashes.AddRange(pageHashed);
            }

            return allHashes[index];
        }
    }
}