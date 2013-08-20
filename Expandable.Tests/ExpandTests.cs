using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Expandable.Tests
{
    class ExpandTests
    {
        [TestFixture]
        public class When_expanding_table
        {
            [Test]
            [ExpectedException(typeof(ArgumentNullException))]
            public void It_requires_string_to_be_specified()
            {
                Expand.Table(null);
            }

            [Test]
            [ExpectedException(typeof(ArgumentException), ExpectedMessage = "String can't be empty")]
            public void It_requires_string_to_contain_data()
            {
                Expand.Table("");
            }

            [Test]
            public void It_will_return_an_Expand_object()
            {
                Assert.IsInstanceOf<Expand>(Expand.Table("some inputData"));
            }
        }

        [TestFixture]
        public class When_expanding_table_to_list_of_objects
        {
            private IEnumerable<Person> employees;
            private Person steve;
            private Person bill;

            [SetUp]
            public void Setup()
            {
                employees = Expand.Table(@"
                    Name    | Age | Salary      | HasPhone
                    Steve   | 55  | 150000      | true
                    Bill    | 56  | 160000.10   | false
                ").ToListOf<Person>();
                steve = employees.ElementAt(0);
                bill = employees.ElementAt(1);
}

            [Test]
            public void It_will_not_return_a_null_pointer()
            {
                Assert.IsNotNull(employees);
            }

            [Test]
            public void It_will_return_back_the_same_number_of_objects_as_rows_in_table()
            {
                Assert.AreEqual(2, employees.Count());
            }

            [Test]
            public void It_is_able_to_set_properties_of_type_string()
            {
                Assert.AreEqual("Steve", steve.Name);
                Assert.AreEqual("Bill", bill.Name);
            }

            [Test]
            public void It_is_able_to_set_properties_of_type_int()
            {
                Assert.AreEqual(55, steve.Age);
                Assert.AreEqual(56, bill.Age);
            }

            [Test]
            public void It_is_able_to_set_properties_of_type_double()
            {
                Assert.AreEqual(150000, steve.Salary);
                Assert.AreEqual(160000.10, bill.Salary);
            }

            [Test]
            public void It_is_able_to_set_properties_of_type_boolean()
            {
                Assert.IsTrue(steve.HasPhone);
                Assert.IsFalse(bill.HasPhone);
            }

            [Test]
            public void It_will_properties_with_complex_types()
            {
                Assert.IsNull(steve.Address);
            }
        }
    }

    internal class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Salary { get; set; }
        public bool HasPhone { get; set; }
        public Address Address { get; set; }
    }

    internal class Address
    {
        public String StreetName { get; set; }
        public int StreetNumber { get; set; }
    }
}
