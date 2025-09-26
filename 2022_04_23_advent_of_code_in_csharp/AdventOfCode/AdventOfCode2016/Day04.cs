
using System.Data;
using static AdventOfCode.Extensions;

namespace AdventOfCode.AdventOfCode2016.Day4;

public record Code(string[] Names, int SectorID, string Checksum)
{
    public string NamesCombined { get; } = string.Join("", Names);
}

public static class Day4
{
    public static IEnumerable<Code> LoadData(string input)
    {
        return input
            .Split(Environment.NewLine, StringSplitOptions.TrimEntries)
            .Select(code =>
            {
                var sectorID = code[^6..^1];
                var names = code[0..^7];
                var namesSegments = names.Split("-");
                return new Code(namesSegments[0..^1], int.Parse(namesSegments[^1]), sectorID);
            });
    }

    public static string Decrypt(this string str, int sectorID)
    {
        var shift = sectorID % 26;
        var chars = str.Select(c =>
            c == '-'
                ? ' '
                : ((char)(c + shift)).Pipe(cc => cc <= 'z' ? cc : (char)('a' + (cc - 'z') - 1)));
        return string.Concat(chars);
    }

    public static string Puzzle1(string input)
    {
        var codes = LoadData(input);

        var sum = codes
            .Where(code =>
            {
                var chars =
                (
                    from kv in code.NamesCombined.CountBy(Identity)
                    orderby kv.Value descending, kv.Key
                    select kv.Key
                ).Take(5);
                return string.Concat(chars) == code.Checksum;
            })
            .Sum(code => code.SectorID);

        return sum.ToString();
    }

    public static string Puzzle2(string input)
    {
        var codes = LoadData(input);
        var code = codes.First(code => code.NamesCombined.Decrypt(code.SectorID) == "northpoleobjectstorage");
        return code.SectorID.ToString();
    }
}
