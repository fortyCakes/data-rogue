using data_rogue_core.Maps;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Systems
{

    public class MapSystem : IMapSystem
    {
        public void Initialise()
        {
            MapCollection = new MapCollection();
        }

        public MapCollection MapCollection { get; private set; }
    }
}