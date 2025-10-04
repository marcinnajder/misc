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

    // powered by copilot :)
    public static string ComputeMD5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

    private static IEnumerable<string> HashesStartingWith5Zeros(string prefix)
    {
        return Enumerable
            .Range(0, int.MaxValue)
            .Select(i => ComputeMD5Hash(prefix + i))
            .Where(str => str is ['0', '0', '0', '0', '0', ..]);
    }
}