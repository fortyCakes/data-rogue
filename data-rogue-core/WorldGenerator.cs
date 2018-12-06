using data_rogue_core.Components;
using data_rogue_core.Data;
using data_rogue_core.EntitySystem;
using data_rogue_core.Renderers;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace data_rogue_core
{
    public class WorldGenerator
    {
        public static WorldState Create(int seed)
        {
            var state = new WorldState(new EntityEngine());

            InitialiseSystems(state);

            var emptyCell = state.EntityEngine.New(
                new Appearance { Color = Color.LightGray, Glyph = '.' },
                new Terrain { Passable = true, Transparent = true }
                );

            var testMap = new Map("testMap", emptyCell);

            var wallCell = state.EntityEngine.New(
                new Appearance { Color = Color.LightGray, Glyph = '#' },
                new Terrain { Passable = false, Transparent = false }
                );

            testMap.SetCell(new MapCoordinate("testMap", 5, 5), wallCell);

            state.Maps = new MapCollection();
            state.Maps.AddMap(testMap);
            state.CurrentMap = testMap;

            state.Player = state.EntityEngine.New(
                new Appearance { Color = Color.White, Glyph = '@', ZOrder = int.MaxValue },
                new Position { MapCoordinate = new MapCoordinate(testMap.MapKey, 0, 0) }
                );

            return state;
        }

        private static void InitialiseSystems(WorldState state)
        {
            state.PositionSystem = new PositionSystem();
            state.EntityEngine.Register(state.PositionSystem);
        }
    }
}