using System.Functional.Monads;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;

namespace System.Domain.Tests
{
    public class ModelTests
    {
        [Property]
        public bool Creates_Maybe_PositiveInt_Only_For_Positive_Integers(int i)
        {
            return PositiveInt
                   .Maybe(i)
                   .Equals(Maybe<PositiveInt>.Nothing)
                   .Equals(i < 0);
        }

        [Property]
        public bool Creates_Result_PositiveInt_Only_For_Positive_Integers(int i)
        {
            return PositiveInt
                   .Result(i)
                   .GetOk()
                   .Equals(Maybe<PositiveInt>.Nothing)
                   .Equals(i < 0);
        }

        [Property]
        public bool Creates_Maybe_NonEmptySeq_Only_For_Not_Empty_Sequences(object[] xs)
        {
            return NonEmptySeq<object>
                   .Maybe(xs)
                   .Equals(Maybe<NonEmptySeq<object>>.Nothing)
                   .Equals(!xs.Any());
        }

        [Property]
        public bool Creates_Result_NonEmptySeq_Only_For_Not_Empty_Sequences(object[] xs)
        {
            return NonEmptySeq<object>
                   .Result(xs)
                   .GetOk()
                   .Equals(Maybe<NonEmptySeq<object>>.Nothing)
                   .Equals(!xs.Any());
        }

        [Property]
        public bool Creates_Maybe_NonEmptyString_Only_For_Not_Empty_Strings(string x)
        {
            return NonEmptyString
                   .Maybe(x)
                   .Equals(Maybe<NonEmptyString>.Nothing)
                   .Equals(string.IsNullOrEmpty(x));
        }

        [Property]
        public bool Creates_Result_NonEmptyString_Only_For_Not_Empty_Strings(string x)
        {
            return NonEmptyString
                   .Result(x)
                   .GetOk()
                   .Equals(Maybe<NonEmptyString>.Nothing)
                   .Equals(string.IsNullOrEmpty(x));
        }
    }
}
