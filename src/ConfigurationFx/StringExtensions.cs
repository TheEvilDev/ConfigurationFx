using System;

namespace ConfigurationFx
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string first, string second)
        {
            return String.Equals(first, second, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}