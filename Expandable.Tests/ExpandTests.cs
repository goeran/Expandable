﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Expandable.Tests.ClassesUsedForTesting;
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
                    Name    | Age | Salary      | Height    | DateOfBirth           | Weight    | HasPhone | EmployeeStatus | Department    | Address   | Performance   | NetWorth  | Comment
                    Steve   | 55  | 150000      | 188.8     | 1955-02-24 13:00:10   | 70.1      | true     | Unknown        | 1             |           | 200           | 10        | This should be ignored
                    Bill    | 56  | 160000.10   | 166.5     | 1955-10-28 12:10:05   | 68.9      | false    | Retired        | 2             |           | 110           | 40        |
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
                Assert.AreEqual(new DateTime(1955, 2, 24, 13, 0, 10), steve.DateOfBirth);
                Assert.AreEqual(new DateTime(1955, 10, 28, 12, 10, 5), bill.DateOfBirth);
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

            [Test]
            public void It_is_possible_to_specify_culture()
            {
                var persons = Expand.Table(@"
                    Name    | Salary
                    Steve   | 100,50
                    bill    | 50000,90
                ").Culture(new CultureInfo("nb-NO")).ToListOf<Person>();

                Assert.AreEqual(2, persons.Count);
                var steve = persons[0];
                var bill = persons[1];
                Assert.AreEqual(100.50, steve.Salary);
                Assert.AreEqual(50000.90, bill.Salary);
            }

            //TODO: test SByte, Int16, UInt16, Int64, UInt64, Single, Char
            //TODO: test for ignore casing on properties
        }

        [TestFixture]
        public class When_expanding_table_to_list_of_objects_that_use_constructor_to_set_values_on_properties
        {
            [Test]
            public void It_will_be_able_to_set_arguments_of_string()
            {
                var programmingLanguages = Expand.Table(@"
                    Name    | YearInvented
                    C#      | 2001 
                ").ToListOf<ProgrammingLanguage>();
                Assert.IsNotNull(programmingLanguages);
                Assert.AreEqual("C#", programmingLanguages.First().Name);
                Assert.AreEqual(2001, programmingLanguages.First().YearInvented);
            }

            [Test]
            public void It_will_ignore_columns_that_does_not_match_ctor()
            {
                var programmingLanguages = Expand.Table(@"
                    Comment                       | Name    | YearInvented  | Comment
                    Great strongly typed language |C#       | 2001          | 
                ").ToListOf<ProgrammingLanguage>();
                Assert.AreEqual(1, programmingLanguages.Count());
                Assert.AreEqual("C#", programmingLanguages.First().Name);
            }

            [Test]
            public void It_will_handle_overloaded_ctors()
            {
                var programmingLanguages = Expand.Table(@"  
                    Name    | YearInvented  | IsOpenSource  | Comment
                    C#      | 2001          | false         |
                    foobar  | 1996          | true          | Something I just invented for the sake of the test
                ").ToListOf<ProgrammingLanguage>();
                Assert.AreEqual(2, programmingLanguages.Count());
                var foobar = programmingLanguages.ElementAt(1);
                Assert.IsTrue(foobar.IsOpoenSource);
            }
        }

        [TestFixture]
        public class When_expanding_groups_of_tables_to_list_of_objects
        {
            private Expand.ExpandGroup groupsOfProgrammingLanguages;

            //TODO: Test input, test that groups data is parsed

            [SetUp]
            public void Setup()
            {
                groupsOfProgrammingLanguages = Expand.GroupOfTables(@"
                    Static:
                    Name    | YearInvented  | IsOpenSource
                    C#      | 2001          | false
                    Java    | 1995          | true 

                    Dynamic:
                    Name        | YearInvented  | IsOpenSource
                    JavaScript  | 1994          | false
                ");
            }

            [Test]
            public void It_will_return_groups()
            {
                Assert.IsInstanceOf<Expand.ExpandGroup>(groupsOfProgrammingLanguages);
            }

            [Test]
            public void It_will_parse_data_for_groups()
            {
                var staticLanguages = groupsOfProgrammingLanguages.Group1.ToListOf<ProgrammingLanguage>();
                Assert.IsNotNull(staticLanguages);
                Assert.AreEqual(2, staticLanguages.Count);
                var java = staticLanguages[1];
                Assert.IsNotNull(java);
                Assert.AreEqual("Java", java.Name);
                Assert.AreEqual(1995, java.YearInvented);

                var dynamicLanguages = groupsOfProgrammingLanguages.Group2.ToListOf<ProgrammingLanguage>();
                Assert.IsNotNull(dynamicLanguages);
                Assert.AreEqual(1, dynamicLanguages.Count);
                var javascript = dynamicLanguages[0];
                Assert.AreEqual("JavaScript", javascript.Name);
                Assert.AreEqual(1994, javascript.YearInvented);
            }

            [Test]
            public void It_will_handle_a_single_group()
            {
                var groups = Expand.GroupOfTables(@"
                    Static:
                    Name    | YearInvented  | IsOpenSource
                    C#      | 2001          | false
                ");

                Assert.IsNotNull(groups.Group1);
                Assert.IsNull(groups.Group2);
                Assert.IsNull(groups.Group3);
            }

            [Test]
            public void It_will_handle_two_groups()
            {
                var groups = Expand.GroupOfTables(@"
                    Static:
                    Name    | YearInvented  | IsOpenSource
                    C#      | 2001          | false

                    dynamic:
                    Name        | YearInvented  | IsOpenSource
                    JavaScript  | 1994      | false
                ");

                Assert.IsNotNull(groups.Group1);
                Assert.IsNotNull(groups.Group2);
                Assert.IsNull(groups.Group3);
            }

            [Test]
            public void It_will_handle_three_groups()
            {
                var groups = Expand.GroupOfTables(@"
                    Static:
                    Name    | YearInvented  | IsOpenSource
                    C#      | 2001          | false

                    dynamic:
                    Name        | YearInvented  | IsOpenSource
                    JavaScript  | 1994          | false

                    functional:
                    Name    | YearInveted | IsOpenSource
                    Lisp    | 1960        | true
                ");

                Assert.IsNotNull(groups.Group1);
                Assert.IsNotNull(groups.Group2);
                Assert.IsNotNull(groups.Group3);
            }

            [Test]
            public void It_is_possible_to_specify_culture()
            {
                var persons = Expand.GroupOfTables(@"
                    employees:
                    Name    | Salary
                    Steve   | 100,50
                    bill    | 50000,90
                ").Culture(new CultureInfo("nb-NO")).Group1.ToListOf<Person>();

                Assert.AreEqual(2, persons.Count);
                var steve = persons[0];
                var bill = persons[1];
                Assert.AreEqual(100.50, steve.Salary);
                Assert.AreEqual(50000.90, bill.Salary);
            }

            [Test]
            public void It_handle_norwegian_datetime_values()
            {
                var employees = Expand.GroupOfTables(@"
                    addresses:
                    StreetName          | StreetNumber
                    Elm street          | 1
                    Washington          | 2

                    employees:
                    Name    | Age | Salary      | Height    | DateOfBirth
                    Steve   | 55  | 150000      | 188,8     | 24.02.1955 13:00:10
                    Bill    | 56  | 160000,10   | 166,5     | 28.10.1955 12:10:05

                ").Culture(new CultureInfo("nb-NO")).Group2.ToListOf<Person>();

                Assert.AreEqual(2, employees.Count());
                Assert.AreEqual(new DateTime(1955, 2, 24, 13, 0, 10), employees.ElementAt(0).DateOfBirth);
            }
        }
    }
}
