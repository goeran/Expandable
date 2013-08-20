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
            var obj = Activator.CreateInstance<T>();
            var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public);
            foreach (var property in properties)
            {
                for (int colIndex = 0; colIndex < columns.Count; colIndex++)
                {
                    if (property.Name == columns[colIndex])
                    {
                        Object val = rowColumns[colIndex];
                        val = TypeConversion(property, val);
                        property.SetValue(obj, val, new object[0]);
                        break;
                    }
                }
            }
            result.Add(obj);
        }

        return result;
    }

    private static object TypeConversion(PropertyInfo property, object val)
    {
        if (property.PropertyType == typeof (String))
            return Convert.ToString(val).Trim();
        if (property.PropertyType == typeof (Int32))
            return Convert.ToInt32(val);
        if (property.PropertyType == typeof (Double))
            return Convert.ToDouble(val, americanNumberFormat);
        if (property.PropertyType == typeof (Boolean))
            return Convert.ToBoolean(val);
        return null;
    }
}
