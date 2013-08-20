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
                    Name    | Age | Salary      | Height    | DateOfBirth   | Weight    | HasPhone | EmployeeStatus | Department    | Address   | Performance   | NetWorth  | Comment
                    Steve   | 55  | 150000      | 188.8     | 1955-02-24    | 70.1      | true     | Unknown        | 1             |           | 200           | 10        | This should be ignored
                    Bill    | 56  | 160000.10   | 166.5     | 1955-10-28    | 68.9      | false    | Retired        | 2             |           | 110           | 40        |
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
            public void It_willbe_able_to_set_properties_of_type_string()
            {
                Assert.AreEqual("Steve", steve.Name);
                Assert.AreEqual("Bill", bill.Name);
            }

            [Test]
            public void It_will_be_able_to_set_properties_of_type_int()
            {
                Assert.AreEqual(55, steve.Age);
                Assert.AreEqual(56, bill.Age);
            }

            [Test]
            public void It_will_be_able_to_set_properties_of_type_double()
            {
                Assert.AreEqual(150000, steve.Salary);
                Assert.AreEqual(160000.10, bill.Salary);
            }

            [Test]
            public void It_will_be_able_to_set_properties_of_type_float()
            {
                Assert.AreEqual(188.8, steve.Height, 0.001);
                Assert.AreEqual(166.5, bill.Height, 0.001);
            }

            [Test]
            public void It_will_be_able_to_set_properties_of_type_boolean()
            {
                Assert.IsTrue(steve.HasPhone);
                Assert.IsFalse(bill.HasPhone);
            }

            [Test]
            public void It_will_be_able_to_set_properties_of_type_enum()
            {
                Assert.AreEqual(EmployeeStatus.Unknown, steve.EmployeeStatus);
                Assert.AreEqual(EmployeeStatus.Retired, bill.EmployeeStatus);
            }

            [Test]
            public void It_will_be_able_to_set_properties_of_datetime()
            {
                Assert.AreEqual(new DateTime(1955, 2, 24), steve.DateOfBirth);
                Assert.AreEqual(new DateTime(1955, 10, 28), bill.DateOfBirth);
            }

            [Test]
            public void It_will_be_able_to_set_properties_of_byte()
            {
                Assert.AreEqual(1, steve.Department);
                Assert.AreEqual(2, bill.Department);
            }

            [Test]
            public void It_will_be_able_to_set_properties_of_uint()
            {
                Assert.AreEqual(200, steve.Performance);
                Assert.AreEqual(110, bill.Performance);
            }

            [Test]
            public void It_will_be_able_to_set_properties_of_long()
            {
                Assert.AreEqual(10, steve.NetWorth);
                Assert.AreEqual(40, bill.NetWorth);
            }

            [Test]
            public void It_will_be_able_to_set_properties_of_decimal()
            {
                Assert.AreEqual(70.1, steve.Weight);    
                Assert.AreEqual(68.9, bill.Weight);
            }

            [Test]
            public void It_will_ignore_properties_with_complex_types()
            {
                Assert.IsNull(steve.Address);
                Assert.IsNull(bill.Address);
            }

            //TODO: test SByte, Int16, UInt16, Int64, UInt64, Single, Char
        }
    }

    internal class Person
    {
        public byte Department { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public double Salary { get; set; }
        public bool HasPhone { get; set; }
        public float Height { get; set; }
        public Address Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public EmployeeStatus EmployeeStatus { get; set; }
        public uint Performance { get; set; }
        public long NetWorth { get; set; }
        public decimal Weight { get; set; }
    }

    internal enum EmployeeStatus
    {
        Unknown,
        Employed,
        Retired
    }

    internal class Address
    {
        public String StreetName { get; set; }
        public int StreetNumber { get; set; }
    }
}
