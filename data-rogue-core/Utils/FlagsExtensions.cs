using System;
using System.Collections.Generic;

namespace data_rogue_core.Utils
{
    public static class FlagsExtensions
    {
        public static IEnumerable<Enum> GetFlags(this Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value) && value.GetHashCode() != 0)
                    yield return value;
        }
    }
}
