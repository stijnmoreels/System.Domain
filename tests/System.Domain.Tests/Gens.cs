using System.Functional;
using System.Functional.Monads;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.FSharp.Core;

namespace System.Domain.Tests
{
    public static class Gens
    {
        public static Arbitrary<Maybe<T>> Maybe<T>()
        {
            return Arb.Default.Option<T>()
               .Generator
               .Select(o =>
                    Equals(o, FSharpOption<T>.None)
                        ? Functional.Monads.Maybe.Nothing<T>()
                        : Functional.Monads.Maybe.Just(o.Value)).ToArbitrary();
        }

        public static Arbitrary<Result<TOk, TError>> Result<TOk, TError>()
        {
            Gen<Result<TOk, TError>> ok = Arb
                .From<TOk>()
                .Generator
                .Select(Domain.Result<TOk, TError>.Ok);


            Gen<Result<TOk, TError>> error = Arb
                .From<TError>()
                .Generator
                .Select(Domain.Result<TOk, TError>.Error);

            return Gen.Frequency(
                Tuple.Create(1, ok), 
                Tuple.Create(1, error))
                      .ToArbitrary();
        }

        public static Arbitrary<NonEmptyString> NonEmptyString()
        {
            return Arb.Default
                .NonEmptyString()
                .Generator
                .Select(s => Domain.NonEmptyString.Result(s.Get).UnsafeGetOk())
                .ToArbitrary();
        }

        public static Arbitrary<PositiveInt> PositiveInt()
        {
            return Arb.Default
                .PositiveInt()
                .Generator
                .Select(i => Domain.PositiveInt.Result(i.Get).UnsafeGetOk())
                .ToArbitrary();
        }

        public static Arbitrary<NonEmptySeq<T>> NonEmptySeq<T>()
        {
            return Arb.Default
                .NonEmptyArray<T>()
                .Generator
                .Select(a => Domain.NonEmptySeq<T>.Result(a.Get).UnsafeGetOk())
                .ToArbitrary();
        }
    }

    public class CustomProperty : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomProperty"/> class.
        /// </summary>
        public CustomProperty()
        {
            Arbitrary = new[] { typeof(Gens) };
        }
    }
}
