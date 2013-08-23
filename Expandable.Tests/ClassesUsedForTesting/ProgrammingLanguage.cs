using System;

namespace Expandable.Tests.ClassesUsedForTesting
{
    internal class ProgrammingLanguage
    {
        public ProgrammingLanguage(string name, int yearInvented) :
            this(name, yearInvented, false)
        {
        }

        public ProgrammingLanguage(string name, int yearInvented, bool isOpenSource)
        {
            Name = name;
            YearInvented = yearInvented;
            IsOpoenSource = isOpenSource;
        }

        public String Name { get; private set; }
        public int YearInvented { get; private set; }
        public bool IsOpoenSource { get; private set; }
    }
}