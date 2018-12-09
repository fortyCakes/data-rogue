using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using System.Linq;
using data_rogue_core.Maps;

namespace data_rogue_core
{
    public class WorldGenerator
    {
        public static WorldState Create(string seed, IEntityEngineSystem entityEngineSystem)
        {
            entityEngineSystem.Initialise();

            var world = new WorldState(entityEngineSystem);

            GenerateInitialMap(seed, entityEngineSystem, world);

            AddPlayerToWorld(entityEngineSystem, world);

            return world;
        }

        private static void GenerateInitialMap(string seed, IEntityEngineSystem entityEngineSystem, WorldState world)
        {
            IMapGenerator mapGenerator = new TestMapGenerator(entityEngineSystem);
            var testMap = mapGenerator.Generate("testMap", seed);
            world.Maps.AddMap(testMap);
            world.CurrentMap = testMap;
        }

        private static void AddPlayerToWorld(IEntityEngineSystem entityEngineSystem, WorldState world)
        {
            var player = EntitySerializer.Deserialize(DataFileLoader.LoadFile(@"Entities\player.edt"), entityEngineSystem);
            player.Get<Position>().MapCoordinate = new MapCoordinate("testMap", 0, 0);

            world.Player = player;
        }
    }
}