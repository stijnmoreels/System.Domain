using System.Functional;
using FsCheck;
using FsCheck.Xunit;

namespace System.Domain.Tests
{
    public class ResultTests
    {
        [Property]
        public bool Functor_Law_Ok_Holds(int x, Func<int, byte> f, Func<byte, int> g)
        {
            return Result
                   .Ok<int, int>(x)
                   .Select(f)
                   .Select(g)
                   .UnsafeGetOk()
                   .Equals(f.Compose(g)(x));
        }

        [Property]
        public bool Applicative_Law_Ok_Holds(byte x, short y, Func<byte, short, int> f)
        {
            return f.Curry()
                    .AsResult<Func<byte, Func<short, int>>, string>()
                    .Apply(x.AsResult<byte, string>())
                    .Apply(y.AsResult<short, string>())
                    .UnsafeGetOk()
                    .Equals(f(x, y));
        }

        [Property]
        public bool Monadic_Law_Ok_Holds(byte x, Func<byte, short> f)
        {
            return x.AsResult<byte, int>()
                    .SelectMany(f.Compose(Result.Ok<short, int>))
                    .UnsafeGetOk()
                    .Equals(f(x));
        }

        [Property]
        public bool Functor_Law_Error_Holds(int x, Func<int, byte> f, Func<byte, int> g)
        {
            return Result
                   .Error<int, int>(x)
                   .SelectError(f)
                   .SelectError(g)
                   .UnsafeGetError()
                   .Equals(f.Compose(g)(x));
        }

        [Property]
        public bool Monadic_Law_Error_Holds(byte x, Func<byte, short> f)
        {
            return x.AsResult<int, byte>()
                    .SelectManyError(f.Compose(Result.Error<int, short>))
                    .UnsafeGetError()
                    .Equals(f(x));
        }

        [CustomProperty]
        public Property Creating_Person_Only_Works_If_Every_Prop_Is_Valid(
            Result<NonEmptyString, NonEmptyString> nameResult, 
            Result<PositiveInt, NonEmptyString> ageResult, 
            Result<NonEmptySeq<string>, NonEmptyString> addressesResult)
        {
            var personResult = Result
                .Ok(Person.Ctor().Curry())
                .CastError<NonEmptyString>()
                .Apply(nameResult)
                .Apply(ageResult)
                .Apply(addressesResult);

            bool allGood = personResult.IsOk == (nameResult.IsOk && ageResult.IsOk && addressesResult.IsOk);
            bool allFail = personResult.IsError == (nameResult.IsError || ageResult.IsError || addressesResult.IsError);

            return allGood.And(allFail);
        }

        class Person
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Person"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="age">The age.</param>
            /// <param name="addresses">The addresses.</param>
            public Person(NonEmptyString name, PositiveInt age, NonEmptySeq<string> addresses)
            {
                Name = name;
                Age = age;
                Addresses = addresses;
            }

            public NonEmptyString Name { get; }

            public PositiveInt Age { get; }

            public NonEmptySeq<string> Addresses { get; }

            public static Func<NonEmptyString, PositiveInt, NonEmptySeq<string>, Person> Ctor()
            {
                return (s, i, xs) => new Person(s, i, xs);
            }
            
        }
    }
}
