using System;
using System.Collections.Generic;
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

            var cells = EntitySerializer.DeserializeMultiple(DataFileLoader.LoadFile(@"Entities\MapCells\GenericCells.edt"), entityEngineSystem);

            var wallCell = cells.Single(e => e.Name == "WallCell");
            var emptyCell = cells.Single(e => e.Name == "EmptyCell");
            var boulderCell = cells.Single(e => e.Name == "BoulderCell");

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