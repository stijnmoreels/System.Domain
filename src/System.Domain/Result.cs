using System.Collections.Generic;
using System.Functional.Monads;
using System.Linq;

namespace System.Domain
{
    /// <summary>
    /// Result type that can be either be successful or failure.
    /// </summary>
    /// <typeparam name="TOk">The type of the ok.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    public class Result<TOk, TError> : IEquatable<Result<TOk, TError>>
    {
        private readonly TOk _ok;
        private readonly TError _error;

        /// <summary>
        /// Gets a value indicating whether this instance is ok.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is ok; otherwise, <c>false</c>.
        /// </value>
        public bool IsOk { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is error; otherwise, <c>false</c>.
        /// </value>
        public bool IsError { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TOk, TError}" /> class.
        /// </summary>
        /// <param name="ok">The ok.</param>
        private Result(TOk ok)
        {
            if (ok == null)
            {
                throw new ArgumentNullException(nameof(ok));
            }

            IsOk = true;
            IsError = false;

            _ok = ok;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{TOk, TError}" /> class.
        /// </summary>
        /// <param name="error">The error.</param>
        private Result(TError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            IsOk = false;
            IsError = true;

            _error = error;
        }

        /// <summary>
        /// Creates an 'Ok' <see cref="Result{TOk,TError}"/>.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Result<TOk, TError> Ok(TOk x)
        {
            return new Result<TOk, TError>(x);
        }

        /// <summary>
        /// Creates an 'Error' <see cref="Result{TOk,TError}"/>.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Result<TOk, TError> Error(TError x)
        {
            return new Result<TOk, TError>(x);
        }

        /// <summary>
        /// Projects the 'Ok' value to another value.
        /// </summary>
        /// <typeparam name="TOther">The type of the other.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Result<TOther, TError> Select<TOther>(Func<TOk, TOther> f)
        {
            return IsOk ? new Result<TOther, TError>(f(_ok)) : new Result<TOther, TError>(_error);
        }

        /// <summary>
        /// Projects the 'Error' value to another value.
        /// </summary>
        /// <typeparam name="TOther">The type of the other.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Result<TOk, TOther> SelectError<TOther>(Func<TError, TOther> f)
        {
            return IsError ? new Result<TOk, TOther>(f(_error)) : new Result<TOk, TOther>(_ok);
        }

        /// <summary>
        /// Binds the 'Ok' value to another value.
        /// </summary>
        /// <typeparam name="TOther">The type of the other.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Result<TOther, TError> SelectMany<TOther>(Func<TOk, Result<TOther, TError>> f)
        {
            return IsOk ? f(_ok) : new Result<TOther, TError>(_error);
        }

        /// <summary>
        /// Binds the 'Error' value to anotehr value.
        /// </summary>
        /// <typeparam name="TOther">The type of the other.</typeparam>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Result<TOk, TOther> SelectManyError<TOther>(Func<TError, Result<TOk, TOther>> f)
        {
            return IsError ? f(_error) : new Result<TOk, TOther>(_ok);
        }

        /// <summary>
        /// Runs a dead-end function on the 'Ok' value.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Result<TOk, TError> Do(Action<TOk> f)
        {
            if (IsOk) f(_ok);
            return this;
        }

        /// <summary>
        /// Runs a dead-end function on the 'Error' value.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Result<TOk, TError> DoError(Action<TError> f)
        {
            if (IsError) f(_error);
            return this;
        }

        /// <summary>
        /// Aggegrates on the 'Ok' value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="seed">The seed.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public TResult Aggegrate<TResult>(TResult seed, Func<TResult, TOk, TResult> f)
        {
            return IsOk ? f(seed, _ok) : seed;
        }

        /// <summary>
        /// Aggegrates on the 'Error' value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="seed">The seed.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public TResult AggegrateError<TResult>(TResult seed, Func<TResult, TError, TResult> f)
        {
            return IsError ? f(seed, _error) : seed;
        }

        /// <summary>
        /// Zips the 'Ok' values of both instances while discarding the 'Error' side of the other instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TOtherOk">The type of the other ok.</typeparam>
        /// <typeparam name="TOtherError">The type of the other error.</typeparam>
        /// <param name="y">The y.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Result<TResult, TError> Zip<TResult, TOtherOk, TOtherError>(
            Result<TOtherOk, TOtherError> y,
            Func<TOk, TOtherOk, TResult> f)
        {
            return IsOk && y.IsOk 
                ? new Result<TResult, TError>(f(_ok, y._ok)) 
                : new Result<TResult, TError>(_error);
        }

        /// <summary>
        /// Zips the 'Error' sides of both instances while discarding the 'Ok' side of the other instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TOtherOk">The type of the other ok.</typeparam>
        /// <typeparam name="TOtherError">The type of the other error.</typeparam>
        /// <param name="y">The y.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public Result<TOk, TResult> ZipError<TResult, TOtherOk, TOtherError>(
            Result<TOtherOk, TOtherError> y,
            Func<TError, TOtherError, TResult> f)
        {
            return IsError && y.IsError
                ? new Result<TOk, TResult>(f(_error, y._error))
                : new Result<TOk, TResult>(_ok);
        }

        /// <summary>
        /// Joins the 'Ok' values of both instances if both 'Ok' values are equal.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TOtherOk">The type of the other ok.</typeparam>
        /// <typeparam name="TOtherError">The type of the other error.</typeparam>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="y">The y.</param>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <param name="h">The h.</param>
        /// <returns></returns>
        public Result<TResult, TError> Join<TResult, TOtherOk, TOtherError, TProp>(
            Result<TOtherOk, TOtherError> y,
            Func<TOk, TProp> f,
            Func<TOtherOk, TProp> g,
            Func<TOk, TOtherOk, TResult> h)
        {
            return IsOk && y.IsOk && EqualityComparer<TProp>.Default.Equals(f(_ok), g(y._ok))
                ? new Result<TResult, TError>(h(_ok, y._ok))
                : new Result<TResult, TError>(_error);
        }

        /// <summary>
        /// Joins the 'Error' values of both instances if both 'Error' values are equal.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TOtherOk">The type of the other ok.</typeparam>
        /// <typeparam name="TOtherError">The type of the other error.</typeparam>
        /// <typeparam name="TProp">The type of the property.</typeparam>
        /// <param name="y">The y.</param>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <param name="h">The h.</param>
        /// <returns></returns>
        public Result<TOk, TResult> JoinError<TResult, TOtherOk, TOtherError, TProp>(
            Result<TOtherOk, TOtherError> y,
            Func<TError, TProp> f,
            Func<TOtherError, TProp> g,
            Func<TError, TOtherError, TResult> h)
        {
            return IsError && y.IsError && EqualityComparer<TProp>.Default.Equals(f(_error), g(y._error))
                ? new Result<TOk, TResult>(h(_error, y._error))
                : new Result<TOk, TResult>(_ok);
        }

        /// <summary>
        /// Casts the 'Ok' value to another sub-type.
        /// </summary>
        /// <typeparam name="TOther">The type of the other.</typeparam>
        /// <returns></returns>
        public Result<TOther, TError> Cast<TOther>() where TOther : TOk
        {
            return IsOk ? new Result<TOther, TError>((TOther) _ok) : new Result<TOther, TError>(_error);
        }

        /// <summary>
        /// Casts the 'Error' value to another sub-type.
        /// </summary>
        /// <typeparam name="TOther">The type of the other.</typeparam>
        /// <returns></returns>
        public Result<TOk, TOther> CastError<TOther>() where TOther : TError
        {
            return IsError ? new Result<TOk, TOther>((TOther) _error) : new Result<TOk, TOther>(_ok);
        }

        /// <summary>
        /// Unsafe way to getting the 'Ok' value.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The 'Result' is an 'Error' with value: " + _error</exception>
        public TOk UnsafeGetOk()
        {
            return IsOk ? _ok : throw new InvalidOperationException("The 'Result' is an 'Error' with value: " + _error);
        }

        /// <summary>
        /// Unsafe way of getting the 'Error' value.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">The 'Result' is an' Ok' with value: " + _ok</exception>
        public TError UnsafeGetError()
        {
            return IsError ? _error : throw new InvalidOperationException("The 'Result' is an' Ok' with value: " + _ok);
        }

        /// <summary>
        /// Gets the 'Ok' value.
        /// </summary>
        /// <returns></returns>
        public Maybe<TOk> GetOk()
        {
            return IsOk ? Maybe.Just(_ok) : Maybe.Nothing<TOk>();
        }

        /// <summary>
        /// Gets the 'Error' value.
        /// </summary>
        /// <returns></returns>
        public Maybe<TError> GetError()
        {
            return IsError ? Maybe.Just(_error) : Maybe.Nothing<TError>();
        }

        /// <summary>
        /// Gets the 'Ok' or a given default.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public TOk GetOkOrElse(Func<TOk> f)
        {
            return IsOk ? _ok : f();
        }

        /// <summary>
        /// Gets the 'Error' or a given default.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public TError GetErrorOrElse(Func<TError> f)
        {
            return IsError ? _error : f();
        }

        /// <summary>
        /// Gets the 'Ok' or 'Error' value.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="f">The f.</param>
        /// <param name="g">The g.</param>
        /// <returns></returns>
        public TResult GetOkOrError<TResult>(Func<TOk, TResult> f, Func<TError, TResult> g)
        {
            return IsOk ? f(_ok) : g(_error);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.</returns>
        public bool Equals(Result<TOk, TError> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (IsOk  && other.IsOk) return EqualityComparer<TOk>.Default.Equals(_ok, other._ok);
            if (IsError && other.IsError) return EqualityComparer<TError>.Default.Equals(_error, other._error);

            return false;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return Equals((Either<TOk, TError>)obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return IsOk ? _ok.GetHashCode() : _error.GetHashCode();
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Result<TOk, TError> x, Result<TOk, TError> y)
        {
            return !(x is null) && x.Equals(y);
        }

        /// <summary>
        /// Indicates whether the current object is not equal to another object of the same type.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Result<TOk, TError> x, Result<TOk, TError> y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return IsOk
                ? $"Ok<{typeof(TOk).Name}>: "   + _ok
                : $"Error<{typeof(TError).Name}>: " + _error;
        }
    }

    /// <summary>
    /// Static helpers for type hiding.
    /// </summary>
    public static class Result
    {
        /// <summary>
        /// Creates an 'Ok' <see cref="Result{TOk,TError}"/>.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Result<TOk, TError> Ok<TOk, TError>(TOk x)
        {
            return Result<TOk, TError>.Ok(x);
        }

        /// <summary>
        /// Creates an 'Ok' <see cref="Result{TOk,TError}"/>.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Result<TOk, object> Ok<TOk>(TOk x)
        {
            return Result<TOk, object>.Ok(x);
        }

        /// <summary>
        /// Creates an 'Error' <see cref="Result{TOk,TError}"/>.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Result<TOk, TError> Error<TOk, TError>(TError x)
        {
            return Result<TOk, TError>.Error(x);
        }

        /// <summary>
        /// Creates an 'Error' <see cref="Result{TOk,TError}"/>.
        /// </summary>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Result<object, TError> Error<TError>(TError x)
        {
            return Result<object, TError>.Error(x);
        }

        /// <summary>
        /// Wraps the value in as a 'Ok' result.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static Result<TOk, TError> AsResult<TOk, TError>(this TOk x)
        {
            return Result<TOk, TError>.Ok(x);
        }

        /// <summary>
        /// Wraps the value as a 'Error' result.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="err">The error.</param>
        /// <returns></returns>
        public static Result<TOk, TError> AsResult<TOk, TError>(this TError err)
        {
            return Result<TOk, TError>.Error(err);
        }

        /// <summary>
        /// Applies the specified x result to the wrapped function.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fResult">The f result.</param>
        /// <param name="xResult">The x result.</param>
        /// <returns></returns>
        public static Result<TResult, IEnumerable<TError>> Apply<TOk, TError, TResult>(
            this Result<Func<TOk, TResult>, TError> fResult,
            Result<TOk, TError> xResult)
        {
            return Apply(fResult.SelectError(AsEnumerable), xResult.SelectError(AsEnumerable));
        }

        /// <summary>
        /// Applies the specified x result to the wrapped function.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fResult">The f result.</param>
        /// <param name="xResult">The x result.</param>
        /// <returns></returns>
        public static Result<TResult, IEnumerable<TError>> Apply<TOk, TError, TResult>(
            this Result<Func<TOk, TResult>, IEnumerable<TError>> fResult,
            Result<TOk, TError> xResult)
        {
            return Apply(fResult, xResult.SelectError(AsEnumerable));
        }

        /// <summary>
        /// Applies the specified x result.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fResult">The f result.</param>
        /// <param name="xResult">The x result.</param>
        /// <returns></returns>
        public static Result<TResult, IEnumerable<TError>> Apply<TOk, TError, TResult>(
            this Result<Func<TOk, TResult>, TError> fResult,
            Result<TOk, IEnumerable<TError>> xResult)
        {
            return Apply(fResult.SelectError(AsEnumerable), xResult);

        }

        /// <summary>
        /// Applies the specified x result.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="fResult">The f result.</param>
        /// <param name="xResult">The x result.</param>
        /// <returns></returns>
        public static Result<TResult, IEnumerable<TError>> Apply<TOk, TError, TResult>(
            this Result<Func<TOk, TResult>, IEnumerable<TError>> fResult,
            Result<TOk, IEnumerable<TError>> xResult)
        {
            if (fResult.IsOk && xResult.IsOk)
            {
                return fResult.SelectMany(xResult.Select);
            }

            if (fResult.IsError && xResult.IsError)
            {
                return fResult
                       .UnsafeGetError()
                       .Concat(xResult.UnsafeGetError())
                       .AsResult<TResult, IEnumerable<TError>>();
            }

            if (fResult.IsError && xResult.IsOk)
            {
                return fResult
                       .UnsafeGetError()
                       .AsResult<TResult, IEnumerable<TError>>();
            }

            return xResult
                   .UnsafeGetError()
                   .AsResult<TResult, IEnumerable<TError>>();
        }

        private static IEnumerable<TError> AsEnumerable<TError>(TError err)
        {
            return new[] { err };
        }

        /// <summary>
        /// Flattens the 'Ok' value.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        public static Result<TOk, TError> Flatten<TOk, TError>(this Result<Result<TOk, TError>, TError> m)
        {
            return m.IsOk ? m.UnsafeGetOk() : Result<TOk, TError>.Error(m.UnsafeGetError());
        }

        /// <summary>
        /// Flatttens the 'Error' value.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        public static Result<TOk, TError> FlatttenError<TOk, TError>(this Result<TOk, Result<TOk, TError>> m)
        {
            return m.IsError ? m.UnsafeGetError() : Result<TOk, TError>.Ok(m.UnsafeGetOk());
        }

        /// <summary>
        /// Tries the specified function and wraps the result.
        /// </summary>
        /// <typeparam name="TOk">The type of the ok.</typeparam>
        /// <typeparam name="TError">The type of the error.</typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">The x.</param>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static Result<TOk, TError> Try<TOk, TError, T>(this T x, Func<T, TOk> f) where TError : Exception
        {
            try
            {
                return Result<TOk, TError>.Ok(f(x));
            }
            catch (TError e)
            {
                return Result<TOk, TError>.Error(e);
            }
        }
    }
}
