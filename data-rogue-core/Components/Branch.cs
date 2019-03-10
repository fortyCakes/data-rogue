using data_rogue_core.EntityEngineSystem;
using data_rogue_core.Maps;

namespace data_rogue_core.Components
{
    public class Branch : IEntityComponent
    {
        public string BranchName;
        public string MapGenerationType;
        public int Depth;
        public bool Generated;

        public string LevelName(int level)
        {
            return $"{BranchName}:{level}";
        }

        public MapCoordinate At(int level, int x, int y)
        {
            return new MapCoordinate(LevelName(level), x, y);
        }
    }
}
