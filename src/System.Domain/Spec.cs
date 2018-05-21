using System.Text.RegularExpressions;

namespace System.Domain
{
    public static class Spec
    {
        public static bool IsTrue(bool x)
        {
            return x;
        }

        public static bool IsFalse(bool x)
        {
            return !x;
        }

        public static bool NotNull<T>(T x)
        {
            return x != null;
        }

        public static bool Matches(this string x, string pattern)
        {
            return new Regex(pattern, RegexOptions.Compiled).IsMatch(x);
        }

        public static bool InclusiveBetween(this int x, int min, int max)
        {
            return min <= x && x >= max;
        }

        public static bool ExclusiveBetween(this int x, int min, int max)
        {
            return min < x && x > max;
        }

        public static bool And(this bool x, bool y)
        {
            return x && y;
        }

        public static bool And(this bool x, Func<bool> y)
        {
            return x && y();
        }

        public static bool Or(this bool x, bool y)
        {
            return x || y;
        }
    }
}
