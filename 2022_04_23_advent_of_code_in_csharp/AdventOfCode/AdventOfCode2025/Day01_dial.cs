using static System.Math;

namespace AdventOfCode.AdventOfCode2025.Day1;

public enum RotationDirection
{
    Left,
    Right
}

public record Rotation(RotationDirection Dir, int Value);

public static class Day1
{
    public static IEnumerable<Rotation> LoadData(string input)
    {
        return input
            .Split(Environment.NewLine, StringSplitOptions.TrimEntries)
            .Select(t => new Rotation(t[0] == 'R' ? RotationDirection.Right : RotationDirection.Left,
                int.Parse(t.Substring(1))));
    }

    public static string Puzzle1(string input)
    {
        var rotations = LoadData(input);
        var zeros = rotations
            .Scan(50, (value, rotation) => rotation.Dir == RotationDirection.Right
                ? (value + rotation.Value) % 100
                : (value - rotation.Value).Pipe(v => v >= 0 ? v : (100 + v) % 100))
            .Count(value => value == 0);

        return zeros.ToString();
    }

    public static string Puzzle2(string input)
    {
        var rotations = LoadData(input).ToArray();
        var zerosCount = rotations
            .Scan((Clicks: 0, Value: 50), (state, rotation) =>
            {
                return rotation.Dir == RotationDirection.Right
                    ? (state.Value + rotation.Value).Pipe(v => DivRem(v, 100))
                    : (state.Value - rotation.Value).Pipe(v =>
                    {
                        switch (v)
                        {
                            case > 0: return (0, v); // without crossing 0
                            case 0: return (1, v); // exactly value 0
                            default: // with crossing 0
                                var (quotient, rem) = DivRem(v, 100);
                                return (Abs(quotient) + (state.Value > 0 ? 1 : 0), rem == 0 ? 0 : rem + 100);
                        }
                    });
            })
            .Sum(state => state.Clicks);

        return zerosCount.ToString();
    }
}