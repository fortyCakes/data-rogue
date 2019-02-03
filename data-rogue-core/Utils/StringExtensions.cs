using System;

namespace data_rogue_core.Utils
{
    public static class StringExtensions
    {
        public static string[] SplitLines(this string input)
        {
            return input.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static string PadBoth(this string str, int length)
        {
            int spaces = length - str.Length;
            int padLeft = spaces / 2 + str.Length;
            return str.PadLeft(padLeft).PadRight(length);
        }
    }
}
