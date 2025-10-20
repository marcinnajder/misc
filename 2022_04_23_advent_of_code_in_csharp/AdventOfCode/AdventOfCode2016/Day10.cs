
using System.Text.RegularExpressions;

namespace AdventOfCode.AdventOfCode2016.Day10;

public interface IInstruction;
public record BotValue(int BotId, int Value) : IInstruction;
public record BotPasses(int BotId, int LowId, bool LowPassedToBot, int HighId, bool HighPassedToBot) : IInstruction;

public static class Day10
{
    public static IEnumerable<IInstruction> LoadData(string input)
    {
        var lines = input.Split(Environment.NewLine);
        var pattern = new Regex(@"\b(bot|output|value)\s+(\d+)"); // extract numbers

        foreach (var line in lines)
        {
            var matches = pattern.Matches(line);

            if (matches.Count == 2) // "value 5 goes to bot 2"
            {
                yield return new BotValue(ParseMatch(matches[1]).Value, ParseMatch(matches[0]).Value);
            }
            else // "bot 1 gives low to output 1 and high to bot 0"
            {
                var (lowType, lowValue) = ParseMatch(matches[1]);
                var (highType, highValue) = ParseMatch(matches[2]);
                yield return new BotPasses(ParseMatch(matches[0]).Value, lowValue, lowType == "bot", highValue, highType == "bot");
            }
        }

        static (string Key, int Value) ParseMatch(Match match)
        {
            string key = match.Groups[1].Value;
            int value = int.Parse(match.Groups[2].Value);
            return (key, value);
        }
    }


    public static IEnumerable<(int BotId, int Low, int High, Dictionary<int, int> Outputs)> Puzzle(string input)
    {
        var bots = new Dictionary<int, int>();
        var passes = new Dictionary<int, BotPasses>();
        var outputs = new Dictionary<int, int>();
        LList<BotValue>? pairedBots = null; // stack

        var instructions = LoadData(input);

        foreach (var ins in instructions)
        {
            switch (ins)
            {
                case BotValue botValue:
                    if (bots.ContainsKey(botValue.BotId))
                    {
                        pairedBots = new(botValue, pairedBots);
                    }
                    else
                    {
                        bots.Add(botValue.BotId, botValue.Value);
                    }
                    break;
                case BotPasses botGivesValues:
                    passes.Add(botGivesValues.BotId, botGivesValues);
                    break;
            }
        }

        while (pairedBots is var ((botId, value1), rest)) // pairedBots is not null
        {
            pairedBots = rest; // pop from the stack

            var value2 = bots[botId];
            var (lowValue, highValue) = value1 < value2 ? (value1, value2) : (value2, value1);

            var pass = passes[botId];
            PassValue(pass.LowId, pass.LowPassedToBot, lowValue);
            PassValue(pass.HighId, pass.HighPassedToBot, highValue);

            bots.Remove(botId);

            yield return (botId, lowValue, highValue, outputs);
        }

        void PassValue(int id, bool passedToBot, int value)
        {
            if (passedToBot)
            {
                if (!bots.TryAdd(id, value))
                {
                    pairedBots = new(new(id, value), pairedBots); // there is a pair of values
                }
            }
            else
            {
                outputs.Add(id, value);
            }
        }
    }

    public static string Puzzle1(string input)
    {
        var result = Puzzle(input).First(r => r.Low == 17 && r.High == 61);
        return result.BotId.ToString();
    }

    public static string Puzzle2(string input)
    {
        var keys = new[] { 0, 1, 2 };
        var result = Puzzle(input).First(r => keys.All(r.Outputs.ContainsKey));
        return keys.Aggregate(1, (total, key) => total * result.Outputs[key]).ToString();
    }
}
