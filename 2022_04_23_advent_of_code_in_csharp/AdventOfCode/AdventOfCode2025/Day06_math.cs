using Line = (long From, long To);

namespace AdventOfCode.AdventOfCode2025.Day6;

public record Problem(List<long> Numbers, (Func<long, long, long> Op, long Init) Operation);

public static class Day6
{
    private static long Add(long a, long b) => a + b;

    private static long Mul(long a, long b) => a * b;

    public static Problem[] LoadData1(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var operators = lines[^1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var numbersCount = lines.Length - 1;
        var problems = Array.ConvertAll(operators,
            o => new Problem(new List<long>(numbersCount), o == "+" ? (Add, 0) : (Mul, 1)));

        for (int r = 0; r < lines.Length - 1; r++)
        {
            var numbers = lines[r].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            for (int c = 0; c < problems.Length; c++)
            {
                problems[c].Numbers.Add(long.Parse(numbers[c]));
            }
        }

        return problems;
    }

    public static string[] Transpose(string[] rows)
    {
        var width = rows.Max(row => row.Length);
        var result = new string[width];
        var buffer = new char[rows.Length];

        for (var c = 0; c < width; c++)
        {
            for (var r = 0; r < rows.Length; r++)
            {
                buffer[r] = c < rows[r].Length ? rows[r][c] : ' '; // fill with spaces
            }

            result[c] = new string(buffer);
        }

        return result;
    }

    public static Problem[] LoadData2(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var operators = lines[^1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        var problems = Array.ConvertAll(operators, o => new Problem(new List<long>(), o == "+" ? (Add, 0) : (Mul, 1)));

        var transposedLines = Transpose(lines);

        var index = 0;
        foreach (var line in transposedLines)
        {
            if (string.IsNullOrEmpty(line.Trim())) // blank line
            {
                index++;
            }
            else
            {
                // var number = line.AggregateBack((Tens: 1, Sum: 0),
                //     (s, c) => char.IsDigit(c) ? (s.Tens * 10, s.Sum + (c - '0') * s.Tens) : s).Sum;

                // var number = Enumerable.Range(1, line.Length).Select(i => line[^i])
                //     .Where(char.IsDigit).Index().Sum(t => Math.Pow(10, t.Index) * (t.Item - '0'));

                var number = long.Parse(string.Concat(line.Where(char.IsDigit)));

                problems[index].Numbers.Add(number);
            }
        }

        return problems;
    }


    public static string Puzzle(string input, Func<string, Problem[]> loadData)
    {
        var problems = loadData(input);
        var result = problems.Sum(p => p.Numbers.Aggregate(p.Operation.Init, p.Operation.Op));
        return result.ToString();
    }

    public static string Puzzle1(string input) => Puzzle(input, LoadData1);

    public static string Puzzle2(string input) => Puzzle(input, LoadData2);
}