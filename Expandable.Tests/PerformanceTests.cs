using System;
using System.Diagnostics;
using System.Text;
using Expandable.Tests.ClassesUsedForTesting;
using NUnit.Framework;

namespace Expandable.Tests
{
    public class PerformanceTests
    {
        [TestFixture]
        public class When_expand_table_to_list_of_objects
        {
            [Test]
            public void It_should_be_able_to_expand_100k_rows_under_1_1_sec()
            {
                AssertObjectsExpandedInTime(ASCIITableWithNumberOfRows(100000), maxTime: 1.1);
            }

            private static void AssertObjectsExpandedInTime(string asciiTable, double maxTime)
            {
                var stopwatch = new Stopwatch();
                
                stopwatch.Start();
                Expand.Table(asciiTable).ToListOf<AccountingTransaction>();
                stopwatch.Stop();

                Console.WriteLine(stopwatch.Elapsed);
                Assert.LessOrEqual(stopwatch.Elapsed.TotalSeconds, maxTime);
            }

            [Test]
            public void It_should_ble_able_to_Expand_500k_rows_in_under_5_5_secs()
            {
                AssertObjectsExpandedInTime(ASCIITableWithNumberOfRows(500000), maxTime: 5.5);
            }

            [Test]
            [Explicit("Using for learning purposes")]
            public void How_long_does_it_take_to_create_500k_objects_using_reflection()
            {
                var stopwatch = new Stopwatch();

                stopwatch.Start();

                for (int i = 0; i < 500*1000; i++)
                {
                    var obj = Activator.CreateInstance(typeof(AccountingTransaction), new object[] { 1, 3000, 250, 250, DateTime.Now, 1, 2015, DateTime.Today });
                }

                stopwatch.Stop();

                Console.WriteLine(stopwatch.Elapsed);
            }

            private static string ASCIITableWithNumberOfRows(int numberOfRows)
            {
                var tableWith100kRows = new StringBuilder();
                tableWith100kRows.AppendFormat(
                    "VoucherNr | AccountNr | Debit   | Credit  | Date        | Period | Year  | RegisteredDate\r\n");
                for (int i = 0; i < numberOfRows; i++)
                {
                    tableWith100kRows.AppendFormat(
                        "1         | 3000      | 200.50  | 0       | 1.1.2015    | 1      | 2015  | 1.1.2015 12:00:00\r\n");
                }
                return tableWith100kRows.ToString();
            }
        }
    }
}
