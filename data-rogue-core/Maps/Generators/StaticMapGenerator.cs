using data_rogue_core.Data;
using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.Generators
{
    public class StaticMapGenerator : IMapGenerator
    {
        public StaticMapGenerator(ISystemContainer systemContainer, string mapFile)
        {
            SystemContainer = systemContainer;
            MapFile = mapFile;
        }

        private ISystemContainer SystemContainer;

        public string MapFile { get; }

        public Map Generate(string mapName, IRandom random)
        {
            var mapData = DataFileLoader.LoadFile(MapFile);
            var map = MapSerializer.Deserialize(SystemContainer, mapData, mapName);

            map.MapKey = new MapKey(mapName);

            return map;
        }
    }
}
