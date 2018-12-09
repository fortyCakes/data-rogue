using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
