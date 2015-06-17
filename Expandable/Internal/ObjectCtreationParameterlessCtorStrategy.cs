using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Expandable.Extensions;

namespace Expandable.Internal
{
    internal class ObjectCtreationParameterlessCtorStrategy<T> : IObjectCreationStrategy<T>
    {
        private const BindingFlags instanceMemeberAndPublic = BindingFlags.Instance | BindingFlags.Public;
        private readonly TableParser tableParser;
        private readonly CultureInfo culture;

        public ObjectCtreationParameterlessCtorStrategy(TableParser tableParser, CultureInfo culture)
        {
            this.culture = culture;
            this.tableParser = tableParser;
        }

        public T CreateObjectFromRow(IList<string> row)
        {
            var obj = Activator.CreateInstance<T>();
            var properties = obj.GetType().GetProperties(instanceMemeberAndPublic | BindingFlags.SetProperty);
            foreach (var property in properties)
            {
                for (int colIndex = 0; colIndex < tableParser.Columns.Count(); colIndex++)
                {
                    if (property.Name.ToLower() == tableParser.Columns.ElementAt(colIndex))
                    {
                        var val = row[colIndex].ConvertUsingType(property.PropertyType, culture);
                        property.SetValue(obj, val, new object[0]);
                        break;
                    }
                }
            }
            return obj;
        }
    }
}
