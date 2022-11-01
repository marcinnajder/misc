using static MonadsInCSharp.IOOperators;

namespace MonadsInCSharp;

public static class IOSamples
{
    public static void Test()
    {
        IO<Unit> result = ReadLineThenGenerateString("0");
        result.Run();

        // pure function
        static IO<Unit> ReadLineThenGenerateString(string postfix)
        {
            IO<Unit> unit = WriteLine("Podaj liczbe:");
            IO<string> line = unit.SelectMany(_ => ReadLine());
            IO<int> number = line.Select(l => int.Parse(l + postfix));
            IO<Unit> result = number.SelectMany(n => WriteLine(GenerateString(n)));
            return result;
        }

        // pure function
        static string GenerateString(int number) => new string('a', number);
    }
}

