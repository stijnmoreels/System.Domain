using System.Collections;
using System.Collections.Generic;
using System.Functional.Monads;
using System.Linq;

namespace System.Domain
{
    /// <summary>
    /// Sequence model representing a non-empty sequence (at least a single element).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
    public class NonEmptySeq<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _xs;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonEmptySeq{T}"/> class.
        /// </summary>
        /// <param name="xs">The xs.</param>
        private NonEmptySeq(IEnumerable<T> xs)
        {
            _xs = xs;
        }

        /// <summary>
        /// Creates a sequence of at least a single element.
        /// </summary>
        /// <param name="xs">The xs.</param>
        /// <returns></returns>
        public static Maybe<NonEmptySeq<T>> Maybe(IEnumerable<T> xs)
        {
            return xs.Any()
                ? Maybe<NonEmptySeq<T>>.Just(new NonEmptySeq<T>(xs)) 
                : Maybe<NonEmptySeq<T>>.Nothing;
        }

        /// <summary>
        /// Creates a sequence of at least a single element.
        /// </summary>
        /// <param name="xs">The xs.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Result<NonEmptySeq<T>, string> Result(IEnumerable<T> xs, string name = "")
        {
            return xs.Any()
                ? Result<NonEmptySeq<T>, string>.Ok(new NonEmptySeq<T>(xs))
                : Result<NonEmptySeq<T>, string>.Error($"The '{name}' Sequence is empty! Please provide a non-empty sequence.");
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _xs.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"NonEmptySeq<{typeof(T).Name}> [ {String.Join(", ", _xs)} ]";
        }
    }
}
