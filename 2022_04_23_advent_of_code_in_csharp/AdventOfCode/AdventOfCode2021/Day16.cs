// #pragma warning disable CS8509

using static AdventOfCode.AdventOfCode2021.Option;

namespace AdventOfCode.AdventOfCode2021;

static class Day16
{
    static IEnumerable<LList<char>> Bin(int n) =>
        n switch
        {
            1 => new LList<char>[] { new('0', null), new('1', null) },
            _ => new[] { '0', '1' }.SelectMany(h => Bin(n - 1).Select(t => new LList<char>(h, t)))
        };

    static long BitsToInt64(this IEnumerable<char> bits) =>
        bits.AggregateBack((Pow2: 1L, Sum: 0L), (a, bit) => (2 * a.Pow2, a.Sum + (bit == '0' ? 0 : a.Pow2))).Sum;

    static int BitsToInt(this IEnumerable<char> bits) => (int)BitsToInt64(bits);

    static Dictionary<char, LList<char>> Mapping =
        Bin(4).ToDictionary(bits => Convert.ToString(BitsToInt(bits.ToEnumerable()), 16).ToUpper()[0], bits => bits);


    public static IEnumerable<char> LoadData(string input) => input.SelectMany(c => Mapping[c].ToEnumerable());


    record Package;
    record ValueLiteral(int Version, int TypeId, long Value) : Package;
    record Operator(int Version, int TypeId, Package[] Packages) : Package;

    public static IEnumerable<T> Share<T>(this IEnumerable<T> source)
    {
        return source.GetEnumerator().Pipe(ShareSequenceImpl);

        static IEnumerable<T> ShareSequenceImpl(IEnumerator<T> e)
        {
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }
    }

    static long ReadValue(IEnumerable<char> reader)
    {
        return ReadValueBits(reader).Pipe(BitsToInt64);

        static IEnumerable<char> ReadValueBits(IEnumerable<char> reader)
        {
            while (true)
            {
                var first = reader.First();
                foreach (var bit in reader.Take(4))
                {
                    yield return bit;
                }
                if (first == '0')
                {
                    yield break;
                }
            }
        }
    }

    static Package[] ReadOperator(IEnumerable<char> reader) =>
        reader.First() switch
        {
            '0' => reader.Take(15).BitsToInt()
                .Pipe(bitsCount => reader.Take(bitsCount).Share())
                .Pipe(newReader => ReadManyPackage(newReader).ToArray()),
            _ => reader.Take(11).BitsToInt()
                .Pipe(packageCount => ReadManyPackage(reader).Take(packageCount).ToArray())
        };

    static Option<Package> TryReadPackage(IEnumerable<char> reader) =>
        reader.Take(3).ToList() switch
        {
            { Count: 3 } first3Bits =>
                (first3Bits.BitsToInt(), reader.Take(3).BitsToInt()) switch
                {
                    var (version, typeId) => Some<Package>(typeId == 4 ?
                        new ValueLiteral(version, typeId, ReadValue(reader)) :
                        new Operator(version, typeId, ReadOperator(reader)))
                },
            _ => None<Package>()
        };

    static IEnumerable<Package> ReadManyPackage(IEnumerable<char> reader)
    {
        while (true)
        {
            if (TryReadPackage(reader) is Some<Package>(var package))
            {
                yield return package;
            }
            else
            {
                yield break;
            }
        }
    }

    static long SumVersions(Package package) =>
        package switch
        {
            ValueLiteral(var version, _, _) => version,
            Operator(var version, _, var packages) => version + packages.Sum(SumVersions),
            _ => throw new NonExhaustivePatternMatchingException()
        };

    static long Calculate(Package package) =>
        package switch
        {
            ValueLiteral(_, _, var value) => value,
            Operator(_, var typeId, var packages) =>
                packages.Select(Calculate).Pipe(values =>
                    typeId switch
                    {
                        0 => values.Sum(),
                        1 => values.Aggregate(1L, (a, b) => a * b),
                        2 => values.Aggregate(long.MaxValue, Math.Min),
                        3 => values.Aggregate(long.MinValue, Math.Max),
                        5 => values.Pairwise().First() is var (a, b) && a > b ? 1L : 0L,
                        6 => values.Pairwise().First() is var (a, b) && a < b ? 1L : 0L,
                        7 => values.Pairwise().First() is var (a, b) && a == b ? 1L : 0L,
                        _ => throw new Exception($"Unknown package typeId={typeId}")
                    }),
            _ => throw new NonExhaustivePatternMatchingException()
        };


    static string Puzzle(string input, Func<Package, long> calculateResult)
    {
        var data = LoadData(input);
        var reader = data.Share();
        var result = TryReadPackage(reader).Pipe(Option.Get).Pipe(calculateResult);
        return result.ToString();
    }

    public static string Puzzle1(string input) => Puzzle(input, SumVersions);
    public static string Puzzle2(string input) => Puzzle(input, Calculate);




    #region statement-based code

    static Package[] ReadOperator_(IEnumerable<char> reader)
    {
        var first = reader.First();
        if (first == '0')
        {
            var bitsCount = reader.Take(15).BitsToInt();
            var newReader = reader.Take(bitsCount).Share();
            return ReadManyPackage(newReader).ToArray();
        }
        else
        {
            var packageCount = reader.Take(11).BitsToInt();
            return ReadManyPackage(reader).Take(packageCount).ToArray();
        }
    }

    static Option<Package> TryReadPackage_(IEnumerable<char> reader)
    {
        var first3Bits = reader.Take(3).ToList();
        if (first3Bits is { Count: 3 })
        {
            var version = first3Bits.BitsToInt();
            var typeId = reader.Take(3).BitsToInt();
            return Some<Package>(typeId == 4 ?
                new ValueLiteral(version, typeId, ReadValue(reader)) :
                new Operator(version, typeId, ReadOperator(reader)));
        }
        else
        {
            return None<Package>();
        }
    }

    #endregion   
}

//#pragma warning restore CS8509