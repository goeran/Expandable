using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Expandable;

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
        var objectType = typeof(T);
        var tableParser = new TableParser(table);
        
        tableParser.Parse();

        foreach (var row in tableParser.Rows)
        {
            var constructors = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (constructors.Any(c => !c.GetParameters().Any()))
            {
                var obj = Activator.CreateInstance<T>();
                var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public);
                foreach (var property in properties)
                {
                    for (int colIndex = 0; colIndex < tableParser.Columns.Count(); colIndex++)
                    {
                        if (property.Name == tableParser.Columns.ElementAt(colIndex))
                        {
                            var val = TypeConversion(property.PropertyType, row.ElementAt(colIndex));
                            property.SetValue(obj, val, new object[0]);
                            break;
                        }
                    }
                }
                result.Add(obj);
            } 
            else if (constructors.Any(c => c.GetParameters().Any()))
            {
                ConstructorInfo ctor = null;
                var ctorMatchesAgainstColumns = new Dictionary<ConstructorInfo, int>();
                foreach (var ctorCandidate in constructors)
                {
                    if (!ctorMatchesAgainstColumns.ContainsKey(ctorCandidate))
                        ctorMatchesAgainstColumns[ctorCandidate] = 0;

                    var ctorCandidateParams = ctorCandidate.GetParameters();
                    foreach (var param in ctorCandidateParams)
                    {
                        foreach (var col in tableParser.Columns)
                        {
                            if (param.Name.ToLower() == col.ToLower())
                            {
                                ctorMatchesAgainstColumns[ctorCandidate]++;
                                break;
                            }
                        }
                    }
                    
                    ctor = ctorMatchesAgainstColumns.OrderByDescending(kv => kv.Value).First().Key;
                }

                if (ctor != null)
                {
                    var ctorParams = new List<Object>();
                    foreach (var param in ctor.GetParameters())
                    {
                        for (var colIndex = 0; colIndex < tableParser.Columns.Count(); colIndex++)
                        {
                            if (param.Name.ToLower() == tableParser.Columns.ElementAt(colIndex).ToLower())
                            {
                                var val = TypeConversion(param.ParameterType, row.ElementAt(colIndex));
                                ctorParams.Add(val);
                                break;
                            }
                        }
                    }
                    var obj = (T)Activator.CreateInstance(typeof(T), ctorParams.ToArray());
                    result.Add(obj);
                }
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
