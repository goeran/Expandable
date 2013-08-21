using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

public class Expand
{
    private string table;
    private static readonly NumberFormatInfo americanNumberFormat = new CultureInfo("en-US").NumberFormat;

    private Expand()
    {
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
                var ctor = constructors.Where(c => c.GetParameters().Any()).First();
                var constructorParams = ctor.GetParameters();

                var ctorMatchesAgainstColumns = new Dictionary<ConstructorInfo, int>();
                /*foreach (var ctorParam in constructorParams)
                {
                    for (var colIndex = 0; colIndex < columns.Count; colIndex++)
                    {
                        if (!ctorMatchesAgainstColumns.ContainsKey(ctor))
                            ctorMatchesAgainstColumns[ctor] = 0;

                        if (ctorParam.Name == columns[colIndex])
                        {
                            ctorMatchesAgainstColumns[ctorParam]++;
                            break;
                        }
                    }
                }*/

                var ctorParams = new List<Object>();
                foreach (var param in ctor.GetParameters())
                {
                    for (var colIndex = 0; colIndex < columns.Count; colIndex++)
                    {
                        if (param.Name.ToLower() == columns[colIndex].ToLower())
                        {
                            var val = TypeConversion(param.ParameterType, rowColumns[colIndex]);
                            ctorParams.Add(val);
                            break;
                        }
                    }
                }
                obj = (T)Activator.CreateInstance(typeof (T), ctorParams.ToArray());
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
                            var val = TypeConversion(property.PropertyType, rowColumns[colIndex]);
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

    private static object TypeConversion(Type memberInfo, string val)
    {
        var memberType = memberInfo;

        if (memberType == typeof (String))
            return val.Trim();
        if (memberType == typeof(Int32))
            return int.Parse(val);
        if (memberType == typeof(uint))
            return uint.Parse(val);
        if (memberType == typeof(Double))
            return double.Parse(val, americanNumberFormat);
        if (memberType == typeof(float))
            return float.Parse(val, americanNumberFormat);
        if (memberType == typeof(Boolean))
            return bool.Parse(val);
        if (memberType.IsEnum)
            return Enum.Parse(memberType, val);
        if (memberType == typeof(DateTime))
            return Convert.ToDateTime(val);
        if (memberType == typeof(Byte))
            return Byte.Parse(val);
        if (memberType == typeof(long))
            return long.Parse(val);
        if (memberType == typeof(decimal))
            return decimal.Parse(val, americanNumberFormat);
        return null;
    }
}
