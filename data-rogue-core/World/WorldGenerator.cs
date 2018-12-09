using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using System.Linq;
using data_rogue_core.Maps;

namespace data_rogue_core
{
    public class WorldGenerator
    {
        public static WorldState Create(int seed, IEntityEngineSystem entityEngineSystem)
        {
            entityEngineSystem.Initialise();

            var state = new WorldState(entityEngineSystem);

            var wallCell = entityEngineSystem.GetEntitiesWithName("Cell:Wall").Single();
            var emptyCell = entityEngineSystem.GetEntitiesWithName("Cell:Empty").Single();
            var boulderCell = entityEngineSystem.GetEntitiesWithName("Cell:Boulder").Single();

            var testMap = new Map("testMap", wallCell);

            testMap.SetCellsInRange(-5, 5, -5, 5, emptyCell);
            testMap.SetCell(-5, -5, boulderCell);

            state.Maps = new MapCollection();
            state.Maps.AddMap(testMap);
            state.CurrentMap = testMap;

            

            var player = EntitySerializer.Deserialize(DataFileLoader.LoadFile(@"Entities\player.edt"), entityEngineSystem);
            player.Get<Position>().MapCoordinate = new MapCoordinate("testMap", 0, 0);

            state.Player = player;

            return state;
        }
    }
}