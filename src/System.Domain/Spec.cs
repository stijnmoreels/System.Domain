using System.Text.RegularExpressions;

namespace System.Domain
{
    /// <summary>
    /// Representation of the model validation.
    /// </summary>
    public static class Spec
    {
        /// <summary>
        /// Determines whether the specified x is true.
        /// </summary>
        /// <param name="x">if set to <c>true</c> [x].</param>
        /// <returns>
        ///   <c>true</c> if the specified x is true; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTrue(bool x)
        {
            return x;
        }

        /// <summary>
        /// Determines whether the specified x is false.
        /// </summary>
        /// <param name="x">if set to <c>true</c> [x].</param>
        /// <returns>
        ///   <c>true</c> if the specified x is false; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFalse(bool x)
        {
            return !x;
        }

        public static bool NotNull<T>(T x)
        {
            return x != null;
        }

        /// <summary>
        /// Determines whether the specified x matches a specifieid pattern.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public static bool Matches(this string x, string pattern)
        {
            return new Regex(pattern, RegexOptions.Compiled).IsMatch(x);
        }

        /// <summary>
        /// Determines whether the specified x is greater than the specified minimum and smaller than the specified maximum inclusive (min &lt;= x &lt;= max).
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public static bool InclusiveBetween(this int x, int min, int max)
        {
            return min <= x && x <= max;
        }

        /// <summary>
        /// Determines whether the specified x is greater than the specified minimum and smaller than the specified maximum exclusive (min &lt; x &lt; max).
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns></returns>
        public static bool ExclusiveBetween(this int x, int min, int max)
        {
            return min < x && x < max;
        }

        /// <summary>
        /// Boolean AND operator.
        /// </summary>
        /// <param name="x">if set to <c>true</c> [x].</param>
        /// <param name="y">if set to <c>true</c> [y].</param>
        /// <returns></returns>
        public static bool And(this bool x, bool y)
        {
            return x && y;
        }

        /// <summary>
        /// Boolean AND operator with lazy right value.
        /// </summary>
        /// <param name="x">if set to <c>true</c> [x].</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static bool And(this bool x, Func<bool> y)
        {
            return x && y();
        }

        /// <summary>
        /// Boolean OR operator.
        /// </summary>
        /// <param name="x">if set to <c>true</c> [x].</param>
        /// <param name="y">if set to <c>true</c> [y].</param>
        /// <returns></returns>
        public static bool Or(this bool x, bool y)
        {
            return x || y;
        }

        /// <summary>
        /// Boolean OR operator with lazy right value.
        /// </summary>
        /// <param name="x">if set to <c>true</c> [x].</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public static bool Or(this bool x, Func<bool> y)
        {
            return x || y();
        }
    }
}
