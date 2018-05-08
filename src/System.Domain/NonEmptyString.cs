using System.Functional.Monads;

namespace System.Domain
{
    /// <summary>
    /// Representation of a non-empty string.
    /// </summary>
    public class NonEmptyString : IEquatable<NonEmptyString>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonEmptyString"/> class.
        /// </summary>
        /// <param name="txt">The text.</param>
        private NonEmptyString(string txt)
        {
            Text = txt;
        }

        /// <summary>
        /// Gets the wrapped-text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; }

        /// <summary>
        /// Creates a <see cref="NonEmptyString"/> based on the given <paramref name="txt"/>.
        /// </summary>
        /// <param name="txt">The text.</param>
        /// <returns></returns>
        public static Maybe<NonEmptyString> Maybe(string txt)
        {
            return string.IsNullOrEmpty(txt)
                ? Maybe<NonEmptyString>.Nothing
                : Maybe<NonEmptyString>.Just(new NonEmptyString(txt));
        }

        /// <summary>
        /// Creates a <see cref="NonEmptyString" /> based on the given <paramref name="txt" />.
        /// </summary>
        /// <param name="txt">The text.</param>
        /// <param name="name">The prefix.</param>
        /// <returns></returns>
        public static Result<NonEmptyString, string> Result(string txt, string name = "")
        {
            return String.IsNullOrEmpty(txt)
                ? Result<NonEmptyString, string>.Error($"The '{name}' String is null or empty! Please provide a non-empty string.")
                : Result<NonEmptyString, string>.Ok(new NonEmptyString(txt));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(NonEmptyString other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return string.Equals(Text, other.Text);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((NonEmptyString) obj);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(NonEmptyString x, NonEmptyString y)
        {
            return x != null && x.Equals(y);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(NonEmptyString x, NonEmptyString y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return "NonEmptySring: " + Text;
        }
    }
}
