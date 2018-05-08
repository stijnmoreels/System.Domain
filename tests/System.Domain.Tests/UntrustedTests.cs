using System.Collections.Generic;
using System.Functional;
using System.Functional.Monads;

namespace System.Domain.Tests
{
    public class UntrustedTests
    {
        [CustomProperty]
        public bool Untrusted_Unwraps_Value_In_Maybe_Based_On_Given_Validation(object x, Maybe<object> m)
        {
            return Untrusted<object>
                   .Wrap(x)
                   .Unwrap(_ => m)
                   .Equals(Maybe<object>.Nothing)
                   .Equals(m.Equals(Maybe<object>.Nothing));
        }

        [CustomProperty]
        public bool Untrusted_Unwraps_Value_In_Result_Based_On_Given_Validation(object x, Result<object, object> m)
        {
            return Untrusted<object>
                   .Wrap(x)
                   .Unwrap(_ => m)
                   .GetOk()
                   .Equals(Maybe<object>.Nothing)
                   .Equals(m.GetOk().Equals(Maybe<object>.Nothing));
        }
    }
}
