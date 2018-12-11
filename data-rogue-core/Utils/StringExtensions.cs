using System;

namespace data_rogue_core.Utils
{
    public static class StringExtensions
    {
        public static string[] SplitLines(this string input)
        {
            return input.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
