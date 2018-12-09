using data_rogue_core.EntitySystem;
using data_rogue_core.Maps;
using System.Linq;

namespace data_rogue_core
{
    internal class TestMapGenerator : IMapGenerator
    {
        private IEntityEngineSystem engine;
        private Entity wallCell;
        private Entity emptyCell;
        private Entity boulderCell;

        public TestMapGenerator(IEntityEngineSystem entityEngineSystem)
        {
            engine = entityEngineSystem;

            wallCell = entityEngineSystem.GetEntitiesWithName("Cell:Wall").Single();
            emptyCell = entityEngineSystem.GetEntitiesWithName("Cell:Empty").Single();
            boulderCell = entityEngineSystem.GetEntitiesWithName("Cell:Boulder").Single();

        }

        public Map Generate(string mapName, string seed)
        {
            var map = new Map("testMap", wallCell);

            map.SetCellsInRange(-5, 5, -5, 5, emptyCell);
            map.SetCell(-5, -5, boulderCell);

            return map;
        }
    }
}