namespace AdventOfCode.AdventOfCode2021;

static class Day2
{
    public record Direction;
    public record Forward(int Value) : Direction;
    public record Down(int Value) : Direction;
    public record Up(int Value) : Direction;

    public static IEnumerable<Direction> LoadData(string input) =>
         input
            .Split(Environment.NewLine)
            .Select(line =>
            {
                var parts = line.Split(' ');
                var value = Int32.Parse(parts[1]);
                return parts[0] switch
                {
                    "forward" => new Forward(value) as Direction,
                    "down" => new Down(value),
                    "up" => new Up(value),
                    var unknownDirection => throw new Exception($"Unknown direction '{unknownDirection}' found in data")
                };
            });


    record Position1(int Horizontal, int Depth);


    public static string Puzzle1(string input)
    {
        var directions = LoadData(input);
        var finalPosition = directions.Aggregate(
           new Position1(0, 0),
           (state, direction) =>
               direction switch
               {
                   Forward(var value) => state with { Horizontal = state.Horizontal + value },
                   Down(var value) => state with { Depth = state.Depth + value },
                   Up(var value) => state with { Depth = state.Depth - value },
                   _ => throw new Exception($"Unknown direction")
               });
        var result = finalPosition.Horizontal * finalPosition.Depth;
        return result.ToString();
    }

    record Position2(int Horizontal, int Depth, int Aim);

    public static string Puzzle2(string input)
    {
        var directions = LoadData(input);
        var finalPosition = directions.Aggregate(
           new Position2(0, 0, 0),
           (state, direction) =>
               direction switch
               {
                   Forward(var value) => state with
                   {
                       Horizontal = state.Horizontal + value,
                       Depth = state.Depth + (state.Aim * value)
                   },
                   Down(var value) => state with { Aim = state.Aim + value },
                   Up(var value) => state with { Aim = state.Aim - value },
                   _ => throw new Exception($"Unknown direction")
               });
        var result = finalPosition.Horizontal * finalPosition.Depth;
        return result.ToString();
    }
}