using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Expandable;
using Expandable.Extensions;
using Expandable.Internal;

public class Expand
{
    private string table;
    private CultureInfo culture = new CultureInfo("en-US");

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
        var groupsAsString = Regex.Split(inputData.Trim(),  @"\w{3,}\:", RegexOptions.IgnoreCase).
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

        public ExpandGroup Culture(CultureInfo cultureInfo)
        {
            if (Group1 != null) Group1.culture = cultureInfo;
            if (Group2 != null) Group2.culture = cultureInfo;
            if (Group3 != null) Group3.culture = cultureInfo;
            return this;
        }
    }

    public IList<T> ToListOf<T>()
    {
        var result = new List<T>();
        var theClass = typeof(T);
        var tableParser = new TableParser(table);
        
        tableParser.Parse();
        IObjectCreationStrategy<T> objectCreationStrategy = null;
        if (theClass.HasAParameterlessPublicCtor())
        {
            objectCreationStrategy = new ObjectCtreationParameterlessCtorStrategy<T>(tableParser, culture);
        }
        else
        {
            objectCreationStrategy = new ObjectCreationCtorStrategy<T>(tableParser, culture);
        }

        foreach (var row in tableParser.Rows)
        {
            var obj = objectCreationStrategy.CreateObjectFromRow(row);
            if (obj != null)
            {
                result.Add(obj);
            }
        }

        return result;
    }

    public Expand Culture(CultureInfo cultureInfo)
    {
        this.culture = cultureInfo;
        return this;
    }
}
