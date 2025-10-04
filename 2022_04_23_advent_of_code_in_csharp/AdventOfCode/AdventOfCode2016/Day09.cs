
using System.Text;
namespace AdventOfCode.AdventOfCode2016.Day9;

public record Marker(int Length, int Repeats);

public static class Day9
{
    public static string LoadData(string input) => input;

    private static Marker ReadMarker(IEnumerator<char> e)
    {
        var builders = new List<StringBuilder>() { new() };

        while (e.MoveNext())
        {
            switch (e.Current)
            {
                case ')':
                    return new Marker(int.Parse(builders[0].ToString()), int.Parse(builders[1].ToString()));
                case 'x':
                    builders.Add(new StringBuilder());
                    break;
                default:
                    builders[^1].Append(e.Current);
                    break;
            }
        }

        throw new Exception("Unreachable code.");
    }

    public static long Decompress(IEnumerator<char> e, bool recursive)
    {
        long count = 0;
        while (e.MoveNext())
        {
            if (char.IsWhiteSpace(e.Current)) // skip whitespace
            {
                break;
            }

            if (e.Current == '(')
            {
                var marker = ReadMarker(e);

                if (recursive)
                {
                    IList<char> markerContent = new char[marker.Length];
                    for (int i = 0; i < marker.Length; i++)
                    {
                        e.MoveNext();
                        markerContent[i] = e.Current;
                    }

                    using var ee = markerContent.GetEnumerator();
                    var ccount = Decompress(ee, recursive);
                    count += ccount * marker.Repeats;
                }
                else
                {
                    for (int i = 0; i < marker.Length; i++)  // consume chars
                    {
                        e.MoveNext();
                    }
                    count += marker.Length * marker.Repeats;
                }
            }
            else
            {
                count += 1;
            }
        }

        return count;
    }

    public static string Puzzle(string input, bool recursive)
    {
        var data = LoadData(input);
        using var e = data.GetEnumerator();
        var result = Decompress(e, recursive);
        return result.ToString();
    }

    public static string Puzzle1(string input) => Puzzle(input, recursive: false);

    public static string Puzzle2(string input) => Puzzle(input, recursive: true);
}