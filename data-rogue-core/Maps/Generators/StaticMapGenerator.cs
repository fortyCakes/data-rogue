using data_rogue_core.Data;
using data_rogue_core.EntitySystem;

namespace data_rogue_core.Maps.Generators
{
    public class StaticMapGenerator : IMapGenerator
    {
        public StaticMapGenerator(IEntityEngineSystem entityEngineSystem, string mapFile)
        {
            EntityEngineSystem = entityEngineSystem;
            MapFile = mapFile;
        }

        public IEntityEngineSystem EntityEngineSystem { get; }
        public string MapFile { get; }

        public Map Generate(string mapName, string seed)
        {
            var mapData = DataFileLoader.LoadFile(MapFile);
            var map = MapSerializer.Deserialize(mapData, EntityEngineSystem);

            map.MapKey = new MapKey(mapName);

            return map;
        }
    }
}
