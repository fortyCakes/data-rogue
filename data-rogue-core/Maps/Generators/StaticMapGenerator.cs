using data_rogue_core.Data;
using data_rogue_core.Systems.Interfaces;
using System;

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

        public IMap Generate(string mapName, IRandom random, IProgress<string> progress)
        {
            var mapData = DataFileLoader.LoadFile(MapFile);
            var map = MapSerializer.Deserialize(SystemContainer, mapData, mapName);

            map.MapKey = new MapKey(mapName);

            return map;
        }
    }
}
