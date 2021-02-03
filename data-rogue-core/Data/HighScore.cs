using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_rogue_core.Data
{
    public class HighScore
    {
        public string Name;
        public decimal Score;

        public HighScore(string name, decimal score)
        {
            Name = name;
            Score = score;
        }

        public string Serialise()
        {
            return $"{Name}: {Score}";
        }

        public static HighScore Deserialise(string text)
        {
            var splits = text.Split(':').Select(s => s.Trim()).ToList();

            var name = splits[0];
            var score = decimal.Parse(splits[1]);

            return new HighScore(name, score);
        }
    }
}
