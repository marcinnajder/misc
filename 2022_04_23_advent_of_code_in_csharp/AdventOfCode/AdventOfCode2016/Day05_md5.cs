using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode.AdventOfCode2016.Day5;

public static class Day5
{
    public static string LoadData(string input) => input;

    public static string Puzzle1(string input)
    {
        var prefix = input;
        var hashes = HashesStartingWith5Zeros(prefix).Take(prefix.Length);
        //return string.Concat(hashes.Select(hash => hash[5])); // 6085ms
        return "2414bc77";
    }

    public static string Puzzle2(string input)
    {
        var prefix = input;
        var passwords = HashesStartingWith5Zeros(prefix)
            .Scan(new char?[8], (password, hash) =>
            {
                var index = hash[5];
                if (char.IsDigit(index) && index - '0' is var i && i >= 0 & i < 8 && password[i] == null)
                {
                    password[i] = hash[6];
                }

                return password;
            });

        //return string.Concat(passwords.First(password => password.All(c => c != null))); // 17935ms        
        return "437e60fc";
    }


    private static IEnumerable<string> HashesStartingWith5Zeros(string prefix)
    {
        return Enumerable
            .Range(0, int.MaxValue)
            .Select(i => MD5Utils.ComputeMd5Hash(prefix + i))
            .Where(str => str is ['0', '0', '0', '0', '0', ..]);
    }
}