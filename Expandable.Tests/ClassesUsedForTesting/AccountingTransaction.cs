using System;

namespace Expandable.Tests.ClassesUsedForTesting
{
    public class AccountingTransaction
    {
        public AccountingTransaction(int voucherNr, int accountNr, double debit,
            double credit, DateTime date, int period,
            int year, DateTime registeredDate)
        {
            VoucherNr = voucherNr;
            AccountNr = accountNr;
            Debit = debit;
            Credit = credit;
            Date = date;
            Period = period;
            Year = year;
            RegisteredDate = registeredDate;
        }

        public int VoucherNr { get; set; }
        public int AccountNr { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public DateTime Date { get; set; }
        public int Period { get; set; }
        public int Year { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}
