using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Expandable.Extensions;

namespace Expandable.Internal
{
    internal class ObjectCreationCtorStrategy<T> : IObjectCreationStrategy<T>
    {
        private TableParser tableParser;
        private CultureInfo culture;
        private ConstructorInfo matchedConstructor;
        private Dictionary<string, int> ctorArgumentToColumnsInTableMapping;

        public ObjectCreationCtorStrategy(TableParser tableParser, CultureInfo culture)
        {
            this.culture = culture;
            this.tableParser = tableParser;

            FindMatchingCtor();
            MapCtorArgumentsToColumnsInTable();
        }

        public T CreateObjectFromRow(IEnumerable<string> row)
        {
            if (matchedConstructor == null)
            {
                return default(T);
            }

            var ctorParams = new List<object>();
            foreach (var param in matchedConstructor.GetParameters())
            {
                ctorParams.Add(row.ElementAt(ctorArgumentToColumnsInTableMapping[param.Name.ToLower()]).ConvertUsingType(param.ParameterType, culture));
            }
            return (T)Activator.CreateInstance(typeof(T), ctorParams.ToArray());
        }

        private void MapCtorArgumentsToColumnsInTable()
        {
            if (ctorArgumentToColumnsInTableMapping != null)
            {
                return;
            }

            ctorArgumentToColumnsInTableMapping = new Dictionary<string, int>();
            foreach (var param in matchedConstructor.GetParameters())
            {
                for (var colIndex = 0; colIndex < tableParser.Columns.Count(); colIndex++)
                {
                    if (param.Name.ToLower() == tableParser.Columns.ElementAt(colIndex).ToLower())
                    {
                        ctorArgumentToColumnsInTableMapping.Add(param.Name.ToLower(), colIndex);
                        break;
                    }
                }
            }
        }

        private void FindMatchingCtor()
        {
            if (matchedConstructor != null)
            {
                return;
            }

            var ctorMatchesAgainstColumns = new Dictionary<ConstructorInfo, int>();
            foreach (var ctorCandidate in typeof(T).PublicConstructors())
            {
                if (!ctorMatchesAgainstColumns.ContainsKey(ctorCandidate))
                    ctorMatchesAgainstColumns[ctorCandidate] = 0;

                var ctorCandidateParams = ctorCandidate.GetParameters();
                foreach (var param in ctorCandidateParams)
                {
                    if (tableParser.HasColumn(param.Name))
                    {
                        ctorMatchesAgainstColumns[ctorCandidate]++;
                    }
                }
            }

            matchedConstructor = ctorMatchesAgainstColumns.OrderByDescending(kv => kv.Value).First().Key;
        }
    }
}
