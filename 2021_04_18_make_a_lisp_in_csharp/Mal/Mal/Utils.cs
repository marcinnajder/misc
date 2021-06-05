using System.Linq;
using PowerFP;
using static Mal.Types;

namespace Mal
{
    internal static class Utils
    {
        internal static string Join<T>(this LList<T>? items, string separator = ", ") =>
            string.Join(separator, items.ToEnumerable());

        internal static string JoinMalTypes(this LList<MalType>? items, string separator = ", ") =>
            items.Select(mal => Printer.PrintStr(mal)).Join(separator);
    }
}