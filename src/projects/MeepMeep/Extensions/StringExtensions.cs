using System;

namespace MeepMeep.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveNewLines(this string value)
        {
            return value == null ? null : value.Replace(Environment.NewLine, string.Empty);
        }
    }
}