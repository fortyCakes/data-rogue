using data_rogue_core.EntityEngineSystem;
using System;
using System.Text.RegularExpressions;

namespace data_rogue_core.Data
{
    public class Counter : ICustomFieldSerialization
    {
        public int Current;
        public int Max;

        public void Deserialize(string value)
        {
            var match = Regex.Match(value, @"(.*)\/(.*)");

            Current = int.Parse(match.Groups[1].Value);
            Max = int.Parse(match.Groups[2].Value);
        }

        public string Serialize()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return $"{Current}/{Max}";
        }

        public void Subtract(int amount)
        {
            Current = Math.Max(Current - amount, 0);
        }

        public void Add(int amount)
        {
            Current = Math.Min(Current + amount, Max);
        }
    }
}
