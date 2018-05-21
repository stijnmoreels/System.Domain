using System.Functional.Monads;
using System.Threading;

namespace System.Domain
{
    /// <summary>
    /// Representation of a value that can only be read-once.
    /// </summary>
    public static class ReadOnce
    {
        /// <summary>
        /// Wraps the read-once value into a <see cref="Maybe{T}"/> instance, 
        /// returning <see cref="Maybe.Just{T}"/> when the value isn't yet been read 
        /// and <see cref="Maybe{T}.Nothing"/> when the value is already been read.
        /// </summary>
        public static Func<Maybe<T>> Wrap<T>(T x)
        {
            Maybe<T> y = Maybe.Just(x);
            return () =>
                y == Maybe<T>.Nothing
                    ? y
                    : Interlocked.Exchange(ref y, Maybe<T>.Nothing);
        }
    }
}