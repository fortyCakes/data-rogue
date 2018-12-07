using System;
using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using System.Drawing;
using System.Linq;
using data_rogue_core.Maps;

namespace data_rogue_core
{
    public class WorldGenerator
    {
        public static WorldState Create(int seed, IEntityEngineSystem entityEngineSystem)
        {
            entityEngineSystem.Initialise();

            EntityDataParser entityParser = SetUpEntityParser(entityEngineSystem);

            var state = new WorldState(entityEngineSystem);

            var cells = entityParser.Parse(DataFileLoader.LoadFile(@"Entities\MapCells\GenericCells.edt"));

            var wallCell = cells.Single(e => e.Name == "WallCell");
            var emptyCell = cells.Single(e => e.Name == "EmptyCell");

            var testMap = new Map("testMap", wallCell);

            

            testMap.SetCellsInRange(-5, 5, -5, 5, emptyCell);

            state.Maps = new MapCollection();
            state.Maps.AddMap(testMap);
            state.CurrentMap = testMap;

            

            var player = entityParser.Parse(DataFileLoader.LoadFile(@"Entities\player.edt")).Single();
            player.Get<Position>().MapCoordinate = new MapCoordinate("testMap", 0, 0);

            state.Player = player;

            return state;
        }

        private static EntityDataParser SetUpEntityParser(IEntityEngineSystem entityEngineSystem)
        {
            var entityParser = new EntityDataParser(
                new List<Type>
                {
                    typeof(Appearance),
                    typeof(Physical),
                    typeof(PlayerControlled),
                    typeof(Position)
                },
                entityEngineSystem);
            return entityParser;
        }
    }
}