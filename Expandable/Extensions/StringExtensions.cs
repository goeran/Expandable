using System;
using System.Globalization;

namespace Expandable.Extensions
{
    public static class StringExtensions
    {
        private static readonly NumberFormatInfo americanNumberFormat = new CultureInfo("en-US").NumberFormat;

        public static object ConvertUsingType(this string val, Type memberType)
        {
            if (memberType == typeof(String))
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
}
