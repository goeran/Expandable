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
        private readonly TableParser tableParser;
        private readonly CultureInfo culture;
        private ConstructorInfo matchedConstructor;
        private Dictionary<int, int> paramToColIndexMapping;
        private ParameterInfo[] matchedConstructorParams;

        public ObjectCreationCtorStrategy(TableParser tableParser, CultureInfo culture)
        {
            this.culture = culture;
            this.tableParser = tableParser;

            FindMatchingCtor();
            MapCtorParamsToColumnsInTable();
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

        private void MapCtorParamsToColumnsInTable()
        {
            if (paramToColIndexMapping == null)
            {
                paramToColIndexMapping = new Dictionary<int, int>();
                matchedConstructorParams = matchedConstructor.GetParameters();

                for (var i = 0; i < matchedConstructorParams.Length; i++)
                {
                    for (var j = 0; j < tableParser.Columns.Count; j++)
                    {
                        if (matchedConstructorParams[i].Name.ToLower().Equals(tableParser.Columns[j]))
                        {
                            paramToColIndexMapping.Add(i, j);
                            break;
                        }
                    }
                }
            }
        }


        public T CreateObjectFromRow(IList<string> row)
        {
            if (matchedConstructor == null)
            {
                return default(T);
            }

            var ctorParams = new object[matchedConstructorParams.Length];
            for (int i = 0; i < matchedConstructorParams.Length; i++)
            {
                var valAsStr = row[paramToColIndexMapping[i]];
                var valConverted = valAsStr.ConvertUsingType(matchedConstructorParams[i].ParameterType, culture);
                ctorParams[i] = valConverted;
            }
            return (T)Activator.CreateInstance(typeof(T), ctorParams);
        }
    }
}
