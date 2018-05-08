using System.Collections.Generic;
using System.Functional.Monads;

namespace System.Domain
{
    /// <summary>
    /// Represents a wrapped value that's still untrusted.
    /// The value can be unwrapped by calling the <see cref="Unwrap{TResult}"/> or <see cref="Unwrap{TResult,TError}"/> functions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Untrusted<T> : IEquatable<Untrusted<T>>
    {
        private readonly T _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Untrusted{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        private Untrusted(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Wraps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Untrusted<T> Wrap(T value)
        {
            return new Untrusted<T>(value);
        }

        /// <summary>
        /// Unwraps the specified value with the given validation function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="validate">The validate.</param>
        /// <returns></returns>
        public Maybe<TResult> Unwrap<TResult>(Func<T, Maybe<TResult>> validate)
        {
            return validate(_value);
        }

        /// <summary>
        /// Unwraps the specified value with the given validation function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="validate">The validate.</param>
        /// <returns></returns>
        public Result<TResult, TError> Unwrap<TResult, TError>(Func<T, Result<TResult, TError>> validate)
        {
            return validate(_value);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Untrusted<T> other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return EqualityComparer<T>.Default.Equals(_value, other._value);
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

            return Equals((Untrusted<T>) obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(_value);
        }
    }
}
