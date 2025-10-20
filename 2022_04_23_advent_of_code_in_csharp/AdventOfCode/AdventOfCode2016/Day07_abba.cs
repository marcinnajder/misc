
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

namespace AdventOfCode.AdventOfCode2016.Day7;

public record IP(string[] Inside, string[] Outside);

public static class Day7
{
    public static IEnumerable<IP> LoadData(string input)
    {
        return input
            .Split(Environment.NewLine, StringSplitOptions.TrimEntries)
            .Select(line =>
            {
                var parts = line.Split('[', ']');
                var inside = new string[(parts.Length + 1) / 2];
                var outside = new string[parts.Length - inside.Length];
                for (int i = 0; i < parts.Length; i++)
                {
                    (i % 2 == 0 ? inside : outside)[i / 2] = parts[i];
                }
                return new IP(inside, outside);
            });
    }


    public static bool IsAbba(string text)
    {
        for (int i = 1; i < text.Length - 2; i++)
        {
            if (text[i] == text[i + 1] && text[i] != text[i - 1] && text[i - 1] == text[i + 2])
            {
                return true;
            }
        }
        return false;
    }

    public static IEnumerable<(char, char, char)> FindAllAba(string text)
    {
        for (int i = 0; i < text.Length - 2; i++)
        {
            if (text[i] != text[i + 1] && text[i] == text[i + 2])
            {
                yield return (text[i], text[i + 1], text[i + 2]);
            }
        }
    }

    public static string Puzzle1(string input)
    {
        var ips = LoadData(input);
        return ips.Count(ip => ip.Inside.Any(IsAbba) && !ip.Outside.Any(IsAbba)).ToString();
    }

    public static string Puzzle2(string input)
    {
        var ips = LoadData(input);
        return ips.Count(ip =>
        {
            foreach (var (a, b, _) in ip.Inside.SelectMany(FindAllAba))
            {
                var search = string.Concat(b, a, b);
                if (ip.Outside.Any(outside => outside.Contains(search)))
                {
                    return true;
                }
            }
            return false;
        }).ToString();
    }

    public static string Puzzle2_(string input)
    {
        var ips = LoadData(input);
        return ips.Count(ip =>
            (
                from inside in ip.Inside
                from aba in FindAllAba(inside)
                select string.Concat(aba.Item2, aba.Item1, aba.Item2)
            ).Any(search => ip.Outside.Any(outside => outside.Contains(search)))
       ).ToString();
    }

}
