using data_rogue_core.Data;
using data_rogue_core.EntityEngine;
using data_rogue_core.Systems.Interfaces;

namespace data_rogue_core.Maps.Generators
{
    public class StaticMapGenerator : IMapGenerator
    {
        public StaticMapGenerator(IEntityEngine entityEngineSystem, IPrototypeSystem prototypeSystem, string mapFile)
        {
            EntityEngineSystem = entityEngineSystem;
            PrototypeSystem = prototypeSystem;
            MapFile = mapFile;
        }

        public IEntityEngine EntityEngineSystem { get; }
        public IPrototypeSystem PrototypeSystem { get; }
        public string MapFile { get; }

        public Map Generate(string mapName, IRandom random)
        {
            var mapData = DataFileLoader.LoadFile(MapFile);
            var map = MapSerializer.Deserialize(mapData, EntityEngineSystem, PrototypeSystem, mapName);

            map.MapKey = new MapKey(mapName);

            return map;
        }
    }
}
