
using static System.Linq.Enumerable;
using static AdventOfCode.AdventOfCode2021.Extensions;
using static AdventOfCode.AdventOfCode2021.Option;

namespace AdventOfCode.AdventOfCode2021;

static class Day17
{
    public record Range(int Min, int Max);
    public record TargetArea(Range X, Range Y);

    public static TargetArea LoadData(string input)
    {
        var parts = input.Substring("target area: ".Length).Split(new[] { "," }, StringSplitOptions.TrimEntries);
        var xParts = parts[0].Substring("x=".Length).Split("..").Select(Int32.Parse).ToArray();
        var yParts = parts[1].Substring("y=".Length).Split("..").Select(Int32.Parse).ToArray();
        return new(new(xParts[0], xParts[1]), new(yParts[1], yParts[0]));
    }


    static IEnumerable<int> Sums() => Range(1, int.MaxValue).Scan(0, (s, v) => s + v);

    static IEnumerable<(int, int)> Shoot((int, int) velocity) =>
        Unfold(
            (velocity, (0, 0)),
            state =>
            {
                var ((xv, yv), (x, y)) = state;
                var position = (x + xv, y + yv);
                var velocity2 = ((xv == 0 ? 0 : xv - 1), yv - 1);
                return Some((position, (velocity2, position)));
            }
        );

    record ShootingResult;
    record Miss(int X, int Y) : ShootingResult;
    record Hit(int X, int Y) : ShootingResult;

    static ShootingResult ShootTarget((int, int) velocity, TargetArea ta) =>
        Shoot(velocity).Pick(point =>
            point switch
            {
                var (x, y) when x >= ta.X.Min && x <= ta.X.Max && y <= ta.Y.Min && y >= ta.Y.Max => Some<ShootingResult>(new Hit(x, y)),
                var (x, y) when x > ta.X.Max || y < ta.Y.Max => Some<ShootingResult>(new Miss(x, y)),
                _ => None<ShootingResult>()
            });


    static int FindMinXVelocity(int minX) => Sums().TakeWhile(s => s < minX).Count();

    public static string Puzzle1(string input)
    {
        var data = LoadData(input);
        var result = Sums().ElementAt(Math.Abs(data.Y.Max) - 1);
        return result.ToString();
    }

    public static string Puzzle2(string input)
    {
        var data = LoadData(input);
        var xs = RangeFromTo(FindMinXVelocity(data.X.Min), data.X.Max);
        var ys = RangeFromTo(data.Y.Max, Math.Abs(data.Y.Max) - 1).ToArray();

        var result = xs
            .SelectMany(x => ys, (x, y) => (x, y))
            .Select(position => ShootTarget(position, data))
            .Count(r => r is Hit);

        return result.ToString();
    }
}