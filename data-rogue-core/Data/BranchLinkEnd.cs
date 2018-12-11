using data_rogue_core.EntitySystem;
using System.Text.RegularExpressions;

namespace data_rogue_core.Components
{
    public class BranchLinkEnd : ICustomFieldSerialization
    {
        public string Branch;
        public int LevelFrom;
        public int LevelTo;

        public override string ToString()
        {
            return $"BranchLinkEnd: {Branch} {LevelFrom}-{LevelTo}";
        }

        public void Deserialize(string value)
        {
            var match = Regex.Match(value, "^BranchLinkEnd: (.*) ([0-9]*)-([0-9]*)$");
            Branch = match.Groups[1].Value;
            LevelFrom = int.Parse(match.Groups[2].Value);
            LevelTo = int.Parse(match.Groups[3].Value);
        }

        public string Serialize()
        {
            return ToString();
        }
    }
}