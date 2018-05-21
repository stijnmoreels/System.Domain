using System.Collections.Generic;
using System.Functional;
using System.Functional.Monads;
using System.Linq;
using FsCheck.Xunit;
using static System.Domain.Spec;

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

        [Property]
        public bool Value_Gets_Only_Been_Read_Once(object x, FsCheck.PositiveInt times)
        {
            IEnumerable<Maybe<object>> xs = 
                Enumerable.Repeat(ReadOnce.Wrap(x), times.Get)
                          .Select(f => f());

            IEnumerable<Maybe<object>> ys =
                new[] { Maybe.Just(x) }
                    .Concat(Enumerable.Repeat(Maybe<object>.Nothing, times.Get - 1));

            return xs.SequenceEqual(ys);
        }

        [CustomProperty]
        public bool Spec_Holds_When_Creating_A_Person(NonEmptyString name, PositiveInt age)
        {
            return name.Text.Matches("^[a-zA-Z\\. ]+$")
                       .And(age.Number.ExclusiveBetween(0, 100))
                       .Equals(Person.Maybe(name, age) != Maybe<Person>.Nothing);
        }

        public class Person
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Person"/> class.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="age"></param>
            public Person(NonEmptyString name, PositiveInt age)
            {
                Name = name;
                Age = age;
            }

            public NonEmptyString Name { get; }

            public PositiveInt Age { get; }

            public static Maybe<Person> Maybe(NonEmptyString name, PositiveInt age)
            {
                return NotNull(name)
                    .And(NotNull(age))
                    .And(() => name.Text.Matches("^[a-zA-Z\\. ]+$"))
                    .And(() => age.Number.ExclusiveBetween(0, 100))
                    .ThenMaybe(name.TupleWith(age))
                    .Select(t => new Person(t.Item1, t.Item2));
            }
        }
    }
}
