using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

public class Expand
{
    private string table;
    private static readonly NumberFormatInfo americanNumberFormat;

    private Expand()
    {
    }

    static Expand()
    {
        americanNumberFormat = new CultureInfo("en-US").NumberFormat;
    }

    public static Expand Table(String inputData)
    {
        if (inputData == null) throw new ArgumentNullException();
        if (String.IsNullOrEmpty(inputData)) throw new ArgumentException("String can't be empty");

        var expand = new Expand();
        expand.table = inputData;

        return expand;
    }

    public IEnumerable<T> ToListOf<T>()
    {
        var result = new List<T>();
        String header = null;
        var columns = new List<String>();
        var rows = new List<String>();

        using (var reader = new StringReader(table))
        {
            while (reader.Peek() != -1)
            {
                var line = reader.ReadLine();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    if (header == null)
                    {
                        header = line;
                        columns.AddRange(header.Split('|').Select(c => c.Trim()));
                    }
                    else
                    {
                        rows.Add(line);
                    }
                }
            }
        }

        foreach (var row in rows)
        {
            var rowColumns = row.Split('|');
            var objectType = typeof (T);
            var constructors = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            T obj = default(T);
            if (constructors.Any(c => c.GetParameters().Count() == 0))
            {
                obj = Activator.CreateInstance<T>();
            } 
            else if (constructors.Any(c => c.GetParameters().Count() > 0))
            {
                var constructorParams = constructors.Where(c => c.GetParameters().Any()).First().GetParameters();
            }

            if (obj != null)
            {
                var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public);
                foreach (var property in properties)
                {
                    for (int colIndex = 0; colIndex < columns.Count; colIndex++)
                    {
                        if (property.Name == columns[colIndex])
                        {
                            var val = TypeConversion(property, rowColumns[colIndex]);
                            property.SetValue(obj, val, new object[0]);
                            break;
                        }
                    }
                }
                result.Add(obj);
            }
        }

        return result;
    }

    private static object TypeConversion(PropertyInfo property, string val)
    {
        if (property.PropertyType == typeof (String))
            return val.Trim();
        if (property.PropertyType == typeof (Int32))
            return int.Parse(val);
        if (property.PropertyType == typeof (uint))
            return uint.Parse(val);
        if (property.PropertyType == typeof (Double))
            return double.Parse(val, americanNumberFormat);
        if (property.PropertyType == typeof (float))
            return float.Parse(val, americanNumberFormat);
        if (property.PropertyType == typeof (Boolean))
            return bool.Parse(val);
        if (property.PropertyType.IsEnum)
            return Enum.Parse(property.PropertyType, val);
        if (property.PropertyType == typeof (DateTime))
            return Convert.ToDateTime(val);
        if (property.PropertyType == typeof (Byte))
            return Byte.Parse(val);
        if (property.PropertyType == typeof (long))
            return long.Parse(val);
        if (property.PropertyType == typeof (decimal))
            return decimal.Parse(val, americanNumberFormat);
        return null;
    }
}
