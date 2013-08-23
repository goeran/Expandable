using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Expandable;
using Expandable.Extensions;

public class Expand
{
    private string table;
    private const BindingFlags instanceMemebersAndPublic = BindingFlags.Instance | BindingFlags.Public;

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

    public static ExpandGroup GroupOfTables(String inputData)
    {
        var groups = new ExpandGroup();
        var groupsAsString = Regex.Split(inputData.Trim(),  @"\w*:", RegexOptions.IgnoreCase).
            Where(g => !String.IsNullOrEmpty(g)).ToArray();
        
        if (groupsAsString.Count() > 0)
            groups.Group1 = Table(groupsAsString[0]);
        if (groupsAsString.Count() > 1)
            groups.Group2 = Table(groupsAsString[1]);
        if (groupsAsString.Count() > 2)
            groups.Group3 = Table(groupsAsString[2]);
        return groups;
    }

    public class ExpandGroup
    {
        public Expand Group1 { get; internal set; }
        public Expand Group2 { get; internal set; }
        public Expand Group3 { get; internal set; }
    }

    public IList<T> ToListOf<T>()
    {
        var result = new List<T>();
        var theClass = typeof(T);
        var tableParser = new TableParser(table);
        
        tableParser.Parse();

        foreach (var row in tableParser.Rows)
        {
            if (theClass.HasAParameterlessPublicCtor())
            {
                var obj = Activator.CreateInstance<T>();
                var properties = obj.GetType().GetProperties(instanceMemebersAndPublic | BindingFlags.SetProperty);
                foreach (var property in properties)
                {
                    for (int colIndex = 0; colIndex < tableParser.Columns.Count(); colIndex++)
                    {
                        if (property.Name == tableParser.Columns.ElementAt(colIndex))
                        {
                            var val = row.ElementAt(colIndex).ConvertUsingType(property.PropertyType);
                            property.SetValue(obj, val, new object[0]);
                            break;
                        }
                    }
                }
                result.Add(obj);
            } 
            else
            {
                var ctorMatchesAgainstColumns = new Dictionary<ConstructorInfo, int>();
                foreach (var ctorCandidate in theClass.PublicConstructors())
                {
                    if (!ctorMatchesAgainstColumns.ContainsKey(ctorCandidate))
                        ctorMatchesAgainstColumns[ctorCandidate] = 0;

                    var ctorCandidateParams = ctorCandidate.GetParameters();
                    foreach (var param in ctorCandidateParams)
                    {
                        if (tableParser.Columns.Any(col => param.Name.ToLower() == col.ToLower()))
                        {
                            ctorMatchesAgainstColumns[ctorCandidate]++;
                        }
                    }
                }

                var ctor = ctorMatchesAgainstColumns.OrderByDescending(kv => kv.Value).First().Key;
                if (ctor != null)
                {
                    var ctorParams = new List<Object>();
                    foreach (var param in ctor.GetParameters())
                    {
                        for (var colIndex = 0; colIndex < tableParser.Columns.Count(); colIndex++)
                        {
                            if (param.Name.ToLower() == tableParser.Columns.ElementAt(colIndex).ToLower())
                            {
                                var val = row.ElementAt(colIndex).ConvertUsingType(param.ParameterType);
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

}
