using data_rogue_core.EntityEngineSystem;
using System.Text.RegularExpressions;
using data_rogue_core.Maps;

namespace data_rogue_core.Components
{
    public class BranchLinkEnd : ICustomFieldSerialization
    {
        public string Branch;
        public int LevelFrom;
        public int LevelTo;
        public MapCoordinate Location;

        public override string ToString()
        {
            if (Location != null)
                return $"BranchLinkEnd: {Branch} {LevelFrom}-{LevelTo} @ {Location.Serialize()}";
            return $"BranchLinkEnd: {Branch} {LevelFrom}-{LevelTo}";
        }

        public void Deserialize(string value)
        {
            var match = Regex.Match(value, "^BranchLinkEnd: (.*) ([0-9]*)-([0-9]*) @ (.*)$");

            if (match.Success)
            {
                Location = new MapCoordinate();
                Location.Deserialize(match.Groups[4].Value);
            }
            else
            {
                match = Regex.Match(value, "^BranchLinkEnd: (.*) ([0-9]*)-([0-9]*)$");
            }
           
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