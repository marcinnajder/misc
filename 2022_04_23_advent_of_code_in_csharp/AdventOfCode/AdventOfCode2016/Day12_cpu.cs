namespace AdventOfCode.AdventOfCode2016.Day12;

public enum Registry { A, B, C, D }

public interface IValue { }
public record NumberValue(int Number) : IValue;
public record RegistryValue(Registry Registry) : IValue;

public interface IInstruction;
public record Inc(Registry Registry) : IInstruction;
public record Dec(Registry Registry) : IInstruction;
public record Copy(IValue Value, Registry Registry) : IInstruction;
public record Jump(IValue Value, int Offset) : IInstruction;

public static class Day12
{
    private static Registry ParseRegistry(string text) => Enum.Parse<Registry>(text, true);

    private static IValue ParseValue(string text) =>
        int.TryParse(text, out var number) ? new NumberValue(number) : new RegistryValue(ParseRegistry(text));

    private static int ExtractValue(IValue value, Dictionary<Registry, int> registries) =>
        value switch
        {
            NumberValue(var number) => number,
            RegistryValue(var registry) => registries[registry],
            _ => throw new Exception("Unknown type of value.")
        };


    public static IEnumerable<IInstruction> LoadData(string input)
    {
        var factories = new Dictionary<string, Func<string, IInstruction>>
        {
            {"inc", args => new Inc(ParseRegistry(args))},
            {"dec", args => new Dec(ParseRegistry(args))},
            {"cpy", args => args.Split(' ').Pipe(parts => new Copy(ParseValue(parts[0]), ParseRegistry(parts[1]))) },
            {"jnz", args => args.Split(' ').Pipe(parts => new Jump(ParseValue(parts[0]), int.Parse(parts[1]))) },
        };

        var lines = input.Split(Environment.NewLine);

        foreach (var line in lines)
        {
            var (prefix, factory) = factories.First(kv => line.StartsWith(kv.Key));
            var instruction = factory(line.Substring(prefix.Length).Trim());
            yield return instruction;
        }
    }


    public static string Puzzle(string input, int cInit)
    {
        var instructions = LoadData(input).ToList();
        var registries = Enum.GetValues<Registry>().ToDictionary(r => r, r => r == Registry.C ? cInit : 0);
        var index = 0;

        while (true)
        {
            var instruction = instructions[index];
            switch (instruction)
            {
                case Inc inc:
                    registries[inc.Registry]++;
                    index++;
                    break;
                case Dec dec:
                    registries[dec.Registry]--;
                    index++;
                    break;
                case Copy copy:
                    registries[copy.Registry] = ExtractValue(copy.Value, registries);
                    index++;
                    break;
                case Jump jump:
                    var value = ExtractValue(jump.Value, registries);
                    if (value != 0)
                    {
                        index += jump.Offset;
                    }
                    else
                    {
                        index++;
                    }
                    break;
            }

            if (index < 0 || index >= instructions.Count) // out of bounds
            {
                return registries[Registry.A].ToString();
            }
        }
    }
    public static string Puzzle1(string input) => Puzzle(input, 0);

    public static string Puzzle2(string input) => Puzzle(input, 1);
}