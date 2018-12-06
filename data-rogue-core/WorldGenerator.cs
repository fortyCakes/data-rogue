using System;
using System.Collections.Generic;
using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using System.Drawing;
using System.Linq;

namespace data_rogue_core
{
    public class WorldGenerator
    {
        public static WorldState Create(int seed, IEntityEngineSystem entityEngineSystem)
        {
            entityEngineSystem.Initialise();

            var state = new WorldState(entityEngineSystem);

            var emptyCell = state.EntityEngineSystem.New(
                new Appearance { Color = Color.LightGray, Glyph = '.' },
                new Physical { Passable = true, Transparent = true }
                );

            var wallCell = state.EntityEngineSystem.New(
                new Appearance { Color = Color.LightGray, Glyph = '#' },
                new Physical { Passable = false, Transparent = false }
            );

            var testMap = new Map("testMap", wallCell);

            

            testMap.SetCellsInRange(-5, 5, -5, 5, emptyCell);

            state.Maps = new MapCollection();
            state.Maps.AddMap(testMap);
            state.CurrentMap = testMap;

            var entityParser = new EntityDataParser(
                new List<Type>
                {
                    typeof(Appearance),
                    typeof(Physical),
                    typeof(PlayerControlled),
                    typeof(Position)
                }, 
                entityEngineSystem);

            var player = entityParser.Parse(DataFileLoader.LoadFile(@"Entities\player.edt")).Single();
            player.Get<Position>().MapCoordinate = new MapCoordinate("testMap", 0, 0);

            state.Player = player;

            //state.Player = state.EntityEngineSystem.New(
            //    new Appearance { Color = Color.White, Glyph = '@', ZOrder = int.MaxValue },
            //    new Position { MapCoordinate = new MapCoordinate(testMap.MapKey, 0, 0) },
            //    new PlayerControlled(),
            //    new Physical { Passable = false, Transparent = true }
            //    );

            return state;
        }
    }
}