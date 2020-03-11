using System.Linq;

namespace ConsoleProgressBar
{
    internal static class Extensions
    {
        public static string Repeat(this string value, int count) => string.Concat(Enumerable.Repeat(value, count));
    }
}
