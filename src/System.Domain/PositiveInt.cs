using System.Functional.Monads;

namespace System.Domain
{
    /// <summary>
    /// Integer representation for which (i >= 0).
    /// </summary>
    public struct PositiveInt : IEquatable<PositiveInt>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositiveInt"/> class.
        /// </summary>
        /// <param name="i">The i.</param>
        private PositiveInt(int i)
        {
            Number = i;
        }

        /// <summary>
        /// Gets the wrapped positive integer value.
        /// </summary>
        /// <value>The value.</value>
        public int Number { get; }

        /// <summary>
        /// Creates an integer for which (i &gt;= 0).
        /// </summary>
        /// <param name="i">The to-be-wrapped integer.</param>
        /// <returns></returns>
        public static Maybe<PositiveInt> Maybe(int i)
        {
            return i >= 0
                ? Functional.Monads.Maybe.Just(new PositiveInt(i))
                : Functional.Monads.Maybe.Nothing<PositiveInt>();
        }

        /// <summary>
        /// Creates an integer for which (i &gt;= 0).
        /// </summary>
        /// <param name="i">The to-be-wrapped integer.</param>
        /// <param name="name">The prefix.</param>
        /// <returns></returns>
        public static Result<PositiveInt, string> Result(int i, string name = "")
        {
            return i >= 0
                ? Domain.Result.Ok<PositiveInt, string>(new PositiveInt(i))
                : Domain.Result.Error<PositiveInt, string>(
                        $"The '{name}' Value: {i}, is less then zero! Needs a value greater (or equal) to zero.");
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>The fully qualified type name.</returns>
        public override string ToString()
        {
            return "PositiveInt: " + Number;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(PositiveInt other)
        {
            return Number == other.Number;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>true if <paramref name="obj">obj</paramref> and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is PositiveInt pi && Equals(pi);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(PositiveInt x, PositiveInt y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(PositiveInt x, PositiveInt y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return Number;
        }
    }
}
