using System.Collections.Generic;
using System.ComponentModel;
using System.Functional;
using System.Functional.Monads;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using FsCheck;
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
        public bool Value_Gets_Only_Been_Read_Once(NonNull<object> x, FsCheck.PositiveInt times)
        {
            IEnumerable<Maybe<object>> xs =
                Enumerable.Repeat(ReadOnce.Wrap(x.Get), times.Get)
                          .Select(f => f());

            IEnumerable<Maybe<object>> ys =
                new[] { Maybe.Just(x.Get) }
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

        [Property]
        public bool Serialize_Person()
        {
            string xml =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?><Person><Name>philip</Name><Age>53</Age></Person>";

            var serializer = new XmlSerializer(typeof(PersonXml));
            return new StringReader(xml)
                    .Use(r => (PersonXml) serializer.Deserialize(r))
                    .PipeTo(Person.FromXml)
                != Maybe<Person>.Nothing;
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

            public static Maybe<Person> FromXml(PersonXml p)
            {
                return p.Name.Unwrap(NonEmptyString.Maybe)
                 .Zip(p.Age.Unwrap(PositiveInt.Maybe), Maybe)
                 .Flatten();
            }
        }

        [Serializable]
        [DesignerCategory("code")]
        [XmlType(AnonymousType = true)]
        [XmlRoot(ElementName = "Person", Namespace = "", IsNullable = false)]
        public class PersonXml
        {
            [XmlElement("Name")]
            public string NameXml { get; set; }

            [XmlIgnore]
            public Untrusted<string> Name => Untrusted.Wrap(NameXml);

            [XmlElement("Age")]
            public int AgeXml { get; set; }

            [XmlIgnore]
            public Untrusted<int> Age => Untrusted.Wrap(AgeXml);
        }
    }
}
