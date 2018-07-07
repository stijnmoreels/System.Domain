using System.Collections.Generic;
using System.Functional;
using System.Functional.Monads;
using FsCheck;

namespace System.Domain.Tests
{
    public class UntrustedTests
    {
        [CustomProperty]
        public bool Untrusted_Unwraps_Value_In_Maybe_Based_On_Given_Validation(NonNull<object> x, Maybe<NonNull<object>> m)
        {
            return Untrusted<object>
                   .Wrap(x.Get)
                   .Unwrap(_ => m.Select(o => o.Get))
                   .Equals(Maybe<object>.Nothing)
                   .Equals(m.Equals(Maybe<NonNull<object>>.Nothing));
        }

        [CustomProperty]
        public bool Untrusted_Unwraps_Value_In_Result_Based_On_Given_Validation(
            NonNull<object> x, 
            Result<NonNull<object>, NonNull<object>> m)
        {
            return Untrusted<object>
                   .Wrap(x)
                   .Unwrap(_ => m)
                   .GetOk()
                   .Equals(Maybe<NonNull<object>>.Nothing)
                   .Equals(m.GetOk().Equals(Maybe<NonNull<object>>.Nothing));
        }
    }
}
